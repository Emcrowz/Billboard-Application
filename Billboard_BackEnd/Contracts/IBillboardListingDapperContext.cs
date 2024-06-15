using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingDapperContext
    {
        // Create
        bool ExecuteCreateBillboardListingSQL(int vehicleId, int userId, string listingType);

        // Read / Get
        BillboardListing? ExecuteFetchBillboardListingRecordByIdSQL(int id);
        IEnumerable<BillboardListing> ExecuteFetchBillboardListingRecordsSQL();
        BillboardListingDTO? ExecuteFetchSpecificBillboardListingDetailsAsDTOSQL(int listingId);
        IEnumerable<BillboardListingDTO> ExecuteFetchBillboardListingDetailsAsDTOSQL();

        // Update
        //bool ExecuteUpdateBillboardListingRecordByIdSQL(int id, BillboardListing billboardListingUpdate);

        // Delete
        bool ExecuteDeleteBillboardListingRecordByIdSQL(int id, int vehicleId);

        // Helpers
        int GetNumberOfBillboardListingsInDb();

        // BillboardListing Specific Actions

    }
}