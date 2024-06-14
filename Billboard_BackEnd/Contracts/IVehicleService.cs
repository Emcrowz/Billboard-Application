using Billboard_BackEnd.Models;

namespace Billboard_BackEnd.Contracts
{
    public interface IVehicleService
    {
        // Create 
        bool CreateNewCar(Car newCar);
        bool CreateNewMotorbike(Motorbike newMotorbike);

        // Read / Get
        List<Vehicle> GetAllVehiclesRecords();
        IEnumerable<Car> GetAllCars();
        Car? GetCarById(int id);
        IEnumerable<Motorbike> GetAllMotorbikes();
        Motorbike? GetMotorbikeById(int id);

        // Update
        bool UpdateCarById(int id, Car carUpdate);
        bool UpdateMotorbikeById(int id, Motorbike motorbikeUpdate);

        // Delete
        bool DeleteVehicle(int id);
    }
}