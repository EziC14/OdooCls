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
        string? companyCode;
        string? connectionString;

        public RegistroProveedoresRepository(IConfiguration configuration)
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

        public async Task<bool> InsertTprov(RegistroProveedor p)
        {
            string query = $@"insert into {library}.tprov 
                (PROCVE, PRONOM, PRODIR, PROCPO, PRODIS, PROPRO, PRODPT, PROPAI, PRORUC, PROSIT, PRORF1, PROARE, CPACVE)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                
                if (!CallLibreria(cn))
                    return false;
                
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
                cmd.Parameters.AddWithValue("@PRORF1", p.PRORF1);
                cmd.Parameters.AddWithValue("@PROARE", p.PROARE);
                cmd.Parameters.AddWithValue("@CPACVE", p.CPACVE);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Repository.InsertTprov] ERROR: {ex.Message}");
                throw;
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
                
                if (!CallLibreria(cn))
                    return false;
                
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
                
                if (!CallLibreria(cn))
                    return false;
                
                cmd.Parameters.AddWithValue("@PROCVE", procve);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteRuc(string ruc)
        {
            string q = $@"select count(1) from {library}.tprov where PRORUC=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                cmd.CommandTimeout = 5;
                await cn.OpenAsync();
                
                if (!CallLibreria(cn))
                    throw new Exception("Error al configurar bibliotecas AS400");
                
                cmd.Parameters.AddWithValue("@PRORUC", ruc);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Repository.ExisteRuc] ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task<List<RegistroProveedor>> GetAllProveedores(int page, int pageSize)
        {
            int offset = (page - 1) * pageSize;
            string query = $@"select PROCVE, PRONOM, PRODIR, PROCPO, PRODIS, PROPRO, PRODPT, PROPAI, PRORUC, PROSIT, PRORF1, PROARE, CPACVE
                              from {library}.tprov
                              order by PROCVE
                              offset {offset} rows fetch next {pageSize} rows only";
            var result = new List<RegistroProveedor>();
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return result;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new RegistroProveedor
                {
                    PROCVE = reader["PROCVE"]?.ToString() ?? string.Empty,
                    PRONOM = reader["PRONOM"]?.ToString() ?? string.Empty,
                    PRODIR = reader["PRODIR"]?.ToString() ?? string.Empty,
                    PROCPO = reader["PROCPO"]?.ToString() ?? string.Empty,
                    PRODIS = reader["PRODIS"]?.ToString() ?? string.Empty,
                    PROPRO = reader["PROPRO"]?.ToString() ?? string.Empty,
                    PRODPT = reader["PRODPT"]?.ToString() ?? string.Empty,
                    PROPAI = reader["PROPAI"]?.ToString() ?? string.Empty,
                    PRORUC = reader["PRORUC"]?.ToString() ?? string.Empty,
                    PROSIT = reader["PROSIT"]?.ToString() ?? string.Empty,
                    PRORF1 = reader["PRORF1"]?.ToString() ?? string.Empty,
                    PROARE = reader["PROARE"]?.ToString() ?? string.Empty,
                    CPACVE = reader["CPACVE"]?.ToString() ?? string.Empty
                });
            }

            return result;
        }

        public async Task<int> GetTotalProveedoresCount()
        {
            string query = $@"select count(*) from {library}.tprov";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return 0;

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<RegistroProveedor?> GetProveedorById(string procve)
        {
            string query = $@"select PROCVE, PRONOM, PRODIR, PROCPO, PRODIS, PROPRO, PRODPT, PROPAI, PRORUC, PROSIT, PRORF1, PROARE, CPACVE
                              from {library}.tprov where PROCVE=?";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return null;

            cmd.Parameters.AddWithValue("@PROCVE", procve);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new RegistroProveedor
            {
                PROCVE = reader["PROCVE"]?.ToString() ?? string.Empty,
                PRONOM = reader["PRONOM"]?.ToString() ?? string.Empty,
                PRODIR = reader["PRODIR"]?.ToString() ?? string.Empty,
                PROCPO = reader["PROCPO"]?.ToString() ?? string.Empty,
                PRODIS = reader["PRODIS"]?.ToString() ?? string.Empty,
                PROPRO = reader["PROPRO"]?.ToString() ?? string.Empty,
                PRODPT = reader["PRODPT"]?.ToString() ?? string.Empty,
                PROPAI = reader["PROPAI"]?.ToString() ?? string.Empty,
                PRORUC = reader["PRORUC"]?.ToString() ?? string.Empty,
                PROSIT = reader["PROSIT"]?.ToString() ?? string.Empty,
                PRORF1 = reader["PRORF1"]?.ToString() ?? string.Empty,
                PROARE = reader["PROARE"]?.ToString() ?? string.Empty,
                CPACVE = reader["CPACVE"]?.ToString() ?? string.Empty
            };
        }
    }
}
