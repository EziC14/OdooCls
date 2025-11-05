using System.ComponentModel.DataAnnotations;

namespace OdooCls.Application.Dtos
{
    public class RegistroProveedoresDto
    {
        // Tabla: TPROV
        [Required]
        [StringLength(10)]
        public string PROCVE { get; set; } = string.Empty; // Código Proveedor

        [Required]
        [StringLength(40)]
        public string PRONOM { get; set; } = string.Empty; // Nombre Proveedor

        [StringLength(40)]
        public string PRODIR { get; set; } = string.Empty; // Dirección Proveedor

        [StringLength(15)]
        public string PROCPO { get; set; } = string.Empty; // Código Postal

        [StringLength(30)]
        public string PRODIS { get; set; } = string.Empty; // Distrito Proveedor

        [StringLength(30)]
        public string PROPRO { get; set; } = string.Empty; // Provincia Proveedor

        [StringLength(30)]
        public string PRODPT { get; set; } = string.Empty; // Departamento Proveedor

        [StringLength(30)]
        public string PROPAI { get; set; } = string.Empty; // País Proveedor

        [StringLength(15)]
        public string PRORUC { get; set; } = string.Empty; // R.U.C. Proveedor

        [Required]
        [StringLength(2)]
        public string PROSIT { get; set; } = string.Empty; // Situación Proveedor (01/02/99)

        [StringLength(3)]
        public string CPACVE { get; set; } = string.Empty; // Clave Condición Pago
    }
}
