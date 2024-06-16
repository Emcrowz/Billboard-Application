using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Billboard_BackEnd.Repositories
{
    public class BillboardListingLocalRepository : IBillboardListingDapperContext
    {
        #region SETUP / INITIALISATION
        readonly IDbConnection _dbConnectionLocal;

        public BillboardListingLocalRepository(string localDbConnectionString)
        {
            _dbConnectionLocal = new SqlConnection(localDbConnectionString);
        }
        #endregion

        #region DAPPER CRUD
        public bool ExecuteCreateBillboardListingSQL(BillboardListing newListing)
        {
            bool action;
            action = _dbConnectionLocal.Execute($"INSERT INTO BillboardListings ( [VehicleId], [UserId], [ListingType]  ) VALUES ( '{newListing.VehicleId}', '{newListing.UserId}', '{newListing.ListingType}' )") > 0;
            int additionIndex = _dbConnectionLocal.QuerySingle<int>($"SELECT MAX(VehicleId) FROM Vehicles");
            newListing.ListingId = additionIndex;
            return action;
        }

        public BillboardListingDTO? ExecuteFetchSpecificBillboardListingDetailsAsDTOSQL(int listingId)
        {
            return _dbConnectionLocal.QuerySingleOrDefault<BillboardListingDTO>(@"SELECT [ListingId], [ListingType], Users.[UserId], [FirstName], [LastName], [Email], Vehicles.[VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [DoorCount], [Engine], [CylinderVolume] FROM BillboardListings
FULL OUTER JOIN Users ON Users.UserId = BillboardListings.UserId
FULL OUTER JOIN Vehicles ON Vehicles.VehicleId = BillboardListings.VehicleId
FULL OUTER JOIN Cars ON Vehicles.VehicleId = Cars.CarId
FULL OUTER JOIN Motorbikes ON Vehicles.VehicleId = Motorbikes.MotorbikeId
WHERE [ListingId] = " + $"{listingId};");
        }

        public IEnumerable<BillboardListingDTO> ExecuteFetchBillboardListingDetailsAsDTOSQL()
        {
            return _dbConnectionLocal.Query<BillboardListingDTO>(@"SELECT [ListingId], [ListingType], Users.[UserId], [FirstName], [LastName], [Email], Vehicles.[VehicleId], [Make], [Model], [Price], [CreationDate], [LastTechnicalCheck], [DoorCount], [Engine], [CylinderVolume] FROM BillboardListings
FULL OUTER JOIN Users ON Users.UserId = BillboardListings.UserId
FULL OUTER JOIN Vehicles ON Vehicles.VehicleId = BillboardListings.VehicleId
FULL OUTER JOIN Cars ON Vehicles.VehicleId = Cars.CarId
FULL OUTER JOIN Motorbikes ON Vehicles.VehicleId = Motorbikes.MotorbikeId;");
        }

        public BillboardListing? ExecuteFetchBillboardListingRecordByIdSQL(int id)
        {
            return _dbConnectionLocal.QuerySingleOrDefault<BillboardListing>($"SELECT [UserId], [VehicleId], [ListingType] FROM BillboardListings WHERE [ListingId] = {id}");
        }

        public IEnumerable<BillboardListing> ExecuteFetchBillboardListingRecordsSQL()
        {
            return _dbConnectionLocal.Query<BillboardListing>($"SELECT [ListingId], [UserId], [VehicleId], [ListingType] FROM BillboardListings");
        }

        public bool ExecuteDeleteBillboardListingRecordByIdSQL(int id, int vehicleId)
        {
            _dbConnectionLocal.Execute($"DELETE FROM Cars WHERE [CarId] = {vehicleId}");
            _dbConnectionLocal.Execute($"DELETE FROM Motorbikes WHERE [MotorbikeId] = {vehicleId}");
            _dbConnectionLocal.Execute($"DELETE FROM Vehicles WHERE [VehicleId] = {vehicleId}");
            return _dbConnectionLocal.Execute($"DELETE FROM BillboardListings WHERE [ListingId] = {id}") > 0;
        }
        #endregion

        #region HELPER OPERATIONS
        public int GetNumberOfBillboardListingsInDb()
        {
            return _dbConnectionLocal.QuerySingle<int>("SELECT COUNT(ListingId) FROM BillboardListings");
        }
        #endregion
    }
}
