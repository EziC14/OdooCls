using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;

namespace OdooCls.Application.Mapper
{
    public static class RegistroClientesMapper
    {
        public static RegistroCliente DtoToEntity(RegistroClientesDto dto)
        {
            return new RegistroCliente
            {
                CLICVE = dto.CLICVE,
                CLINOM = dto.CLINOM,
                CLIDIR = dto.CLIDIR,
                CLICPO = dto.CLICPO,
                CLIDIS = dto.CLIDIS,
                CLIPRO = dto.CLIPRO,
                CLIDPT = dto.CLIDPT,
                CLIPAI = dto.CLIPAI,
                CLIRUC = dto.CLIRUC,
                CLISIT = dto.CLISIT,
                CLILCR = dto.CLILCR,
                CPACVE = dto.CPACVE
            };
        }
    }
}
