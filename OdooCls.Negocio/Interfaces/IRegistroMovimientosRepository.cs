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
        
        // Métodos para transferencias entre almacenes
        bool EsTransferencia(string tipoMovimiento);
        Task<bool> ActualizarStockTransito(string almacen, string articulo, decimal cantidad, string operacion);
        Task<bool> InsertTmotr(string almacenOrigen, int ejercicio, int periodo, string claseOrigen, int valeOrigen, int correlativo,
            string tipoMovimiento, string tipo, string almacenDestino, string claseDestino, int valeDestino);
        Task<bool> ConfirmarRecepcionTransferencia(string almacenDestino, int ejercicio, int periodo, int valeIngreso);
        
        // Para INVENTARIO solo se usan InsertTmovh e InsertTmovd
    }
}
