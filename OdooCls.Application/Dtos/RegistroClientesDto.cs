using System.ComponentModel.DataAnnotations;

namespace OdooCls.Application.Dtos
{
    public class RegistroClientesDto
    {
        // Tabla: TCLIE
        [Required]
        [StringLength(10)]
        public string CLICVE { get; set; } = string.Empty; // Clave de cliente

        [Required]
        [StringLength(40)]
        public string CLINOM { get; set; } = string.Empty; // Nombre de cliente

        [StringLength(40)]
        public string CLIDIR { get; set; } = string.Empty; // Dirección cliente

        [StringLength(15)]
        public string CLICPO { get; set; } = string.Empty; // Código Postal

        [StringLength(30)]
        public string CLIDIS { get; set; } = string.Empty; // Distrito cliente

        [StringLength(30)]
        public string CLIPRO { get; set; } = string.Empty; // Provincia cliente

        [StringLength(30)]
        public string CLIDPT { get; set; } = string.Empty; // Departamento cliente

        [StringLength(30)]
        public string CLIPAI { get; set; } = string.Empty; // País cliente

        [StringLength(15)]
        public string CLIRUC { get; set; } = string.Empty; // R.U.C. cliente

        [Required]
        [StringLength(2)]
        public string CLISIT { get; set; } = string.Empty; // Situación Cliente (01/02/99)

        public decimal CLILCR { get; set; } = 0; // Límite de crédito

        [StringLength(3)]
        public string CPACVE { get; set; } = string.Empty; // Clave Condición Pago
    }
}
