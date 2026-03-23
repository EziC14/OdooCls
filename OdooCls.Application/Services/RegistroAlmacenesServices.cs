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
                
                if (!sit.Equals("01") && !sit.Equals("02") && !sit.Equals("99"))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5002, "ALSITU debe ser uno de: 01 (Activo), 02 (Bloqueado), 99 (Anulado)");

                var alcant = dto.ALCANT.ToString().Trim();

                if (int.TryParse(alcant, out _) && int.Parse(alcant) > 1)
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5008, "ALCANT (Cantidad) no puede tener más de 1 caracteres");

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

        public async Task<ApiResponse<PaginationResponseDto<RegistroAlmacenesDto>>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var rows = await repo.GetAllAlmacenes(page, pageSize);
                var totalCount = await repo.GetTotalAlmacenesCount();
                
                var data = rows.Select(x => new RegistroAlmacenesDto
                {
                    ALCODI = x.ALCODI,
                    ALNOMB = x.ALNOMB,
                    ALRESP = x.ALRESP,
                    ALVALO = x.ALVALO,
                    ALSITU = x.ALSITU,
                    ALINGR = x.ALINGR,
                    ALSALI = x.ALSALI,
                    ALTRAN = x.ALTRAN,
                    ALDIRE = x.ALDIRE,
                    ALCANT = x.ALCANT,
                    ALDISD = x.ALDISD,
                    ALUBGD = x.ALUBGD,
                    ALCPLD = x.ALCPLD,
                    ALREF1 = x.ALREF1,
                    ALREF2 = x.ALREF2,
                    ALFLG1 = x.ALFLG1,
                    ALFLG2 = x.ALFLG2
                }).ToList();

                var paginatedData = new PaginationResponseDto<RegistroAlmacenesDto>(data, totalCount, page, pageSize);
                return new ApiResponse<PaginationResponseDto<RegistroAlmacenesDto>>(200, 1000, "Consulta realizada correctamente", paginatedData);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginationResponseDto<RegistroAlmacenesDto>>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroAlmacenesDto>> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return new ApiResponse<RegistroAlmacenesDto>(400, 5003, "ALCODI es obligatorio");

                var row = await repo.GetAlmacenById(id);
                if (row == null)
                    return new ApiResponse<RegistroAlmacenesDto>(404, 5004, $"Almacén {id} no existe");

                var data = new RegistroAlmacenesDto
                {
                    ALCODI = row.ALCODI,
                    ALNOMB = row.ALNOMB,
                    ALRESP = row.ALRESP,
                    ALVALO = row.ALVALO,
                    ALSITU = row.ALSITU,
                    ALINGR = row.ALINGR,
                    ALSALI = row.ALSALI,
                    ALTRAN = row.ALTRAN,
                    ALDIRE = row.ALDIRE,
                    ALCANT = row.ALCANT,
                    ALDISD = row.ALDISD,
                    ALUBGD = row.ALUBGD,
                    ALCPLD = row.ALCPLD,
                    ALREF1 = row.ALREF1,
                    ALREF2 = row.ALREF2,
                    ALFLG1 = row.ALFLG1,
                    ALFLG2 = row.ALFLG2
                };

                return new ApiResponse<RegistroAlmacenesDto>(200, 1000, "Consulta realizada correctamente", data);
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroAlmacenesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
