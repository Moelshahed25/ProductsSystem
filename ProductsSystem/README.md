# Products System (ASP.NET Core API + MVC)

ProductsApi  -> http://localhost:5164  (SQL Server + EF Core)
ProductsMvc  -> http://localhost:5200  (UI, calls the API using HttpClient)

## Run
1) Create the database (from the ProductsApi folder):
   dotnet tool install --global dotnet-ef      (once)
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   (If DB "ProductApi2" already has old tables, drop it first: dotnet ef database drop)

2) Start the API:   cd ProductsApi && dotnet run
3) Start the MVC:   cd ProductsMvc && dotnet run      (open http://localhost:5200)

In Visual Studio: Solution -> Properties -> Multiple startup projects -> start both.
The API URL used by the MVC is in ProductsMvc/appsettings.json (ApiSettings:BaseUrl).
