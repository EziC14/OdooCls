using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Entities
{
    public class RegistroMovimientoDetail
    {
        public string MDACTI { get; set; } = string.Empty;  // Actividad (CHAR 10)
        public string MDALMA { get; set; } = string.Empty;  // Codigo Almacen (CHAR 2)
        public decimal MDCANA { get; set; } = 0;                    // Cantidad Almacen (NUMERIC 15,3)
        public decimal MDCANR { get; set; } = 0;                    // Cantidad Regist. (NUMERIC 15,3)
        public string MDCMOV { get; set; } = string.Empty;  // Clase Movim. (CHAR 1)
        public string MDCOAR { get; set; } = string.Empty;  // Codigo Articulo (CHAR 20)
        public decimal MDCOEP { get; set; } = 0;                   // Costo Promed. US$ (NUMERIC 15,6)
        public decimal MDCOMO { get; set; } = 0;                   // Total Costo Moneda Orig (NUMERIC 15,3)
        public int MDCOMP { get; set; } = 0;                      // Num.Vale Alm. (NUMERIC 6)
        public int MDCORR { get; set; } = 0;                        // Correlativo (NUMERIC 4)
        public decimal MDCOSP { get; set; }                 // Costo Promed. S/. (NUMERIC 15,6)
        public string MDCOST { get; set; } = string.Empty;  // CENTRO DE COSTO (CHAR 15)
        public decimal MDCUEA { get; set; } = 0;                    // Costo Almacen US$ (NUMERIC 15,3)
        public decimal MDCUER { get; set; } = 0;                    // Costo Regist.US$ (NUMERIC 15,3)
        public decimal MDCUNA { get; set; } = 0;                    // Costo Almacen S/. (NUMERIC 15,3)
        public decimal MDCUNR { get; set; } = 0;                    // Costo Regist.S/. (NUMERIC 15,3)
        public string MDDRE0 { get; set; } = string.Empty;  // REFERENCIA AUX.10 (CHAR 40)
        public string MDDRE1 { get; set; } = string.Empty;  // REF.AUXIL. 1 (CHAR 10)
        public string MDDRE2 { get; set; } = string.Empty;  // REF.AUXIL. 2 (CHAR 10)
        public string MDDRE4 { get; set; } = string.Empty;  // REFERENCIA AUX. 4 (CHAR 10)
        public string MDDRE5 { get; set; } = string.Empty;  // REFERENCIA AUX. 5 (CHAR 20)
        public int MDEJER { get; set; } = 0;                        // Ejercicio (NUMERIC 4)
        public int MDFECH { get; set; } = 0;                        // Fecha Movimiento (NUMERIC 8)
        public int MDFVTO { get; set; } = 0;                        // FECHA DE VTO (NUMERIC 8)
        public string MDLOTE { get; set; } = string.Empty;  // Lote (CHAR 10)
        public int MDMONO { get; set; } = 0;                       // Moneda de Origen (NUMERIC 2)
        public int MDPERI { get; set; } = 0;                       // Periodo (NUMERIC 2)
        public string MDSITD { get; set; } = string.Empty;  // SITUACION DESPACHO (CHAR 10)
    [System.ComponentModel.DataAnnotations.RegularExpression("^(01|02|99)$", ErrorMessage = "MDSITU solo permite 01, 02 o 99")]
    public string MDSITU { get; set; } = string.Empty;  // Situacion (CHAR 2)
        public decimal MDTCAM { get; set; } = 0;                    // Tipo de Cambio (NUMERIC 15,6)
        public decimal MDTCMO { get; set; } = 0;                    // Tipo Cambio M.Orig. (NUMERIC 15,6)
        public string MDTMOV { get; set; } = string.Empty;  // Tipo Movimiento (CHAR 2)
        public decimal MDTOEC { get; set; } = 0;                    // Total Costo US$ (NUMERIC 15,3)
        public decimal MDTOTC { get; set; } = 0;                    // Total Costo S/. (NUMERIC 15,3)
        public string MDUMER { get; set; } = string.Empty;  // Uni.Med.Regist. (CHAR 3)
    }
}
