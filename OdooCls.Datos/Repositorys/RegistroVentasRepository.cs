
using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroVentasRepository : IRegistroVentasRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;


        public RegistroVentasRepository(IConfiguration configuration)

        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        public async Task<bool> InsertCtxC(int ejercicio, int mes, string Tipodoc, string nrodoc)
        {
            bool rp = false;

            string query = $@" INSERT INTO   {library}.TCTXC 
            SELECT RVEJER, RVPERI, RVTDOC, RVNDOC, RVFECH, RVFEVE, RVCCLI, RVMONE, RVTCAM, RVCPAG, RVPVTA, 0 ,RVPVTA , '02',
            CASE WHEN RVMONE =0 THEN RVPVTA ELSE round((RVPVTA*RVTCAM), 2) END, 0 ,RVTCAM,
            CASE WHEN RVMONE =0 THEN round((RVPVTA/RVTCAM), 2)  ELSE RVPVTA END, 0 , '', '', '',
            RVCCOB, RVCVEN, '', '', '', '', '', '',RVACTI,RVTGAS,RVCPVT,RVCOST,'','','', 0 , 0 
            FROM  {library}.TREGV WHERE   rvejer={ejercicio} and rvperi={mes} AND RVTDOC='{Tipodoc}' AND RVNDOC IN ('{nrodoc}')";
            try
            {
                using OdbcConnection cn = new OdbcConnection(connectionString);
                {
                    using OdbcCommand cmd = new OdbcCommand(query, cn);
                    {
                      await  cn.OpenAsync();
                        cmd.CommandType = CommandType.Text;
                      await  cmd.ExecuteNonQueryAsync();
                        cn.Close();
                    }
                }
                rp = true;
            }
            catch (Exception ex)
            {// Log de error si es necesario
                Console.WriteLine($"Error: {ex.Message}");
                rp = false;
            }
           return rp;
        }

        public async Task<bool> InsertTregv(RegistroVentas registroVentas)
        {
            bool rp = false;
            string Query = $@"insert into {library}.tregv
                           RVEJER, RVPERI, RVTDOC, RVNDOC, RVFECH, RVCCLI, RVCLIE,  RVMONE, RVTCAM, RVVALV, RVCVAL, RVMVAL, RVVALI, RVCVAI, RVMVAI, RVDSCT, RVCDSC, RVMDSC, RVIGV, RVCIGV, RVMIGV, RVIMP2, RVCIM2,  
                           RVMIM2, RVIMP3, RVCIM3, RVMIM3, RVRET1, RVCRE1, RVMRE1, RVRET2, RVCRE2, RVMRE2, RVPVTA, RVCPVT, RVMPVT, RVCONC, RVTREF, RVNREF, RVASTO, RVGRAB, RVFPRO, RVHPRO, RVFEVE, RVNDOM, RVCPAG,
                           RVRUC,RVSITU, RVCOST, RVCVEN, RVCCOB, RVACTI, RVTGAS, RVBANC, RVNBCO, RVUSIN, RVFEIN, RVHOIN, RVUSMD, RVFEMD, RVHOMD, RVREF1, RVREF2, RVREF3, RVREF4, RVREF5, RVHASH, RVSUNA, RVGLOS       
                           values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?) ";

            try
            {
              using OdbcConnection cn = new OdbcConnection(connectionString);
                {
                    using OdbcCommand cmd = new OdbcCommand(Query, cn);
                    {
                        cn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@RVEJER", registroVentas.RVEJER);
                        cmd.Parameters.AddWithValue("@RVPERI", registroVentas.RVPERI);
                        cmd.Parameters.AddWithValue("@RVTDOC", registroVentas.RVTDOC);
                        cmd.Parameters.AddWithValue("@RVNDOC", registroVentas.RVNDOC);
                        cmd.Parameters.AddWithValue("@RVFECH", registroVentas.RVFECH);
                        cmd.Parameters.AddWithValue("@RVCCLI", registroVentas.RVCCLI);
                        cmd.Parameters.AddWithValue("@RVCLIE", registroVentas.RVCLIE);
                        cmd.Parameters.AddWithValue("@RVMONE", registroVentas.RVMONE);
                        cmd.Parameters.AddWithValue("@RVTCAM", registroVentas.RVTCAM);
                        cmd.Parameters.AddWithValue("@RVVALV", registroVentas.RVVALV);
                        cmd.Parameters.AddWithValue("@RVCVAL", registroVentas.RVCVAL);
                        cmd.Parameters.AddWithValue("@RVMVAL", registroVentas.RVMVAL);
                        cmd.Parameters.AddWithValue("@RVVALI", registroVentas.RVVALI);
                        cmd.Parameters.AddWithValue("@RVCVAI", registroVentas.RVCVAI);
                        cmd.Parameters.AddWithValue("@RVMVAI", registroVentas.RVMVAI);
                        cmd.Parameters.AddWithValue("@RVDSCT", registroVentas.RVDSCT);
                        cmd.Parameters.AddWithValue("@RVCDSC", registroVentas.RVCDSC);
                        cmd.Parameters.AddWithValue("@RVMDSC", registroVentas.RVMDSC);
                        cmd.Parameters.AddWithValue("@RVIGV", registroVentas.RVIGV);
                        cmd.Parameters.AddWithValue("@RVCIGV", registroVentas.RVCIGV);
                        cmd.Parameters.AddWithValue("@RVMIGV", registroVentas.RVMIGV);
                        cmd.Parameters.AddWithValue("@RVIMP2", registroVentas.RVIMP2);
                        cmd.Parameters.AddWithValue("@RVCIM2", registroVentas.RVCIM2);
                        cmd.Parameters.AddWithValue("@RVMIM2", registroVentas.RVMIM2);
                        cmd.Parameters.AddWithValue("@RVIMP3", registroVentas.RVIMP3);
                        cmd.Parameters.AddWithValue("@RVCIM3", registroVentas.RVCIM3);
                        cmd.Parameters.AddWithValue("@RVMIM3", registroVentas.RVMIM3);
                        cmd.Parameters.AddWithValue("@RVRET1", registroVentas.RVRET1);
                        cmd.Parameters.AddWithValue("@RVCRE1", registroVentas.RVCRE1);
                        cmd.Parameters.AddWithValue("@RVMRE1", registroVentas.RVMRE1);
                        cmd.Parameters.AddWithValue("@RVRET2", registroVentas.RVRET2);
                        cmd.Parameters.AddWithValue("@RVCRE2", registroVentas.RVCRE2);
                        cmd.Parameters.AddWithValue("@RVMRE2", registroVentas.RVMRE2);
                        cmd.Parameters.AddWithValue("@RVPVTA", registroVentas.RVPVTA);
                        cmd.Parameters.AddWithValue("@RVCPVT", registroVentas.RVCPVT);
                        cmd.Parameters.AddWithValue("@RVMPVT", registroVentas.RVMPVT);
                        cmd.Parameters.AddWithValue("@RVCONC", registroVentas.RVCONC);
                        cmd.Parameters.AddWithValue("@RVTREF", registroVentas.RVTREF);
                        cmd.Parameters.AddWithValue("@RVNREF", registroVentas.RVNREF);
                        cmd.Parameters.AddWithValue("@RVASTO", registroVentas.RVASTO);
                        cmd.Parameters.AddWithValue("@RVGRAB", registroVentas.RVGRAB);
                        cmd.Parameters.AddWithValue("@RVFPRO", registroVentas.RVFPRO);
                        cmd.Parameters.AddWithValue("@RVHPRO", registroVentas.RVHPRO);
                        cmd.Parameters.AddWithValue("@RVFEVE", registroVentas.RVFEVE);
                        cmd.Parameters.AddWithValue("@RVNDOM", registroVentas.RVNDOM);
                        cmd.Parameters.AddWithValue("@RVCPAG", registroVentas.RVCPAG);
                        cmd.Parameters.AddWithValue("@RVRUC", registroVentas.RVRUC);
                        cmd.Parameters.AddWithValue("@RVSITU", registroVentas.RVSITU);
                        cmd.Parameters.AddWithValue("@RVCOST", registroVentas.RVCOST);
                        cmd.Parameters.AddWithValue("@RVCVEN", registroVentas.RVCVEN);
                        cmd.Parameters.AddWithValue("@RVCCOB", registroVentas.RVCCOB);
                        cmd.Parameters.AddWithValue("@RVACTI", registroVentas.RVACTI);
                        cmd.Parameters.AddWithValue("@RVTGAS", registroVentas.RVTGAS);
                        cmd.Parameters.AddWithValue("@RVBANC", registroVentas.RVBANC);
                        cmd.Parameters.AddWithValue("@RVNBCO", registroVentas.RVNBCO);
                        cmd.Parameters.AddWithValue("@RVUSIN", registroVentas.RVUSIN);
                        cmd.Parameters.AddWithValue("@RVFEIN", registroVentas.RVFEIN);
                        cmd.Parameters.AddWithValue("@RVHOIN", registroVentas.RVHOIN);
                        cmd.Parameters.AddWithValue("@RVUSMD", registroVentas.RVUSMD);
                        cmd.Parameters.AddWithValue("@RVFEMD", registroVentas.RVFEMD);
                        cmd.Parameters.AddWithValue("@RVHOMD", registroVentas.RVHOMD);
                        cmd.Parameters.AddWithValue("@RVREF1", registroVentas.RVREF1);
                        cmd.Parameters.AddWithValue("@RVREF2", registroVentas.RVREF2);
                        cmd.Parameters.AddWithValue("@RVREF3", registroVentas.RVREF3);
                        cmd.Parameters.AddWithValue("@RVREF4", registroVentas.RVREF4);
                        cmd.Parameters.AddWithValue("@RVREF5", registroVentas.RVREF5);
                        cmd.Parameters.AddWithValue("@RVHASH", registroVentas.RVHASH);
                        cmd.Parameters.AddWithValue("@RVSUNA", registroVentas.RVSUNA);
                        cmd.Parameters.AddWithValue("@RVGLOS", registroVentas.RVGLOS);
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        foreach (var item in registroVentas.RegistroVentasDetail)
                        {
                         await InsertTregvD(item);
                        }
                    }
                }
                rp = true;
            }
            catch (Exception ex)
            {
                // Log de error si es necesario
                Console.WriteLine($"Error: {ex.Message}");
                rp = false;  // Si hay un error, retornamos false
            }

            return rp;  
        }

        public async Task<bool> InsertTregvD(RegistroVentasDetail registro)
        {
            bool rp = false;
            string Query = $@"Insert into {library}.tregv
                           RVEJER, RVPERI, RVTDOC, RVNDOC, RVSECU, RVDCTA, RVDCCO,         
                           RVDIMP, RVDACT, RVDTGA, RDTIAX, RDCOAX, RDRFAX, RDRFA1, RDRFA2, 
                           RDRFA3, RDRFA4, RDRFA5 values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?) ";
            try
            {
                   using OdbcConnection cn = new OdbcConnection(connectionString);
                            {
               

                                using OdbcCommand cmd = new OdbcCommand(Query, cn);
                                {
                                    await cn.OpenAsync();
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@RVEJER", registro.RVEJER);
                                    cmd.Parameters.AddWithValue("@RVPERI", registro.RVPERI);
                                    cmd.Parameters.AddWithValue("@RVTDOC", registro.RVTDOC);
                                    cmd.Parameters.AddWithValue("@RVNDOC", registro.RVNDOC);
                                    cmd.Parameters.AddWithValue("@RVSECU", registro.RVSECU);
                                    cmd.Parameters.AddWithValue("@RVDCTA", registro.RVDCTA);
                                    cmd.Parameters.AddWithValue("@RVDCCO", registro.RVDCCO);
                                    cmd.Parameters.AddWithValue("@RVDIMP", registro.RVDIMP);
                                    cmd.Parameters.AddWithValue("@RVDACT", registro.RVDACT);
                                    cmd.Parameters.AddWithValue("@RVDTGA", registro.RVDTGA);
                                    cmd.Parameters.AddWithValue("@RDTIAX", registro.RDTIAX);
                                    cmd.Parameters.AddWithValue("@RDCOAX", registro.RDCOAX);
                                    cmd.Parameters.AddWithValue("@RDRFAX", registro.RDRFAX);
                                    cmd.Parameters.AddWithValue("@RDRFA1", registro.RDRFA1);
                                    cmd.Parameters.AddWithValue("@RDRFA2", registro.RDRFA2);
                                    cmd.Parameters.AddWithValue("@RDRFA3", registro.RDRFA3);
                                    cmd.Parameters.AddWithValue("@RDRFA4", registro.RDRFA4);
                                    cmd.Parameters.AddWithValue("@RDRFA5", registro.RDRFA5);
                                    await cmd.ExecuteNonQueryAsync();
                                    cn.Close();
                                        
                                    rp = true;  // Si todo va bien, retornamos true
                                 }
                            }
            }
            catch (Exception ex)
            {

                // Log de error si es necesario
                Console.WriteLine($"Error: {ex.Message}");
                rp = false;  // Si hay un error, retornamos false
            }
            return rp;
        }

        public async Task<bool> ValidaCliente(string codclie)
        {
            bool rp = false;
            string Query = @$"select count(*) from {library}.tclie where clicve='{codclie}'";

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

        public async Task<bool> ValidarExistenciaDocumento(int ejercicio, int mes, string Tipodoc, string nrodoc) 
        {
            bool rp = false;
            var Query = $"select * from {library}.tregv where RVEJER={ejercicio} and RVPERI={mes} and RVTDOC='{Tipodoc}' AND RVNDOC='{nrodoc}'";

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

        public async Task<bool> ValidarStatusRV(int ejercicio, int mes, string stconta)
        {
            bool rp = false;
            string vc="", vrv="";
            string Query = @$"select persit,persrv from {library}.tperc where perano={ejercicio} and pernum={mes}";
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
                        vrv= reader.GetString(1); 
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

        /// <summary>
        /// Valida que el tipo de documento Exista, false que no se encuentra
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns> </returns>
        public async Task<bool> ValidatTipoDoc(string tipo)
        {
            bool rp=false;
            string Query = @$"select * from {library}.ttido where tdregi='V' AND TDTIPO='{tipo}'";

            using (var connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(Query, connection);
                await connection.OpenAsync();
                using (OdbcDataReader reader = (OdbcDataReader) await command.ExecuteReaderAsync())
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
