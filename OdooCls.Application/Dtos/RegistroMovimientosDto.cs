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
        [StringLength(2)] public string MHALMA { get; set; } = string.Empty;
        [StringLength(1)] [RegularExpression("^[SI]$", ErrorMessage = "MHCMOV solo permite 'S' (Salida) o 'I' (Ingreso)")] 
        public string MHCMOV { get; set; } = string.Empty;
        public int MHCOMP { get; set; }
        public int MHEJER { get; set; }
        public int MHFECH { get; set; }
        public int MHPERI { get; set; }
        [StringLength(2)] public string MHSITU { get; set; } = string.Empty;
        [StringLength(2)] public string MHTMOV { get; set; } = string.Empty;

        // MHASTO - NO SE USA - Removido para evitar confusión
        [StringLength(10)] public string MHCHOF { get; set; } = string.Empty;
        public int MHFEIN { get; set; }
        public int MHFEMD { get; set; }
        public int MHHOIN { get; set; }
        public int MHHOMD { get; set; }
        [StringLength(10)] public string MHHRE1 { get; set; } = string.Empty;
        [StringLength(10)] public string MHHRE2 { get; set; } = string.Empty;
        [StringLength(10)] public string MHHRE3 { get; set; } = string.Empty;
        [StringLength(12)] public string MHREF1 { get; set; } = string.Empty;
        [StringLength(12)] public string MHREF2 { get; set; } = string.Empty;
        [StringLength(12)] public string MHREF3 { get; set; } = string.Empty;
        [StringLength(12)] public string MHREF4 { get; set; } = string.Empty;
        [StringLength(12)] public string MHREF5 { get; set; } = string.Empty;
        [StringLength(10)] public string MHSITD { get; set; } = string.Empty;
        [StringLength(10)] public string MHUSEA { get; set; } = string.Empty;
        [StringLength(10)] public string MHUSER { get; set; } = string.Empty;
        [StringLength(10)] public string MHUSIN { get; set; } = string.Empty;
        [StringLength(10)] public string MHUSMD { get; set; } = string.Empty;
        [StringLength(10)] public string MHVEHI { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detalle del Movimiento (TMOVD) - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// MDCMOV: Solo permite "S" (Salida) o "I" (Ingreso)
    /// MDCOMP: Debe coincidir con MHCOMP del header
    /// Formatos: Fechas YYYYMMDD (ej: 20210218)
    /// </summary>
    public class MovimientoDetailDto
    {
        [StringLength(2)] public string MDALMA { get; set; } = string.Empty;
        public decimal MDCANR { get; set; }
        [StringLength(1)] [RegularExpression("^[SI]$", ErrorMessage = "MDCMOV solo permite 'S' (Salida) o 'I' (Ingreso)")] 
        public string MDCMOV { get; set; } = string.Empty;
        [StringLength(20)] public string MDCOAR { get; set; } = string.Empty;
        public int MDCOMP { get; set; }
        public int MDCORR { get; set; }
        public int MDEJER { get; set; }
        public int MDFECH { get; set; }
        public int MDPERI { get; set; }
        [StringLength(2)] public string MDSITU { get; set; } = string.Empty;
        [StringLength(2)] public string MDTMOV { get; set; } = string.Empty;

        [StringLength(10)] public string MDACTI { get; set; } = string.Empty;
        public decimal MDCANA { get; set; }
        public decimal MDCOEP { get; set; }
        public decimal MDCOMO { get; set; }
        public decimal MDCOSP { get; set; }
        [StringLength(15)] public string MDCOST { get; set; } = string.Empty;
        public decimal MDCUEA { get; set; }
        public decimal MDCUER { get; set; }
        public decimal MDCUNA { get; set; }
        public decimal MDCUNR { get; set; }
        [StringLength(40)] public string MDDRE0 { get; set; } = string.Empty;
        [StringLength(10)] public string MDDRE1 { get; set; } = string.Empty;
        [StringLength(10)] public string MDDRE2 { get; set; } = string.Empty;
        [StringLength(10)] public string MDDRE4 { get; set; } = string.Empty;
        [StringLength(20)] public string MDDRE5 { get; set; } = string.Empty;
        public int MDFVTO { get; set; }
        [StringLength(10)] public string MDLOTE { get; set; } = string.Empty;
        public int MDMONO { get; set; }
        [StringLength(10)] public string MDSITD { get; set; } = string.Empty;
        public decimal MDTCAM { get; set; }
        public decimal MDTCMO { get; set; }
        public decimal MDTOEC { get; set; }
        public decimal MDTOTC { get; set; }
        [StringLength(3)] public string MDUMER { get; set; } = string.Empty;
    }

    public class PedidoHeaderDto
    {
        [StringLength(10)] public string PHCLIE { get; set; } = string.Empty;
        public int PHFECP { get; set; }
        public int PHFEIN { get; set; }
        public int PHHOIN { get; set; }
        public int PHMONE { get; set; }
        public int PHNUME { get; set; }
        public int PHPVTA { get; set; }
        [StringLength(2)] public string PHSITU { get; set; } = string.Empty;
        [StringLength(10)] public string PHUSIN { get; set; } = string.Empty;

        [StringLength(2)] public string PHALMA { get; set; } = string.Empty;
        [StringLength(10)] public string PHAUTO { get; set; } = string.Empty;
        [StringLength(6)] public string PHCLVT { get; set; } = string.Empty;
        [StringLength(15)] public string PHCOST { get; set; } = string.Empty;
        [StringLength(3)] public string PHCPAG { get; set; } = string.Empty;
        [StringLength(40)] public string PHDIRC { get; set; } = string.Empty;
        [StringLength(30)] public string PHDISC { get; set; } = string.Empty;
        public decimal PHDTIA { get; set; }
        public decimal PHDTOA { get; set; }
        public decimal PHEDS2 { get; set; }
        public decimal PHEIGV { get; set; }
        public decimal PHEPVT { get; set; }
        public decimal PHEVVA { get; set; }
        public decimal PHEVVI { get; set; }
        public int PHFECE { get; set; }
        public int PHFEMD { get; set; }
        [StringLength(1)] public string PHFL01 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL03 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL06 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL07 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL08 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL09 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL10 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL11 { get; set; } = string.Empty;
        [StringLength(1)] public string PHFL12 { get; set; } = string.Empty;
        public int PHHOMD { get; set; }
        public int PHHORP { get; set; }
        public decimal PHNDS2 { get; set; }
        [StringLength(15)] public string PHNIDE { get; set; } = string.Empty;
        public decimal PHNIGV { get; set; }
        [StringLength(40)] public string PHNOMC { get; set; } = string.Empty;
        public decimal PHNPVT { get; set; }
        public decimal PHNVVA { get; set; }
        public decimal PHNVVI { get; set; }
        [StringLength(1)] public string PHORIG { get; set; } = string.Empty;
        public decimal PHPERE { get; set; }
        public decimal PHPERN { get; set; }
        [StringLength(25)] public string PHREF1 { get; set; } = string.Empty;
        [StringLength(10)] public string PHREF2 { get; set; } = string.Empty;
        [StringLength(10)] public string PHREF3 { get; set; } = string.Empty;
        [StringLength(10)] public string PHREF4 { get; set; } = string.Empty;
        [StringLength(15)] public string PHREF6 { get; set; } = string.Empty;
        [StringLength(15)] public string PHREF7 { get; set; } = string.Empty;
        public decimal PHREF8 { get; set; }
        public decimal PHREF9 { get; set; }
        [StringLength(60)] public string PHREFA { get; set; } = string.Empty;
        [StringLength(1)] public string PHRUBR { get; set; } = string.Empty;
        [StringLength(15)] public string PHRUCC { get; set; } = string.Empty;
        [StringLength(10)] public string PHSITD { get; set; } = string.Empty;
        public decimal PHTCAM { get; set; }
        [StringLength(2)] public string PHTDOC { get; set; } = string.Empty;
        [StringLength(2)] public string PHTIDE { get; set; } = string.Empty;
        [StringLength(2)] public string PHTVTA { get; set; } = string.Empty;
        [StringLength(3)] public string PHUSAP { get; set; } = string.Empty;
        [StringLength(10)] public string PHUSMD { get; set; } = string.Empty;
        [StringLength(3)] public string PHZONA { get; set; } = string.Empty;
    }

    public class PedidoDetailDto
    {
        [StringLength(20)] public string PDARTI { get; set; } = string.Empty;
        public decimal PDCANT { get; set; }
        [StringLength(10)] public string PDCLIE { get; set; } = string.Empty;
        public int PDFECP { get; set; }
        public int PDNUME { get; set; }
        public int PDPVTA { get; set; }
        public int PDSECU { get; set; }
        [StringLength(2)] public string PDTDOC { get; set; } = string.Empty;
        [StringLength(2)] public string PDTVTA { get; set; } = string.Empty;
        public decimal PDUNIT { get; set; }
        // PDVALE se asigna automáticamente = MHCOMP

        public decimal PDCAN1 { get; set; }
        [StringLength(25)] public string PDCEQU { get; set; } = string.Empty;
        [StringLength(6)] public string PDCMOA { get; set; } = string.Empty;
        public decimal PDEDS2 { get; set; }
        public decimal PDEIGV { get; set; }
        public decimal PDEPVT { get; set; }
        public decimal PDEVVA { get; set; }
        public decimal PDEVVI { get; set; }
        public int PDFABO { get; set; }
        [StringLength(1)] public string PDFBMA { get; set; } = string.Empty;
        public int PDFECF { get; set; }
        public int PDFECG { get; set; }
        public int PDFECV { get; set; }
        public int PDFEXF { get; set; }
        public int PDFVTA { get; set; }
        public int PDGUIA { get; set; }
        public int PDHORF { get; set; }
        public int PDHORG { get; set; }
        public int PDHOXF { get; set; }
        public int PDITEM { get; set; }
        [StringLength(10)] public string PDLOTE { get; set; } = string.Empty;
        public int PDLPCO { get; set; }
        public int PDMONE { get; set; }
        [StringLength(15)] public string PDNART { get; set; } = string.Empty;
        public int PDNCOT { get; set; }
        public decimal PDNDS2 { get; set; }
        public decimal PDNIGV { get; set; }
        public decimal PDNPVT { get; set; }
        public decimal PDNVVA { get; set; }
        public decimal PDNVVI { get; set; }
        [StringLength(100)] public string PDOBSA { get; set; } = string.Empty;
        [StringLength(15)] public string PDREF0 { get; set; } = string.Empty;
        [StringLength(10)] public string PDREF1 { get; set; } = string.Empty;
        [StringLength(10)] public string PDREF2 { get; set; } = string.Empty;
        [StringLength(20)] public string PDREF3 { get; set; } = string.Empty;
        [StringLength(20)] public string PDREF4 { get; set; } = string.Empty;
        [StringLength(15)] public string PDREF5 { get; set; } = string.Empty;
        public int PDREF7 { get; set; }
        [StringLength(15)] public string PDREF9 { get; set; } = string.Empty;
        [StringLength(15)] public string PDREFA { get; set; } = string.Empty;
        [StringLength(15)] public string PDRFDE { get; set; } = string.Empty;
        public int PDSCOT { get; set; }
        public int PDSECC { get; set; }
        [StringLength(25)] public string PDSERI { get; set; } = string.Empty;
        [StringLength(10)] public string PDSITD { get; set; } = string.Empty;
        [StringLength(3)] public string PDUNVT { get; set; } = string.Empty;
        [StringLength(10)] public string PDUSAF { get; set; } = string.Empty;
        [StringLength(10)] public string PDUSAG { get; set; } = string.Empty;
        [StringLength(10)] public string PDUSXF { get; set; } = string.Empty;
        [StringLength(3)] public string PDZONA { get; set; } = string.Empty;
        public int REQNRO { get; set; }
    }

    /// <summary>
    /// Header de Nota de Crédito (TNCDH) - SOLO para Tipo = NOTA_CREDITO - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// Nota: No existe anulación de pedido, siempre se hace NC
    /// </summary>
    public class NotaCreditoHeaderDto
    {
        [StringLength(2)] public string NHALMA { get; set; } = string.Empty;
        [StringLength(10)] public string NHCLIE { get; set; } = string.Empty;
        [StringLength(15)] public string NHCOST { get; set; } = string.Empty;
        [StringLength(3)] public string NHCPAG { get; set; } = string.Empty;
        [StringLength(40)] public string NHDIRC { get; set; } = string.Empty;
        [StringLength(30)] public string NHDISC { get; set; } = string.Empty;
        public decimal NHEDS2 { get; set; }
        public decimal NHEIGV { get; set; }
        public decimal NHEPVT { get; set; }
        public decimal NHEVVA { get; set; }
        public decimal NHEVVI { get; set; }
        public int NHFABO { get; set; }
        public int NHFECP { get; set; }  // Formato: 20210218
        public int NHMONE { get; set; }
        public decimal NHNDS2 { get; set; }
        [StringLength(15)] public string NHNIDE { get; set; } = string.Empty;
        public decimal NHNIGV { get; set; }
        [StringLength(40)] public string NHNOMC { get; set; } = string.Empty;
        public decimal NHNPVT { get; set; }
        public int NHNUME { get; set; }
        public decimal NHNVVA { get; set; }
        public decimal NHNVVI { get; set; }
        [StringLength(1)] public string NHORIG { get; set; } = string.Empty;
        public int NHPVTA { get; set; }
        public int NHPVTN { get; set; }
        [StringLength(1)] public string NHRUBR { get; set; } = string.Empty;
        [StringLength(15)] public string NHRUCC { get; set; } = string.Empty;
        [StringLength(2)] public string NHSITU { get; set; } = string.Empty;
        public decimal NHTCAM { get; set; }
        [StringLength(2)] public string NHTDOC { get; set; } = string.Empty;
        [StringLength(2)] public string NHTIDE { get; set; } = string.Empty;
        [StringLength(2)] public string NHTVTA { get; set; } = string.Empty;
        [StringLength(3)] public string NHZONA { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detalle de Nota de Crédito (TNCDD) - SOLO para Tipo = NOTA_CREDITO - TODOS LOS CAMPOS SON OBLIGATORIOS
    /// </summary>
    public class NotaCreditoDetailDto
    {
        [StringLength(20)] public string NCARTI { get; set; } = string.Empty;
        public decimal NCCANT { get; set; }
        [StringLength(25)] public string NCCEQU { get; set; } = string.Empty;
        [StringLength(10)] public string NCCLIE { get; set; } = string.Empty;
        public decimal NCEDS2 { get; set; }
        public decimal NCEIGV { get; set; }
        public decimal NCEPVT { get; set; }
        public decimal NCEVVA { get; set; }
        public decimal NCEVVI { get; set; }
        public int NCFECV { get; set; }  // Formato: 20210218
        public int NCFVTA { get; set; }
        public int NCITEM { get; set; }
        [StringLength(10)] public string NCLOTE { get; set; } = string.Empty;
        public int NCLPCO { get; set; }
        public int NCMONE { get; set; }
        [StringLength(15)] public string NCNART { get; set; } = string.Empty;
        public int NCNCOT { get; set; }
        public decimal NCNDS2 { get; set; }
        public decimal NCNIGV { get; set; }
        public decimal NCNPVT { get; set; }
        public int NCNUME { get; set; }
        public decimal NCNVVA { get; set; }
        public decimal NCNVVI { get; set; }
        public int NCPVTA { get; set; }
        [StringLength(15)] public string NCREF0 { get; set; } = string.Empty;
        [StringLength(10)] public string NCREF1 { get; set; } = string.Empty;
        [StringLength(10)] public string NCREF2 { get; set; } = string.Empty;
        [StringLength(15)] public string NCREF5 { get; set; } = string.Empty;
        public int NCSCOT { get; set; }
        public int NCSECC { get; set; }
        public int NCSECU { get; set; }
        [StringLength(25)] public string NCSERI { get; set; } = string.Empty;
        [StringLength(2)] public string NCTVTA { get; set; } = string.Empty;
        public decimal NCUNIT { get; set; }
        [StringLength(3)] public string NCUNVT { get; set; } = string.Empty;
        [StringLength(10)] public string NCUSAD { get; set; } = string.Empty;
        // NCVALE se asigna automáticamente = MHCOMP
        [StringLength(3)] public string NCZONA { get; set; } = string.Empty;
        public int REQNRO { get; set; }
    }
}
