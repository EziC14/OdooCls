using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistrocomprasRepository : IRegistroComprasRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? companyCode;
        string? connectionString;

        public RegistrocomprasRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            companyCode = ObtenerCompanyCode(library);
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        private static string ObtenerCompanyCode(string? libraryName)
        {
            if (string.IsNullOrWhiteSpace(libraryName) || libraryName.Length < 2)
                throw new InvalidOperationException("Authentication:Library no tiene un formato válido.");

            var trimmed = libraryName.Trim();
            return trimmed.Substring(trimmed.Length - 2).ToUpperInvariant();
        }

        private static string Trunc(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        private bool CallLibreria(OdbcConnection cn)
        {
            string sql = $"CALL SPEED407.MA1004 ('{companyCode}')";
            using var cmd = new OdbcCommand(sql, cn);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine($"Error configurando bibliotecas: {E.Message}");
                return false;
            }
        }

        public int GetNextCorr(string periodo)
        {
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    if (!CallLibreria(connection))
                        return 0;

                    var activeLibrary = library?.Trim();
                    if (string.IsNullOrWhiteSpace(activeLibrary))
                        throw new InvalidOperationException("Authentication:Library no está configurada.");

                    try
                    {
                        return ExecuteGetNextCorr(connection, activeLibrary, periodo);
                    }
                    catch (OdbcException ex) when (ex.Message.Contains("SQL0204"))
                    {
                        return GetNextCorrFromTables(connection, activeLibrary, periodo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GetNextCorr: {ex.Message}");
                throw new Exception($"[GetNextCorr] {ex.Message}", ex);
            }
        }

        private static int ExecuteGetNextCorr(OdbcConnection connection, string schema, string periodo)
        {
            using var cmd = new OdbcCommand($"{{ CALL {schema}.SP_GET_NEXT_TTABD(?, ?) }}", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var pIn = new OdbcParameter("@v_period", OdbcType.Char, 6)
            {
                Direction = ParameterDirection.Input,
                Value = periodo
            };
            cmd.Parameters.Add(pIn);

            var pOut = new OdbcParameter("@next_corr", OdbcType.Char, 15)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pOut);

            cmd.ExecuteNonQuery();

            var rawValue = pOut.Value?.ToString()?.Trim() ?? "0";
            return int.TryParse(rawValue, out int result) ? result : 0;
        }

        private static int GetNextCorrFromTables(OdbcConnection connection, string schema, string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo) || periodo.Length != 6)
                throw new InvalidOperationException("Periodo inválido para correlativo local. Formato esperado: yyyyMM.");

            int year = int.Parse(periodo.Substring(0, 4));
            int month = int.Parse(periodo.Substring(4, 2));

            string sql = $@"
                SELECT COALESCE(MAX(CORR), 0) + 1
                FROM (
                    SELECT INT(RIGHT(TRIM(RCRCXP), 5)) AS CORR
                    FROM {schema}.TREGC
                    WHERE RCEJER = ? AND RCPERI = ?
                    UNION ALL
                    SELECT INT(RIGHT(TRIM(XPRCXP), 5)) AS CORR
                    FROM {schema}.TCTXP
                    WHERE XPEJER = ? AND XPPERI = ?
                ) X";

            using var cmd = new OdbcCommand(sql, connection);
            cmd.Parameters.AddWithValue("@RCEJER", year);
            cmd.Parameters.AddWithValue("@RCPERI", month);
            cmd.Parameters.AddWithValue("@XPEJER", year);
            cmd.Parameters.AddWithValue("@XPPERI", month);

            var value = cmd.ExecuteScalar();
            int corr = Convert.ToInt32(value ?? 1);
            return corr <= 0 ? 1 : corr;
        }

        public async Task<bool> InsertTregc(RegistroCompras registro)
        {
            bool rp = false;
            string Query = $@"insert into {library}.tregc (
            RCEJER,RCPERI,RCTDOC,RCNDOC,RCFECH,RCRCXP,RCCPRO,RCPROV,RCRUC,RCARTI,RCMONE,RCTCAM,RCVALV,RCCVAL,RCMVAL,RCVALI,
            RCCVAI,RCMVAI,RCDSCT,RCCDSC,RCMDSC,RCIMP1,RCCIM1,RCMIM1,RCPVTA,RCCPVT,RCMPVT,RCCONC,RCASTO,RCCOST,RCTREF,RCNREF,
            RCFEVE,RCNDOM,RCCPAG,RCSITU,RCUSIN,RCFEIN,RCHOIN,RCRVVA,RCREF7,RCCBSA
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using OdbcConnection cn = new OdbcConnection(connectionString);
                {
                    using OdbcCommand cmd = new OdbcCommand(Query, cn);
                    {  
                       await cn.OpenAsync();
                       
                       if (!CallLibreria(cn))
                           return false;
                       
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@RCEJER", registro.RCEJER);
                        cmd.Parameters.AddWithValue("@RCPERI", registro.RCPERI);
                        cmd.Parameters.AddWithValue("@RCTDOC", Trunc(registro.RCTDOC, 2));
                        cmd.Parameters.AddWithValue("@RCNDOC", Trunc(registro.RCNDOC, 15));
                        cmd.Parameters.AddWithValue("@RCFECH", registro.RCFECH);
                        cmd.Parameters.AddWithValue("@RCRCXP", Trunc(registro.RCRCXP, 11));
                        cmd.Parameters.AddWithValue("@RCCPRO", Trunc(registro.RCCPRO, 10));
                        cmd.Parameters.AddWithValue("@RCPROV", Trunc(registro.RCPROV, 40));
                        cmd.Parameters.AddWithValue("@RCRUC",  Trunc(registro.RCRUC, 15));
                        cmd.Parameters.AddWithValue("@RCARTI", Trunc(registro.RCARTI, 40));
                        cmd.Parameters.AddWithValue("@RCMONE", registro.RCMONE);
                        cmd.Parameters.AddWithValue("@RCTCAM", registro.RCTCAM);
                        cmd.Parameters.AddWithValue("@RCVALV", registro.RCVALV);
                        cmd.Parameters.AddWithValue("@RCCVAL", Trunc(registro.RCCVAL, 15));
                        cmd.Parameters.AddWithValue("@RCMVAL", Trunc(registro.RCMVAL, 1));
                        cmd.Parameters.AddWithValue("@RCVALI", registro.RCVALI);
                        cmd.Parameters.AddWithValue("@RCCVAI", Trunc(registro.RCCVAI, 15));
                        cmd.Parameters.AddWithValue("@RCMVAI", Trunc(registro.RCMVAI, 1));
                        cmd.Parameters.AddWithValue("@RCDSCT", registro.RCDSCT);
                        cmd.Parameters.AddWithValue("@RCCDSC", Trunc(registro.RCCDSC, 15));
                        cmd.Parameters.AddWithValue("@RCMDSC", Trunc(registro.RCMDSC, 1));
                        cmd.Parameters.AddWithValue("@RCIMP1", registro.RCIMP1);
                        cmd.Parameters.AddWithValue("@RCCIM1", Trunc(registro.RCCIM1, 15));
                        cmd.Parameters.AddWithValue("@RCMIM1", Trunc(registro.RCMIM1, 1));
                        cmd.Parameters.AddWithValue("@RCPVTA", registro.RCPVTA);
                        cmd.Parameters.AddWithValue("@RCCPVT", Trunc(registro.RCCPVT, 15));
                        cmd.Parameters.AddWithValue("@RCMPVT", Trunc(registro.RCMPVT, 1));
                        cmd.Parameters.AddWithValue("@RCCONC", registro.RCCONC);
                        cmd.Parameters.AddWithValue("@RCASTO", Trunc(registro.RCASTO, 10));
                        cmd.Parameters.AddWithValue("@RCCOST", Trunc(registro.RCCOST, 15));
                        cmd.Parameters.AddWithValue("@RCTREF", Trunc(registro.RCTREF, 2));
                        cmd.Parameters.AddWithValue("@RCNREF", Trunc(registro.RCNREF, 15));
                        cmd.Parameters.AddWithValue("@RCFEVE", registro.RCFEVE);
                        cmd.Parameters.AddWithValue("@RCNDOM", Trunc(registro.RCNDOM, 1));
                        cmd.Parameters.AddWithValue("@RCCPAG", Trunc(registro.RCCPAG, 3));
                        cmd.Parameters.AddWithValue("@RCSITU", Trunc(registro.RCSITU, 2));
                        cmd.Parameters.AddWithValue("@RCUSIN", Trunc(registro.RCUSIN, 10));
                        cmd.Parameters.AddWithValue("@RCFEIN", registro.RCFEIN);
                        cmd.Parameters.AddWithValue("@RCHOIN", registro.RCHOIN);
                        cmd.Parameters.AddWithValue("@RCRVVA", Trunc(registro.RCRVVA, 10));
                        cmd.Parameters.AddWithValue("@RCREF7", Trunc(registro.RCREF7, 15));
                        cmd.Parameters.AddWithValue("@RCCBSA", Trunc(registro.RCCBSA, 1));

                        var rowsAffected = await cmd.ExecuteNonQueryAsync();
                        rp = rowsAffected > 0;
                     }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertTregc: {ex.Message}");
                rp = false;
            }

            return rp;
          
        }

        public async Task<bool> InsertTregcd(RegistroComprasDetail registro)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertTregcAndCtxp(RegistroCompras registro)
        {
            string queryTregc = $@"insert into {library}.tregc (
            RCEJER,RCPERI,RCTDOC,RCNDOC,RCFECH,RCRCXP,RCCPRO,RCPROV,RCRUC,RCARTI,RCMONE,RCTCAM,RCVALV,RCCVAL,RCMVAL,RCVALI,
            RCCVAI,RCMVAI,RCDSCT,RCCDSC,RCMDSC,RCIMP1,RCCIM1,RCMIM1,RCPVTA,RCCPVT,RCMPVT,RCCONC,RCASTO,RCCOST,RCTREF,RCNREF,
            RCFEVE,RCNDOM,RCCPAG,RCSITU,RCUSIN,RCFEIN,RCHOIN,RCRVVA,RCREF7,RCCBSA
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using OdbcConnection cn = new OdbcConnection(connectionString);
                await cn.OpenAsync();

                if (!CallLibreria(cn))
                    return false;

                using (var cmdTregc = new OdbcCommand(queryTregc, cn))
                {
                    cmdTregc.CommandType = CommandType.Text;
                    cmdTregc.Parameters.AddWithValue("@RCEJER", registro.RCEJER);
                    cmdTregc.Parameters.AddWithValue("@RCPERI", registro.RCPERI);
                    cmdTregc.Parameters.AddWithValue("@RCTDOC", Trunc(registro.RCTDOC, 2));
                    cmdTregc.Parameters.AddWithValue("@RCNDOC", Trunc(registro.RCNDOC, 15));
                    cmdTregc.Parameters.AddWithValue("@RCFECH", registro.RCFECH);
                    cmdTregc.Parameters.AddWithValue("@RCRCXP", Trunc(registro.RCRCXP, 11));
                    cmdTregc.Parameters.AddWithValue("@RCCPRO", Trunc(registro.RCCPRO, 10));
                    cmdTregc.Parameters.AddWithValue("@RCPROV", Trunc(registro.RCPROV, 40));
                    cmdTregc.Parameters.AddWithValue("@RCRUC", Trunc(registro.RCRUC, 15));
                    cmdTregc.Parameters.AddWithValue("@RCARTI", Trunc(registro.RCARTI, 40));
                    cmdTregc.Parameters.AddWithValue("@RCMONE", registro.RCMONE);
                    cmdTregc.Parameters.AddWithValue("@RCTCAM", registro.RCTCAM);
                    cmdTregc.Parameters.AddWithValue("@RCVALV", registro.RCVALV);
                    cmdTregc.Parameters.AddWithValue("@RCCVAL", Trunc(registro.RCCVAL, 15));
                    cmdTregc.Parameters.AddWithValue("@RCMVAL", Trunc(registro.RCMVAL, 1));
                    cmdTregc.Parameters.AddWithValue("@RCVALI", registro.RCVALI);
                    cmdTregc.Parameters.AddWithValue("@RCCVAI", Trunc(registro.RCCVAI, 15));
                    cmdTregc.Parameters.AddWithValue("@RCMVAI", Trunc(registro.RCMVAI, 1));
                    cmdTregc.Parameters.AddWithValue("@RCDSCT", registro.RCDSCT);
                    cmdTregc.Parameters.AddWithValue("@RCCDSC", Trunc(registro.RCCDSC, 15));
                    cmdTregc.Parameters.AddWithValue("@RCMDSC", Trunc(registro.RCMDSC, 1));
                    cmdTregc.Parameters.AddWithValue("@RCIMP1", registro.RCIMP1);
                    cmdTregc.Parameters.AddWithValue("@RCCIM1", Trunc(registro.RCCIM1, 15));
                    cmdTregc.Parameters.AddWithValue("@RCMIM1", Trunc(registro.RCMIM1, 1));
                    cmdTregc.Parameters.AddWithValue("@RCPVTA", registro.RCPVTA);
                    cmdTregc.Parameters.AddWithValue("@RCCPVT", Trunc(registro.RCCPVT, 15));
                    cmdTregc.Parameters.AddWithValue("@RCMPVT", Trunc(registro.RCMPVT, 1));
                    cmdTregc.Parameters.AddWithValue("@RCCONC", registro.RCCONC);
                    cmdTregc.Parameters.AddWithValue("@RCASTO", Trunc(registro.RCASTO, 10));
                    cmdTregc.Parameters.AddWithValue("@RCCOST", Trunc(registro.RCCOST, 15));
                    cmdTregc.Parameters.AddWithValue("@RCTREF", Trunc(registro.RCTREF, 2));
                    cmdTregc.Parameters.AddWithValue("@RCNREF", Trunc(registro.RCNREF, 15));
                    cmdTregc.Parameters.AddWithValue("@RCFEVE", registro.RCFEVE);
                    cmdTregc.Parameters.AddWithValue("@RCNDOM", Trunc(registro.RCNDOM, 1));
                    cmdTregc.Parameters.AddWithValue("@RCCPAG", Trunc(registro.RCCPAG, 3));
                    cmdTregc.Parameters.AddWithValue("@RCSITU", Trunc(registro.RCSITU, 2));
                    cmdTregc.Parameters.AddWithValue("@RCUSIN", Trunc(registro.RCUSIN, 10));
                    cmdTregc.Parameters.AddWithValue("@RCFEIN", registro.RCFEIN);
                    cmdTregc.Parameters.AddWithValue("@RCHOIN", registro.RCHOIN);
                    cmdTregc.Parameters.AddWithValue("@RCRVVA", Trunc(registro.RCRVVA, 10));
                    cmdTregc.Parameters.AddWithValue("@RCREF7", Trunc(registro.RCREF7, 15));
                    cmdTregc.Parameters.AddWithValue("@RCCBSA", Trunc(registro.RCCBSA, 1));

                    var rowsTregc = await cmdTregc.ExecuteNonQueryAsync();
                    if (rowsTregc <= 0)
                        return false;
                }

                await InsertCtxpInConnection(cn, registro.RCEJER, registro.RCPERI, registro.RCTDOC, registro.RCNDOC);
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    using OdbcConnection cnCleanup = new OdbcConnection(connectionString);
                    await cnCleanup.OpenAsync();
                    if (CallLibreria(cnCleanup))
                        await CleanupPartialPurchasesInserts(cnCleanup, registro.RCEJER, registro.RCPERI, registro.RCTDOC, registro.RCNDOC, registro.RCRCXP);
                }
                catch { }

                throw new Exception($"[InsertTregcAndCtxp] {ex.Message}", ex);
            }
        }

        private async Task InsertCtxpInConnection(OdbcConnection cn, int ejercicio, int mes, string tipodoc, string nrodoc)
        {
            string queryCtxp = $@"INSERT INTO {library}.tctxp (
                XPEJER, XPPERI, XPTDOC, XPNDOC, XPFECH, XPFEVE, XPRCXP, XPCPRO, XPCPAG,
                XPMONE, XPTCMO, XPPVMO, XPPAMO, XPPVMN, XPPAMN, XPTCDO, XPPVDO, XPPADO,
                XPSITU, XPPFPA, XPPBCO, XPPIMP, XPPMON, XPACTI, XPTGAS, XPCTAC, XPCCTO,
                XPRF01, XPRF02, XPRF03, XPRF04, XPRF05
            )
            SELECT
                RCEJER, RCPERI, RCTDOC, RCNDOC, RCFECH, RCFEVE, SUBSTR(RCRCXP, 1, 10), RCCPRO, RCCPAG,
                RCMONE, RCTCAM, RCPVTA, 0,
                CASE WHEN RCMONE = 0 THEN RCPVTA ELSE ROUND(RCPVTA * RCTCAM, 2) END, 0,
                RCTCAM,
                CASE WHEN RCMONE = 0 THEN ROUND(RCPVTA / RCTCAM, 2) ELSE RCPVTA END, 0,
                RCSITU, 0, '', 0, 0, '', '', RCCPVT, RCCOST,
                '', '', '', 0, 0
            FROM {library}.tregc
            WHERE RCEJER = ? AND RCPERI = ? AND RCTDOC = ? AND RCNDOC = ?";

            using OdbcCommand cmdCtxp = new OdbcCommand(queryCtxp, cn);
            cmdCtxp.CommandType = CommandType.Text;
            cmdCtxp.Parameters.AddWithValue("@RCEJER", ejercicio);
            cmdCtxp.Parameters.AddWithValue("@RCPERI", mes);
            cmdCtxp.Parameters.AddWithValue("@RCTDOC", Trunc(tipodoc, 2));
            cmdCtxp.Parameters.AddWithValue("@RCNDOC", Trunc(nrodoc, 15));

            try
            {
                var rowsCtxp = await cmdCtxp.ExecuteNonQueryAsync();
                if (rowsCtxp <= 0)
                    throw new Exception("No se insertaron filas en TCTXP");
            }
            catch (OdbcException ex) when (ex.Message.Contains("SQL0803"))
            {
                string rcxp = "";
                using (var cmdRc = new OdbcCommand($"SELECT COALESCE(MAX(RCRCXP), '') FROM {library}.TREGC WHERE RCEJER=? AND RCPERI=? AND RCTDOC=? AND RCNDOC=?", cn))
                {
                    cmdRc.Parameters.AddWithValue("@RCEJER", ejercicio);
                    cmdRc.Parameters.AddWithValue("@RCPERI", mes);
                    cmdRc.Parameters.AddWithValue("@RCTDOC", Trunc(tipodoc, 2));
                    cmdRc.Parameters.AddWithValue("@RCNDOC", Trunc(nrodoc, 15));
                    rcxp = Convert.ToString(await cmdRc.ExecuteScalarAsync()) ?? "";
                }

                int countDoc = 0;
                int countRcxp = 0;

                using (var cmdDoc = new OdbcCommand($"SELECT COUNT(*) FROM {library}.TCTXP WHERE XPEJER=? AND XPPERI=? AND XPTDOC=? AND XPNDOC=?", cn))
                {
                    cmdDoc.Parameters.AddWithValue("@XPEJER", ejercicio);
                    cmdDoc.Parameters.AddWithValue("@XPPERI", mes);
                    cmdDoc.Parameters.AddWithValue("@XPTDOC", Trunc(tipodoc, 2));
                    cmdDoc.Parameters.AddWithValue("@XPNDOC", Trunc(nrodoc, 15));
                    countDoc = Convert.ToInt32(await cmdDoc.ExecuteScalarAsync() ?? 0);
                }

                using (var cmdRcxp = new OdbcCommand($"SELECT COUNT(*) FROM {library}.TCTXP WHERE XPEJER=? AND XPPERI=? AND XPRCXP=?", cn))
                {
                    cmdRcxp.Parameters.AddWithValue("@XPEJER", ejercicio);
                    cmdRcxp.Parameters.AddWithValue("@XPPERI", mes);
                    cmdRcxp.Parameters.AddWithValue("@XPRCXP", Trunc(rcxp, 10));
                    countRcxp = Convert.ToInt32(await cmdRcxp.ExecuteScalarAsync() ?? 0);
                }

                throw;
            }
        }

        private async Task CleanupPartialPurchasesInserts(OdbcConnection cn, int ejercicio, int mes, string tipodoc, string nrodoc, string rcxpx)
        {
            string deleteTctxp = $@"DELETE FROM {library}.TCTXP WHERE XPEJER = ? AND XPPERI = ? AND XPTDOC = ? AND XPNDOC = ?";
            string deleteTregc = $@"DELETE FROM {library}.TREGC WHERE RCEJER = ? AND RCPERI = ? AND RCTDOC = ? AND RCNDOC = ? AND RCRCXP = ?";

            using (OdbcCommand cmd1 = new OdbcCommand(deleteTctxp, cn))
            {
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddWithValue("@XPEJER", ejercicio);
                cmd1.Parameters.AddWithValue("@XPPERI", mes);
                cmd1.Parameters.AddWithValue("@XPTDOC", Trunc(tipodoc, 2));
                cmd1.Parameters.AddWithValue("@XPNDOC", Trunc(nrodoc, 15));
                await cmd1.ExecuteNonQueryAsync();
            }

            using (OdbcCommand cmd2 = new OdbcCommand(deleteTregc, cn))
            {
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.AddWithValue("@RCEJER", ejercicio);
                cmd2.Parameters.AddWithValue("@RCPERI", mes);
                cmd2.Parameters.AddWithValue("@RCTDOC", Trunc(tipodoc, 2));
                cmd2.Parameters.AddWithValue("@RCNDOC", Trunc(nrodoc, 15));
                cmd2.Parameters.AddWithValue("@RCRCXP", Trunc(rcxpx, 11));
                await cmd2.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> InsertCtxp(int ejercicio, int mes, string tipodoc, string nrodoc)
        {
            try
            {
                using OdbcConnection cn = new OdbcConnection(connectionString);
                await cn.OpenAsync();

                if (!CallLibreria(cn))
                    return false;
                await InsertCtxpInConnection(cn, ejercicio, mes, tipodoc, nrodoc);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertCtxp: {ex.Message}");
                throw new Exception($"[InsertCtxp] {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidaMoneda(int moneda)
        {

            bool rp = false;
            string Query = @$"select count(*) from {library}.tmone where moncve={moneda}";

            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                
                if (!CallLibreria(connection))
                    return false;
                
                using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                {
                    // Verificar si se encontró algún dato
                    if (await reader.ReadAsync())
                    {
                        // Obtener el valor de la primera columna (el resultado de COUNT(1))
                        int count = reader.GetInt32(0); // Obtener el valor de la primera columna
                        rp = count > 0; // Si hay alguna fila, existe la moneda
                    }
                }
            }
            return rp;
        }

        public async Task<bool> ValidaProveedor(string codprov)
        {
            bool rp = false;
            string Query = @$"select count(*) from {library}.tprov where procve='{codprov}'";

            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                
                if (!CallLibreria(connection))
                    return false;
                
                using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                {
                    // Verificar si se encontró algún dato
                    if (await reader.ReadAsync())
                    {
                        // Obtener el valor de la primera columna (el resultado de COUNT(1))
                        int count = reader.GetInt32(0); // Obtener el valor de la primera columna
                        rp = count > 0; // Si hay alguna fila, existe el cliente
                    }
                }
            }
            return rp;
        }

        public async Task<bool> ValidarExistenciaDocumento(int ejercicio, int mes, string Tipodoc, string nrodoc)
        {
            const string qTregc = @"SELECT COUNT(*) FROM {0}.TREGC WHERE RCEJER=? AND RCPERI=? AND RCTDOC=? AND RCNDOC=?";
            const string qTctxp = @"SELECT COUNT(*) FROM {0}.TCTXP WHERE XPEJER=? AND XPPERI=? AND XPTDOC=? AND XPNDOC=?";

            using (var connection = new OdbcConnection(connectionString))
            {
                await connection.OpenAsync();

                if (!CallLibreria(connection))
                    return false;

                int countTregc = 0;
                int countTctxp = 0;

                using (var cmdTregc = new OdbcCommand(string.Format(qTregc, library), connection))
                {
                    cmdTregc.Parameters.AddWithValue("@RCEJER", ejercicio);
                    cmdTregc.Parameters.AddWithValue("@RCPERI", mes);
                    cmdTregc.Parameters.AddWithValue("@RCTDOC", Trunc(Tipodoc, 2));
                    cmdTregc.Parameters.AddWithValue("@RCNDOC", Trunc(nrodoc, 15));
                    countTregc = Convert.ToInt32(await cmdTregc.ExecuteScalarAsync() ?? 0);
                }

                using (var cmdTctxp = new OdbcCommand(string.Format(qTctxp, library), connection))
                {
                    cmdTctxp.Parameters.AddWithValue("@XPEJER", ejercicio);
                    cmdTctxp.Parameters.AddWithValue("@XPPERI", mes);
                    cmdTctxp.Parameters.AddWithValue("@XPTDOC", Trunc(Tipodoc, 2));
                    cmdTctxp.Parameters.AddWithValue("@XPNDOC", Trunc(nrodoc, 15));
                    countTctxp = Convert.ToInt32(await cmdTctxp.ExecuteScalarAsync() ?? 0);
                }

                return countTregc > 0 || countTctxp > 0;
            }
        }

        public async Task<bool> ValidarStatusRC(int ejercicio, int mes, string stconta)
        {

            bool rp = false;
            string vc = "", vrv = "";
            string Query = @$"select persit,persrc from {library}.tperc where perano={ejercicio} and pernum={mes}";
            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                
                if (!CallLibreria(connection))
                    return false;
                
                using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                {
                    // Verificar si se encontró algún dato
                    if (await reader.ReadAsync())
                    {
                        vc = reader.GetString(0); // Obtener el valor de la primera columna
                        vrv = reader.GetString(1);
                    }
                }
            }

            if (stconta == "CO")
            {
                if (vc == "A")
                {
                    rp = true;
                }
                else
                {
                    rp = false;
                }
            }
            else
            {
                if (vrv == "A")
                {
                    rp = true;
                }
                else
                {
                    rp = false;
                }

            }

            return rp;
        }

        public async Task<bool> ValidatTipoDoc(string tipo)
        {
            bool rp = false;
            string Query = @$"select count(*) from {library}.ttido where tdregi='C' AND TDTIPO='{tipo}'";

            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                
                if (!CallLibreria(connection))
                    return false;
                
                using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int count = reader.GetInt32(0);
                        rp = count > 0;
                    }
                }
            }
            return rp;
        }
    }
}
