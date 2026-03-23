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

                // Validar RUC único
                if (!string.IsNullOrWhiteSpace(dto.PRORUC) && await repo.ExisteRuc(dto.PRORUC))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4003, $"El RUC {dto.PRORUC} ya está registrado");

                // Validar situación 01/02/99
                var sit = (dto.PROSIT ?? string.Empty).Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4002, "PROSIT debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                // Validar PRORF1 (S/N)
                var rf1 = (dto.PRORF1 ?? string.Empty).Trim().ToUpper();
                if (rf1 != "S" && rf1 != "N")
                    return new ApiResponse<RegistroProveedoresDto>(400, 4004, "PRORF1 (Aplica Retención) debe ser 'S' o 'N'");

                // Validar PROARE (S/N)
                var are = (dto.PROARE ?? string.Empty).Trim().ToUpper();
                if (are != "S" && are != "N")
                    return new ApiResponse<RegistroProveedoresDto>(400, 4005, "PROARE (Acepta Recojos) debe ser 'S' o 'N'");

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

        public async Task<ApiResponse<PaginationResponseDto<RegistroProveedoresDto>>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var rows = await repo.GetAllProveedores(page, pageSize);
                var totalCount = await repo.GetTotalProveedoresCount();
                
                var data = rows.Select(x => new RegistroProveedoresDto
                {
                    PROCVE = x.PROCVE,
                    PRONOM = x.PRONOM,
                    PRODIR = x.PRODIR,
                    PROCPO = x.PROCPO,
                    PRODIS = x.PRODIS,
                    PROPRO = x.PROPRO,
                    PRODPT = x.PRODPT,
                    PROPAI = x.PROPAI,
                    PRORUC = x.PRORUC,
                    PROSIT = x.PROSIT,
                    PRORF1 = x.PRORF1,
                    PROARE = x.PROARE,
                    CPACVE = x.CPACVE
                }).ToList();

                var paginatedData = new PaginationResponseDto<RegistroProveedoresDto>(data, totalCount, page, pageSize);
                return new ApiResponse<PaginationResponseDto<RegistroProveedoresDto>>(200, 1000, "Consulta realizada correctamente", paginatedData);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginationResponseDto<RegistroProveedoresDto>>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroProveedoresDto>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new ApiResponse<RegistroProveedoresDto>(400, 4003, "PROCVE es obligatorio");

                var row = await repo.GetProveedorById(id);
                if (row == null)
                    return new ApiResponse<RegistroProveedoresDto>(404, 4004, $"Proveedor {id} no existe");

                var data = new RegistroProveedoresDto
                {
                    PROCVE = row.PROCVE,
                    PRONOM = row.PRONOM,
                    PRODIR = row.PRODIR,
                    PROCPO = row.PROCPO,
                    PRODIS = row.PRODIS,
                    PROPRO = row.PROPRO,
                    PRODPT = row.PRODPT,
                    PROPAI = row.PROPAI,
                    PRORUC = row.PRORUC,
                    PROSIT = row.PROSIT,
                    PRORF1 = row.PRORF1,
                    PROARE = row.PROARE,
                    CPACVE = row.CPACVE
                };

                return new ApiResponse<RegistroProveedoresDto>(200, 1000, "Consulta realizada correctamente", data);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroProveedoresDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
