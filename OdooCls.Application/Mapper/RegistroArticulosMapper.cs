using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;

namespace OdooCls.Application.Mapper
{
    public static class RegistroArticulosMapper
    {
        public static RegistroArticulo DtoToEntity(RegistroArticulosDto dto)
        {
            return new RegistroArticulo
            {
                ARTCOD = dto.ARTCOD,
                ARTDES = dto.ARTDES,
                ARTMED = dto.ARTMED,
                ARTTIP = dto.ARTTIP,
                ARTFAM = dto.ARTFAM,
                ARTSFA = dto.ARTSFA,
                ARCTAC = dto.ARCTAC,
                ARSITU = dto.ARSITU,
                ARCVTA = dto.ARCVTA,
                ARTMAR = dto.ARTMAR
            };
        }
    }
}
