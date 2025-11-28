using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroMovimientosServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroMovimientosRepository repo;

        public RegistroMovimientosServices(IConfiguration configuration, IRegistroMovimientosRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroMovimientosDto>> CreateAsync(RegistroMovimientosDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6001, "No se recibieron datos en el archivo");

                // VALIDAR CONSISTENCIA SEGÚN EL TIPO
                var validacion = await ValidarConsistenciaAsync(dto);
                if (validacion.HttpStatusCode != 200)
                    return validacion;

                // AUTO-GENERAR NÚMERO DE VALE (MHCOMP) DESDE TALMA
                int nuevoVale = await repo.ObtenerYActualizarCorrelativo(dto.Movimiento.MHALMA, dto.Movimiento.MHCMOV);
                if (nuevoVale == 0)
                    return new ApiResponse<RegistroMovimientosDto>(500, 6001, 
                        $"Error al generar número de vale desde TALMA para almacén {dto.Movimiento.MHALMA}");
                
                // Asignar el vale generado al movimiento y a todos los detalles
                dto.Movimiento.MHCOMP = nuevoVale;
                foreach (var detalle in dto.MovimientoDetails)
                {
                    detalle.MDCOMP = nuevoVale;
                }

                // AUTO-GENERAR CORRELATIVOS DE PEDIDO O NOTA DE CRÉDITO SEGÚN EL TIPO
                if (dto.Tipo == TipoMovimiento.PEDIDO && dto.Pedido != null)
                {
                    int puntoVenta = dto.Pedido.PHPVTA;
                    int nuevoPedido = await repo.ObtenerYActualizarCorrelativoPedido(puntoVenta);
                    if (nuevoPedido == 0)
                        return new ApiResponse<RegistroMovimientosDto>(500, 6002, 
                            $"Error al generar número de pedido desde TPTOV para punto de venta {puntoVenta}");
                    
                    // Asignar el número de pedido generado
                    dto.Pedido.PHNUME = nuevoPedido;
                    
                    // Asignar también a todos los detalles de pedido
                    if (dto.PedidoDetails != null)
                    {
                        foreach (var detalle in dto.PedidoDetails)
                        {
                            detalle.PDNUME = nuevoPedido;
                        }
                    }
                }
                else if (dto.Tipo == TipoMovimiento.NOTA_CREDITO && dto.NotaCredito != null)
                {
                    int puntoVenta = dto.NotaCredito.NHPVTA;
                    int nuevaNC = await repo.ObtenerYActualizarCorrelativoNotaCredito(puntoVenta);
                    if (nuevaNC == 0)
                        return new ApiResponse<RegistroMovimientosDto>(500, 6002, 
                            $"Error al generar número de nota de crédito desde TPTOV para punto de venta {puntoVenta}");
                    
                    // Asignar el número de NC generado
                    dto.NotaCredito.NHNUME = nuevaNC;
                    
                    // Asignar también a todos los detalles de NC
                    if (dto.NotaCreditoDetails != null)
                    {
                        foreach (var detalle in dto.NotaCreditoDetails)
                        {
                            detalle.NCNUME = nuevaNC;
                        }
                    }
                }

                // ASIGNAR REFERENCIAS AUTOMÁTICAS SEGÚN EL TIPO (ANTES DE INSERTAR)
                AsignarReferencias(dto);

                // 1. INSERT TMOVH (Header Movimiento) - SIEMPRE
                var movimientoHeader = RegistroMovimientosMapper.MovimientoHeaderToEntity(dto.Movimiento);
                if (!await repo.InsertTmovh(movimientoHeader))
                    return new ApiResponse<RegistroMovimientosDto>(500, 6003, "Error al insertar TMOVH (Movimiento Header)");

                // 2. INSERT TMOVD (Detalles Movimiento) Y ACTUALIZAR STOCK - SIEMPRE
                foreach (var detalle in dto.MovimientoDetails)
                {
                    // 2a. Insertar detalle de movimiento
                    var movimientoDetail = RegistroMovimientosMapper.MovimientoDetailToEntity(detalle);
                    if (!await repo.InsertTmovd(movimientoDetail))
                        return new ApiResponse<RegistroMovimientosDto>(500, 6004, 
                            $"Error al insertar TMOVD (Movimiento Detail) para artículo {detalle.MDCOAR}");

                    // 2b. ⭐ ACTUALIZAR STOCK en TSALM inmediatamente
                    var stockOk = await repo.ActualizarStock(
                        detalle.MDALMA,
                        detalle.MDCOAR,
                        detalle.MDCANA,
                        detalle.MDCMOV
                    );
                    
                    if (!stockOk)
                    {
                        Console.WriteLine($"⚠️ Advertencia: No se actualizó stock para artículo {detalle.MDCOAR}");
                    }
                }

                // 2c. ⭐ VALORIZAR movimiento (llama SPL0010) DESPUÉS de todos los detalles
                var valorizacionOk = await repo.ValorizarMovimiento(
                    dto.Movimiento.MHEJER,
                    dto.Movimiento.MHPERI,
                    dto.Movimiento.MHALMA,
                    dto.Movimiento.MHCMOV,
                    dto.Movimiento.MHCOMP
                );

                if (!valorizacionOk)
                {
                    Console.WriteLine("⚠️ Advertencia: Movimiento insertado pero valorización falló");
                }

                // 3 Y 4. INSERTAR SEGÚN EL TIPO
                switch (dto.Tipo)
                {
                    case TipoMovimiento.PEDIDO:
                        return await ProcesarPedido(dto);

                    case TipoMovimiento.NOTA_CREDITO:
                        return await ProcesarNotaCredito(dto);

                    case TipoMovimiento.INVENTARIO:
                        return await ProcesarInventario(dto);

                    default:
                        return new ApiResponse<RegistroMovimientosDto>(400, 6098, 
                            $"Tipo de movimiento '{dto.Tipo}' no reconocido");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroMovimientosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
        private void AsignarReferencias(RegistroMovimientosDto dto)
        {
            if (dto.Tipo == TipoMovimiento.PEDIDO && dto.Pedido != null)
            {
                // MHREF1 = Código de cliente (max 12 caracteres)
                dto.Movimiento.MHREF1 = dto.Pedido.PHCLIE.PadLeft(12, ' ').Substring(0, 12);
                
                // MHREF2 = Número de documento / Tipo documento (max 12 caracteres)
                dto.Movimiento.MHREF2 = dto.Pedido.PHTDOC.PadLeft(12, ' ').Substring(0, 12);
                
                // MHREF3 = Número de pedido (max 12 caracteres)
                dto.Movimiento.MHREF3 = dto.Pedido.PHNUME.ToString().PadLeft(12, '0').Substring(0, 12);
            }
            else if (dto.Tipo == TipoMovimiento.NOTA_CREDITO && dto.NotaCredito != null)
            {
                // MHREF1 = Código de cliente (max 12 caracteres)
                dto.Movimiento.MHREF1 = dto.NotaCredito.NHCLIE.PadLeft(12, ' ').Substring(0, 12);
                
                // MHREF2 = Tipo documento (max 12 caracteres)
                dto.Movimiento.MHREF2 = dto.NotaCredito.NHTDOC.PadLeft(12, ' ').Substring(0, 12);
                
                // MHREF3 = Número de nota de crédito (max 12 caracteres)
                dto.Movimiento.MHREF3 = dto.NotaCredito.NHNUME.ToString().PadLeft(12, '0').Substring(0, 12);
            }
            // INVENTARIO no necesita referencias adicionales
        }

        // ==================== PROCESAR PEDIDO ====================

        private async Task<ApiResponse<RegistroMovimientosDto>> ProcesarPedido(RegistroMovimientosDto dto)
        {
            if (dto.Pedido == null)
                return new ApiResponse<RegistroMovimientosDto>(400, 6010, "Pedido es requerido para tipo PEDIDO");

            if (dto.PedidoDetails == null || !dto.PedidoDetails.Any())
                return new ApiResponse<RegistroMovimientosDto>(400, 6011, "PedidoDetails es requerido para tipo PEDIDO");

            // 3. INSERT TPEDH (Header Pedido)
            var pedidoHeader = RegistroMovimientosMapper.PedidoHeaderToEntity(dto.Pedido);
            if (!await repo.InsertTpedh(pedidoHeader))
                return new ApiResponse<RegistroMovimientosDto>(500, 6012, "Error al insertar TPEDH (Pedido Header)");

            // 4. INSERT TPEDD (Detalles Pedido) - PDVALE se asigna automáticamente
            foreach (var detalle in dto.PedidoDetails)
            {
                // PDVALE = MHCOMP (se asigna automático en el mapper)
                var pedidoDetail = RegistroMovimientosMapper.PedidoDetailToEntity(detalle, dto.Movimiento.MHCOMP);
                if (!await repo.InsertTpedd(pedidoDetail))
                    return new ApiResponse<RegistroMovimientosDto>(500, 6013, 
                        $"Error al insertar TPEDD (Pedido Detail) para artículo {detalle.PDARTI}");
            }

            return new ApiResponse<RegistroMovimientosDto>(200, 1000, 
                "Movimiento tipo PEDIDO registrado correctamente (TMOVH + TMOVD + TPEDH + TPEDD)");
        }

        private async Task<ApiResponse<RegistroMovimientosDto>> ValidarConsistenciaAsync(RegistroMovimientosDto dto)
        {
            // 1. Validar que hay al menos 1 detalle
            if (dto.MovimientoDetails == null || !dto.MovimientoDetails.Any())
                return new ApiResponse<RegistroMovimientosDto>(400, 6020, "Debe incluir al menos un detalle de movimiento");

            // 1.1 Validar MHCMOV solo permite "S" o "I"
            if (dto.Movimiento.MHCMOV != "S" && dto.Movimiento.MHCMOV != "I")
                return new ApiResponse<RegistroMovimientosDto>(400, 6021, 
                    $"MHCMOV solo permite 'S' (Salida) o 'I' (Ingreso). Valor recibido: '{dto.Movimiento.MHCMOV}'");

            // 1.2 Validar tipo de movimiento según negocio
            if (dto.Tipo == TipoMovimiento.PEDIDO && dto.Movimiento.MHCMOV != "S")
                return new ApiResponse<RegistroMovimientosDto>(400, 6021, 
                    "Para PEDIDO, MHCMOV debe ser 'S' (Salida)");
            if (dto.Tipo == TipoMovimiento.NOTA_CREDITO && dto.Movimiento.MHCMOV != "I")
                return new ApiResponse<RegistroMovimientosDto>(400, 6021, 
                    "Para NOTA_CREDITO, MHCMOV debe ser 'I' (Ingreso)");

            // 1.3 CRÍTICO: Validar que el tipo de movimiento exista en TTIMA
            // Esto evita que la contabilidad se descuadre al no poder clasificar el movimiento
            string clase = dto.Movimiento.MHCMOV;
            string tipo = dto.Movimiento.MHTMOV; 
            
            bool existeTipo = await repo.ExisteTipoMovimiento(clase, tipo);
            if (!existeTipo)
                return new ApiResponse<RegistroMovimientosDto>(400, 6019, 
                    $"El tipo de movimiento MHTMOV='{tipo}' con clase MHCMOV='{clase}' no existe en la tabla TTIMA. " +
                    $"Este tipo debe estar registrado en el sistema para que la contabilidad no se descuadre.");

            // 2. Validar consistencia entre TMOVH y TMOVD
            foreach (var detalle in dto.MovimientoDetails)
            {
                // 2.1 Validar MDCMOV solo permite "S" o "I"
                if (detalle.MDCMOV != "S" && detalle.MDCMOV != "I")
                    return new ApiResponse<RegistroMovimientosDto>(400, 6022, 
                        $"MDCMOV solo permite 'S' (Salida) o 'I' (Ingreso). Valor recibido: '{detalle.MDCMOV}'");

                // 2.1.1 Validar tipo de movimiento en detalles
                if (dto.Tipo == TipoMovimiento.PEDIDO && detalle.MDCMOV != "S")
                    return new ApiResponse<RegistroMovimientosDto>(400, 6022, 
                        "Para PEDIDO, MDCMOV debe ser 'S' (Salida) en todos los detalles");
                if (dto.Tipo == TipoMovimiento.NOTA_CREDITO && detalle.MDCMOV != "I")
                    return new ApiResponse<RegistroMovimientosDto>(400, 6022, 
                        "Para NOTA_CREDITO, MDCMOV debe ser 'I' (Ingreso) en todos los detalles");

                if (detalle.MDALMA != dto.Movimiento.MHALMA)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6023, 
                        $"MDALMA ({detalle.MDALMA}) debe coincidir con MHALMA ({dto.Movimiento.MHALMA})");

                if (detalle.MDCMOV != dto.Movimiento.MHCMOV)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6024, 
                        $"MDCMOV ({detalle.MDCMOV}) debe coincidir con MHCMOV ({dto.Movimiento.MHCMOV})");

                // NOTA: MDCOMP y MHCOMP se auto-generan desde TALMA, no se validan aquí

                if (detalle.MDEJER != dto.Movimiento.MHEJER)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6026, 
                        $"MDEJER debe coincidir con MHEJER ({dto.Movimiento.MHEJER})");

                if (detalle.MDFECH != dto.Movimiento.MHFECH)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6027, 
                        $"MDFECH debe coincidir con MHFECH ({dto.Movimiento.MHFECH})");

                if (detalle.MDPERI != dto.Movimiento.MHPERI)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6028, 
                        $"MDPERI debe coincidir con MHPERI ({dto.Movimiento.MHPERI})");

                if (detalle.MDTMOV != dto.Movimiento.MHTMOV)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6029, 
                        $"MDTMOV debe coincidir con MHTMOV ({dto.Movimiento.MHTMOV})");
            }

            // 3. VALIDAR SEGÚN EL TIPO
            if (dto.Tipo == TipoMovimiento.PEDIDO)
            {
                if (dto.Pedido == null)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6030, "Pedido es obligatorio para tipo PEDIDO");

                if (dto.PedidoDetails == null || !dto.PedidoDetails.Any())
                    return new ApiResponse<RegistroMovimientosDto>(400, 6031, "PedidoDetails es obligatorio para tipo PEDIDO");

                // Validar que la cantidad de detalles coincida
                if (dto.MovimientoDetails.Count != dto.PedidoDetails.Count)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6032, 
                        "La cantidad de MovimientoDetails debe coincidir con PedidoDetails");

                // Validar consistencia entre TPEDH y TPEDD
                foreach (var detalle in dto.PedidoDetails)
                {
                    if (detalle.PDCLIE != dto.Pedido.PHCLIE)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6033, 
                            $"PDCLIE debe coincidir con PHCLIE ({dto.Pedido.PHCLIE})");

                    if (detalle.PDFECP != dto.Pedido.PHFECP)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6034, 
                            $"PDFECP debe coincidir con PHFECP ({dto.Pedido.PHFECP})");

                    if (detalle.PDNUME != dto.Pedido.PHNUME)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6035, 
                            $"PDNUME debe coincidir con PHNUME ({dto.Pedido.PHNUME})");

                    if (detalle.PDPVTA != dto.Pedido.PHPVTA)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6036, 
                            $"PDPVTA debe coincidir con PHPVTA ({dto.Pedido.PHPVTA})");
                }
            }

            // 4. VALIDAR NOTA DE CRÉDITO
            if (dto.Tipo == TipoMovimiento.NOTA_CREDITO)
            {
                if (dto.NotaCredito == null)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6040, "NotaCredito es obligatorio para tipo NOTA_CREDITO");

                if (dto.NotaCreditoDetails == null || !dto.NotaCreditoDetails.Any())
                    return new ApiResponse<RegistroMovimientosDto>(400, 6041, "NotaCreditoDetails es obligatorio para tipo NOTA_CREDITO");

                // Validar que la cantidad de detalles coincida
                if (dto.MovimientoDetails.Count != dto.NotaCreditoDetails.Count)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6042, 
                        "La cantidad de MovimientoDetails debe coincidir con NotaCreditoDetails");

                // Validar consistencia entre TNCDH y TNCDD
                foreach (var detalle in dto.NotaCreditoDetails)
                {
                    if (detalle.NCCLIE != dto.NotaCredito.NHCLIE)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6043, 
                            $"NCCLIE debe coincidir con NHCLIE ({dto.NotaCredito.NHCLIE})");

                    if (detalle.NCNUME != dto.NotaCredito.NHNUME)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6044, 
                            $"NCNUME debe coincidir con NHNUME ({dto.NotaCredito.NHNUME})");

                    if (detalle.NCPVTA != dto.NotaCredito.NHPVTA)
                        return new ApiResponse<RegistroMovimientosDto>(400, 6045, 
                            $"NCPVTA debe coincidir con NHPVTA ({dto.NotaCredito.NHPVTA})");
                }
            }

            // 5. INVENTARIO solo necesita MovimientoDetails (ya validado arriba)

            return new ApiResponse<RegistroMovimientosDto>(200, 1000, "Validación exitosa");
        }

        // ==================== PROCESAR NOTA DE CRÉDITO ====================

        private async Task<ApiResponse<RegistroMovimientosDto>> ProcesarNotaCredito(RegistroMovimientosDto dto)
        {
            if (dto.NotaCredito == null)
                return new ApiResponse<RegistroMovimientosDto>(400, 6040, "NotaCredito es requerido para tipo NOTA_CREDITO");

            if (dto.NotaCreditoDetails == null || !dto.NotaCreditoDetails.Any())
                return new ApiResponse<RegistroMovimientosDto>(400, 6041, "NotaCreditoDetails es requerido para tipo NOTA_CREDITO");

            // 3. INSERT TNCDH (Header Nota de Crédito)
            var notaCreditoHeader = RegistroMovimientosMapper.NotaCreditoHeaderToEntity(dto.NotaCredito);
            if (!await repo.InsertTncdh(notaCreditoHeader))
                return new ApiResponse<RegistroMovimientosDto>(500, 6046, "Error al insertar TNCDH (NC Header)");

            // 4. INSERT TNCDD (Detalles Nota de Crédito) - NCVALE se asigna automáticamente
            foreach (var detalle in dto.NotaCreditoDetails)
            {
                // NCVALE = MHCOMP (se asigna automático en el mapper)
                var notaCreditoDetail = RegistroMovimientosMapper.NotaCreditoDetailToEntity(detalle, dto.Movimiento.MHCOMP);
                if (!await repo.InsertTncdd(notaCreditoDetail))
                    return new ApiResponse<RegistroMovimientosDto>(500, 6047, 
                        $"Error al insertar TNCDD (NC Detail) para artículo {detalle.NCARTI}");
            }

            return new ApiResponse<RegistroMovimientosDto>(200, 1000, 
                "Movimiento tipo NOTA_CREDITO registrado correctamente (TMOVH + TMOVD + TNCDH + TNCDD)");
        }

        // ==================== PROCESAR INVENTARIO ====================

        private async Task<ApiResponse<RegistroMovimientosDto>> ProcesarInventario(RegistroMovimientosDto dto)
        {
            // Para INVENTARIO solo se insertan TMOVH y TMOVD (ya se hicieron en CreateAsync)
            // No hay tablas adicionales que insertar
            return new ApiResponse<RegistroMovimientosDto>(200, 1000, 
                "Movimiento tipo INVENTARIO registrado correctamente (TMOVH + TMOVD)");
        }
    }
}
