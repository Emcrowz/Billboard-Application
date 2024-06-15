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
        public async Task CreateBillboardListingMongoAsync(BillboardListingDTO listingDTO) => await _listingDTOMongo.InsertOneAsync(listingDTO);

        // Get
        public async Task<IEnumerable<BillboardListingDTO>> FetchAllBillboardListingRecordsMongoAsync()
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

        public async Task<IEnumerable<BillboardListingDTO>> FetchAllBillboardListingsRecordsMongoAsync_Force()
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
    }
}
