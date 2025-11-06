namespace OdooCls.Core.Entities
{
    public class RegistroAlmacen
    {
        // Tabla: TALMA
        public string ALCODI { get; set; } = string.Empty; // Almacenes (código)
        public string ALNOMB { get; set; } = string.Empty; // Nombre Almacén
        public string ALRESP { get; set; } = string.Empty; // Responsable
        public string ALVALO { get; set; } = string.Empty; // Interv. Valorizac.
        public string ALSITU { get; set; } = string.Empty; // Situación
        public int ALINGR { get; set; } = 0;              // Corr. Ingresos
        public int ALSALI { get; set; } = 0;              // Corr. Salidas
        public int ALTRAN { get; set; } = 0;              // Corr. Transf.
        public string ALDIRE { get; set; } = string.Empty; // Dirección
        public int ALCANT { get; set; } = 0;               // Cant. Tramos
        public string ALDISD { get; set; } = string.Empty; // Distrito Dirección
        public string ALUBGD { get; set; } = string.Empty; // Ubigeo Dirección
        public string ALCPLD { get; set; } = string.Empty; // Cod. Post. Direcc.
        public string ALREF1 { get; set; } = string.Empty; // Referencia 1
        public string ALREF2 { get; set; } = string.Empty; // Referencia 2
        public string ALFLG1 { get; set; } = string.Empty; // Flag 1
        public string ALFLG2 { get; set; } = string.Empty; // Flag 2
    }
}
