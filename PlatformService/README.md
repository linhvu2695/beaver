# Start .NET WebApi Project
- `dotnet new webapi -n <project-name>`

## Import packages
- `dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection`
- `dotnet add package Microsoft.EntityFrameworkCore`
- `dotnet add package Microsoft.EntityFrameworkCore.Design`
- `dotnet add package Microsoft.EntityFrameworkCore.InMemory`
- `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`

## DbContext
- A DbContext instance represents a session with the database and can be used to query and save instances of your entities.
- Create class `AppDbContext` inheriting from `DbContext` and register the given context in `Program.cs`

## Repository
- A repository comprises of an interface for user to interact with the database via `DbContext`.
- `IPlatformRepo` is the interface class defining the available APIs
- `PlatformRepo` is the concrete class defining the logic of interactions with the AppDbContext

## DTOs
- The external representations of our data (vs `Models` which consists of the internal representations)
- Why do we differentiate these two?
    - **Data Privacy**: we do not want clients to know the internal structure of our data
    - **Contractual Coupling**: we might want to change the internal structure of data without disrupting the contract between us and clients