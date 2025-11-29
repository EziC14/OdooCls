namespace OdooCls.Application.Dtos
{
    /// <summary>
    /// DTO para confirmar la recepción de una transferencia
    /// </summary>
    public class ConfirmarTransferenciaDto
    {
        /// <summary>
        /// Almacén de destino donde se confirma la recepción
        /// </summary>
        public string AlmacenDestino { get; set; } = string.Empty;

        /// <summary>
        /// Ejercicio del movimiento de ingreso
        /// </summary>
        public int Ejercicio { get; set; }

        /// <summary>
        /// Periodo del movimiento de ingreso
        /// </summary>
        public int Periodo { get; set; }

        /// <summary>
        /// Número de vale del ingreso en tránsito a confirmar
        /// </summary>
        public int ValeIngreso { get; set; }
    }
}
