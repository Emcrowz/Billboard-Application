using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using MongoDB.Bson;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingService
    {
        #region LOCAL OPERATIONS
        bool CreateListing(string username, string password, VehicleDTO vehicleForListing);

        BillboardListing? GetListing(int id);
        IEnumerable<BillboardListing?> GetListings();
        BillboardListingDTO? GetListingDTO(int id);
        IEnumerable<BillboardListingDTO?> GetListingsDTO();

        bool UpdateListing(string username, string password, int listingId, VehicleDTO vehicleToUpdate);

        bool DeleteListing(string username, string password, int id);

        IEnumerable<BillboardListingDTO?> SearchInTheListings(string srchString);
        IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMin();
        IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMax();
        #endregion

        #region REMOTE | ASYNC OPERATIONS
        // Create
        Task CreateListingMongo(BillboardListingDTO listingDTO);

        // Get / Fetch
        Task GetListingsToMongo();
        Task GetListingsToMongo_Force();
        Task GetListingsFromMongo();
        Task GetListingFromMongoById(ObjectId id);

        // Update / Edit
        Task UpdateListingMongo(BillboardListingDTO listingDTO);

        // Delete
        Task DeleteListingMongo(ObjectId id);
        Task DeleteAllListingsMongo();
        #endregion
    }
}