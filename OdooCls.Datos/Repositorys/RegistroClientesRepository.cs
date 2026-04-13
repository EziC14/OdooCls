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
        string? companyCode;
        string? connectionString;

        public RegistroClientesRepository(IConfiguration configuration)
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
                
                if (!CallLibreria(cn))
                    return false;
                
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
            catch (Exception ex)
            {
                Console.WriteLine($"[Repository.InsertTclie] ERROR: {ex.Message}");
                throw;
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
                
                if (!CallLibreria(cn))
                    return false;
                
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
                
                if (!CallLibreria(cn))
                    return false;
                
                cmd.Parameters.AddWithValue("@CLICVE", clicve);
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
            string q = $@"select count(1) from {library}.tclie where CLIRUC=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                cmd.CommandTimeout = 5;
                await cn.OpenAsync();
                
                if (!CallLibreria(cn))
                    throw new Exception("Error al configurar bibliotecas AS400");
                
                cmd.Parameters.AddWithValue("@CLIRUC", ruc);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Repository.ExisteRuc] ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task<List<RegistroCliente>> GetAllClientes(int page, int pageSize)
        {
            int offset = (page - 1) * pageSize;
            string query = $@"select CLICVE, CLINOM, CLIDIR, CLICPO, CLIDIS, CLIPRO, CLIDPT, CLIPAI, CLIRUC, CLISIT, CLILCR, CPACVE
                              from {library}.tclie
                              order by CLICVE
                              offset {offset} rows fetch next {pageSize} rows only";
            var result = new List<RegistroCliente>();
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return result;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new RegistroCliente
                {
                    CLICVE = reader["CLICVE"]?.ToString() ?? string.Empty,
                    CLINOM = reader["CLINOM"]?.ToString() ?? string.Empty,
                    CLIDIR = reader["CLIDIR"]?.ToString() ?? string.Empty,
                    CLICPO = reader["CLICPO"]?.ToString() ?? string.Empty,
                    CLIDIS = reader["CLIDIS"]?.ToString() ?? string.Empty,
                    CLIPRO = reader["CLIPRO"]?.ToString() ?? string.Empty,
                    CLIDPT = reader["CLIDPT"]?.ToString() ?? string.Empty,
                    CLIPAI = reader["CLIPAI"]?.ToString() ?? string.Empty,
                    CLIRUC = reader["CLIRUC"]?.ToString() ?? string.Empty,
                    CLISIT = reader["CLISIT"]?.ToString() ?? string.Empty,
                    CLILCR = reader["CLILCR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CLILCR"]),
                    CPACVE = reader["CPACVE"]?.ToString() ?? string.Empty
                });
            }

            return result;
        }

        public async Task<int> GetTotalClientesCount()
        {
            string query = $@"select count(*) from {library}.tclie";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return 0;

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<RegistroCliente?> GetClienteById(string clicve)
        {
            string query = $@"select CLICVE, CLINOM, CLIDIR, CLICPO, CLIDIS, CLIPRO, CLIDPT, CLIPAI, CLIRUC, CLISIT, CLILCR, CPACVE
                              from {library}.tclie where CLICVE=?";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return null;

            cmd.Parameters.AddWithValue("@CLICVE", clicve);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new RegistroCliente
            {
                CLICVE = reader["CLICVE"]?.ToString() ?? string.Empty,
                CLINOM = reader["CLINOM"]?.ToString() ?? string.Empty,
                CLIDIR = reader["CLIDIR"]?.ToString() ?? string.Empty,
                CLICPO = reader["CLICPO"]?.ToString() ?? string.Empty,
                CLIDIS = reader["CLIDIS"]?.ToString() ?? string.Empty,
                CLIPRO = reader["CLIPRO"]?.ToString() ?? string.Empty,
                CLIDPT = reader["CLIDPT"]?.ToString() ?? string.Empty,
                CLIPAI = reader["CLIPAI"]?.ToString() ?? string.Empty,
                CLIRUC = reader["CLIRUC"]?.ToString() ?? string.Empty,
                CLISIT = reader["CLISIT"]?.ToString() ?? string.Empty,
                CLILCR = reader["CLILCR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CLILCR"]),
                CPACVE = reader["CPACVE"]?.ToString() ?? string.Empty
            };
        }
    }
}
