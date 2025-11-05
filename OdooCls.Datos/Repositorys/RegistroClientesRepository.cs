using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System.Data;
using System.Data.Odbc;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroClientesRepository : IRegistroClientesRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;

        public RegistroClientesRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        public async Task<bool> InsertTclie(RegistroCliente c)
        {
            string query = $@"insert into {library}.tclie 
                (CLICVE, CLINOM, CLIDIR, CLICPO, CLIDIS, CLIPRO, CLIDPT, CLIPAI, CLIRUC, CLISIT, CLILCR, CPACVE)
                values (?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CLICVE", c.CLICVE);
                cmd.Parameters.AddWithValue("@CLINOM", c.CLINOM);
                cmd.Parameters.AddWithValue("@CLIDIR", c.CLIDIR);
                cmd.Parameters.AddWithValue("@CLICPO", c.CLICPO);
                cmd.Parameters.AddWithValue("@CLIDIS", c.CLIDIS);
                cmd.Parameters.AddWithValue("@CLIPRO", c.CLIPRO);
                cmd.Parameters.AddWithValue("@CLIDPT", c.CLIDPT);
                cmd.Parameters.AddWithValue("@CLIPAI", c.CLIPAI);
                cmd.Parameters.AddWithValue("@CLIRUC", c.CLIRUC);
                cmd.Parameters.AddWithValue("@CLISIT", c.CLISIT);
                cmd.Parameters.AddWithValue("@CLILCR", c.CLILCR);
                cmd.Parameters.AddWithValue("@CPACVE", c.CPACVE);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateNombreYSituacion(string clicve, string nombre, string situacion)
        {
            string query = $@"update {library}.tclie set CLINOM=?, CLISIT=? where CLICVE=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@CLINOM", nombre);
                cmd.Parameters.AddWithValue("@CLISIT", situacion);
                cmd.Parameters.AddWithValue("@CLICVE", clicve);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteCliente(string clicve)
        {
            string q = $@"select count(1) from {library}.tclie where CLICVE=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                await cn.OpenAsync();
                cmd.Parameters.AddWithValue("@CLICVE", clicve);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
