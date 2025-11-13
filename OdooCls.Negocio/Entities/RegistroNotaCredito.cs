using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Entities
{
    /// <summary>
    /// Entidad para TNCDH - Nota de Crédito Header
    /// Todos los campos son obligatorios según documento
    /// </summary>
    public class RegistroNotaCredito
    {
        public string NHALMA { get; set; } = string.Empty;  // ALMACEN (CHAR 2)
        public string NHCLIE { get; set; } = string.Empty;  // Codigo Cliente (CHAR 10)
        public string NHCOST { get; set; } = string.Empty;  // CENTRO DE COSTO (CHAR 15)
        public string NHCPAG { get; set; } = string.Empty;  // Condicion Pago (CHAR 3)
        public string NHDIRC { get; set; } = string.Empty;  // Dirección cliente (CHAR 40)
        public string NHDISC { get; set; } = string.Empty;  // Distrito cliente (CHAR 30)
        public decimal NHEDS2 { get; set; } = 0;            // T.Valor Descto.2 (NUMERIC 15,2)
        public decimal NHEIGV { get; set; } = 0;            // T.Valor IGV. (NUMERIC 15,2)
        public decimal NHEPVT { get; set; } = 0;            // T.Valor Prec.Vta. (NUMERIC 15,2)
        public decimal NHEVVA { get; set; } = 0;            // T.Valor Vta.Afect (NUMERIC 15,2)
        public decimal NHEVVI { get; set; } = 0;            // T.Valor Vta.Inaf. (NUMERIC 15,2)
        public int NHFABO { get; set; } = 0;                // Nro.Fact./Boleta (NUMERIC 7,0)
        public int NHFECP { get; set; } = 0;                // Fecha de Nota (NUMERIC 8,0) - Formato: 20210218
        public int NHMONE { get; set; } = 0;                // TIPO DE MONEDA (NUMERIC 2,0)
        public decimal NHNDS2 { get; set; } = 0;            // T.Valor Descto.2 (NUMERIC 15,2)
        public string NHNIDE { get; set; } = string.Empty;  // Nro.Doc.Ident. (CHAR 15)
        public decimal NHNIGV { get; set; } = 0;            // T.Valor IGV. (NUMERIC 15,2)
        public string NHNOMC { get; set; } = string.Empty;  // Nombre Cliente (CHAR 40)
        public decimal NHNPVT { get; set; } = 0;            // T.Valor Prec.Vta. (NUMERIC 15,2)
        public int NHNUME { get; set; } = 0;                // Numero De Nota (NUMERIC 7,0)
        public decimal NHNVVA { get; set; } = 0;            // T.Valor Vta.Afect (NUMERIC 15,2)
        public decimal NHNVVI { get; set; } = 0;            // T.Valor Vta.Inaf. (NUMERIC 15,2)
        public string NHORIG { get; set; } = string.Empty;  // Origen Pedido (CHAR 1)
        public int NHPVTA { get; set; } = 0;                // Punto de Venta (NUMERIC 3,0)
        public int NHPVTN { get; set; } = 0;                // Punto de Venta (NUMERIC 3,0)
        public string NHRUBR { get; set; } = string.Empty;  // Rubro de Venta (CHAR 1)
        public string NHRUCC { get; set; } = string.Empty;  // R.U.C. cliente (CHAR 15)
        public string NHSITU { get; set; } = string.Empty;  // Situación Pedido (CHAR 2)
        public decimal NHTCAM { get; set; } = 0;            // Tipo de Cambio (NUMERIC 15,6)
        public string NHTDOC { get; set; } = string.Empty;  // Fact./Boleta (CHAR 2)
        public string NHTIDE { get; set; } = string.Empty;  // Tipo Doc.Ident. (CHAR 2)
        public string NHTVTA { get; set; } = string.Empty;  // Tipo de Venta (CHAR 2)
        public string NHZONA { get; set; } = string.Empty;  // Zona Cliente (CHAR 3)
    }
}
