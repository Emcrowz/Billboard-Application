using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
        [HttpGet("Listings")] // Serves from MONGO DB
        public async Task<IActionResult> Listings()
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records.");
            List<BillboardListingDTO?> listings = await _billboardService.GetListingsFromMongoAsync();
            if (listings != null)
            {
                Log.Information($"Successfully read records from Mongo DB. Count of records: {listings.Count()}");
                return Ok(listings);
            }
            else
            {
                Log.Error($"Failed to fetch {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }

        [HttpGet("Listings/{id}")]
        public async Task<IActionResult> ListingById(int id)
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records.");
            // BillboardListingDTO? listing = _billboardService.GetListingDTO(id); // <- To serve LocalDB
            List<BillboardListingDTO?> currentListings = await _billboardService.GetListingsFromMongoAsync();          
            try
            {
                BillboardListingDTO? searchedListing = currentListings.FirstOrDefault(l => l?.ListingId == id);
                if(searchedListing != null)
                {
                    ObjectId listingInternalId = searchedListing.InternalBillboardListingId; // Redundant search. Need more optimal solution - directly accessing ObjectId's string.

                    Log.Information($"Successfully read record with ID: {listingInternalId} from DB");
                    return Ok(await _billboardService.GetListingFromMongoByIdAsync(listingInternalId));
                }
                else
                {
                    Log.Error($"Failed to find {typeof(BillboardListingDTO).Name} record with ID: {id}");
                    return NotFound($"Record with {id} does not exist.");
                }
            }  
            catch (Exception)
            {
                Log.Error($"Error trying to search for a record with ID: {id}");
                return BadRequest();
            }
        }

        [HttpGet("Listings/Search")]
        public async Task<IActionResult> ListingsSearch(string srchString, bool srchByMinPrice, bool srchByMinDate)
        {
            bool srchAsPrice = decimal.TryParse(srchString, out decimal srchPrice);
            bool srchAsDate = DateTime.TryParse(srchString, out DateTime srchDate);
            List<BillboardListingDTO> results;

            Log.Information($"Attempt to search {typeof(BillboardListingDTO).Name} records with: [{srchString}].");
            // IEnumerable<BillboardListingDTO?> listings = _billboardService.SearchInTheListings(srchString); // <- Search the LocalDB

            if (srchAsPrice && srchByMinPrice)
            {
                results = await _billboardService.SearchByListedMinPriceAsync(srchPrice);
                Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Price\"] Count of records: {results.Count()}");
                return Ok(results);
            }
            else if (srchAsPrice && !srchByMinPrice)
            {
                results = await _billboardService.SearchByListedMaxPriceAsync(srchPrice);
                Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Price\"] Count of records: {results.Count()}");
                return Ok(results);
            }
            else if (srchAsDate && srchByMinDate)
            {
                results = await _billboardService.SearchByListedMinVehicleCreationDateAsync(srchDate);
                Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Date\"] Count of records: {results.Count()}");
                return Ok(results);
            }
            else if(srchAsDate && !srchByMinDate)
            {
                results = await _billboardService.SearchByListedMaxVehicleCreationDateAsync(srchDate);
                Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Date\"] Count of records: {results.Count()}");
                return Ok(results);
            }
            else
            {
                switch (srchString)
                {
                    case "":
                        Log.Error("Search attempt with empty string.");
                        return BadRequest("Empty string search");
                    case "Car":
                        results = await _billboardService.SearchByVehicleTypeAsync(srchString);
                        Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Type\"] Count of records: {results.Count()}");
                        return Ok(results);
                    case "Motorbike":
                        results = await _billboardService.SearchByVehicleTypeAsync(srchString);
                        Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Type\"] Count of records: {results.Count()}");
                        return Ok(results);
                    default:
                        results = await _billboardService.SearchByVehicleMakeOrModelAsync(srchString);
                        Log.Information($"Successfully completed search. Search string: [{srchString}], Search type: [\"Model Or Make\"] Count of records: {results.Count()}");
                        return Ok(results);
                } 
            }
        }

        [HttpPost("Listings")] // Serves both LocalDB (record keeping) and MongoDB (for users to interact)
        public async Task<IActionResult> CreateListing(string username, string password, VehicleDTO vehicleListing)
        {
            Log.Information($"Attempt to create {typeof(BillboardListing).Name} record.");
            if(await _billboardService.CreateListing(username, password, vehicleListing))
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
        public async Task<IActionResult> UpdateListing(string username, string password, int id, VehicleDTO vehicleToUpdate)
        {
            Log.Information($"Attempt to update {typeof(BillboardListing).Name} record with ID - [{id}].");
            if(await _billboardService.UpdateListing(username, password, id, vehicleToUpdate))
            {
                Log.Information($"Successfully updated record with ID: {id} inside DB");
                return Ok($"Listing with ID: {id} was successfully updated.");
            }
            else
            {
                Log.Error($"Failed to find {typeof(BillboardListingDTO).Name} record with ID: {id}");
                return NotFound($"Record with {id} does not exist.");
            }
        }

        [HttpDelete("Listings/{id}")]
        public async Task<IActionResult> DeleteListing(string username, string password, int id)
        {
            Log.Information($"Attempt to delete {typeof(BillboardListing).Name} record with ID - [{id}].");
            if (await _billboardService.DeleteListing(username, password, id))
            {
                Log.Information($"Successfully deleted record with ID: {id} from DB");
                return Ok($"Listing with ID: {id} was successfully deleted.");
            }
            else
            {
                Log.Error($"Failed to delete {typeof(BillboardListing).Name} record from DB with ID - [{id}]");
                return NotFound();
            }
        }
        #endregion
    }

    [ApiController]
    [Route("Mongo")]
    public class MongoController : ControllerBase
    {
        #region SETUP / INITIALISATION
        readonly IBillboardListingService _billboardService;

        public MongoController(IBillboardListingService billboardService)
        {
            _billboardService = billboardService;
        }
        #endregion

        #region MONGO
        [HttpGet("GetListingsFromDB")]
        public async Task<IActionResult> GetListingsToMongo()
        {
            Log.Information($"Attempt to fetch {typeof(BillboardListingDTO).Name} records to Mongo DB.");
            try
            {
                await _billboardService.GetListingsToMongoAsync();

                Log.Information($"Successfully fetched records from DB to Mongo DB.");
                return Ok();
            }
            catch (Exception)
            {
                Log.Error($"Failed fetching {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }

        [HttpDelete("DeleteListingsInMongo")]
        public async Task<IActionResult> DeleteListingsFromMongo()
        {
            Log.Information($"Attempt to delete {typeof(BillboardListingDTO).Name} records to Mongo DB.");
            try
            {
                await _billboardService.DeleteAllListingsMongoAsync();
                Log.Information($"Successfully deleted records from Mongo DB.");
                return Ok();
            }
            catch (Exception)
            {
                Log.Error($"Failed deleting {typeof(BillboardListingDTO).Name} records from the DB.");
                return NotFound();
            }
        }
        #endregion
    }
}
