using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Entities
{

    public class RegistroMovimiento
    {
        public string MHALMA { get; set; } = string.Empty;  // CODIGO ALMACEN (CHAR 2)
        // MHASTO - NO SE USA - Removido para evitar confusión
        public string MHCHOF { get; set; } = string.Empty;  // CHOFER (CHAR 10)
        public string MHCMOV { get; set; } = string.Empty;  // CLASE MOVIMI. (CHAR 1) - Solo "S" o "I"
        public int MHCOMP { get; set; } = 0;                   // NRO.VALE ALM. (NUMERIC 6)
        public int MHEJER { get; set; } = 0;                      // EJERCICIO (NUMERIC 4)
        public int MHFECH { get; set; } = 0;                       // FECHA MOVIM. (NUMERIC 8) - Formato: 20210218
        public int MHFEIN { get; set; } = 0;                       // FECHA DE INGRESO (NUMERIC 8) - Formato: 20210218
        public int MHFEMD { get; set; } = 0;                       // FECHA DE MODIF. (NUMERIC 8) - Formato: 20210218
        public int MHHOIN { get; set; } = 0;                       // HORA DE INGRESO (NUMERIC 6) - Formato: 144426
        public int MHHOMD { get; set; } = 0;                       // HORA DE MODIF. (NUMERIC 6) - Formato: 144426
        public string MHHRE1 { get; set; } = string.Empty;  // REF. AUXIL. 1 (CHAR 10)
        public string MHHRE2 { get; set; } = string.Empty;  // REF. AUXIL. 2 (CHAR 10)
        public string MHHRE3 { get; set; } = string.Empty;  // REF. AUXIL. 3 (CHAR 10)
        public int MHPERI { get; set; } = 0;                   // PERIODO (NUMERIC 2)
        public string MHREF1 { get; set; } = string.Empty;  // Referencia 1 (CHAR 12)
        public string MHREF2 { get; set; } = string.Empty;  // Referencia 2 (CHAR 12)
        public string MHREF3 { get; set; } = string.Empty;  // Referencia 3 (CHAR 12)
        public string MHREF4 { get; set; } = string.Empty;  // Referencia 4 (CHAR 12)
        public string MHREF5 { get; set; } = string.Empty;  // Referencia 5 (CHAR 12)
        public string MHSITD { get; set; } = string.Empty;  // SITUACION DESPACHO (CHAR 10)
    [System.ComponentModel.DataAnnotations.RegularExpression("^(01|02|99)$", ErrorMessage = "MHSITU solo permite 01, 02 o 99")]
    public string MHSITU { get; set; } = string.Empty;  // SITUACION (CHAR 2)
        public string MHTMOV { get; set; } = string.Empty;  // Tipo Movimiento (CHAR 2)
        public string MHUSEA { get; set; } = string.Empty;  // USUAR.-ANULO (CHAR 10)
        public string MHUSER { get; set; } = string.Empty;  // USUAR.-REGISTRO (CHAR 10)
        public string MHUSIN { get; set; } = string.Empty;  // USUARIO DE INGRESO (CHAR 10)
        public string MHUSMD { get; set; } = string.Empty;  // USUARIO DE MODIF. (CHAR 10)
        public string MHVEHI { get; set; } = string.Empty;  // VEHICULO (CHAR 10)
    }
}
