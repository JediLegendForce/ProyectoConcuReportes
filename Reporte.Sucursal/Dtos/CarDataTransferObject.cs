namespace Reporte.Sucursal.Dtos
{
    public class CarDataTransferObject
    {
        public string Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }
        
        public int Year { get; set; }

        public int Division_id { get; set; }
    }
}
