namespace OdooCls.Application.Dtos
{
    public class ExcedenteLineaCreditoDto
    {
        public string ClienteId { get; set; } = string.Empty;
        public decimal LineaCredito { get; set; } = 0;
        public decimal SaldoActual { get; set; } = 0;
        public decimal Excedente { get; set; } = 0;
        public int CantidadDocumentosVencidos { get; set; } = 0;
    }
}
