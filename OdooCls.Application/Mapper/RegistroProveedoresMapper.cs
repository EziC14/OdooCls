using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;

namespace OdooCls.Application.Mapper
{
    public static class RegistroProveedoresMapper
    {
        public static RegistroProveedor DtoToEntity(RegistroProveedoresDto dto)
        {
            return new RegistroProveedor
            {
                PROCVE = dto.PROCVE,
                PRONOM = dto.PRONOM,
                PRODIR = dto.PRODIR,
                PROCPO = dto.PROCPO,
                PRODIS = dto.PRODIS,
                PROPRO = dto.PROPRO,
                PRODPT = dto.PRODPT,
                PROPAI = dto.PROPAI,
                PRORUC = dto.PRORUC,
                PROSIT = dto.PROSIT,
                CPACVE = dto.CPACVE
            };
        }
    }
}
