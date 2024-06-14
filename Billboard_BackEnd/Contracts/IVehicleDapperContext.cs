using Billboard_BackEnd.Models;

namespace Billboard_BackEnd.Contracts
{
    public interface IVehicleDapperContext
    {
        // Create
        bool ExecuteCreateVehicleRecordSQL(Vehicle newVehicle);

        // Read / Get
        Car ExecuteFetchCarRecordByIdSQL(int id);
        Motorbike ExecuteFetchMotorbikeRecordByIdSQL(int id);
        IEnumerable<Car> ExecuteFetchCarRecordsSQL();
        IEnumerable<Motorbike> ExecuteFetchMotorbikeRecordsSQL();

        // Update
        bool ExecuteUpdateVehicleRecordByIdSQL(int id, Vehicle vehicleUpdate);

        // Delete
        bool ExecuteDeleteVehicleRecordByIdSQL(int id);

        // Helpers
        int GetNumberOfVehicleRecordsInDb();
    }
}