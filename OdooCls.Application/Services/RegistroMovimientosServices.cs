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
                var validacion = ValidarConsistencia(dto);
                if (validacion.HttpStatusCode != 200)
                    return validacion;

                // VERIFICAR SI YA EXISTE EL MOVIMIENTO
                if (await repo.ExisteMovimiento(dto.Movimiento.MHEJER, dto.Movimiento.MHPERI, 
                                                 dto.Movimiento.MHALMA, dto.Movimiento.MHCOMP))
                    return new ApiResponse<RegistroMovimientosDto>(400, 6002, 
                        $"Ya existe un movimiento con EJER={dto.Movimiento.MHEJER}, PERI={dto.Movimiento.MHPERI}, ALMA={dto.Movimiento.MHALMA}, COMP={dto.Movimiento.MHCOMP}");

                // ASIGNAR REFERENCIAS AUTOMÁTICAS SEGÚN EL TIPO (ANTES DE INSERTAR)
                AsignarReferencias(dto);

                // 1. INSERT TMOVH (Header Movimiento) - SIEMPRE
                var movimientoHeader = RegistroMovimientosMapper.MovimientoHeaderToEntity(dto.Movimiento);
                if (!await repo.InsertTmovh(movimientoHeader))
                    return new ApiResponse<RegistroMovimientosDto>(500, 6003, "Error al insertar TMOVH (Movimiento Header)");

                // 2. INSERT TMOVD (Detalles Movimiento) - SIEMPRE
                foreach (var detalle in dto.MovimientoDetails)
                {
                    var movimientoDetail = RegistroMovimientosMapper.MovimientoDetailToEntity(detalle);
                    if (!await repo.InsertTmovd(movimientoDetail))
                        return new ApiResponse<RegistroMovimientosDto>(500, 6004, 
                            $"Error al insertar TMOVD (Movimiento Detail) para artículo {detalle.MDCOAR}");
                }

                // 3 Y 4. INSERTAR SEGÚN EL TIPO
                switch (dto.Tipo)
                {
                    case TipoMovimiento.PEDIDO:
                        return await ProcesarPedido(dto);

                    case TipoMovimiento.NOTA_CREDITO:
                        return new ApiResponse<RegistroMovimientosDto>(400, 6099, 
                            "Tipo NOTA_CREDITO aún no implementado. Pendiente de información.");

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

        /// <summary>
        /// Asigna referencias automáticas en TMOVH según el tipo de movimiento
        /// Según indicaciones: MHREF1=cliente, MHREF2=doc, MHREF3=número pedido
        /// </summary>
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
            // TODO: FALTA NOTA DE CRÉDITO
        }

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

        private ApiResponse<RegistroMovimientosDto> ValidarConsistencia(RegistroMovimientosDto dto)
        {
            // 1. Validar que hay al menos 1 detalle
            if (dto.MovimientoDetails == null || !dto.MovimientoDetails.Any())
                return new ApiResponse<RegistroMovimientosDto>(400, 6020, "Debe incluir al menos un detalle de movimiento");

            // 2. Validar consistencia entre TMOVH y TMOVD
            foreach (var detalle in dto.MovimientoDetails)
            {
                if (detalle.MDALMA != dto.Movimiento.MHALMA)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6021, 
                        $"MDALMA ({detalle.MDALMA}) debe coincidir con MHALMA ({dto.Movimiento.MHALMA})");

                if (detalle.MDCMOV != dto.Movimiento.MHCMOV)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6022, 
                        $"MDCMOV debe coincidir con MHCMOV ({dto.Movimiento.MHCMOV})");

                if (detalle.MDCOMP != dto.Movimiento.MHCOMP)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6023, 
                        $"MDCOMP debe coincidir con MHCOMP ({dto.Movimiento.MHCOMP})");

                if (detalle.MDEJER != dto.Movimiento.MHEJER)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6024, 
                        $"MDEJER debe coincidir con MHEJER ({dto.Movimiento.MHEJER})");

                if (detalle.MDFECH != dto.Movimiento.MHFECH)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6025, 
                        $"MDFECH debe coincidir con MHFECH ({dto.Movimiento.MHFECH})");

                if (detalle.MDPERI != dto.Movimiento.MHPERI)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6026, 
                        $"MDPERI debe coincidir con MHPERI ({dto.Movimiento.MHPERI})");

                if (detalle.MDTMOV != dto.Movimiento.MHTMOV)
                    return new ApiResponse<RegistroMovimientosDto>(400, 6027, 
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

            return new ApiResponse<RegistroMovimientosDto>(200, 1000, "Validación exitosa");
        }
    }
}
