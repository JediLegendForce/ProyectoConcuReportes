namespace Reporte.Validacion.Dtos
{
    public class TransactionDataTransferObject
    {
        public Guid Id { get; set; }

        public List<string> errors { get; set; }
    }
}