using Billboard_BackEnd.ModelsDTO;
using MongoDB.Bson;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingMongoContext
    {
        // Create
        Task<bool> CreateBillboardListingMongoAsync(BillboardListingDTO? listingDTO);

        // Get
        Task<IEnumerable<BillboardListingDTO?>> FetchAllBillboardListingRecordsMongoAsync();
        Task<IEnumerable<BillboardListingDTO?>> FetchAllBillboardListingsRecordsMongoAsync_Force();

        Task<List<BillboardListingDTO?>> GetAllBillboardListingsMongoAsync();
        Task<BillboardListingDTO?> GetBillboardListingByIdMongoAsync(ObjectId id);

        // Update
        Task UpdateBillboardListingMongoAsync(BillboardListingDTO? listingDTO);

        // Delete
        Task DeleteBillboardListingMongoAsync(ObjectId id);
        Task DeleteAllBillboardListingRecordsMongoAsync();

        // Search Queries
        Task<List<BillboardListingDTO>> SearchBillboardListingsByVehicleMakeOrModelAsync(string srchString);
        Task<List<BillboardListingDTO>> SearchBillboardListingsByVehicleTypeAsync(string srchString);
        Task<List<BillboardListingDTO>> SearchBillboardListingsByMinimumListingPriceAsync(decimal srchPrice);
        Task<List<BillboardListingDTO>> SearchBillboardListingsByMaximumListingPriceAsync(decimal srchPrice);
        Task<List<BillboardListingDTO>> SearchBillboardListingsByMinimumVehicleCreationTimeAsync(DateTime srchDate);
        Task<List<BillboardListingDTO>> SearchBillboardListingsByMaximumVehicleCreationTimeAsync(DateTime srchDate);
    }
}