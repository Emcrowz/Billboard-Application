using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Billboard_BackEnd.Repositories;
using MongoDB.Bson;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Billboard_BackEnd.Services
{
    public class BillboardListingService : IBillboardListingService
    {
        #region SETUP / INITIALISATION
        private readonly IUserService _userService;
        private readonly IVehicleService _vehicleService;
        private readonly IBillboardListingDapperContext _dbRepoDapper; // LocalDB
        private readonly IBillboardListingMongoContext _dbRepoMongo; // RemoteDB - Mongo

        public BillboardListingService(string localConnectionString, string remoteConnectionString)
        {
            _userService = new UserService(localConnectionString);
            _vehicleService = new VehicleService(localConnectionString);
            _dbRepoDapper = new BillboardListingLocalRepository(localConnectionString);
            _dbRepoMongo = new BillboardListingMongoRepository(remoteConnectionString, localConnectionString);
        }
        #endregion

        #region SERVICES
        public async Task<bool> CreateListing(string username, string password, VehicleDTO vehicleForListing)
        {
            User? user = _userService.UserLoginService(username, password); // LocalDB will serve the client.
            if (user != null)
            {
                User? userCreds = _userService.GetUserById(user.UserId);
                // Enum.GetNames(typeof(EngineType)).Length
                if (vehicleForListing.DoorCount > 0 && vehicleForListing.CylinderVolume == 0)
                {
                    Car car = new()
                    {
                        Make = vehicleForListing.Make,
                        Model = vehicleForListing.Model,
                        Price = vehicleForListing.Price,
                        CreationDate = vehicleForListing.CreationDate,
                        LastTechnicalCheck = vehicleForListing.LastTechnicalCheck,
                        DoorCount = vehicleForListing.DoorCount,
                        Engine = vehicleForListing.Engine
                    };

                    _vehicleService.CreateNewCar(car); // Local DB Serve

                    BillboardListing newListing = new()
                    {
                        VehicleId = car.VehicleId,
                        UserId = user.UserId,
                        ListingType = car.GetType().Name
                    };

                    _dbRepoDapper.ExecuteCreateBillboardListingSQL(newListing);

                    // Sends listing to MONGO.
                    await CreateListingMongoAsync(new BillboardListingDTO()
                    {
                        ListingId = newListing.ListingId,
                        ListingType = car.GetType().Name,
                        UserId = user.UserId,
                        FirstName = userCreds.FirstName,
                        LastName = userCreds.LastName,
                        Email = userCreds.Email,
                        VehicleId = car.VehicleId,
                        Make = car.Make,
                        Model = car.Model,
                        Price = car.Price,
                        CreationDate = car.CreationDate,
                        LastTechnicalCheck = car.LastTechnicalCheck,
                        DoorCount = car.DoorCount,
                        Engine = car.Engine
                    });
                    return true;
                }
                else if (vehicleForListing.DoorCount == 0 && vehicleForListing.CylinderVolume > 0)
                {
                    Motorbike motorbike = new()
                    {
                        Make = vehicleForListing.Make,
                        Model = vehicleForListing.Model,
                        Price = vehicleForListing.Price,
                        CreationDate = vehicleForListing.CreationDate,
                        LastTechnicalCheck = vehicleForListing.LastTechnicalCheck,
                        CylinderVolume = vehicleForListing.CylinderVolume
                    };

                    _vehicleService.CreateNewMotorbike(motorbike);

                    BillboardListing newListing = new()
                    {
                        VehicleId = motorbike.VehicleId,
                        UserId = user.UserId,
                        ListingType = motorbike.GetType().Name
                    };

                    _dbRepoDapper.ExecuteCreateBillboardListingSQL(newListing);

                    // Sends listing to MONGO.
                    await CreateListingMongoAsync(new BillboardListingDTO()
                    {
                        ListingId = newListing.ListingId,
                        ListingType = motorbike.GetType().Name,
                        UserId = user.UserId,
                        FirstName = userCreds.FirstName,
                        LastName = userCreds.LastName,
                        Email = userCreds.Email,
                        VehicleId = motorbike.VehicleId,
                        Make = motorbike.Make,
                        Model = motorbike.Model,
                        Price = motorbike.Price,
                        CreationDate = motorbike.CreationDate,
                        LastTechnicalCheck = motorbike.LastTechnicalCheck,
                        CylinderVolume = motorbike.CylinderVolume
                    });
                    return true;
                }
                else
                {
                    throw new Exception("Bad data provided. Can't make new vehicle and listing record.");
                }
            }
            else
                return false;
        }

        public IEnumerable<BillboardListing?> GetListings() => _dbRepoDapper.ExecuteFetchBillboardListingRecordsSQL();

        public BillboardListing? GetListing(int id)
        {
            try
            {
                return _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(id);
            }
            catch(ArgumentOutOfRangeException)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error with the request./nMessage: [{ex.Message}]/nSource: [{ex.Source}]");
            }
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

        public async Task<bool> UpdateListing(string username, string password, int id, VehicleDTO vehicleToUpdate)
        {
            User? user = _userService.UserLoginService(username, password);
            //BillboardListing? billboardListing = _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(listingId); // <- Serving for LocalDB.


            List<BillboardListingDTO?> currentListings = await _dbRepoMongo.GetAllBillboardListingsMongoAsync();
            BillboardListingDTO? listingToAlter = currentListings.FirstOrDefault(l => l?.ListingId == id);
            if (listingToAlter != null)
            {
                ObjectId listingInternalId = listingToAlter.InternalBillboardListingId; // For sending to Mongo.

                if (user != null && user.UserId == listingToAlter.UserId)
                {
                    if (vehicleToUpdate.DoorCount > 0 && vehicleToUpdate.CylinderVolume == 0)
                    {
                        // Update Localy.
                        _vehicleService.UpdateCarById(listingToAlter.VehicleId, new Car()
                        {
                            Make = vehicleToUpdate.Make,
                            Model = vehicleToUpdate.Model,
                            Price = vehicleToUpdate.Price,
                            CreationDate = vehicleToUpdate.CreationDate,
                            LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck,
                            DoorCount = vehicleToUpdate.DoorCount,
                            Engine = vehicleToUpdate.Engine
                        });

                        // For Mongo UPDATE
                        BillboardListingDTO listingToUpdate = new()
                        {
                            ListingId = listingToAlter.ListingId, // Same
                            ListingType = listingToAlter.ListingType, // Same
                            UserId = listingToAlter.UserId, // Same
                            FirstName = listingToAlter.FirstName, // Same
                            LastName = listingToAlter.LastName, // Same
                            Email = listingToAlter.Email, // Same
                            VehicleId = listingToAlter.VehicleId, // Same
                            Make = vehicleToUpdate.Make, // Alter
                            Model = vehicleToUpdate.Model, // Alter
                            Price = vehicleToUpdate.Price, // Alter
                            CreationDate = vehicleToUpdate.CreationDate, // Alter
                            LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck, // Alter
                            DoorCount = vehicleToUpdate.DoorCount, // Alter
                            Engine = vehicleToUpdate.Engine, // Alter
                            InternalBillboardListingId = listingToAlter.InternalBillboardListingId // Must be Same.
                        };

                        await _dbRepoMongo.UpdateBillboardListingMongoAsync(listingToUpdate);
                        return true;
                    }
                    else if (vehicleToUpdate.DoorCount == 0 && vehicleToUpdate.CylinderVolume > 0)
                    {
                        // Update Localy
                        _vehicleService.UpdateMotorbikeById(listingToAlter.VehicleId, new Motorbike()
                        {
                            Make = vehicleToUpdate.Make,
                            Model = vehicleToUpdate.Model,
                            Price = vehicleToUpdate.Price,
                            CreationDate = vehicleToUpdate.CreationDate,
                            LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck,
                            CylinderVolume = vehicleToUpdate.CylinderVolume
                        });

                        // For Mongo UPDATE
                        BillboardListingDTO listingToUpdate = new()
                        {
                            ListingId = listingToAlter.ListingId, // Same
                            ListingType = listingToAlter.ListingType, // Same
                            UserId = listingToAlter.UserId, // Same
                            FirstName = listingToAlter.FirstName, // Same
                            LastName = listingToAlter.LastName, // Same
                            Email = listingToAlter.Email, // Same
                            VehicleId = listingToAlter.VehicleId, // Same
                            Make = vehicleToUpdate.Make, // Alter
                            Model = vehicleToUpdate.Model, // Alter
                            Price = vehicleToUpdate.Price, // Alter
                            CreationDate = vehicleToUpdate.CreationDate, // Alter
                            LastTechnicalCheck = vehicleToUpdate.LastTechnicalCheck, // Alter
                            CylinderVolume = vehicleToUpdate.CylinderVolume, // Alter
                            InternalBillboardListingId = listingToAlter.InternalBillboardListingId // Must be Same.
                        };

                        await _dbRepoMongo.UpdateBillboardListingMongoAsync(listingToUpdate);
                        return true;
                    }
                    else
                    {
                        throw new Exception("Validation failed. Nothing was updated.");
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        public async Task<bool> DeleteListing(string username, string password, int id)
        {
            User? user = _userService.UserLoginService(username, password);
            if (user != null)
            {
                // BillboardListing? listing = _dbRepoDapper.ExecuteFetchBillboardListingRecordByIdSQL(id);
                
                List<BillboardListingDTO?> currentListings = await _dbRepoMongo.GetAllBillboardListingsMongoAsync();
                BillboardListingDTO? listingToDelete = currentListings.FirstOrDefault(l => l?.ListingId == id);


                if (listingToDelete != null && listingToDelete.UserId == user.UserId)
                {
                    ObjectId listingInternalId = listingToDelete.InternalBillboardListingId; // For sending to Mongo.

                    // _dbRepoDapper.ExecuteDeleteBillboardListingRecordByIdSQL(id, listing.VehicleId); // Delete locally.

                    await _dbRepoMongo.DeleteBillboardListingMongoAsync(listingInternalId);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        

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
        #endregion

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

        #region MONGO
        // Create
        public async Task CreateListingMongoAsync(BillboardListingDTO listingDTO) => await _dbRepoMongo.CreateBillboardListingMongoAsync(listingDTO);

        // Get / Fetch
        public async Task GetListingsToMongoAsync() => await _dbRepoMongo.FetchAllBillboardListingRecordsMongoAsync();
        public async Task GetListingsToMongoAsync_Force() => await _dbRepoMongo.FetchAllBillboardListingsRecordsMongoAsync_Force();
        public async Task<List<BillboardListingDTO?>> GetListingsFromMongoAsync() => await _dbRepoMongo.GetAllBillboardListingsMongoAsync();
        public async Task<BillboardListingDTO?> GetListingFromMongoByIdAsync(ObjectId id) => await _dbRepoMongo.GetBillboardListingByIdMongoAsync(id);

        // Update / Edit
        public async Task UpdateListingMongoAsync(BillboardListingDTO listingDTO) => await _dbRepoMongo.UpdateBillboardListingMongoAsync(listingDTO);

        // Delete
        public async Task DeleteListingMongoAsync(ObjectId id) => await _dbRepoMongo.DeleteBillboardListingMongoAsync(id);

        public async Task DeleteAllListingsMongoAsync() => await _dbRepoMongo.DeleteAllBillboardListingRecordsMongoAsync();

        // Search Operations
        public async Task<List<BillboardListingDTO>> SearchByVehicleMakeOrModelAsync(string srchString) => await _dbRepoMongo.SearchBillboardListingsByVehicleMakeOrModelAsync(srchString);
        public async Task<List<BillboardListingDTO>> SearchByVehicleTypeAsync(string srchString) => await _dbRepoMongo.SearchBillboardListingsByVehicleTypeAsync(srchString);
        public async Task<List<BillboardListingDTO>> SearchByListedMinPriceAsync(decimal srchString) => await _dbRepoMongo.SearchBillboardListingsByMinimumListingPriceAsync(srchString);
        public async Task<List<BillboardListingDTO>> SearchByListedMaxPriceAsync(decimal srchString) => await _dbRepoMongo.SearchBillboardListingsByMaximumListingPriceAsync(srchString);
        public async Task<List<BillboardListingDTO>> SearchByListedMinVehicleCreationDateAsync(DateTime srchString) => await _dbRepoMongo.SearchBillboardListingsByMinimumVehicleCreationTimeAsync(srchString);
        public async Task<List<BillboardListingDTO>> SearchByListedMaxVehicleCreationDateAsync(DateTime srchString) => await _dbRepoMongo.SearchBillboardListingsByMaximumVehicleCreationTimeAsync(srchString);
        #endregion
    }
}
