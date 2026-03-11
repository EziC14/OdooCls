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
        string? connectionString;

        public RegistrocomprasRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        private static string Trunc(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        private static bool CallLibreria(OdbcConnection cn)
        {
            string sql = "CALL SPEED407.MA1004 ('XX')";
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

                    using var cmd = new OdbcCommand("{ CALL speed400xx.SP_GET_NEXT_TTABD(?, ?) }", connection)
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
