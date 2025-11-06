using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroAlmacenesServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroAlmacenesRepository repo;

        public RegistroAlmacenesServices(IConfiguration configuration, IRegistroAlmacenesRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroAlmacenesDto>> CreateAsync(RegistroAlmacenesDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroAlmacenesDto>(400, 1, "No se recibio datos en el Archivo");

                // Validar código único
                if (await repo.ExisteAlmacen(dto.ALCODI))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5001, $"Almacén {dto.ALCODI} ya existe");

                // Validar situación (ajustar según reglas de negocio, similar a clientes/proveedores si aplica)
                var sit = (dto.ALSITU ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(sit))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5002, "ALSITU (Situación) es obligatorio");

                var entity = RegistroAlmacenesMapper.DtoToEntity(dto);
                var ok = await repo.InsertTalma(entity);
                if (ok)
                    return new ApiResponse<RegistroAlmacenesDto>(200, 1000, "Almacén registrado correctamente");

                return new ApiResponse<RegistroAlmacenesDto>(400, 1001, "No se pudo registrar el almacén");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroAlmacenesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroAlmacenesDto>> UpdateAsync(RegistroAlmacenesDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroAlmacenesDto>(400, 1, "No se recibio datos en el Archivo");

                if (string.IsNullOrWhiteSpace(dto.ALCODI))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5003, "ALCODI es obligatorio");

                if (!await repo.ExisteAlmacen(dto.ALCODI))
                    return new ApiResponse<RegistroAlmacenesDto>(404, 5004, $"Almacén {dto.ALCODI} no existe");

                if (string.IsNullOrWhiteSpace(dto.ALNOMB))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5005, "ALNOMB (Nombre) es obligatorio para actualizar");

                if (string.IsNullOrWhiteSpace(dto.ALSITU))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5006, "ALSITU (Situación) es obligatorio para actualizar");

                var ok = await repo.UpdateNombreYSituacion(dto.ALCODI, dto.ALNOMB, dto.ALSITU);
                if (ok)
                    return new ApiResponse<RegistroAlmacenesDto>(200, 1000, "Almacén actualizado correctamente");

                return new ApiResponse<RegistroAlmacenesDto>(400, 1001, "No se pudo actualizar el almacén");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroAlmacenesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
