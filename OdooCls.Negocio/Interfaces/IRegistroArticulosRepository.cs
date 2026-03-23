using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroArticulosRepository
    {
        Task<bool> InsertTarti(RegistroArticulo articulo);
        Task<bool> UpdateDescripcionYSituacion(string artcod, string descripcion, string situacion);
        Task<bool> ExisteArticulo(string artcod);
        Task<List<RegistroArticulo>> GetAllArticulos(int page, int pageSize);
        Task<int> GetTotalArticulosCount();
        Task<RegistroArticulo?> GetArticuloById(string artcod);

    }
}
