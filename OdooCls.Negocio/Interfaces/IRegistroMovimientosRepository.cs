using OdooCls.Core.Entities;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroMovimientosRepository
    {
        // Métodos comunes (siempre se usan)
        Task<bool> ExisteMovimiento(int ejercicio, int periodo, string almacen, int comprobante);
        Task<int> ObtenerYActualizarCorrelativo(string almacen, string tipoMovimiento);
        Task<bool> InsertTmovh(RegistroMovimiento movimiento);
        Task<bool> InsertTmovd(RegistroMovimientoDetail detalle);
        
        // Validación de tipo de movimiento contra TTIMA
        Task<bool> ExisteTipoMovimiento(string clase, string tipo);
        
        // Métodos para PEDIDO
        Task<int> ObtenerYActualizarCorrelativoPedido(int puntoVenta);
        Task<bool> InsertTpedh(RegistroPedido pedido);
        Task<bool> InsertTpedd(RegistroPedidoDetail detalle);
        
        // Métodos para NOTA_CREDITO
        Task<int> ObtenerYActualizarCorrelativoNotaCredito(int puntoVenta);
        Task<bool> InsertTncdh(RegistroNotaCredito notaCredito);
        Task<bool> InsertTncdd(RegistroNotaCreditoDetail detalle);
        
        // Métodos para actualización de stock y valorización
        Task<bool> ActualizarStock(string almacen, string articulo, decimal cantidad, string tipoMovimiento);
        Task<bool> ValorizarMovimiento(int ejercicio, int periodo, string almacen, string clase, int comprobante);
        
        // Para INVENTARIO solo se usan InsertTmovh e InsertTmovd
    }
}
