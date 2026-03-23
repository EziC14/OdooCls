using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroArticulosServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroArticulosRepository repo;

        public RegistroArticulosServices(IConfiguration configuration, IRegistroArticulosRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroArticulosDto>> CreateAsync(RegistroArticulosDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroArticulosDto>(400, 1, "No se recibio datos en el Archivo");

                // Duplicado
                if (await repo.ExisteArticulo(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(400, 2007, $"Artículo {dto.ARTCOD} ya existe");

                // Validar situación 01/02/99
                var sit = (dto.ARSITU ?? string.Empty).Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroArticulosDto>(400, 2013, "ARSITU debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var entity = RegistroArticulosMapper.DtoToEntity(dto);
                var ok = await repo.InsertTarti(entity);
                if (ok)
                    return new ApiResponse<RegistroArticulosDto>(200, 1000, "Artículo registrado correctamente");

                return new ApiResponse<RegistroArticulosDto>(400, 1001, "No se pudo registrar el artículo");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroArticulosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroArticulosDto>> UpdateAsync(RegistroArticulosDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroArticulosDto>(400, 1, "No se recibio datos en el Archivo");

                if (string.IsNullOrWhiteSpace(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(400, 2009, "ARTCOD es obligatorio");

                if (!await repo.ExisteArticulo(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(404, 2008, $"Artículo {dto.ARTCOD} no existe");

                if (string.IsNullOrWhiteSpace(dto.ARTDES))
                    return new ApiResponse<RegistroArticulosDto>(400, 2011, "ARTDES (Nombre del artículo) es obligatorio para actualizar");

                if (string.IsNullOrWhiteSpace(dto.ARSITU))
                    return new ApiResponse<RegistroArticulosDto>(400, 2012, "ARSITU (Situación) es obligatorio para actualizar");

                // Validar situación 01/02/99
                var sit = dto.ARSITU.Trim();
                var allowedSit = new HashSet<string>(new[] { "01", "02", "99" });
                if (!allowedSit.Contains(sit))
                    return new ApiResponse<RegistroArticulosDto>(400, 2013, "ARSITU debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var ok = await repo.UpdateDescripcionYSituacion(dto.ARTCOD, dto.ARTDES, dto.ARSITU);
                if (ok)
                    return new ApiResponse<RegistroArticulosDto>(200, 1000, "Artículo actualizado correctamente");

                return new ApiResponse<RegistroArticulosDto>(400, 1001, "No se pudo actualizar el artículo");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroArticulosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaginationResponseDto<RegistroArticulosDto>>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var rows = await repo.GetAllArticulos(page, pageSize);
                var totalCount = await repo.GetTotalArticulosCount();
                
                var data = rows.Select(x => new RegistroArticulosDto
                {
                    ARTCOD = x.ARTCOD,
                    ARTDES = x.ARTDES,
                    ARTMED = x.ARTMED,
                    ARTTIP = x.ARTTIP,
                    ARTFAM = x.ARTFAM,
                    ARTSFA = x.ARTSFA,
                    ARCTAC = x.ARCTAC,
                    ARSITU = x.ARSITU,
                    ARCVTA = x.ARCVTA,
                    ARTMAR = x.ARTMAR
                }).ToList();

                var paginatedData = new PaginationResponseDto<RegistroArticulosDto>(data, totalCount, page, pageSize);
                return new ApiResponse<PaginationResponseDto<RegistroArticulosDto>>(200, 1000, "Consulta realizada correctamente", paginatedData);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginationResponseDto<RegistroArticulosDto>>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroArticulosDto>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new ApiResponse<RegistroArticulosDto>(400, 2009, "ARTCOD es obligatorio");

                var row = await repo.GetArticuloById(id);
                if (row == null)
                    return new ApiResponse<RegistroArticulosDto>(404, 2008, $"Artículo {id} no existe");

                var data = new RegistroArticulosDto
                {
                    ARTCOD = row.ARTCOD,
                    ARTDES = row.ARTDES,
                    ARTMED = row.ARTMED,
                    ARTTIP = row.ARTTIP,
                    ARTFAM = row.ARTFAM,
                    ARTSFA = row.ARTSFA,
                    ARCTAC = row.ARCTAC,
                    ARSITU = row.ARSITU,
                    ARCVTA = row.ARCVTA,
                    ARTMAR = row.ARTMAR
                };

                return new ApiResponse<RegistroArticulosDto>(200, 1000, "Consulta realizada correctamente", data);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroArticulosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
