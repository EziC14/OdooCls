using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroClientesRepository
    {
        Task<bool> InsertTclie(RegistroCliente cliente);
        Task<bool> UpdateNombreYSituacion(string clicve, string nombre, string situacion);
        Task<bool> ExisteCliente(string clicve);
    }
}
