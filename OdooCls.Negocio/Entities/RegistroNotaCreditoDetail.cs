using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Entities
{
    /// <summary>
    /// Entidad para TNCDD - Nota de Crédito Detail
    /// Todos los campos son obligatorios según documento
    /// </summary>
    public class RegistroNotaCreditoDetail
    {
        public string NCARTI { get; set; } = string.Empty;  // Codigo Articulo (CHAR 20)
        public decimal NCCANT { get; set; } = 0;            // Cantidad (NUMERIC 10,3)
        public string NCCEQU { get; set; } = string.Empty;  // NCCEQU (CHAR 25)
        public string NCCLIE { get; set; } = string.Empty;  // Codigo Cliente (CHAR 10)
        public decimal NCEDS2 { get; set; } = 0;            // T.Valor Descto.2 (NUMERIC 15,6)
        public decimal NCEIGV { get; set; } = 0;            // T.Valor IGV. (NUMERIC 15,6)
        public decimal NCEPVT { get; set; } = 0;            // T.Valor Prec.Vta. (NUMERIC 15,6)
        public decimal NCEVVA { get; set; } = 0;            // T.Valor Vta.Afect (NUMERIC 15,6)
        public decimal NCEVVI { get; set; } = 0;            // T.Valor Vta.Inaf. (NUMERIC 15,6)
        public int NCFECV { get; set; } = 0;                // Fecha de Vale (NUMERIC 8,0) - Formato: 20210218
        public int NCFVTA { get; set; } = 0;                // NCFVTA (NUMERIC 5,0)
        public int NCITEM { get; set; } = 0;                // ITEM CLIENTE (NUMERIC 3,0)
        public string NCLOTE { get; set; } = string.Empty;  // PDLOTE (CHAR 10)
        public int NCLPCO { get; set; } = 0;                // Codigo L.Precio (NUMERIC 7,0)
        public int NCMONE { get; set; } = 0;                // Moneda del Prec. (NUMERIC 2,0)
        public string NCNART { get; set; } = string.Empty;  // Nombre Articulo (CHAR 15)
        public int NCNCOT { get; set; } = 0;                // NUM.COTIZACION (NUMERIC 7,0)
        public decimal NCNDS2 { get; set; } = 0;            // T.Valor Descto.2 (NUMERIC 15,6)
        public decimal NCNIGV { get; set; } = 0;            // T.Valor IGV. (NUMERIC 15,6)
        public decimal NCNPVT { get; set; } = 0;            // T.Valor Prec.Vta. (NUMERIC 15,6)
        public int NCNUME { get; set; } = 0;                // Numero De Nota (NUMERIC 7,0)
        public decimal NCNVVA { get; set; } = 0;            // T.Valor Vta.Afect (NUMERIC 15,6)
        public decimal NCNVVI { get; set; } = 0;            // T.Valor Vta.Inaf. (NUMERIC 15,6)
        public int NCPVTA { get; set; } = 0;                // Punto de Venta (NUMERIC 3,0)
        public string NCREF0 { get; set; } = string.Empty;  // NCREF0 (CHAR 15)
        public string NCREF1 { get; set; } = string.Empty;  // PDREF1 (CHAR 10)
        public string NCREF2 { get; set; } = string.Empty;  // PDREF2 (CHAR 10)
        public string NCREF5 { get; set; } = string.Empty;  // NCREF5 (CHAR 15)
        public int NCSCOT { get; set; } = 0;                // SERIE COTIZACION (NUMERIC 3,0)
        public int NCSECC { get; set; } = 0;                // SECUENCIA COTIZ. (NUMERIC 3,0)
        public int NCSECU { get; set; } = 0;                // Secuencia (NUMERIC 5,0)
        public string NCSERI { get; set; } = string.Empty;  // PDSERI (CHAR 25)
        public string NCTVTA { get; set; } = string.Empty;  // Tipo de Venta (CHAR 2)
        public decimal NCUNIT { get; set; } = 0;            // NCUNIT (NUMERIC 15,4)
        public string NCUNVT { get; set; } = string.Empty;  // NCUNVT (CHAR 3)
        public string NCUSAD { get; set; } = string.Empty;  // Usr.Autoriz.Anul. (CHAR 10)
        public int NCVALE { get; set; } = 0;                // Numero Vale (NUMERIC 6,0) - Auto-asigna = MHCOMP
        public string NCZONA { get; set; } = string.Empty;  // Zona Cliente (CHAR 3)
        public int REQNRO { get; set; } = 0;                // Num.Requisicion (NUMERIC 8,0)
    }
}
