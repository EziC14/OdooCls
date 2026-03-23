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

                // Validar RUC único
                if (!string.IsNullOrWhiteSpace(dto.CLIRUC) && await repo.ExisteRuc(dto.CLIRUC))
                    return new ApiResponse<RegistroClientesDto>(400, 3003, $"El RUC {dto.CLIRUC} ya está registrado");

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

        public async Task<ApiResponse<PaginationResponseDto<RegistroClientesDto>>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var rows = await repo.GetAllClientes(page, pageSize);
                var totalCount = await repo.GetTotalClientesCount();
                
                var data = rows.Select(x => new RegistroClientesDto
                {
                    CLICVE = x.CLICVE,
                    CLINOM = x.CLINOM,
                    CLIDIR = x.CLIDIR,
                    CLICPO = x.CLICPO,
                    CLIDIS = x.CLIDIS,
                    CLIPRO = x.CLIPRO,
                    CLIDPT = x.CLIDPT,
                    CLIPAI = x.CLIPAI,
                    CLIRUC = x.CLIRUC,
                    CLISIT = x.CLISIT,
                    CLILCR = x.CLILCR,
                    CPACVE = x.CPACVE
                }).ToList();

                var paginatedData = new PaginationResponseDto<RegistroClientesDto>(data, totalCount, page, pageSize);
                return new ApiResponse<PaginationResponseDto<RegistroClientesDto>>(200, 1000, "Consulta realizada correctamente", paginatedData);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginationResponseDto<RegistroClientesDto>>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroClientesDto>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new ApiResponse<RegistroClientesDto>(400, 3003, "CLICVE es obligatorio");

                var row = await repo.GetClienteById(id);
                if (row == null)
                    return new ApiResponse<RegistroClientesDto>(404, 3004, $"Cliente {id} no existe");

                var data = new RegistroClientesDto
                {
                    CLICVE = row.CLICVE,
                    CLINOM = row.CLINOM,
                    CLIDIR = row.CLIDIR,
                    CLICPO = row.CLICPO,
                    CLIDIS = row.CLIDIS,
                    CLIPRO = row.CLIPRO,
                    CLIDPT = row.CLIDPT,
                    CLIPAI = row.CLIPAI,
                    CLIRUC = row.CLIRUC,
                    CLISIT = row.CLISIT,
                    CLILCR = row.CLILCR,
                    CPACVE = row.CPACVE
                };

                return new ApiResponse<RegistroClientesDto>(200, 1000, "Consulta realizada correctamente", data);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroClientesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
