namespace Reporte.Validacion.Dtos
{
    public class InfoBlocksDataTransferObject
    {
        public Guid Id { get; set; }

        public List<SaleDataTransferObject> Information { get; set; }
    }
}
