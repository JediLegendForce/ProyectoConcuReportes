namespace Reporte.Recolector.Dtos
{
    public class ValidacionDTO
    {
        public TransactionDTO transaction { get; set; }
        public List<SaleDTO> registros { get; set; }
        public int offset { get; set; }
    }
}
