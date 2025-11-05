using OdooCls.Application.Dtos;
using OdooCls.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Mapper
{
    public class RegistroComprasMapper
    {
        public static RegistroCompras DtoToEntity(RegistroComprasDto registro)
        {
            var entity = new RegistroCompras()
            {
                RCEJER = registro.RCEJER,
                RCPERI = registro.RCPERI,
                RCTDOC = registro.RCTDOC,
                RCNDOC = registro.RCNDOC,
                RCFECH = registro.RCFECH,
                RCRCXP = registro.RCRCXP,
                RCCPRO = registro.RCCPRO,
                RCPROV = registro.RCPROV,
                RCRUC = registro.RCRUC,
                RCARTI = registro.RCARTI,
                RCMONE = registro.RCMONE,
                RCTCAM = registro.RCTCAM,
                RCVALV = registro.RCVALV,
                RCCVAL = registro.RCCVAL,
                RCMVAL = registro.RCMVAL,
                RCVALI = registro.RCVALI,
                RCCVAI = registro.RCCVAI,
                RCMVAI = registro.RCMVAI,
                RCDSCT = registro.RCDSCT,
                RCCDSC = registro.RCCDSC,
                RCMDSC = registro.RCMDSC,
                RCIMP1 = registro.RCIMP1,
                RCCIM1 = registro.RCCIM1,
                RCMIM1 = registro.RCMIM1,
                RCPVTA = registro.RCPVTA,
                RCCPVT = registro.RCCPVT,
                RCMPVT = registro.RCMPVT,
                RCCONC = registro.RCCONC,
                RCASTO = registro.RCASTO,
                RCCOST = registro.RCCOST,
                RCTREF = registro.RCTREF,
                RCNREF = registro.RCNREF,
                RCFEVE = registro.RCFEVE,
                RCNDOM = registro.RCNDOM,
                RCCPAG = registro.RCCPAG,
                RCSITU = registro.RCSITU,
                RCUSIN = registro.RCUSIN,
                RCFEIN = registro.RCFEIN,
                RCHOIN = registro.RCHOIN,
                RCRVVA = registro.RCRVVA,
                RCREF7 = registro.RCREF7,
                RCCBSA = registro.RCCBSA
            };

            return entity;
        }
    }
}
