using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;

namespace OdooCls.Application.Mapper
{
    public static class RegistroAlmacenesMapper
    {
        public static RegistroAlmacen DtoToEntity(RegistroAlmacenesDto dto)
        {
            return new RegistroAlmacen
            {
                ALCODI = dto.ALCODI,
                ALNOMB = dto.ALNOMB,
                ALRESP = dto.ALRESP,
                ALVALO = dto.ALVALO,
                ALSITU = dto.ALSITU,
                ALINGR = dto.ALINGR,
                ALSALI = dto.ALSALI,
                ALTRAN = dto.ALTRAN,
                ALDIRE = dto.ALDIRE,
                ALCANT = dto.ALCANT,
                ALDISD = dto.ALDISD,
                ALUBGD = dto.ALUBGD,
                ALCPLD = dto.ALCPLD,
                ALREF1 = dto.ALREF1,
                ALREF2 = dto.ALREF2,
                ALFLG1 = dto.ALFLG1,
                ALFLG2 = dto.ALFLG2
            };
        }
    }
}
