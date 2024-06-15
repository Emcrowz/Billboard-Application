using Billboard_BackEnd.ModelsDTO;
using MongoDB.Bson;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingMongoContext
    {
        // Create
        Task CreateBillboardListingMongoAsync(BillboardListingDTO listingDTO);

        // Get
        Task<IEnumerable<BillboardListingDTO>> FetchAllBillboardListingRecordsMongoAsync();
        Task<IEnumerable<BillboardListingDTO>> FetchAllBillboardListingsRecordsMongoAsync_Force();

        Task<List<BillboardListingDTO>> GetAllBillboardListingsMongoAsync();
        Task<BillboardListingDTO> GetBillboardListingByIdMongoAsync(ObjectId id);

        // Update
        Task UpdateBillboardListingMongoAsync(BillboardListingDTO listingDTO);

        // Delete
        Task DeleteBillboardListingMongoAsync(ObjectId id);
        Task DeleteAllBillboardListingRecordsMongoAsync();
    }
}