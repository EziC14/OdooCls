using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System.Data.Odbc;
using System.Data;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroArticulosRepository : IRegistroArticulosRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;

        public RegistroArticulosRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
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

        public async Task<bool> InsertTarti(RegistroArticulo a)
        {
            // TODO: Ajustar columnas exactas de TARTI según BD
            string query = $@"insert into {library}.tarti (ARTCOD, ARTDES, ARTMED, ARTTIP, ARTFAM, ARTSFA, ARCTAC, ARSITU, ARCVTA, ARTMAR)
                               values (?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                
                if (!CallLibreria(cn))
                    return false;
                
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ARTCOD", a.ARTCOD);
                cmd.Parameters.AddWithValue("@ARTDES", a.ARTDES);
                cmd.Parameters.AddWithValue("@ARTMED", a.ARTMED);
                cmd.Parameters.AddWithValue("@ARTTIP", a.ARTTIP);
                cmd.Parameters.AddWithValue("@ARTFAM", a.ARTFAM);
                cmd.Parameters.AddWithValue("@ARTSFA", a.ARTSFA);
                cmd.Parameters.AddWithValue("@ARCTAC", a.ARCTAC);
                cmd.Parameters.AddWithValue("@ARSITU", a.ARSITU);
                cmd.Parameters.AddWithValue("@ARCVTA", a.ARCVTA);
                cmd.Parameters.AddWithValue("@ARTMAR", a.ARTMAR);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        

        public async Task<bool> UpdateDescripcionYSituacion(string artcod, string descripcion, string situacion)
        {
            string query = $@"update {library}.tarti set ARTDES=?, ARSITU=? where ARTCOD=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                
                if (!CallLibreria(cn))
                    return false;
                
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ARTDES", descripcion);
                cmd.Parameters.AddWithValue("@ARSITU", situacion);
                cmd.Parameters.AddWithValue("@ARTCOD", artcod);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteArticulo(string artcod)
        {
            string q = $@"select count(1) from {library}.tarti where ARTCOD=?";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(q, cn);
            await cn.OpenAsync();
            
            if (!CallLibreria(cn))
                return false;
            
            cmd.Parameters.AddWithValue("@ARTCOD", artcod);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<List<RegistroArticulo>> GetAllArticulos(int page, int pageSize)
        {
            int offset = (page - 1) * pageSize;
            string query = $@"select ARTCOD, ARTDES, ARTMED, ARTTIP, ARTFAM, ARTSFA, ARCTAC, ARSITU, ARCVTA, ARTMAR
                              from {library}.tarti
                              order by ARTCOD
                              offset {offset} rows fetch next {pageSize} rows only";
            var result = new List<RegistroArticulo>();
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return result;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new RegistroArticulo
                {
                    ARTCOD = reader["ARTCOD"]?.ToString() ?? string.Empty,
                    ARTDES = reader["ARTDES"]?.ToString() ?? string.Empty,
                    ARTMED = reader["ARTMED"]?.ToString() ?? string.Empty,
                    ARTTIP = reader["ARTTIP"]?.ToString() ?? string.Empty,
                    ARTFAM = reader["ARTFAM"]?.ToString() ?? string.Empty,
                    ARTSFA = reader["ARTSFA"]?.ToString() ?? string.Empty,
                    ARCTAC = reader["ARCTAC"]?.ToString() ?? string.Empty,
                    ARSITU = reader["ARSITU"]?.ToString() ?? string.Empty,
                    ARCVTA = reader["ARCVTA"]?.ToString() ?? string.Empty,
                    ARTMAR = reader["ARTMAR"]?.ToString() ?? string.Empty
                });
            }

            return result;
        }

        public async Task<int> GetTotalArticulosCount()
        {
            string query = $@"select count(*) from {library}.tarti";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return 0;

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }

        public async Task<RegistroArticulo?> GetArticuloById(string artcod)
        {
            string query = $@"select ARTCOD, ARTDES, ARTMED, ARTTIP, ARTFAM, ARTSFA, ARCTAC, ARSITU, ARCVTA, ARTMAR
                              from {library}.tarti where ARTCOD=?";
            using var cn = new OdbcConnection(connectionString);
            using var cmd = new OdbcCommand(query, cn);
            await cn.OpenAsync();

            if (!CallLibreria(cn))
                return null;

            cmd.Parameters.AddWithValue("@ARTCOD", artcod);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new RegistroArticulo
            {
                ARTCOD = reader["ARTCOD"]?.ToString() ?? string.Empty,
                ARTDES = reader["ARTDES"]?.ToString() ?? string.Empty,
                ARTMED = reader["ARTMED"]?.ToString() ?? string.Empty,
                ARTTIP = reader["ARTTIP"]?.ToString() ?? string.Empty,
                ARTFAM = reader["ARTFAM"]?.ToString() ?? string.Empty,
                ARTSFA = reader["ARTSFA"]?.ToString() ?? string.Empty,
                ARCTAC = reader["ARCTAC"]?.ToString() ?? string.Empty,
                ARSITU = reader["ARSITU"]?.ToString() ?? string.Empty,
                ARCVTA = reader["ARCVTA"]?.ToString() ?? string.Empty,
                ARTMAR = reader["ARTMAR"]?.ToString() ?? string.Empty
            };
        }
    }
}
