using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroMovimientosRepository
    {
        // Métodos comunes (siempre se usan)
        Task<bool> ExisteMovimiento(int ejercicio, int periodo, string almacen, int comprobante);
        Task<bool> InsertTmovh(RegistroMovimiento movimiento);
        Task<bool> InsertTmovd(RegistroMovimientoDetail detalle);
        
        // Métodos para PEDIDO
        Task<bool> InsertTpedh(RegistroPedido pedido);
        Task<bool> InsertTpedd(RegistroPedidoDetail detalle);
        
        // Métodos para NOTA_CREDITO
        Task<bool> InsertTncdh(RegistroNotaCredito notaCredito);
        Task<bool> InsertTncdd(RegistroNotaCreditoDetail detalle);
        
        // Para INVENTARIO solo se usan InsertTmovh e InsertTmovd
    }
}
