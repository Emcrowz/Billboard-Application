# Billboard application

## Theme / Core idea
An application that is responsible for billboard listing of vehicles (cars and motorbikes) for users to sell.

## Back-End
Project name: `Billboard_BackEnd`
Project type: `Class library`

NuGet packages used: 
- Dapper (2.1.35)
- System.Data.SqlClient(4.8.6)
- MongoDB.Driver(2.26.0)

Actions: 
> [!NOTE]
> Responsible for creating users, vehicles and billboard listings, storing, updating and deleting inside databases - both local (SSMS) and remote (MongoDB). Also contains servises that are crucial for the user validation actions, type checkings, search functions and routing interactions from the API.
>
> MongoDB currently stores billboard listings for users to interact with and it's sole purpose is that - get listings from local db. Local DB (SSMS) is used to store vehicle and user data. Creation of billboard records happens on both databases. User and vehicle creation happens only on the local database.

## API
Project name: `Billboard_API`
Project type: `ASP.NET Core Web API`

NuGet packages used: 
- Microsoft.AspNetCore.OpenApi (8.0.6)
- Swashbuckle.AspNetCore (6.6.2)
- Serilog.AspNetCore (8.0.1)
- Serilog.Sinks.Console (6.0.0)
- Serilog.Sinks.File (5.0.0)

References: `Billboard_BackEnd`

Actions:
BillboardListing
- __Get__ - _/BillboardListings/Listings_ - Fetches the billboard listing records **from the MongoDB.**
- __Post__ - _/BillboardListings/Listings_ - Creates a billboard listing (IF USER IS REGISTERED). **Created on the SSMS and MongoDB.**
- __Get__ - _/BillboardListing/Listings/{id}_ - Fetches the billboard listing with specified / provided ID **from the MongoDB.**
- __Put__ - _/BillboardListing/Listings/{id}_ - Updates the billboard listing information - vehicle information (IF USER IS REGISTERED AND HAS A LISTING ASSOCIATED). **Updated on the SSMS and MongoDB.**
- __Delete__ - _/BillboardListing/Listings/{id}_ - Deletes the billboard listing (IF USER IS REGISTERED AND HAS A LISTING ASSOCIATED) **from the MongoDB.** SSMS record is kept. 
- __Get__ - _/BillboardListing/Listings/Search_ - Fetches billboard listings by: 1) Price search (higher than, lower than); 2) Vehicle creation date search (higher than, lower than); 3) Vehicle type search (Car or Motorbike); 4) Vehicle make or model search
  
Mongo
- __Get__ - _/Mongo/GetListingsFromDB_ - Fetches records from the SSMS and stores them to the MongoDB. A Testing and development action. To be removed.
- __Delete__ - _/Mongo/GetListingsFromDB_ - Deletes records from the MongoDB. A Testing and development action. To be removed.

User
- __Post__ - _/User/Register_ - Creates a user locally inside SSMS. 
- __Post__ - _/User/UpdateUser/{id}_ - Updates user details inside SSMS. (Does not update inside the listings yet!)
- __Post__ - _/User/LoginUser_ - Request for user to login. Details correct - logs in. Not correct - tells that you can't log in. Interacts with service that does the validation and used for the billboard listing interactions.

## Front-End
Project name: `Billboard_FrontEnd`
Project type: `Blazor Web App`

> [!NOTE]
> Project initiated but not yet started. To be created.

## Unit Tests
Project name: `Billboard_Tests`
Project type: `xUnit test`

NuGet packages used: 
- Moq (4.20.70)

References: `Billboard_BackEnd`; `Billboard_API`; `Billboard_FrontEnd`

> [!NOTE]
> Currently Unit tests are made only for `Billboard_BackEnd` service that is responsible for the billboard listings.
