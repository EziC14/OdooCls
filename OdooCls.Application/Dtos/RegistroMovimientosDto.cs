using System.ComponentModel.DataAnnotations;

namespace OdooCls.Application.Dtos
{
    /// <summary>
    /// Enum para los diferentes tipos de movimientos
    /// PEDIDO: Crea factura - Inserta TMOVH + TMOVD + TPEDH + TPEDD
    /// NOTA_CREDITO: Anula pedido - Inserta TMOVH + TMOVD + TNCDH + TNCDD
    /// INVENTARIO: Movimientos normales - Inserta solo TMOVH + TMOVD
    /// </summary>
    public enum TipoMovimiento
    {
        PEDIDO = 1,
        NOTA_CREDITO = 2,
        INVENTARIO = 3
    }

    /// <summary>
    /// DTO para registro de Movimientos con sus diferentes tipos
    /// - PEDIDO: TMOVH + TMOVD + TPEDH + TPEDD
    /// - NOTA_CREDITO: TMOVH + TMOVD + TNCDH + TNCDD
    /// - INVENTARIO: TMOVH + TMOVD (solo)
    /// IMPORTANTE: No existe anulación de pedido, siempre se hace Nota de Crédito
    /// </summary>
    public class RegistroMovimientosDto
    {
        [Required]
        public TipoMovimiento Tipo { get; set; }

        // Datos del Movimiento (TMOVH) - SIEMPRE SE USA
        [Required]
        public MovimientoHeaderDto Movimiento { get; set; } = new();

        // Detalles del Movimiento (TMOVD) - SIEMPRE SE USA
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un detalle de movimiento")]
        public List<MovimientoDetailDto> MovimientoDetails { get; set; } = new();

        // Datos del Pedido (TPEDH) - SOLO SI Tipo = PEDIDO
        public PedidoHeaderDto? Pedido { get; set; }

        // Detalles del Pedido (TPEDD) - SOLO SI Tipo = PEDIDO
        public List<PedidoDetailDto>? PedidoDetails { get; set; }

        // Datos de Nota de Crédito (TNCDH) - SOLO SI Tipo = NOTA_CREDITO
        public NotaCreditoHeaderDto? NotaCredito { get; set; }

        // Detalles de Nota de Crédito (TNCDD) - SOLO SI Tipo = NOTA_CREDITO
        public List<NotaCreditoDetailDto>? NotaCreditoDetails { get; set; }

        // Si Tipo = INVENTARIO, solo se usan Movimiento y MovimientoDetails
    }

    /// <summary>
    /// Header del Movimiento (TMOVH) - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// MHCMOV: Solo permite "S" (Salida) o "I" (Ingreso)
    /// Formatos: Fechas YYYYMMDD (ej: 20210218), Horas HHMMSS (ej: 144426)
    /// </summary>
    public class MovimientoHeaderDto
    {
        [Required] [StringLength(2)] public string MHALMA { get; set; } = string.Empty;
        [Required] [StringLength(1)] [RegularExpression("^[SI]$", ErrorMessage = "MHCMOV solo permite 'S' (Salida) o 'I' (Ingreso)")] 
        public string MHCMOV { get; set; } = string.Empty;
        [Required] public int MHCOMP { get; set; }
        [Required] public int MHEJER { get; set; }
        [Required] public int MHFECH { get; set; }
        [Required] public int MHPERI { get; set; }
        [Required] [StringLength(2)] public string MHSITU { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string MHTMOV { get; set; } = string.Empty;

        // MHASTO - NO SE USA - Removido para evitar confusión
        [Required] [StringLength(10)] public string MHCHOF { get; set; } = string.Empty;
        [Required] public int MHFEIN { get; set; }
        [Required] public int MHFEMD { get; set; }
        [Required] public int MHHOIN { get; set; }
        [Required] public int MHHOMD { get; set; }
        [Required] [StringLength(10)] public string MHHRE1 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHHRE2 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHHRE3 { get; set; } = string.Empty;
        [Required] [StringLength(12)] public string MHREF1 { get; set; } = string.Empty;
        [Required] [StringLength(12)] public string MHREF2 { get; set; } = string.Empty;
        [Required] [StringLength(12)] public string MHREF3 { get; set; } = string.Empty;
        [Required] [StringLength(12)] public string MHREF4 { get; set; } = string.Empty;
        [Required] [StringLength(12)] public string MHREF5 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHSITD { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHUSEA { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHUSER { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHUSIN { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHUSMD { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MHVEHI { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detalle del Movimiento (TMOVD) - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// MDCMOV: Solo permite "S" (Salida) o "I" (Ingreso)
    /// MDCOMP: Debe coincidir con MHCOMP del header
    /// Formatos: Fechas YYYYMMDD (ej: 20210218)
    /// </summary>
    public class MovimientoDetailDto
    {
        [Required] [StringLength(2)] public string MDALMA { get; set; } = string.Empty;
        [Required] public decimal MDCANR { get; set; }
        [Required] [StringLength(1)] [RegularExpression("^[SI]$", ErrorMessage = "MDCMOV solo permite 'S' (Salida) o 'I' (Ingreso)")] 
        public string MDCMOV { get; set; } = string.Empty;
        [Required] [StringLength(20)] public string MDCOAR { get; set; } = string.Empty;
        [Required] public int MDCOMP { get; set; }
        [Required] public int MDCORR { get; set; }
        [Required] public int MDEJER { get; set; }
        [Required] public int MDFECH { get; set; }
        [Required] public int MDPERI { get; set; }
        [Required] [StringLength(2)] public string MDSITU { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string MDTMOV { get; set; } = string.Empty;

        [Required] [StringLength(10)] public string MDACTI { get; set; } = string.Empty;
        [Required] public decimal MDCANA { get; set; }
        [Required] public decimal MDCOEP { get; set; }
        [Required] public decimal MDCOMO { get; set; }
        [Required] public decimal MDCOSP { get; set; }
        [Required] [StringLength(15)] public string MDCOST { get; set; } = string.Empty;
        [Required] public decimal MDCUEA { get; set; }
        [Required] public decimal MDCUER { get; set; }
        [Required] public decimal MDCUNA { get; set; }
        [Required] public decimal MDCUNR { get; set; }
        [Required] [StringLength(40)] public string MDDRE0 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MDDRE1 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MDDRE2 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string MDDRE4 { get; set; } = string.Empty;
        [Required] [StringLength(20)] public string MDDRE5 { get; set; } = string.Empty;
        [Required] public int MDFVTO { get; set; }
        [Required] [StringLength(10)] public string MDLOTE { get; set; } = string.Empty;
        [Required] public int MDMONO { get; set; }
        [Required] [StringLength(10)] public string MDSITD { get; set; } = string.Empty;
        [Required] public decimal MDTCAM { get; set; }
        [Required] public decimal MDTCMO { get; set; }
        [Required] public decimal MDTOEC { get; set; }
        [Required] public decimal MDTOTC { get; set; }
        [Required] [StringLength(3)] public string MDUMER { get; set; } = string.Empty;
    }

    public class PedidoHeaderDto
    {
        [Required] [StringLength(10)] public string PHCLIE { get; set; } = string.Empty;
        [Required] public int PHFECP { get; set; }
        [Required] public int PHFEIN { get; set; }
        [Required] public int PHHOIN { get; set; }
        [Required] public int PHMONE { get; set; }
        [Required] public int PHNUME { get; set; }
        [Required] public int PHPVTA { get; set; }
        [Required] [StringLength(2)] public string PHSITU { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHUSIN { get; set; } = string.Empty;

        [Required] [StringLength(2)] public string PHALMA { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHAUTO { get; set; } = string.Empty;
        [Required] [StringLength(6)] public string PHCLVT { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PHCOST { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string PHCPAG { get; set; } = string.Empty;
        [Required] [StringLength(40)] public string PHDIRC { get; set; } = string.Empty;
        [Required] [StringLength(30)] public string PHDISC { get; set; } = string.Empty;
        [Required] public decimal PHDTIA { get; set; }
        [Required] public decimal PHDTOA { get; set; }
        [Required] public decimal PHEDS2 { get; set; }
        [Required] public decimal PHEIGV { get; set; }
        [Required] public decimal PHEPVT { get; set; }
        [Required] public decimal PHEVVA { get; set; }
        [Required] public decimal PHEVVI { get; set; }
        [Required] public int PHFECE { get; set; }
        [Required] public int PHFEMD { get; set; }
        [Required] [StringLength(1)] public string PHFL01 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL03 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL06 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL07 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL08 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL09 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL10 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL11 { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHFL12 { get; set; } = string.Empty;
        [Required] public int PHHOMD { get; set; }
        [Required] public int PHHORP { get; set; }
        [Required] public decimal PHNDS2 { get; set; }
        [Required] [StringLength(15)] public string PHNIDE { get; set; } = string.Empty;
        [Required] public decimal PHNIGV { get; set; }
        [Required] [StringLength(40)] public string PHNOMC { get; set; } = string.Empty;
        [Required] public decimal PHNPVT { get; set; }
        [Required] public decimal PHNVVA { get; set; }
        [Required] public decimal PHNVVI { get; set; }
        [Required] [StringLength(1)] public string PHORIG { get; set; } = string.Empty;
        [Required] public decimal PHPERE { get; set; }
        [Required] public decimal PHPERN { get; set; }
        [Required] [StringLength(25)] public string PHREF1 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHREF2 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHREF3 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHREF4 { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PHREF6 { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PHREF7 { get; set; } = string.Empty;
        [Required] public decimal PHREF8 { get; set; }
        [Required] public decimal PHREF9 { get; set; }
        [Required] [StringLength(60)] public string PHREFA { get; set; } = string.Empty;
        [Required] [StringLength(1)] public string PHRUBR { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PHRUCC { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHSITD { get; set; } = string.Empty;
        [Required] public decimal PHTCAM { get; set; }
        [Required] [StringLength(2)] public string PHTDOC { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string PHTIDE { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string PHTVTA { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string PHUSAP { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PHUSMD { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string PHZONA { get; set; } = string.Empty;
    }

    public class PedidoDetailDto
    {
        [Required] [StringLength(20)] public string PDARTI { get; set; } = string.Empty;
        [Required] public decimal PDCANT { get; set; }
        [Required] [StringLength(10)] public string PDCLIE { get; set; } = string.Empty;
        [Required] public int PDFECP { get; set; }
        [Required] public int PDNUME { get; set; }
        [Required] public int PDPVTA { get; set; }
        [Required] public int PDSECU { get; set; }
        [Required] [StringLength(2)] public string PDTDOC { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string PDTVTA { get; set; } = string.Empty;
        [Required] public decimal PDUNIT { get; set; }
        // PDVALE se asigna automáticamente = MHCOMP

        [Required] public decimal PDCAN1 { get; set; }
        [Required] [StringLength(25)] public string PDCEQU { get; set; } = string.Empty;
        [Required] [StringLength(6)] public string PDCMOA { get; set; } = string.Empty;
        [Required] public decimal PDEDS2 { get; set; }
        [Required] public decimal PDEIGV { get; set; }
        [Required] public decimal PDEPVT { get; set; }
        [Required] public decimal PDEVVA { get; set; }
        [Required] public decimal PDEVVI { get; set; }
        [Required] public int PDFABO { get; set; }
        [Required] [StringLength(1)] public string PDFBMA { get; set; } = string.Empty;
        [Required] public int PDFECF { get; set; }
        [Required] public int PDFECG { get; set; }
        [Required] public int PDFECV { get; set; }
        [Required] public int PDFEXF { get; set; }
        [Required] public int PDFVTA { get; set; }
        [Required] public int PDGUIA { get; set; }
        [Required] public int PDHORF { get; set; }
        [Required] public int PDHORG { get; set; }
        [Required] public int PDHOXF { get; set; }
        [Required] public int PDITEM { get; set; }
        [Required] [StringLength(10)] public string PDLOTE { get; set; } = string.Empty;
        [Required] public int PDLPCO { get; set; }
        [Required] public int PDMONE { get; set; }
        [Required] [StringLength(15)] public string PDNART { get; set; } = string.Empty;
        [Required] public int PDNCOT { get; set; }
        [Required] public decimal PDNDS2 { get; set; }
        [Required] public decimal PDNIGV { get; set; }
        [Required] public decimal PDNPVT { get; set; }
        [Required] public decimal PDNVVA { get; set; }
        [Required] public decimal PDNVVI { get; set; }
        [Required] [StringLength(100)] public string PDOBSA { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PDREF0 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDREF1 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDREF2 { get; set; } = string.Empty;
        [Required] [StringLength(20)] public string PDREF3 { get; set; } = string.Empty;
        [Required] [StringLength(20)] public string PDREF4 { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PDREF5 { get; set; } = string.Empty;
        [Required] public int PDREF7 { get; set; }
        [Required] [StringLength(15)] public string PDREF9 { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PDREFA { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string PDRFDE { get; set; } = string.Empty;
        [Required] public int PDSCOT { get; set; }
        [Required] public int PDSECC { get; set; }
        [Required] [StringLength(25)] public string PDSERI { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDSITD { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string PDUNVT { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDUSAF { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDUSAG { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string PDUSXF { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string PDZONA { get; set; } = string.Empty;
        [Required] public int REQNRO { get; set; }
    }

    /// <summary>
    /// Header de Nota de Crédito (TNCDH) - SOLO para Tipo = NOTA_CREDITO - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// Nota: No existe anulación de pedido, siempre se hace NC
    /// </summary>
    public class NotaCreditoHeaderDto
    {
        [Required] [StringLength(2)] public string NHALMA { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string NHCLIE { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string NHCOST { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string NHCPAG { get; set; } = string.Empty;
        [Required] [StringLength(40)] public string NHDIRC { get; set; } = string.Empty;
        [Required] [StringLength(30)] public string NHDISC { get; set; } = string.Empty;
        [Required] public decimal NHEDS2 { get; set; }
        [Required] public decimal NHEIGV { get; set; }
        [Required] public decimal NHEPVT { get; set; }
        [Required] public decimal NHEVVA { get; set; }
        [Required] public decimal NHEVVI { get; set; }
        [Required] public int NHFABO { get; set; }
        [Required] public int NHFECP { get; set; }  // Formato: 20210218
        [Required] public int NHMONE { get; set; }
        [Required] public decimal NHNDS2 { get; set; }
        [Required] [StringLength(15)] public string NHNIDE { get; set; } = string.Empty;
        [Required] public decimal NHNIGV { get; set; }
        [Required] [StringLength(40)] public string NHNOMC { get; set; } = string.Empty;
        [Required] public decimal NHNPVT { get; set; }
        [Required] public int NHNUME { get; set; }
        [Required] public decimal NHNVVA { get; set; }
        [Required] public decimal NHNVVI { get; set; }
        [Required] [StringLength(1)] public string NHORIG { get; set; } = string.Empty;
        [Required] public int NHPVTA { get; set; }
        [Required] public int NHPVTN { get; set; }
        [Required] [StringLength(1)] public string NHRUBR { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string NHRUCC { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string NHSITU { get; set; } = string.Empty;
        [Required] public decimal NHTCAM { get; set; }
        [Required] [StringLength(2)] public string NHTDOC { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string NHTIDE { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string NHTVTA { get; set; } = string.Empty;
        [Required] [StringLength(3)] public string NHZONA { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detalle de Nota de Crédito (TNCDD) - SOLO para Tipo = NOTA_CREDITO - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// </summary>
    public class NotaCreditoDetailDto
    {
        [Required] [StringLength(20)] public string NCARTI { get; set; } = string.Empty;
        [Required] public decimal NCCANT { get; set; }
        [Required] [StringLength(25)] public string NCCEQU { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string NCCLIE { get; set; } = string.Empty;
        [Required] public decimal NCEDS2 { get; set; }
        [Required] public decimal NCEIGV { get; set; }
        [Required] public decimal NCEPVT { get; set; }
        [Required] public decimal NCEVVA { get; set; }
        [Required] public decimal NCEVVI { get; set; }
        [Required] public int NCFECV { get; set; }  // Formato: 20210218
        [Required] public int NCFVTA { get; set; }
        [Required] public int NCITEM { get; set; }
        [Required] [StringLength(10)] public string NCLOTE { get; set; } = string.Empty;
        [Required] public int NCLPCO { get; set; }
        [Required] public int NCMONE { get; set; }
        [Required] [StringLength(15)] public string NCNART { get; set; } = string.Empty;
        [Required] public int NCNCOT { get; set; }
        [Required] public decimal NCNDS2 { get; set; }
        [Required] public decimal NCNIGV { get; set; }
        [Required] public decimal NCNPVT { get; set; }
        [Required] public int NCNUME { get; set; }
        [Required] public decimal NCNVVA { get; set; }
        [Required] public decimal NCNVVI { get; set; }
        [Required] public int NCPVTA { get; set; }
        [Required] [StringLength(15)] public string NCREF0 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string NCREF1 { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string NCREF2 { get; set; } = string.Empty;
        [Required] [StringLength(15)] public string NCREF5 { get; set; } = string.Empty;
        [Required] public int NCSCOT { get; set; }
        [Required] public int NCSECC { get; set; }
        [Required] public int NCSECU { get; set; }
        [Required] [StringLength(25)] public string NCSERI { get; set; } = string.Empty;
        [Required] [StringLength(2)] public string NCTVTA { get; set; } = string.Empty;
        [Required] public decimal NCUNIT { get; set; }
        [Required] [StringLength(3)] public string NCUNVT { get; set; } = string.Empty;
        [Required] [StringLength(10)] public string NCUSAD { get; set; } = string.Empty;
        // NCVALE se asigna automáticamente = MHCOMP
        [Required] [StringLength(3)] public string NCZONA { get; set; } = string.Empty;
        [Required] public int REQNRO { get; set; }
    }
}
