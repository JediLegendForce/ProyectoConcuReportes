namespace Reporte.Validacion.Dtos
{
    public class InfoBlocksDataTransferObject
    {
        public Guid Id { get; set; }

        public List<List<SaleDataTransferObject>> Information { get; set; }
    }
}
