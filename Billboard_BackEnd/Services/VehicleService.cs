using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.Repositories;

namespace Billboard_BackEnd.Services
{
    public class VehicleService : IVehicleService
    {
        #region SETUP / INITIALISATION
        private readonly IVehicleDapperContext _dbRepoDapper;
        //private readonly IVehicleMongoDBContext _dbRepoMongo;

        public VehicleService()
        {
            _dbRepoDapper = new VehicleRepository();
        }

        public VehicleService(string connectionString)
        {
            _dbRepoDapper = new VehicleRepository(connectionString);
        }
        #endregion

        #region SERVICES RELATED TO LOCAL ACTIONS
        // public IEnumerable<Vehicle> GetAllVehicles() => _dbRepoDapper.ExecuteGetAllVehiclesRecords();
        public IEnumerable<Car> GetAllCars() => _dbRepoDapper.ExecuteFetchCarRecordsSQL();
        public Car GetCarById(int id) => _dbRepoDapper.ExecuteFetchCarRecordByIdSQL(id);
        public IEnumerable<Motorbike> GetAllMotorbikes() => _dbRepoDapper.ExecuteFetchMotorbikeRecordsSQL();
        public Motorbike GetMotorbikeById(int id) => _dbRepoDapper.ExecuteFetchMotorbikeRecordByIdSQL(id);
        public List<Vehicle> GetAllVehiclesRecords()
        {
            var listOfCars = _dbRepoDapper.ExecuteFetchCarRecordsSQL().ToList();
            var listOfMotorbikes = _dbRepoDapper.ExecuteFetchMotorbikeRecordsSQL().ToList();
            List<Vehicle> listOfVehicles = [];

            // Solution in very 'primitive' way. Will find better in due time.
            foreach (var car in listOfCars)
            {
                listOfVehicles.Add(car);
            }
            foreach (var motorbike in listOfMotorbikes)
            {
                listOfVehicles.Add(motorbike);
            }

            return listOfVehicles;
        }


        public bool CreateNewCar(Car newCar)
        {
            if (_dbRepoDapper.ExecuteCreateVehicleRecordSQL(newCar))
                return true;
            else
                return false;
        }

        public bool CreateNewMotorbike(Motorbike newMotorbike)
        {
            if (_dbRepoDapper.ExecuteCreateVehicleRecordSQL(newMotorbike))
                return true;
            else
                return false;
        }

        public bool UpdateCarById(int id, Car carUpdate)
        {
            int recordCount = _dbRepoDapper.GetNumberOfVehicleRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteUpdateVehicleRecordByIdSQL(id, carUpdate);
            }
            else
                return false;  
        }

        public bool UpdateMotorbikeById(int id, Motorbike motorbikeUpdate)
        {
            int recordCount = _dbRepoDapper.GetNumberOfVehicleRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteUpdateVehicleRecordByIdSQL(id, motorbikeUpdate);
            }
            else
                return false;
        }

        public bool DeleteVehicle(int id)
        {
            int recordCount = _dbRepoDapper.GetNumberOfVehicleRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteDeleteVehicleRecordByIdSQL(id);
            }
            else
                return false;
        }
        #endregion
    }
}
