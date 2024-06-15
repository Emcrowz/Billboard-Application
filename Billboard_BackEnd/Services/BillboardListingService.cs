using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Billboard_BackEnd.Repositories;
using System.Text.RegularExpressions;

namespace Billboard_BackEnd.Services
{
    public class BillboardListingService : IBillboardListingService
    {
        #region SETUP / INITIALISATION
        private readonly IBillboardListingDapperContext _dbRepoDapper;
        private readonly IUserService _userService;
        private readonly IVehicleService _vehicleService;
        //private readonly IBillboardListingMongoDBContext _dbRepoMongo;

        public BillboardListingService(string connectionString)
        {
            _userService = new UserService(connectionString);
            _vehicleService = new VehicleService(connectionString);
            _dbRepoDapper = new BillboardListingRepository(connectionString);
        }
        #endregion

        #region
        public bool CreateListing(string username, string password, VehicleDTO vehicleForListing)
        {
            User? user = _userService.UserLoginService(username, password);
            if (user != null)
            {
                // Enum.GetNames(typeof(EngineType)).Length
                if (vehicleForListing.DoorCount > 0 && vehicleForListing.CylinderVolume == 0)
                {
                    Car car = new Car()
                    {
                        Make = vehicleForListing.Make,
                        Model = vehicleForListing.Model,
                        Price = vehicleForListing.Price,
                        CreationDate = vehicleForListing.CreationDate,
                        LastTechnicalCheck = vehicleForListing.LastTechnicalCheck,
                        DoorCount = vehicleForListing.DoorCount,
                        Engine = vehicleForListing.Engine
                    };

                    _vehicleService.CreateNewCar(car);
                    return _dbRepoDapper.ExecuteCreateBillboardListingSQL(car.VehicleId, user.UserId, car.GetType().Name);
                }
                else if (vehicleForListing.DoorCount == 0 && vehicleForListing.CylinderVolume > 0)
                {
                    Motorbike motorbike = new Motorbike()
                    {
                        Make = vehicleForListing.Make,
                        Model = vehicleForListing.Model,
                        Price = vehicleForListing.Price,
                        CreationDate = vehicleForListing.CreationDate,
                        LastTechnicalCheck = vehicleForListing.LastTechnicalCheck,
                        CylinderVolume = vehicleForListing.CylinderVolume
                    };

                    _vehicleService.CreateNewMotorbike(motorbike);
                    return _dbRepoDapper.ExecuteCreateBillboardListingSQL(motorbike.VehicleId, user.UserId, motorbike.GetType().Name);
                }
                else
                {
                    throw new Exception("Validation failed. Can't make new vehicle record.");
                }
            }
            else
                return false;
        }

        public IEnumerable<BillboardListing?> GetListings() => _dbRepoDapper.ExecuteFetchBillboardListingRecordsSQL();

        public BillboardListing? GetListing(int id)
        {
            int recordCount = _dbRepoDapper.GetNumberOfBillboardListingsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(id);
            }
            else
                return null;
        }

        public IEnumerable<BillboardListingDTO?> GetListingsDTO() => _dbRepoDapper.ExecuteFetchBillboardListingDetailsAsDTOSQL();

        public BillboardListingDTO? GetListingDTO(int id)
        {
            int recordCount = _dbRepoDapper.GetNumberOfBillboardListingsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteFetchSpecificBillboardListingDetailsAsDTOSQL(id);
            }
            else
                return null;
        }

        public bool UpdateListing(string username, string password, int listingId, VehicleDTO vehicleToUpdate)
        {
            User? user = _userService.UserLoginService(username, password);
            BillboardListing? billboardListing = _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(listingId);
            if (user != null && user.UserId == billboardListing.UserId)
            {
                if (vehicleToUpdate.DoorCount > 0 && vehicleToUpdate.CylinderVolume == 0)
                {
                    return _vehicleService.UpdateCarById(billboardListing.VehicleId, new Car()
                    {
                        Make = vehicleToUpdate.Make,
                        Model = vehicleToUpdate.Model,
                        Price = vehicleToUpdate.Price,
                        CreationDate = vehicleToUpdate.CreationDate,
                        LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck,
                        DoorCount = vehicleToUpdate.DoorCount,
                        Engine = vehicleToUpdate.Engine
                    });
                }
                else if (vehicleToUpdate.DoorCount == 0 && vehicleToUpdate.CylinderVolume > 0)
                {
                    return _vehicleService.UpdateMotorbikeById(billboardListing.VehicleId, new Motorbike()
                    {
                        Make = vehicleToUpdate.Make,
                        Model = vehicleToUpdate.Model,
                        Price = vehicleToUpdate.Price,
                        CreationDate = vehicleToUpdate.CreationDate,
                        LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck,
                        CylinderVolume = vehicleToUpdate.CylinderVolume
                    });
                }
                else
                {
                    throw new Exception("Validation failed. Nothing was updated.");
                }
            }
            else
                return false;
        }

        public bool DeleteListing(string username, string password, int id)
        {
            User? user = _userService.UserLoginService(username, password);
            if (user != null)
            {
                BillboardListing? listing = _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(id);
                if (listing != null && listing.UserId == user.UserId)
                {
                    return _dbRepoDapper.ExecuteDeleteBillboardListingRecordByIdSQL(id, listing.VehicleId);
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion

        public IEnumerable<BillboardListingDTO?> SearchInTheListings(string srchString)
        {
            Regex regex = new Regex(srchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool priceToSearch = decimal.TryParse(srchString, out decimal priceSearch);
            bool dateToSearch = DateTime.TryParse(srchString, out DateTime dateSearch);

            IEnumerable<BillboardListingDTO?> billboardListings = _dbRepoDapper.ExecuteFetchBillboardListingDetailsAsDTOSQL();

            if (priceToSearch)
            {
                return billboardListings.Where(l => l?.Price == priceSearch);
            }
            else if(dateToSearch)
            {
                return billboardListings.Where(l => l?.CreationDate == dateSearch);
            }
            else
            {
                switch(srchString)
                {
                    case "Car":
                        return billboardListings.Where(l => l?.ListingType == typeof(Car).Name);
                    case "Motorbike":
                        return billboardListings.Where(l => l?.ListingType == typeof(Motorbike).Name);
                    default:
                        return billboardListings.Where(l => regex.IsMatch(l?.Make)|| regex.IsMatch(l?.Model));
                }
            }
        }

        public IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMin()
        {
            IEnumerable<BillboardListingDTO?> billboardListings = _dbRepoDapper.ExecuteFetchBillboardListingDetailsAsDTOSQL();

            return billboardListings.OrderBy(l => l.Price);
        }

        public IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMax()
        {
            IEnumerable<BillboardListingDTO?> billboardListings = _dbRepoDapper.ExecuteFetchBillboardListingDetailsAsDTOSQL();

            return billboardListings.OrderByDescending(l => l.Price);
        }

        //public IEnumerable<BillboardListingDTO?> SearchInTheListingsByPriceRange(string srchString)
        //{
        //    Regex regex = new Regex(@"(\d+(\.\d+)?)\s*-\s*(\d+(\.\d+)?)", RegexOptions.Compiled);
        //    Match match = regex.Match(srchString);

        //    if (match.Success)
        //    {
        //        decimal minPrice = decimal.Parse(match.Groups[1].Value);
        //        decimal maxPrice = decimal.Parse(match.Groups[3].Value);

        //        IEnumerable<BillboardListingDTO?> billboardListings = _dbRepoDapper.ExecuteFetchBillboardListingDetailsAsDTOSQL();

        //        return billboardListings.Where(l => l?.Price >= minPrice && l?.Price <= maxPrice).OrderDescending();
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Invalid price range format. Please provide a valid range in the format 'minPrice - maxPrice'.");
        //    }
        //}
    }
}
