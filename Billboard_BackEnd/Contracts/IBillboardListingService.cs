using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using MongoDB.Bson;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingService
    {
        #region LOCAL OPERATIONS
        Task<bool> CreateListing(string username, string password, VehicleDTO vehicleForListing);

        BillboardListing? GetListing(int id);
        IEnumerable<BillboardListing?> GetListings();
        BillboardListingDTO? GetListingDTO(int id);
        IEnumerable<BillboardListingDTO?> GetListingsDTO();

        Task<bool> UpdateListing(string username, string password, int listingId, VehicleDTO vehicleToUpdate);

        Task<bool> DeleteListing(string username, string password, int id);

        IEnumerable<BillboardListingDTO?> SearchInTheListings(string srchString);
        IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMin();
        IEnumerable<BillboardListingDTO?> SearchInTheListingByPriceFromMax();
        #endregion

        #region REMOTE | ASYNC OPERATIONS
        // Create
        Task CreateListingMongoAsync(BillboardListingDTO listingDTO);

        // Get / Fetch
        Task GetListingsToMongoAsync(); // Operation to get records TO MongoDB FROM LocalDB
        Task GetListingsToMongoAsync_Force(); // Operation to get records TO MongoDB FROM LocalDB. By 'Force'
        Task<List<BillboardListingDTO?>> GetListingsFromMongoAsync();
        Task<BillboardListingDTO?> GetListingFromMongoByIdAsync(ObjectId id);

        // Update / Edit
        Task UpdateListingMongoAsync(BillboardListingDTO listingDTO);

        // Delete
        Task DeleteListingMongoAsync(ObjectId id);
        Task DeleteAllListingsMongoAsync();

        // Search Operations
        Task<List<BillboardListingDTO>> SearchByVehicleMakeOrModelAsync(string srchString);
        Task<List<BillboardListingDTO>> SearchByVehicleTypeAsync(string srchString);
        Task<List<BillboardListingDTO>> SearchByListedMinPriceAsync(decimal srchString);
        Task<List<BillboardListingDTO>> SearchByListedMaxPriceAsync(decimal srchString);
        Task<List<BillboardListingDTO>> SearchByListedMinVehicleCreationDateAsync(DateTime srchString);
        Task<List<BillboardListingDTO>> SearchByListedMaxVehicleCreationDateAsync(DateTime srchString);
        #endregion
    }
}