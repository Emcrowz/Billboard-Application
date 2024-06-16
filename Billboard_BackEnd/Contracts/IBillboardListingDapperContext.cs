using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingDapperContext
    {
        // Create
        bool ExecuteCreateBillboardListingSQL(BillboardListing newListing);

        // Read / Get
        BillboardListing? ExecuteFetchBillboardListingRecordByIdSQL(int id);
        IEnumerable<BillboardListing> ExecuteFetchBillboardListingRecordsSQL();
        BillboardListingDTO? ExecuteFetchSpecificBillboardListingDetailsAsDTOSQL(int listingId);
        IEnumerable<BillboardListingDTO> ExecuteFetchBillboardListingDetailsAsDTOSQL();

        // Delete
        bool ExecuteDeleteBillboardListingRecordByIdSQL(int id, int vehicleId);

        // Helpers
        int GetNumberOfBillboardListingsInDb();

        // BillboardListing Specific Actions

    }
}