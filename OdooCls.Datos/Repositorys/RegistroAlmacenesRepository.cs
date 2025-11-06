using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System.Data;
using System.Data.Odbc;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroAlmacenesRepository : IRegistroAlmacenesRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;

        public RegistroAlmacenesRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        public async Task<bool> InsertTalma(RegistroAlmacen a)
        {
            string query = $@"insert into {library}.talma 
                (ALCODI, ALNOMB, ALRESP, ALVALO, ALSITU, ALINGR, ALSALI, ALTRAN, ALDIRE, ALCANT, ALDISD, ALUBGD, ALCPLD, ALREF1, ALREF2, ALFLG1, ALFLG2)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ALCODI", a.ALCODI);
                cmd.Parameters.AddWithValue("@ALNOMB", a.ALNOMB);
                cmd.Parameters.AddWithValue("@ALRESP", a.ALRESP);
                cmd.Parameters.AddWithValue("@ALVALO", a.ALVALO);
                cmd.Parameters.AddWithValue("@ALSITU", a.ALSITU);
                cmd.Parameters.AddWithValue("@ALINGR", a.ALINGR);
                cmd.Parameters.AddWithValue("@ALSALI", a.ALSALI);
                cmd.Parameters.AddWithValue("@ALTRAN", a.ALTRAN);
                cmd.Parameters.AddWithValue("@ALDIRE", a.ALDIRE);
                cmd.Parameters.AddWithValue("@ALCANT", a.ALCANT);
                cmd.Parameters.AddWithValue("@ALDISD", a.ALDISD);
                cmd.Parameters.AddWithValue("@ALUBGD", a.ALUBGD);
                cmd.Parameters.AddWithValue("@ALCPLD", a.ALCPLD);
                cmd.Parameters.AddWithValue("@ALREF1", a.ALREF1);
                cmd.Parameters.AddWithValue("@ALREF2", a.ALREF2);
                cmd.Parameters.AddWithValue("@ALFLG1", a.ALFLG1);
                cmd.Parameters.AddWithValue("@ALFLG2", a.ALFLG2);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateNombreYSituacion(string alcodi, string nombre, string situacion)
        {
            string query = $@"update {library}.talma set ALNOMB=?, ALSITU=? where ALCODI=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ALNOMB", nombre);
                cmd.Parameters.AddWithValue("@ALSITU", situacion);
                cmd.Parameters.AddWithValue("@ALCODI", alcodi);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExisteAlmacen(string alcodi)
        {
            string q = $@"select count(1) from {library}.talma where ALCODI=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                await cn.OpenAsync();
                cmd.Parameters.AddWithValue("@ALCODI", alcodi);
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
