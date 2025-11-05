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
            cmd.Parameters.AddWithValue("@ARTCOD", artcod);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
    }
}
