using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Billboard_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        #region SETUP / INITIALISATION
        readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        #endregion

        [HttpGet("GetVehicles")]
        public IActionResult GetVehicles()
        {
            Log.Information($"Attempt to fetch {typeof(Vehicle).Name} records.");
            IEnumerable<Vehicle>? vehicles = _vehicleService.GetAllVehiclesRecords();
            if (vehicles != null)
            {
                Log.Information($"Successfully read records from DB. Count of records: {vehicles.Count()}");
                return Ok(vehicles);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(Car).Name} records from DB.");
                return NotFound();
            }
        }

        [HttpGet("GetCars")]
        public IActionResult GetCars()
        {
            Log.Information($"Attempt to fetch {typeof(Car).Name} records.");
            IEnumerable<Car>? cars = _vehicleService.GetAllCars();
            if (cars != null)
            {
                Log.Information($"Successfully read records from DB. Count of records: {cars.Count()}");
                return Ok(cars);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(Car).Name} records from DB.");
                return NotFound();
            }
        }

        [HttpGet("GetMotorbikes")]
        public IActionResult GetMotorbikes()
        {
            Log.Information($"Attempt to fetch {typeof(Motorbike).Name} records.");
            IEnumerable<Motorbike>? motorbikes = _vehicleService.GetAllMotorbikes();
            if (motorbikes != null)
            {
                Log.Information($"Successfully read records from DB. Count of records: {motorbikes.Count()}");
                return Ok(motorbikes);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(Motorbike).Name} records from DB.");
                return NotFound();
            }
        }

        [HttpGet("GetCarById/{id}")]
        public IActionResult GetCarById(int id)
        {
            Log.Information($"Attempt to fetch {typeof(Car).Name} record. ID: {id}");
            Car? car = _vehicleService.GetCarById(id);
            if (car != null)
            {
                Log.Information($"Successfully read record with ID: {id} from DB");
                return Ok(car);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(Car).Name} record from DB with ID: {id}");
                return NotFound();
            }
        }

        [HttpGet("GetMotorbikeById/{id}")]
        public IActionResult GetMotorbikeById(int id)
        {
            Log.Information($"Attempt to fetch {typeof(Motorbike).Name} record. ID: {id}");
            Motorbike? motorbike = _vehicleService.GetMotorbikeById(id);
            if (motorbike != null)
            {
                Log.Information($"Successfully read record with ID: {id} from DB");
                return Ok(motorbike);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(Motorbike).Name} record from DB with ID: {id}");
                return NotFound();
            }        
        }

        [HttpPost("InsertCar")]
        public IActionResult InsertCar(Car newCar)
        {
            Log.Information($"Attempt to create {typeof(Car).Name} record.");
            if (_vehicleService.CreateNewCar(newCar))
            {
                Log.Information($"Successfully created new {typeof(Car).Name} instance.");
                return Ok(newCar);
            }
            else
            {
                Log.Error($"Failed to create {typeof(Car).Name} instance in the DB.");
                return NotFound();
            }
        }

        [HttpPost("InsertMotorbike")]
        public IActionResult InsertMotorbike(Motorbike newMotorbike)
        {
            Log.Information($"Attempt to create {typeof(Motorbike).Name} record.");
            if (_vehicleService.CreateNewMotorbike(newMotorbike))
            {
                Log.Information($"Successfully created new {typeof(Motorbike).Name} instance.");
                return Ok(newMotorbike);
            }
            else
            {
                Log.Error($"Failed to create {typeof(Motorbike).Name} instance in the DB.");
                return NotFound();
            }
        }

        [HttpPut("UpdateCarById/{id}")]
        public IActionResult UpdateCarById(int id, Car carUpdate)
        {
            Log.Information($"Attempt to update {typeof(Car).Name} record with ID: {id}");
            if (_vehicleService.UpdateCarById(id, carUpdate))
            {
                Log.Information($"Successfully updated record with ID: {id} inside DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to update {typeof(Car).Name} record inside DB with ID: {id}");
                return NotFound();
            }
        }

        [HttpPut("UpdateMotorbikeById/{id}")]
        public IActionResult UpdateMotorbikeById(int id, Motorbike motorbikeUpdate)
        {
            Log.Information($"Attempt to update {typeof(Motorbike).Name} record with ID: {id}");
            if (_vehicleService.UpdateMotorbikeById(id, motorbikeUpdate))
            {
                Log.Information($"Successfully updated record with ID: {id} inside DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to update {typeof(Motorbike).Name} record inside DB with ID: {id}");
                return NotFound();
            }
        }

        [HttpDelete("DeleteVehicleById/{id}")]
        public IActionResult DeleteVehicleById(int id)
        {
            Log.Information($"Attempt to delete {typeof(Vehicle).Name} record with ID: {id}");
            if (_vehicleService.DeleteVehicle(id))
            {
                Log.Information($"Successfully deleted record with ID: {id} inside DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to delete {typeof(Vehicle).Name} record inside DB with ID: {id}");
                return NotFound();
            }
        }
    }
}
