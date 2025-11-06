using System.ComponentModel.DataAnnotations;

namespace OdooCls.Application.Dtos
{
    public class RegistroAlmacenesDto
    {
        // Tabla: TALMA
        [Required]
        [StringLength(2)]
        public string ALCODI { get; set; } = string.Empty; // Almacenes

        [Required]
        [StringLength(20)]
        public string ALNOMB { get; set; } = string.Empty; // Nombre Almacén

        [StringLength(20)]
        public string ALRESP { get; set; } = string.Empty; // Responsable

        [StringLength(1)]
        public string ALVALO { get; set; } = string.Empty; // Interv. Valorizac.

        [Required]
        [StringLength(2)]
        public string ALSITU { get; set; } = string.Empty; // Situación

        public int ALINGR { get; set; } = 0; // Corr. Ingresos

        public int ALSALI { get; set; } = 0; // Corr. Salidas

        public int ALTRAN { get; set; } = 0; // Corr. Transf.

        [StringLength(50)]
        public string ALDIRE { get; set; } = string.Empty; // Dirección

        public int ALCANT { get; set; } = 0; // Cant. Tramos

        [StringLength(30)]
        public string ALDISD { get; set; } = string.Empty; // Distrito Dirección

        [StringLength(6)]
        public string ALUBGD { get; set; } = string.Empty; // Ubigeo Dirección

        [StringLength(6)]
        public string ALCPLD { get; set; } = string.Empty; // Cod. Post. Direcc.

        [StringLength(6)]
        public string ALREF1 { get; set; } = string.Empty; // Referencia 1

        [StringLength(6)]
        public string ALREF2 { get; set; } = string.Empty; // Referencia 2

        [StringLength(1)]
        public string ALFLG1 { get; set; } = string.Empty; // Flag 1

        [StringLength(1)]
        public string ALFLG2 { get; set; } = string.Empty; // Flag 2
    }
}
