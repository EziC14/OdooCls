using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroClientesRepository
    {
        Task<bool> InsertTclie(RegistroCliente cliente);
        Task<bool> UpdateNombreYSituacion(string clicve, string nombre, string situacion);
        Task<bool> ExisteCliente(string clicve);
        Task<bool> ExisteRuc(string ruc);
        Task<List<RegistroCliente>> GetAllClientes(int page, int pageSize);
        Task<int> GetTotalClientesCount();
        Task<RegistroCliente?> GetClienteById(string clicve);
    }
}
