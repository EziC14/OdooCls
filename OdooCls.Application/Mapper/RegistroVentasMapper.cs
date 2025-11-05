using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Mapper
{
    public class RegistroVentasMapper
    {
        public static RegistroVentas DtoToEntity(RegistroVentasDto registro)
        {
            var entity = new RegistroVentas
            {
                RVEJER = registro.RVEJER,
                RVPERI = registro.RVPERI,
                RVTDOC = registro.RVTDOC,
                RVNDOC = registro.RVNDOC,
                RVFECH = registro.RVFECH,
                RVCCLI = registro.RVCCLI,
                RVCLIE = registro.RVCLIE,
                RVMONE = registro.RVMONE,
                RVTCAM = registro.RVTCAM,
                RVVALV = registro.RVVALV,
                RVMVAL = registro.RVMVAL,
                RVVALI = registro.RVVALI,
                RVMVAI=registro.RVMVAI,
                RVDSCT = registro.RVDSCT,
                RVMDSC=registro.RVMDSC,
                RVIGV = registro.RVIGV,
                RVMIGV=registro.RVMIGV,
                RVPVTA = registro.RVPVTA,
                RVCPVT = registro.RVCPVT,
                RVMPVT = registro.RVMPVT,
                RVCONC = registro.RVCONC,
                RVTREF = registro.RVTREF,
                RVNREF = registro.RVNREF,
                RVGRAB = registro.RVGRAB,
                RVFPRO = registro.RVFPRO,
                RVHPRO = registro.RVHPRO,
                RVFEVE = registro.RVFEVE,
                RVNDOM = registro.RVNDOM,
                RVCPAG = registro.RVCPAG,
                RVRUC = registro.RVRUC,
                RVSITU = registro.RVSITU,
                RVCOST = registro.RVCOST,
                RVCVEN = registro.RVCVEN,
                RVUSIN = registro.RVUSIN,
                RVFEIN = registro.RVFEIN,
                RVHOIN = registro.RVHOIN
            };

            List<RegistroVentasDetail> detalle = new List<RegistroVentasDetail>();
            foreach (RegistroVentasDetailDto q in registro.detalle)
            {
                detalle.Add(DtoToEntity(q));
            }
            entity.RegistroVentasDetail = detalle;
            return entity;
        }

        public static RegistroVentasDetail DtoToEntity(RegistroVentasDetailDto registro)
        {
            var entity = new RegistroVentasDetail
            {
                RVEJER = registro.RVEJER,
                RVPERI = registro.RVPERI,
                RVTDOC = registro.RVTDOC,
                RVNDOC = registro.RVNDOC,
                RVSECU = registro.RVSECU,
                RVDCTA = registro.RVDCTA,
                RVDCCO = registro.RVDCCO,
                RVDIMP = registro.RVDIMP
            };
            return entity;

        }
    }
}
