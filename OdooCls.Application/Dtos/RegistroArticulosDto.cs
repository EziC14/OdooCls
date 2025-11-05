using System.ComponentModel.DataAnnotations;

namespace OdooCls.Application.Dtos
{
    public class RegistroArticulosDto
    {
        // Tabla: TARTI
        [Required]
        [StringLength(20)]
        public string ARTCOD { get; set; } = string.Empty; // Código Artículo

        [Required]
        [StringLength(70)]
        public string ARTDES { get; set; } = string.Empty; // Descripción Artículo

        [Required]
        [StringLength(3)]
        public string ARTMED { get; set; } = string.Empty; // Unidad Medida (TTABD/UGMED)

        [Required]
        [StringLength(2)]
        public string ARTTIP { get; set; } = string.Empty; // Tipo Artículo (TTABD/INTAR)

        [Required]
        [StringLength(3)]
        public string ARTFAM { get; set; } = string.Empty; // Familia (TTABD/INFAM)

        [Required]
        [StringLength(3)]
        public string ARTSFA { get; set; } = string.Empty; // Sub-Familia (TTABD/INSFA)

        [Required]
        [StringLength(15)]
        public string ARCTAC { get; set; } = string.Empty; // Cuenta Contable (TCATC)

        [Required]
        [StringLength(2)]
        public string ARSITU { get; set; } = string.Empty; // Situación Artículo

        [StringLength(2)]
        public string ARCVTA { get; set; } = string.Empty; // Condición Venta (TTABD/INCVT)

        [Required]
        [StringLength(20)]
        public string ARTMAR { get; set; } = string.Empty; // Marca
    }
}
