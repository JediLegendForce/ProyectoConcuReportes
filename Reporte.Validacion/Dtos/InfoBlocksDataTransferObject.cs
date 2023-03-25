namespace Reporte.Validacion.Dtos
{
    public class InfoBlocksDataTransferObject
    {
        public TransactionDataTransferObject transaction { get; set; }
        public List<SaleDataTransferObject> Information { get; set; }
    }
}
