using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroAlmacenesRepository
    {
        Task<bool> InsertTalma(RegistroAlmacen almacen);
        Task<bool> UpdateNombreYSituacion(string alcodi, string nombre, string situacion);
        Task<bool> ExisteAlmacen(string alcodi);
    }
}
