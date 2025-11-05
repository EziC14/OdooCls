using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroClientesServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroClientesRepository repo;

        public RegistroClientesServices(IConfiguration configuration, IRegistroClientesRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroClientesDto>> CreateAsync(RegistroClientesDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroClientesDto>(400, 1, "No se recibio datos en el Archivo");

                if (await repo.ExisteCliente(dto.CLICVE))
                    return new ApiResponse<RegistroClientesDto>(400, 3001, $"Cliente {dto.CLICVE} ya existe");

                var sit = (dto.CLISIT ?? string.Empty).Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroClientesDto>(400, 3002, "CLISIT debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var entity = RegistroClientesMapper.DtoToEntity(dto);
                var ok = await repo.InsertTclie(entity);
                if (ok)
                    return new ApiResponse<RegistroClientesDto>(200, 1000, "Cliente registrado correctamente");

                return new ApiResponse<RegistroClientesDto>(400, 1001, "No se pudo registrar el cliente");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroClientesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroClientesDto>> UpdateAsync(RegistroClientesDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroClientesDto>(400, 1, "No se recibio datos en el Archivo");

                if (string.IsNullOrWhiteSpace(dto.CLICVE))
                    return new ApiResponse<RegistroClientesDto>(400, 3003, "CLICVE es obligatorio");

                if (!await repo.ExisteCliente(dto.CLICVE))
                    return new ApiResponse<RegistroClientesDto>(404, 3004, $"Cliente {dto.CLICVE} no existe");

                if (string.IsNullOrWhiteSpace(dto.CLINOM))
                    return new ApiResponse<RegistroClientesDto>(400, 3005, "CLINOM (Nombre) es obligatorio para actualizar");

                if (string.IsNullOrWhiteSpace(dto.CLISIT))
                    return new ApiResponse<RegistroClientesDto>(400, 3006, "CLISIT (Situación) es obligatorio para actualizar");

                // Validar situación 01/02/99
                var sit = dto.CLISIT.Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroClientesDto>(400, 3002, "CLISIT debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var ok = await repo.UpdateNombreYSituacion(dto.CLICVE, dto.CLINOM, dto.CLISIT);
                if (ok)
                    return new ApiResponse<RegistroClientesDto>(200, 1000, "Cliente actualizado correctamente");

                return new ApiResponse<RegistroClientesDto>(400, 1001, "No se pudo actualizar el cliente");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroClientesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
