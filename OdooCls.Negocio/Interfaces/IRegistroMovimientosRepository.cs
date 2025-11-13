using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroMovimientosRepository
    {
        Task<bool> ExisteMovimiento(int ejercicio, int periodo, string almacen, int comprobante);
        Task<bool> InsertTmovh(RegistroMovimiento movimiento);
        Task<bool> InsertTmovd(RegistroMovimientoDetail detalle);
        Task<bool> InsertTpedh(RegistroPedido pedido);
        Task<bool> InsertTpedd(RegistroPedidoDetail detalle);
        
        // TODO: Agregar métodos para NotaCredito cuando se tenga la info
        // Task<bool> InsertTnch(RegistroNotaCredito notaCredito);
        // Task<bool> InsertTncd(RegistroNotaCreditoDetail detalle);
    }
}
