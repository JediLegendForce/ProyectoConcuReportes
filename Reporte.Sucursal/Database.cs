using Newtonsoft.Json;
using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal
{
    public class Database
    {
        public Database() {
            Parallel.Invoke(
                () => { this.Employees = readFile<EmployeeDataTransferObject>("employees.json"); },
                //() => { this.Sales = readFile<SaleDataTransferObject>("sales.json"); },
                () => { this.Sucursales = readFile<SucursalDataTransferObject>("sucursales.json"); },
                () => { this.Cars = readFile<CarDataTransferObject>("cars.json"); });
        }

        private static List<T> readFile<T>(string fileDirectory){
            using (StreamReader r = new StreamReader(fileDirectory))
            {
                string json = r.ReadToEnd();
                List<T> items = JsonConvert.DeserializeObject<List<T>>(json);
                return items;
            }
        }
        
        public List<EmployeeDataTransferObject> Employees { get; set; }

        //public List<SaleDataTransferObject> Sales { get; set; }

        public List<CarDataTransferObject> Cars { get; set; }

        public List<SucursalDataTransferObject> Sucursales { get; set; }


    }
}
