using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System.Data;
using System.Data.Odbc;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroProveedoresRepository : IRegistroProveedoresRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;

        public RegistroProveedoresRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        public async Task<bool> InsertTprov(RegistroProveedor p)
        {
            string query = $@"insert into {library}.tprov 
                (PROCVE, PRONOM, PRODIR, PROCPO, PRODIS, PROPRO, PRODPT, PROPAI, PRORUC, PROSIT, CPACVE)
                values (?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PROCVE", p.PROCVE);
                cmd.Parameters.AddWithValue("@PRONOM", p.PRONOM);
                cmd.Parameters.AddWithValue("@PRODIR", p.PRODIR);
                cmd.Parameters.AddWithValue("@PROCPO", p.PROCPO);
                cmd.Parameters.AddWithValue("@PRODIS", p.PRODIS);
                cmd.Parameters.AddWithValue("@PROPRO", p.PROPRO);
                cmd.Parameters.AddWithValue("@PRODPT", p.PRODPT);
                cmd.Parameters.AddWithValue("@PROPAI", p.PROPAI);
                cmd.Parameters.AddWithValue("@PRORUC", p.PRORUC);
                cmd.Parameters.AddWithValue("@PROSIT", p.PROSIT);
                cmd.Parameters.AddWithValue("@CPACVE", p.CPACVE);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateNombreYSituacion(string procve, string nombre, string situacion)
        {
            string query = $@"update {library}.tprov set PRONOM=?, PROSIT=? where PROCVE=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PRONOM", nombre);
                cmd.Parameters.AddWithValue("@PROSIT", situacion);
                cmd.Parameters.AddWithValue("@PROCVE", procve);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteProveedor(string procve)
        {
            string q = $@"select count(1) from {library}.tprov where PROCVE=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                await cn.OpenAsync();
                cmd.Parameters.AddWithValue("@PROCVE", procve);
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
