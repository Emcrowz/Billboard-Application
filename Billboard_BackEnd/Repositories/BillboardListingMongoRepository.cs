using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.ModelsDTO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Billboard_BackEnd.Repositories
{
    public class BillboardListingMongoRepository : IBillboardListingMongoContext
    {
        #region SETUP / INITIALISATION
        readonly IMongoClient _mongoClient;
        readonly IMongoDatabase _mongoDb;
        readonly IMongoCollection<BillboardListingDTO> _listingDTOMongo;

        readonly IBillboardListingDapperContext _localDbRepo; // Local DB
        public BillboardListingMongoRepository(string remoteDbConnectionString, string localConnectionString)
        {
            _mongoClient = new MongoClient(remoteDbConnectionString);
            _mongoDb = _mongoClient.GetDatabase("BillboardCluster");
            _listingDTOMongo = _mongoDb.GetCollection<BillboardListingDTO>("BillboardListings");

            _localDbRepo = new BillboardListingLocalRepository(localConnectionString); // Local DB
        }
        #endregion

        #region MONGO CRUD
        // Create
        public async Task<bool> CreateBillboardListingMongoAsync(BillboardListingDTO listingDTO)
        {
            try
            {
                await _listingDTOMongo.InsertOneAsync(listingDTO);
                return true;
            }
            catch(Exception)
            {
                throw new Exception("Could not insert record to MongoDB.");
            }
        }

        // Get
        public async Task<IEnumerable<BillboardListingDTO?>> FetchAllBillboardListingRecordsMongoAsync()
        {
            List<BillboardListingDTO> billboardListings = await GetAllBillboardListingsMongoAsync();

            if (billboardListings == null || !billboardListings.Any())
            {
                billboardListings = _localDbRepo.ExecuteFetchBillboardListingDetailsAsDTOSQL().ToList();

                foreach (BillboardListingDTO rent in billboardListings)
                {
                    await CreateBillboardListingMongoAsync(rent);
                }
            }

            return billboardListings;
        }

        public async Task<IEnumerable<BillboardListingDTO?>> FetchAllBillboardListingsRecordsMongoAsync_Force()
        {
            List<BillboardListingDTO> billboardListings = await GetAllBillboardListingsMongoAsync();

            billboardListings = _localDbRepo.ExecuteFetchBillboardListingDetailsAsDTOSQL().ToList();

            foreach (BillboardListingDTO rent in billboardListings)
            {
                await CreateBillboardListingMongoAsync(rent);
            }
            
            return billboardListings;
        }

        public async Task<List<BillboardListingDTO>> GetAllBillboardListingsMongoAsync() => await _listingDTOMongo.Find(_ => true).ToListAsync();


        public async Task<BillboardListingDTO> GetBillboardListingByIdMongoAsync(ObjectId id) => await _listingDTOMongo.Find(l => l.InternalBillboardListingId == id).FirstOrDefaultAsync();


        // Update
        public async Task UpdateBillboardListingMongoAsync(BillboardListingDTO listingDTO) => await _listingDTOMongo.ReplaceOneAsync(l => l.InternalBillboardListingId == listingDTO.InternalBillboardListingId, listingDTO);

        // Delete
        public async Task DeleteBillboardListingMongoAsync(ObjectId id) => await _listingDTOMongo.DeleteOneAsync(l => l.InternalBillboardListingId == id);

        public async Task DeleteAllBillboardListingRecordsMongoAsync() => await _listingDTOMongo.DeleteManyAsync(new BsonDocument());
        #endregion

        #region SEARCH OPERATIONS
        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByVehicleMakeOrModelAsync(string srchString)
        {
            FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Regex("Make", new BsonRegularExpression(srchString, "i")) | Builders<BillboardListingDTO>.Filter.Regex("Model", new BsonRegularExpression(srchString, "i"));

            return await _listingDTOMongo.Find(filter).ToListAsync();
        }

        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByVehicleTypeAsync(string srchString)
        {
            FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Regex("ListingType", new BsonRegularExpression(srchString, "i"));

            return await _listingDTOMongo.Find(filter).ToListAsync();
        }

        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByMinimumListingPriceAsync(decimal srchPrice)
        {
            if (srchPrice > 0)
            {
                FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Gte("ListingType", srchPrice);

                return await _listingDTOMongo.Find(filter).ToListAsync();
            }
            else
                return [];
        }

        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByMaximumListingPriceAsync(decimal srchPrice)
        {
            if (srchPrice > 0)
            {
                FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Lte("ListingType", srchPrice);

                return await _listingDTOMongo.Find(filter).ToListAsync();
            }
            else
                return [];
        }

        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByMinimumVehicleCreationTimeAsync(DateTime srchDate)
        {
            FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Gte("CreationDate", srchDate);

            return await _listingDTOMongo.Find(filter).ToListAsync();
        }

        public async Task<List<BillboardListingDTO>> SearchBillboardListingsByMaximumVehicleCreationTimeAsync(DateTime srchDate)
        {
            FilterDefinition<BillboardListingDTO> filter = Builders<BillboardListingDTO>.Filter.Lte("CreationDate", srchDate);

            return await _listingDTOMongo.Find(filter).ToListAsync();
        }
        #endregion
    }
}
