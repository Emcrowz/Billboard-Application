using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Billboard_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillboardListingController : ControllerBase
    {
        #region SETUP / INITIALISATION
        readonly IBillboardListingService _billboardService;

        public BillboardListingController(IBillboardListingService billboardService)
        {
            _billboardService = billboardService;
        }
        #endregion

        #region CORE OPERATIONS
        [HttpGet("Listings")]
        public IActionResult Listings()
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records.");
            IEnumerable<BillboardListingDTO?> listings = _billboardService.GetListingsDTO();
            if (listings != null)
            {
                Log.Information($"Successfully read records from DB. Count of records: {listings.Count()}");
                return Ok(listings);
            }
            else
            {
                Log.Error($"Failed to fetch {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }

        [HttpGet("Listings/{id}")]
        public IActionResult ListingById(int id)
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records.");
            BillboardListingDTO? listing = _billboardService.GetListingDTO(id);
            if (listing != null)
            {
                Log.Information($"Successfully read record with ID: {id} from DB");
                return Ok(listing);
            }
            else
            {
                Log.Error($"Failed to recover {typeof(BillboardListingDTO).Name} record from DB with ID: {id}");
                return NotFound();
            }
        }

        [HttpGet("Listings/Search")]
        public IActionResult ListingsSearch(string srchString)
        {
            Log.Information($"Attempt to search {typeof(BillboardListingDTO).Name} records with: [{srchString}].");
            IEnumerable<BillboardListingDTO?> listings = _billboardService.SearchInTheListings(srchString);
            if (listings != null)
            {
                Log.Information($"Successfully completed search. Search string: [{srchString}] Count of records: {listings.Count()}");
                return Ok(listings);
            }
            else
            {
                Log.Error($"Failed to search {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }

        [HttpGet("Listings/Sort/FromMin")]
        public IActionResult ListingsSortPriceFromMin()
        {
            Log.Information($"Attempt to sort {typeof(BillboardListingDTO).Name} records by ascending order");
            IEnumerable<BillboardListingDTO?> listings = _billboardService.SearchInTheListingByPriceFromMin();

            if (listings != null)
            {
                Log.Information($"Successfully completed sorting by ascending order.");
                return Ok(listings);
            }
            else
            {
                Log.Error($"Failed to sort {typeof(BillboardListingDTO).Name} records.");
                return NotFound();
            }
        }

        [HttpGet("Listings/Sort/FromMax")]
        public IActionResult ListingsSortPriceFromMax()
        {
            Log.Information($"Attempt to sort {typeof(BillboardListingDTO).Name} records by descending order.");
            IEnumerable<BillboardListingDTO?> listings = _billboardService.SearchInTheListingByPriceFromMax();

            if (listings != null)
            {
                Log.Information($"Successfully completed sorting by ascending order.");
                return Ok(listings);
            }
            else
            {
                Log.Error($"Failed to sort {typeof(BillboardListingDTO).Name} records.");
                return NotFound();
            }
        }

        [HttpPost("Listings")]
        public IActionResult CreateListing(string username, string password, VehicleDTO vehicleListing)
        {
            Log.Information($"Attempt to create {typeof(BillboardListing).Name} record.");
            if(_billboardService.CreateListing(username, password, vehicleListing))
            {
                Log.Information($"Successfully created record inside DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to create {typeof(BillboardListing).Name} record inside DB");
                return NotFound();
            }
        }

        [HttpPut("Listings/{id}")]
        public IActionResult UpdateListing(string username, string password, int id, VehicleDTO vehicleToUpdate)
        {
            Log.Information($"Attempt to update {typeof(BillboardListing).Name} record with ID - [{id}].");
            if(_billboardService.UpdateListing(username, password, id, vehicleToUpdate))
            {
                Log.Information($"Successfully updated record with ID: {id} inside DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to update {typeof(BillboardListing).Name} record inside DB with ID - [{id}]");
                return BadRequest();
            }
        }

        [HttpDelete("Listings/{id}")]
        public IActionResult DeleteListing(string username, string password, int id)
        {
            Log.Information($"Attempt to delete {typeof(BillboardListing).Name} record with ID - [{id}].");
            if (_billboardService.DeleteListing(username, password, id))
            {
                Log.Information($"Successfully deleted record with ID: {id} from DB");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to delete {typeof(BillboardListing).Name} record from DB with ID - [{id}]");
                return NotFound();
            }
        }
        #endregion

        #region MONGO
        [HttpGet("Mongo/Listings")]
        public async Task<IActionResult> GetListingsToMongo()
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records to Mongo DB.");
            try
            {
                await _billboardService.GetListingsToMongo();

                Log.Information($"Successfully fetched records from DB to Mongo DB.");
                return Ok();
            }
            catch(Exception)
            {
                Log.Error($"Failed fetching {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }
        #endregion
    }
}
