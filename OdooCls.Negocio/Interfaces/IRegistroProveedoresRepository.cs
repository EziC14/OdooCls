using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroProveedoresRepository
    {
        Task<bool> InsertTprov(RegistroProveedor proveedor);
        Task<bool> UpdateNombreYSituacion(string procve, string nombre, string situacion);
        Task<bool> ExisteProveedor(string procve);
    }
}
