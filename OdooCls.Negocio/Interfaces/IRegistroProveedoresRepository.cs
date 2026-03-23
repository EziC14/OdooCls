using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroProveedoresRepository
    {
        Task<bool> InsertTprov(RegistroProveedor proveedor);
        Task<bool> UpdateNombreYSituacion(string procve, string nombre, string situacion);
        Task<bool> ExisteProveedor(string procve);
        Task<bool> ExisteRuc(string ruc);
        Task<List<RegistroProveedor>> GetAllProveedores(int page, int pageSize);
        Task<int> GetTotalProveedoresCount();
        Task<RegistroProveedor?> GetProveedorById(string procve);
    }
}
