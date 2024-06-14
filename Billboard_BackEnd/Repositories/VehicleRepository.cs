using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace Billboard_BackEnd.Repositories
{
    public class VehicleRepository : IVehicleDapperContext
    {
        #region SETUP / INITIALISATION
        private readonly string  _connectionStringLocal = "Server=localhost;Database=BillboardApp;Trusted_Connection=True;TrustServerCertificate=True";

        private IDbConnection _dbConnectionLocal;

        public VehicleRepository()
        {
            _dbConnectionLocal = new SqlConnection(_connectionStringLocal);
        }

        public VehicleRepository(string specifiedConnection)
        {
            _dbConnectionLocal = new SqlConnection(specifiedConnection);
        }
        #endregion

        #region DAPPER CRUD
        // Create
        public bool ExecuteCreateVehicleRecordSQL(Vehicle newVehicle)
        {
            _dbConnectionLocal.Execute($"INSERT INTO Vehicles ( [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck] ) VALUES ( '{newVehicle.Make}', '{newVehicle.Model}', '{newVehicle.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}', '{newVehicle.CreationDate}', '{newVehicle.LastTechnicalCheck}' )");
            int additionIndex = _dbConnectionLocal.QuerySingle<int>($"SELECT MAX(VehicleId) FROM Vehicles");
            newVehicle.VehicleId = additionIndex;
            switch(newVehicle)
            {
                case Car:
                    _dbConnectionLocal.Execute($"INSERT INTO Cars ( [CarId], [DoorCount], [Engine] ) VALUES ( '{newVehicle.VehicleId}', '{((Car)newVehicle).DoorCount}', '{((Car)newVehicle).Engine.ToString()}' )");
                    return true;
                case Motorbike:
                    _dbConnectionLocal.Execute($"INSERT INTO Motorbikes ( [MotorbikeId], [CylinderVolume] ) VALUES ( '{newVehicle.VehicleId}', '{((Motorbike)newVehicle).CylinderVolume}' )");
                    return true;
                default:
                    throw new Exception("Unable to create vehicle record");
            }
        }

        // Get
        public Car ExecuteFetchCarRecordByIdSQL(int id)
        {
            return _dbConnectionLocal.QuerySingle<Car>(@"SELECT [VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [DoorCount], [Engine] FROM Vehicles
INNER JOIN Cars ON Vehicles.VehicleId = Cars.CarId WHERE [VehicleId] = " + $"{id}");
        }

        public Motorbike ExecuteFetchMotorbikeRecordByIdSQL(int id)
        {
            return _dbConnectionLocal.QuerySingle<Motorbike>(@"SELECT [VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [CylinderVolume] FROM Vehicles
INNER JOIN Motorbikes ON Vehicles.VehicleId = Motorbikes.MotorbikeId WHERE [VehicleId] = " + $"{id}");
        }

        public IEnumerable<Car> ExecuteFetchCarRecordsSQL()
        {
            return _dbConnectionLocal.Query<Car>(@"SELECT [VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [DoorCount], [Engine] FROM Vehicles
INNER JOIN Cars ON Vehicles.VehicleId = Cars.CarId");
        }

        public IEnumerable<Motorbike> ExecuteFetchMotorbikeRecordsSQL()
        {
            return _dbConnectionLocal.Query<Motorbike>(@"SELECT [VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [CylinderVolume] FROM Vehicles
INNER JOIN Motorbikes ON Vehicles.VehicleId = Motorbikes.MotorbikeId");
        }

        // Update
        public bool ExecuteUpdateVehicleRecordByIdSQL(int id, Vehicle vehicleUpdate)
        {
            switch(vehicleUpdate)
            {
                case Car:
                    _dbConnectionLocal.Execute($"UPDATE Vehicles SET [Make] = '{vehicleUpdate.Make}', [Model] = '{vehicleUpdate.Model}', [Price] = '{vehicleUpdate.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}', [CreationDate] = '{vehicleUpdate.CreationDate}', [LastTechnicalCheck] = '{vehicleUpdate.LastTechnicalCheck}' WHERE [VehicleId] = {id}");
                    return _dbConnectionLocal.Execute($"UPDATE Cars SET [DoorCount] = '{((Car)vehicleUpdate).DoorCount}', [Engine] = '{((Car)vehicleUpdate).Engine}' WHERE [CarId] = {id}") > 0;
                case Motorbike:
                    _dbConnectionLocal.Execute($"UPDATE Vehicles SET [Make] = '{vehicleUpdate.Make}', [Model] = '{vehicleUpdate.Model}', [Price] = '{vehicleUpdate.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}', [CreationDate] = '{vehicleUpdate.CreationDate}', [LastTechnicalCheck] = '{vehicleUpdate.LastTechnicalCheck}' WHERE [VehicleId] = {id}");
                    return _dbConnectionLocal.Execute($"UPDATE Motorbikes SET [CylinderVolume] = '{((Motorbike)vehicleUpdate).CylinderVolume}' WHERE [MotorbikeId] = {id}") > 0;
                default:
                    throw new Exception();
            }
        }

        // Delete
        public bool ExecuteDeleteVehicleRecordByIdSQL(int id)
        {
            _dbConnectionLocal.Execute($"DELETE FROM Cars WHERE [CarId] = {id}");
            _dbConnectionLocal.Execute($"DELETE FROM Motorbikes WHERE [MotorbikeId] = {id}");
            return _dbConnectionLocal.Execute($"DELETE FROM Vehicles WHERE [VehicleId] = {id}") > 0;
        }
        #endregion

        #region HELPER OPERATIONS
        public int GetNumberOfVehicleRecordsInDb()
        {
            return _dbConnectionLocal.QuerySingle<int>("SELECT COUNT(VehicleId) FROM Vehicles");
        }
        #endregion
    }
}
