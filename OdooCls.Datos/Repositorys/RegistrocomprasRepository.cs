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

                    using var cmd = new OdbcCommand($"{{ CALL {library}.SP_GET_NEXT_TTABD(?, ?) }}", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // IN v_period CHAR(6)
                    var pIn = new OdbcParameter("@v_period", OdbcType.Char, 6)
                    {
                        Direction = ParameterDirection.Input,
                        Value = periodo
                    };
                    cmd.Parameters.Add(pIn);

                    // OUT next_corr: IBM i Packed Decimal, se lee como Char para evitar ERROR 22018
                    var pOut = new OdbcParameter("@next_corr", OdbcType.Char, 15)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pOut);

                    cmd.ExecuteNonQuery();

                    var rawValue = pOut.Value?.ToString()?.Trim() ?? "0";
                    return int.TryParse(rawValue, out int result) ? result : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GetNextCorr: {ex.Message}");
                throw new Exception($"[GetNextCorr] {ex.Message}", ex);
            }
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
            Console.WriteLine($"[RC] InsertTregcAndCtxp inicio | EJER={registro.RCEJER} PERI={registro.RCPERI} TDOC={registro.RCTDOC} NDOC={registro.RCNDOC} RCRCXP={registro.RCRCXP} PROV={registro.RCPROV} MONE={registro.RCMONE} VALV={registro.RCVALV} IMP1={registro.RCIMP1} PVT={registro.RCPVTA}");
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
                    Console.WriteLine($"[RC] Insertando TREGC | RCEJER={registro.RCEJER} RCPERI={registro.RCPERI} RCTDOC={registro.RCTDOC} RCNDOC={registro.RCNDOC} RCRCXP={registro.RCRCXP}");
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
                    Console.WriteLine($"[RC] TREGC filas afectadas={rowsTregc}");
                    if (rowsTregc <= 0)
                        return false;
                }

                Console.WriteLine($"[RC] Insertando TCTXP | EJER={registro.RCEJER} PERI={registro.RCPERI} TDOC={registro.RCTDOC} NDOC={registro.RCNDOC} RCRCXP={registro.RCRCXP}");
                await InsertCtxpInConnection(cn, registro.RCEJER, registro.RCPERI, registro.RCTDOC, registro.RCNDOC);
                Console.WriteLine($"[RC] InsertTregcAndCtxp OK | EJER={registro.RCEJER} PERI={registro.RCPERI} TDOC={registro.RCTDOC} NDOC={registro.RCNDOC} RCRCXP={registro.RCRCXP}");
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

                Console.WriteLine($"[RC] ERROR InsertTregcAndCtxp | EJER={registro.RCEJER} PERI={registro.RCPERI} TDOC={registro.RCTDOC} NDOC={registro.RCNDOC} RCRCXP={registro.RCRCXP} | {ex.Message}");
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
            Console.WriteLine($"[RC] Preparando TCTXP | EJER={ejercicio} PERI={mes} TDOC={tipodoc} NDOC={nrodoc}");
            cmdCtxp.Parameters.AddWithValue("@RCEJER", ejercicio);
            cmdCtxp.Parameters.AddWithValue("@RCPERI", mes);
            cmdCtxp.Parameters.AddWithValue("@RCTDOC", Trunc(tipodoc, 2));
            cmdCtxp.Parameters.AddWithValue("@RCNDOC", Trunc(nrodoc, 15));

            var rowsCtxp = await cmdCtxp.ExecuteNonQueryAsync();
            Console.WriteLine($"[RC] TCTXP filas afectadas={rowsCtxp} | EJER={ejercicio} PERI={mes} TDOC={tipodoc} NDOC={nrodoc}");
            if (rowsCtxp <= 0)
                throw new Exception("No se insertaron filas en TCTXP");
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
            bool rp = false;
            var Query = $"select * from {library}.tregc where RCEJER={ejercicio} and RCPERI={mes} and RCTDOC='{Tipodoc}' AND RCNDOC='{nrodoc}'";

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

                        int count = reader.GetInt32(0); // Obtener el valor de la primera columna
                        rp = count > 0; // Si hay alguna fila, existe el tipo de documento
                    }
                }
            }
            return rp;
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
