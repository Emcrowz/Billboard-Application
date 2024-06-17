using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Billboard_BackEnd.Repositories;
using Billboard_BackEnd.Services;
using Moq;

namespace Billboard_Tests.Services;

public class BillboardListingServiceTests
{
    private readonly Mock<IUserService> userService;
    private readonly Mock<IUserDapperContext> userLocalDb;
    private readonly Mock<IUserMongoContext> userMongoDb;

    private readonly Mock<IVehicleService> vehicleService;
    private readonly Mock<IVehicleDapperContext> vehicleLocalDb;

    private readonly Mock<IBillboardListingService> listingService;
    private readonly Mock<IBillboardListingDapperContext> listingLocalDb;
    private readonly Mock<IBillboardListingMongoContext> listingMongoDb;

    // Reusables
    User testUser_Bad_empty = new();

    List<User> users = new() { 
        new User() { UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", Username = "Test", Password = "ForScience123", UserCategory = 0 },
        new User() { UserId = 2, FirstName = "Correct", LastName = "User", Email = "Correct@user.com", Username = "Coruser", Password = "1234", UserCategory = 0 },
        new User() { UserId = 5, FirstName = "Some", LastName = "User", Email = "user@some.com", Username = "User", Password = "Password", UserCategory = 0 },    
    };
    List<Vehicle> vehicles = new() { 
        new Car() { VehicleId = 0, Make = "A", Model = "B", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)4 }, 
        new Motorbike() { VehicleId = 1, Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
        new Motorbike() { VehicleId = 2, Make = "Bikest", Model = "Testes", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 210 },
        new Car () { VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
        new Motorbike() { VehicleId = 4, Make = "Testa", Model = "Bikest", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 105 },
        new Car () { VehicleId = 5, Make = "E", Model = "F", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)8 }
    };
    List<BillboardListing> billboardListings = new() {
            new BillboardListing() { ListingId = 0, ListingType = "Car", UserId = 4, VehicleId = 0 },
            new BillboardListing() { ListingId = 1, ListingType = "Car", UserId = 4, VehicleId = 3 },
            new BillboardListing() { ListingId = 2, ListingType = "Motorbike", UserId = 4, VehicleId = 1 },
            new BillboardListing() { ListingId = 3, ListingType = "Motorbike", UserId = 4, VehicleId = 4 },
        };
    List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 0, ListingType = "Car", UserId = 4, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 0, Make = "A", Model = "B", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)4 },
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 4, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 4, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
            new BillboardListingDTO() { ListingId = 3, ListingType = "Motorbike", UserId = 4, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Testa", Model = "Bikest", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 105 }
        };

    public BillboardListingServiceTests()
    {
        userService = new();
        userLocalDb = new();
        userMongoDb = new();
        vehicleService = new();
        vehicleLocalDb = new();
        listingService = new();
        listingLocalDb = new();
        listingMongoDb = new();
    }

    #region CreateListing METHOD TESTS
    [Fact]
    public void CreateListing_ListingSuccessfull_Car()
    {
        // Arange
        List<Vehicle> vehicles = [];
        List<BillboardListing> billboardListings = [];
        List<BillboardListingDTO> billboardListingsDTO = [];

        string usernameInput = "Coruser";
        string passwordInput = "1234";
        VehicleDTO vehicleDTOInput = new() { Make = "A", Model = "B", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), DoorCount = 4, Engine = (EngineType)2 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput));

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);

        Car car = new()
        {
            Make = vehicleDTOInput.Make,
            Model = vehicleDTOInput.Model,
            Price = vehicleDTOInput.Price,
            CreationDate = vehicleDTOInput.CreationDate,
            LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck,
            DoorCount = vehicleDTOInput.DoorCount,
            Engine = vehicleDTOInput.Engine
        };
        vehicles.Add(car);
        vehicleService.Setup(create => create.CreateNewCar(car)).Returns(true).Verifiable();
        Mock.Verify();

        BillboardListing newListing = new()
        {
            VehicleId = car.VehicleId,
            UserId = user.UserId,
            ListingType = car.GetType().Name
        };
        billboardListings.Add(newListing);
        listingLocalDb.Setup(create => create.ExecuteCreateBillboardListingSQL(newListing)).Returns(true).Verifiable();
        Mock.Verify();

        BillboardListingDTO newListingToMongo = new()
        {
            ListingId = newListing.ListingId,
            ListingType = car.GetType().Name,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            VehicleId = car.VehicleId,
            Make = car.Make,
            Model = car.Model,
            Price = car.Price,
            CreationDate = car.CreationDate,
            LastTechnicalCheck = car.LastTechnicalCheck,
            DoorCount = car.DoorCount,
            Engine = car.Engine
        };
        billboardListingsDTO.Add(newListingToMongo);

        // Assert
        Assert.Contains(user, users);
        Assert.Equal("Correct", user.FirstName);
        Assert.Contains(car, vehicles);
        Assert.Contains(newListing, billboardListings);
        Assert.Contains(newListingToMongo, billboardListingsDTO);
    }

    [Fact]
    public void CreateListing_ListingSuccessfull_Motorbike()
    {
        List<Vehicle> vehicles = [];
        List<BillboardListing> billboardListings = [];
        List<BillboardListingDTO> billboardListingsDTO = [];

        string usernameInput = "Coruser";
        string passwordInput = "1234";
        VehicleDTO vehicleDTOInput = new() { Make = "A", Model = "B", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), CylinderVolume = 666 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput));

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);

        Motorbike motorbike = new()
        {
            Make = vehicleDTOInput.Make,
            Model = vehicleDTOInput.Model,
            Price = vehicleDTOInput.Price,
            CreationDate = vehicleDTOInput.CreationDate,
            LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck,
            CylinderVolume = vehicleDTOInput.CylinderVolume
        };
        vehicles.Add(motorbike);
        vehicleService.Setup(create => create.CreateNewMotorbike(motorbike)).Returns(true).Verifiable();
        Mock.Verify();

        BillboardListing newListing = new()
        {
            VehicleId = motorbike.VehicleId,
            UserId = user.UserId,
            ListingType = motorbike.GetType().Name
        };
        billboardListings.Add(newListing);
        listingLocalDb.Setup(create => create.ExecuteCreateBillboardListingSQL(newListing)).Returns(true).Verifiable();
        Mock.Verify();

        BillboardListingDTO newListingToMongo = new()
        {
            ListingId = newListing.ListingId,
            ListingType = motorbike.GetType().Name,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            VehicleId = motorbike.VehicleId,
            Make = motorbike.Make,
            Model = motorbike.Model,
            Price = motorbike.Price,
            CreationDate = motorbike.CreationDate,
            LastTechnicalCheck = motorbike.LastTechnicalCheck,
            CylinderVolume = motorbike.CylinderVolume
        };
        billboardListingsDTO.Add(newListingToMongo);

        // Assert
        Assert.Contains(user, users);
        Assert.Equal("Correct", user.FirstName);
        Assert.Contains(motorbike, vehicles);
        Assert.Contains(newListing, billboardListings);
        Assert.Contains(newListingToMongo, billboardListingsDTO);
    }

    [Fact]
    public void CreateListing_UnauthorizedUserWillNotCreateListing()
    {
        // Arange
        List<Vehicle> vehicles = [];
        List<BillboardListing> billboardListings = [];
        List<BillboardListingDTO> billboardListingsDTO = [];

        string usernameInput = "ABSNNOISFOs";
        string passwordInput = "SDINSIODNSIDMSD";
        VehicleDTO vehicleDTOInput = new() { Make = "A", Model = "B", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), CylinderVolume = 666 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput));

        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);
        // Act
        if (user != null)
        {
            Motorbike motorbike = new()
            {
                Make = vehicleDTOInput.Make,
                Model = vehicleDTOInput.Model,
                Price = vehicleDTOInput.Price,
                CreationDate = vehicleDTOInput.CreationDate,
                LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck,
                CylinderVolume = vehicleDTOInput.CylinderVolume
            };
            vehicles.Add(motorbike);
            vehicleService.Setup(create => create.CreateNewMotorbike(motorbike)).Returns(true).Verifiable();
            Mock.Verify();

            BillboardListing newListing = new()
            {
                VehicleId = motorbike.VehicleId,
                UserId = user.UserId,
                ListingType = motorbike.GetType().Name
            };
            billboardListings.Add(newListing);
            listingLocalDb.Setup(create => create.ExecuteCreateBillboardListingSQL(newListing)).Returns(true).Verifiable();
            Mock.Verify();

            BillboardListingDTO newListingToMongo = new()
            {
                ListingId = newListing.ListingId,
                ListingType = motorbike.GetType().Name,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                VehicleId = motorbike.VehicleId,
                Make = motorbike.Make,
                Model = motorbike.Model,
                Price = motorbike.Price,
                CreationDate = motorbike.CreationDate,
                LastTechnicalCheck = motorbike.LastTechnicalCheck,
                CylinderVolume = motorbike.CylinderVolume
            };
            billboardListingsDTO.Add(newListingToMongo);
        }

        // Assert
        Assert.DoesNotContain(user, users);
        Assert.Null(user);
        Assert.Empty(vehicles);
        Assert.Empty(billboardListings);
        Assert.Empty(billboardListingsDTO);
    }
    #endregion

    #region UpdateListing METHOD Tests
    [Fact]
    public void UpdateListing_UserValidates_UpdateSuccessful_Car()
    {
        // Arange
        List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 0, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 0, Make = "A", Model = "B", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)4 },
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
            new BillboardListingDTO() { ListingId = 3, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Testa", Model = "Bikest", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 105 }
        };

        string usernameInput = "Test";
        string passwordInput = "ForScience123";
        int indexInput = 1;
        VehicleDTO vehicleDTOInput = new() { Make = "REPLACE", Model = "REPLACY", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), DoorCount = 4, Engine = (EngineType)12 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput)).Verifiable();

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);
        bool userAuthenticated = user.Username == usernameInput && user.Password == passwordInput;
        bool userHasListing = billboardListingsDTO.Any(l => l.ListingId == indexInput && l.UserId == user.UserId);
        bool isCar = false;
        bool isMotorbike = false;

        if (user != null && userHasListing)
        {
            BillboardListingDTO carToChange = billboardListingsDTO.Find(l => l.ListingId == indexInput);
            if (vehicleDTOInput.DoorCount > 0 && vehicleDTOInput.CylinderVolume == 0)
            {
                vehicleService.Setup(v => v.UpdateCarById(indexInput, new Car() { Make = vehicleDTOInput.Make, Model = vehicleDTOInput.Model, Price = vehicleDTOInput.Price, CreationDate = vehicleDTOInput.CreationDate, LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, DoorCount = vehicleDTOInput.DoorCount, Engine = vehicleDTOInput.Engine })).Returns(true);

                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    DoorCount = vehicleDTOInput.DoorCount, // Alter
                    Engine = vehicleDTOInput.Engine, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isCar = true;
            }
            else if (vehicleDTOInput.DoorCount == 0 && vehicleDTOInput.CylinderVolume > 0)
            {
                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    CylinderVolume = vehicleDTOInput.CylinderVolume, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isMotorbike = true;
            }
        }

        // Arrange
        Mock.Verify();
        Assert.True(userAuthenticated);
        Assert.True(userHasListing);
        Assert.True(isCar);
    }

    [Fact]
    public void UpdateListing_UserValidates_UpdateSuccessful_Motorbike()
    {
        // Arange
        List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 0, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 0, Make = "A", Model = "B", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)4 },
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
            new BillboardListingDTO() { ListingId = 3, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Testa", Model = "Bikest", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 105 }
        };

        string usernameInput = "Test";
        string passwordInput = "ForScience123";
        int indexInput = 2;
        VehicleDTO vehicleDTOInput = new() { Make = "REPLACE", Model = "REPLACY", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), CylinderVolume = 666 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput)).Verifiable();

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);
        bool userAuthenticated = user.Username == usernameInput && user.Password == passwordInput;
        bool userHasListing = billboardListingsDTO.Any(l => l.ListingId == indexInput && l.UserId == user.UserId);
        bool isCar = false;
        bool isMotorbike = false;

        if (user != null && userHasListing)
        {
            BillboardListingDTO carToChange = billboardListingsDTO.Find(l => l.ListingId == indexInput);
            if (vehicleDTOInput.DoorCount > 0 && vehicleDTOInput.CylinderVolume == 0)
            {
                vehicleService.Setup(v => v.UpdateCarById(indexInput, new Car() { Make = vehicleDTOInput.Make, Model = vehicleDTOInput.Model, Price = vehicleDTOInput.Price, CreationDate = vehicleDTOInput.CreationDate, LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, DoorCount = vehicleDTOInput.DoorCount, Engine = vehicleDTOInput.Engine })).Returns(true);

                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    DoorCount = vehicleDTOInput.DoorCount, // Alter
                    Engine = vehicleDTOInput.Engine, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isCar = true;
            }
            else if (vehicleDTOInput.DoorCount == 0 && vehicleDTOInput.CylinderVolume > 0)
            {
                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    CylinderVolume = vehicleDTOInput.CylinderVolume, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isMotorbike = true;
            }
        }

        // Arrange
        Mock.Verify();
        Assert.True(userAuthenticated);
        Assert.True(userHasListing);
        Assert.True(isMotorbike);
    }

    [Fact]
    public void UpdateListing_UserDoesNotValidate_UpdateFails()
    {
        // Arange
        List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 0, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 0, Make = "A", Model = "B", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 4, Engine = (EngineType)4 },
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
            new BillboardListingDTO() { ListingId = 3, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Testa", Model = "Bikest", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 105 }
        };

        string usernameInput = "SNIODNSODMS";
        string passwordInput = "ForSciDOFINNDOFOIFDSDence123";
        int indexInput = 2;
        VehicleDTO vehicleDTOInput = new() { Make = "REPLACE", Model = "REPLACY", Price = 123.45m, CreationDate = DateTime.Parse("2024-06-16"), LastTechnicalCheck = DateTime.Parse("2024-06-16"), CylinderVolume = 666 };

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput)).Verifiable();

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);
        //bool userAuthenticated = user.Username == usernameInput && user.Password == passwordInput;
        //bool userHasListing = billboardListingsDTO.Any(l => l.ListingId == indexInput && l.UserId == user.UserId);
        bool isCar = false;
        bool isMotorbike = false;

        if (user != null)
        {
            BillboardListingDTO carToChange = billboardListingsDTO.Find(l => l.ListingId == indexInput);
            if (vehicleDTOInput.DoorCount > 0 && vehicleDTOInput.CylinderVolume == 0)
            {
                vehicleService.Setup(v => v.UpdateCarById(indexInput, new Car() { Make = vehicleDTOInput.Make, Model = vehicleDTOInput.Model, Price = vehicleDTOInput.Price, CreationDate = vehicleDTOInput.CreationDate, LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, DoorCount = vehicleDTOInput.DoorCount, Engine = vehicleDTOInput.Engine })).Returns(true);

                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    DoorCount = vehicleDTOInput.DoorCount, // Alter
                    Engine = vehicleDTOInput.Engine, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isCar = true;
            }
            else if (vehicleDTOInput.DoorCount == 0 && vehicleDTOInput.CylinderVolume > 0)
            {
                BillboardListingDTO listingToUpdate = new()
                {
                    ListingId = carToChange.ListingId, // Same
                    ListingType = carToChange.ListingType, // Same
                    UserId = carToChange.UserId, // Same
                    FirstName = carToChange.FirstName, // Same
                    LastName = carToChange.LastName, // Same
                    Email = carToChange.Email, // Same
                    VehicleId = carToChange.VehicleId, // Same
                    Make = vehicleDTOInput.Make, // Alter
                    Model = vehicleDTOInput.Model, // Alter
                    Price = vehicleDTOInput.Price, // Alter
                    CreationDate = vehicleDTOInput.CreationDate, // Alter
                    LastTechnicalCheck = vehicleDTOInput.LastTechnicalCheck, // Alter
                    CylinderVolume = vehicleDTOInput.CylinderVolume, // Alter
                    InternalBillboardListingId = carToChange.InternalBillboardListingId // Must be Same.
                };

                listingMongoDb.Setup(l => l.UpdateBillboardListingMongoAsync(listingToUpdate)).Verifiable();
                isMotorbike = true;
            }
        }

        // Arrange
        Mock.Verify();
        Assert.Null(user);
    }
    #endregion

    #region DeleteListing
    [Fact]
    public void DeleteListing_UserAuthenticated_SuccessfullDelete()
    {
        // Assert
        List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 }, 
        };

        string usernameInput = "Test";
        string passwordInput = "ForScience123";
        int indexInput = 2;

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput)).Verifiable();

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);

        BillboardListingDTO? listing = billboardListingsDTO.FirstOrDefault(l => l.ListingId == indexInput);
        if (user != null)
        {
            
            if(listing != null && listing.UserId == user.UserId)
            {
                listingMongoDb.Setup(delete => delete.DeleteBillboardListingMongoAsync(listing.InternalBillboardListingId));
                billboardListingsDTO.Remove(listing);
            }
        }

        // Assert
        Assert.NotNull(user);
        Assert.DoesNotContain(listing, billboardListingsDTO);
        Assert.Equal(1, billboardListingsDTO.Count());
    }

    [Fact]
    public void DeleteListing_UserDidNotAuthenticate_FailedDelete()
    {
        // Assert
        List<BillboardListingDTO> billboardListingsDTO = new() {
            new BillboardListingDTO() { ListingId = 1, ListingType = "Car", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 3, Make = "C", Model = "D", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, DoorCount = 2, Engine = (EngineType)6 },
            new BillboardListingDTO() { ListingId = 2, ListingType = "Motorbike", UserId = 0, FirstName = "Test", LastName = "Dummy", Email = "Testy@Test.com", VehicleId = 1 , Make = "Bike", Model = "Test", Price = 420.69m, CreationDate = DateTime.Now, LastTechnicalCheck = DateTime.Now, CylinderVolume = 420 },
        };

        string usernameInput = "SIBDBSINDS";
        string passwordInput = "ForSIOSONDMSMSScience123";
        int indexInput = 2;

        userService.Setup(login => login.UserLoginService(usernameInput, passwordInput)).Returns(users.Find(u => u.Username == usernameInput && u.Password == passwordInput)).Verifiable();

        // Act
        User? user = userService.Object.UserLoginService(usernameInput, passwordInput);

        BillboardListingDTO? listing = billboardListingsDTO.FirstOrDefault(l => l.ListingId == indexInput);
        if (user != null)
        {

            if (listing != null && listing.UserId == user.UserId)
            {
                listingMongoDb.Setup(delete => delete.DeleteBillboardListingMongoAsync(listing.InternalBillboardListingId));
                billboardListingsDTO.Remove(listing);
            }
        }

        // Assert
        Assert.Null(user);
        Assert.Contains(listing, billboardListingsDTO);
        Assert.Equal(2, billboardListingsDTO.Count());
    }
    #endregion
}