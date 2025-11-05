using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroProveedoresServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroProveedoresRepository repo;

        public RegistroProveedoresServices(IConfiguration configuration, IRegistroProveedoresRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroProveedoresDto>> CreateAsync(RegistroProveedoresDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroProveedoresDto>(400, 1, "No se recibio datos en el Archivo");

                // Validar código único
                if (await repo.ExisteProveedor(dto.PROCVE))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4001, $"Proveedor {dto.PROCVE} ya existe");

                // Validar situación 01/02/99
                var sit = (dto.PROSIT ?? string.Empty).Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4002, "PROSIT debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var entity = RegistroProveedoresMapper.DtoToEntity(dto);
                var ok = await repo.InsertTprov(entity);
                if (ok)
                    return new ApiResponse<RegistroProveedoresDto>(200, 1000, "Proveedor registrado correctamente");

                return new ApiResponse<RegistroProveedoresDto>(400, 1001, "No se pudo registrar el proveedor");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroProveedoresDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroProveedoresDto>> UpdateAsync(RegistroProveedoresDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroProveedoresDto>(400, 1, "No se recibio datos en el Archivo");

                if (string.IsNullOrWhiteSpace(dto.PROCVE))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4003, "PROCVE es obligatorio");

                if (!await repo.ExisteProveedor(dto.PROCVE))
                    return new ApiResponse<RegistroProveedoresDto>(404, 4004, $"Proveedor {dto.PROCVE} no existe");

                if (string.IsNullOrWhiteSpace(dto.PRONOM))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4005, "PRONOM (Nombre) es obligatorio para actualizar");

                if (string.IsNullOrWhiteSpace(dto.PROSIT))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4006, "PROSIT (Situación) es obligatorio para actualizar");

                // Validar situación 01/02/99
                var sit = dto.PROSIT.Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4002, "PROSIT debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var ok = await repo.UpdateNombreYSituacion(dto.PROCVE, dto.PRONOM, dto.PROSIT);
                if (ok)
                    return new ApiResponse<RegistroProveedoresDto>(200, 1000, "Proveedor actualizado correctamente");

                return new ApiResponse<RegistroProveedoresDto>(400, 1001, "No se pudo actualizar el proveedor");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroProveedoresDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
