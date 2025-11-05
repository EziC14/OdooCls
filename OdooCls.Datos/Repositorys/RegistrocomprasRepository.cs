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

        public int GetNextCorr(string periodo)
        {
           
            using (var connection = new OdbcConnection(connectionString))
            {
               connection.Open();
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

                // OUT next_corr INTEGER
                var pOut = new OdbcParameter("@next_corr", OdbcType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pOut);

                cmd.ExecuteNonQuery();

                return Convert.ToInt32(pOut.Value);
            }  

        }

        public async Task<bool> InsertTregc(RegistroCompras registro)
        {
            bool rp = false;
            string Query = $@"insert into {library}.tregc 
            RCEJER,RCPERI,RCTDOC,RCNDOC,RCFECH,RCRCXP,RCCPRO,RCPROV,RCRUC,RCARTI,RCMONE,RCTCAM,RCVALV,RCCVAL,RCMVAL,RCVALI,
            RCCVAI,RCMVAI,RCDSCT,RCCDSC,RCMDSC,RCIMP1,RCCIM1,RCMIM1,RCPVTA,RCCPVT,RCMPVT,RCCONC,RCASTO,RCCOST,RCTREF,RCNREF,
            RCFEVE,RCNDOM,RCCPAG,RCSITU,RCUSIN,RCFEIN,RCHOIN,RCRVVA,RCREF7,RCCBSA
            VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using OdbcConnection cn = new OdbcConnection(connectionString);
                {
                    using OdbcCommand cmd = new OdbcCommand(Query, cn);
                    {  
                       await cn.OpenAsync();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@RCEJER", registro.RCEJER);
                        cmd.Parameters.AddWithValue("@RCPERI", registro.RCPERI);
                        cmd.Parameters.AddWithValue("@RCTDOC", registro.RCTDOC);
                        cmd.Parameters.AddWithValue("@RCNDOC", registro.RCNDOC);
                        cmd.Parameters.AddWithValue("@RCFECH", registro.RCFECH);
                        cmd.Parameters.AddWithValue("@RCRCXP", registro.RCRCXP);
                        cmd.Parameters.AddWithValue("@RCCPRO", registro.RCCPRO);
                        cmd.Parameters.AddWithValue("@RCPROV", registro.RCPROV);
                        cmd.Parameters.AddWithValue("@RCRUC", registro.RCRUC);
                        cmd.Parameters.AddWithValue("@RCARTI", registro.RCARTI);
                        cmd.Parameters.AddWithValue("@RCMONE", registro.RCMONE);
                        cmd.Parameters.AddWithValue("@RCTCAM", registro.RCTCAM);
                        cmd.Parameters.AddWithValue("@RCVALV", registro.RCVALV);
                        cmd.Parameters.AddWithValue("@RCCVAL", registro.RCCVAL);
                        cmd.Parameters.AddWithValue("@RCMVAL", registro.RCMVAL);
                        cmd.Parameters.AddWithValue("@RCVALI", registro.RCVALI);
                        cmd.Parameters.AddWithValue("@RCCVAI", registro.RCCVAI);
                        cmd.Parameters.AddWithValue("@RCMVAI", registro.RCMVAI);
                        cmd.Parameters.AddWithValue("@RCDSCT", registro.RCDSCT);
                        cmd.Parameters.AddWithValue("@RCCDSC", registro.RCCDSC);
                        cmd.Parameters.AddWithValue("@RCMDSC", registro.RCMDSC);
                        cmd.Parameters.AddWithValue("@RCIMP1", registro.RCIMP1);
                        cmd.Parameters.AddWithValue("@RCCIM1", registro.RCCIM1);
                        cmd.Parameters.AddWithValue("@RCMIM1", registro.RCMIM1);
                        cmd.Parameters.AddWithValue("@RCPVTA", registro.RCPVTA);
                        cmd.Parameters.AddWithValue("@RCCPVT", registro.RCCPVT);
                        cmd.Parameters.AddWithValue("@RCMPVT", registro.RCMPVT);
                        cmd.Parameters.AddWithValue("@RCCONC", registro.RCCONC);
                        cmd.Parameters.AddWithValue("@RCASTO", registro.RCASTO);
                        cmd.Parameters.AddWithValue("@RCCOST", registro.RCCOST);
                        cmd.Parameters.AddWithValue("@RCTREF", registro.RCTREF);
                        cmd.Parameters.AddWithValue("@RCNREF", registro.RCNREF);
                        cmd.Parameters.AddWithValue("@RCFEVE", registro.RCFEVE);
                        cmd.Parameters.AddWithValue("@RCNDOM", registro.RCNDOM);
                        cmd.Parameters.AddWithValue("@RCCPAG", registro.RCCPAG);
                        cmd.Parameters.AddWithValue("@RCSITU", registro.RCSITU);
                        cmd.Parameters.AddWithValue("@RCUSIN", registro.RCUSIN);
                        cmd.Parameters.AddWithValue("@RCFEIN", registro.RCFEIN);
                        cmd.Parameters.AddWithValue("@RCHOIN", registro.RCHOIN);
                        cmd.Parameters.AddWithValue("@RCRVVA", registro.RCRVVA);
                        cmd.Parameters.AddWithValue("@RCREF7", registro.RCREF7);
                        cmd.Parameters.AddWithValue("@RCCBSA", registro.RCCBSA);
                     }
                }
                rp = true;
            }
            catch (Exception ex)
            {

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
            string Query = @$"select * from {library}.ttido where tdregi='C' AND TDTIPO='{tipo}'";

            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                using (OdbcDataReader reader = (OdbcDataReader)await command.ExecuteReaderAsync())
                {
                    // Verificar si se encontró algún dato
                    if (await reader.ReadAsync())
                    {
                        // Obtener el valor de la primera columna (el resultado de COUNT(1))
                        int count = reader.GetInt32(0); // Obtener el valor de la primera columna
                        rp = count > 0; // Si hay alguna fila, existe el tipo de documento
                    }
                }
            }
            return rp;
        }
    }
}
