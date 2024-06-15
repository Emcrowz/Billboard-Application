using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;

namespace Billboard_BackEnd.Contracts
{
    public interface IBillboardListingService
    {
        bool CreateListing(string username, string password, VehicleDTO vehicleForListing);

        BillboardListing? GetListing(int id);
        IEnumerable<BillboardListing?> GetListings();
        BillboardListingDTO? GetListingDTO(int id);
        IEnumerable<BillboardListingDTO?> GetListingsDTO();

        bool UpdateListing(string username, string password, int listingId, VehicleDTO vehicleToUpdate);

        bool DeleteListing(string username, string password, int id);
    }
}