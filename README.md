# Video Game Catalogue

A .NET 10 web API for managing a video game catalog with support for games, platforms, and genres.

## 🎮 Overview

Video Game Catalogue is a RESTful API built with ASP.NET Core that provides endpoints for managing and querying video game data. The application follows a layered architecture with clear separation of concerns.

## 📋 Project Structure

### `VideoGameCatalogue.Api`
The presentation layer containing ASP.NET Core controllers and API configuration.
- **Purpose**: Exposes RESTful endpoints for managing video games, platforms, and genres
- **Framework**: ASP.NET Core 10.0
- **Key Features**:
  - RESTful API controllers for Video Games, Platforms, and Genres
  - OpenAPI/Swagger documentation with Scalar UI
  - Dependency injection configuration
  - Database connection management

### `VideoGameCatalogue.BusinessLogic`
The business logic layer containing services and repositories.
- **Purpose**: Encapsulates core business rules and data access patterns
- **Key Components**:
  - Service classes that implement business logic
  - Repository pattern implementation for data access
  - Dependency injection service registration
  - Tests for business logic validation

### `VideoGameCatalogue.Data`
The data access layer with Entity Framework Core integration.
- **Purpose**: Handles all database operations and entity management
- **Framework**: Entity Framework Core 10.0 with SQL Server
- **Key Features**:
  - DbContext configuration
  - Database migrations
  - Entity models and configurations
  - SQL Server integration

### `VideoGameCatalogue.Shared`
Shared utilities and configurations used across all projects.
- **Purpose**: Contains reusable code, enums, configurations, and base classes
- **Key Components**:
  - System configuration management
  - Enumeration utilities
  - Base service classes
  - Shared models and constants
  - Configuration settings

### `Testing/VideoGameCatalogue.BusinessLogic.Tests`
Unit tests for business logic validation.
- **Purpose**: Ensures business logic correctness and reliability
- **Framework**: .NET 10.0
- **Scope**: Tests for services and repositories

## 🏗️ Architecture

The application follows a **Layered Architecture** pattern:

```
API Layer (Controllers)
        ↓
Business Logic Layer (Services & Repositories)
        ↓
Data Layer (DbContext & EF Core)
        ↓
SQL Server Database
```

**Shared** layer provides utilities and base classes across all layers.

## 🛠️ Technologies

- **.NET**: 10.0
- **Framework**: ASP.NET Core
- **ORM**: Entity Framework Core 10.0
- **Database**: SQL Server
- **API Documentation**: OpenAPI with Scalar UI
- **Dependency Injection**: Built-in .NET DI container

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (local or remote)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/inAnimateCarbonRo/VideoGameCatalogue.git
cd VideoGameCatalogue
```

2. Configure the database connection string in `appsettings.json`

3. Restore dependencies:
```bash
dotnet restore
```

4. Apply migrations:
```bash
dotnet ef database update
```

5. Run the API:
```bash
dotnet run --project VideoGameCatalogue.Api
```

The API will be available at `https://localhost:[port]`

## 💾 Database & Sample Data

The project includes a database file with pre-populated sample data, allowing you to test the API immediately after setup without having to manually create data entries.
VideoGameCatalogueDB_Feb22.bak

### Related UI Project

A complementary web UI for this API is available at:
- **Repository**: [VIDEOGAMECATALOGUE-UI](https://github.com/inAnimateCarbonRo/VIDEOGAMECATALOGUE-UI)
- **Purpose**: Provides a user-friendly interface for browsing and managing video games, platforms, and genres

You can run both the API and UI together for a complete application experience.

## 📚 API Endpoints

### Video Games
- `GET /api/videogames` - Get all video games
- `GET /api/videogames/{id}` - Get a specific video game
- `POST /api/videogames` - Create a new video game
- `PUT /api/videogames/{id}` - Update a video game
- `DELETE /api/videogames/{id}` - Delete a video game

### Platforms
- `GET /api/platforms` - Get all platforms
- `GET /api/platforms/{id}` - Get a specific platform
- `POST /api/platforms` - Create a new platform
- `PUT /api/platforms/{id}` - Update a platform
- `DELETE /api/platforms/{id}` - Delete a platform

### Genres
- `GET /api/genres` - Get all genres
- `GET /api/genres/{id}` - Get a specific genre
- `POST /api/genres` - Create a new genre
- `PUT /api/genres/{id}` - Update a genre
- `DELETE /api/genres/{id}` - Delete a genre

## 📖 API Documentation

Interactive API documentation is available at:
- Scalar UI: `https://localhost:[port]/scalar/v1`
- OpenAPI JSON: `https://localhost:[port]/openapi/v1.json`

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## 📝 Development Guidelines

### Adding New Features

1. **Create Data Models** in the Data layer
2. **Configure DbContext** in the Data layer
3. **Implement Repository** in BusinessLogic layer
4. **Create Service** in BusinessLogic layer
5. **Add Controller** in Api layer
6. **Write Tests** in Testing project
7. **Update this README** if needed

### Code Structure
- Follow the existing project organization
- Use dependency injection for loose coupling
- Implement the repository pattern for data access
- Keep business logic in services, not controllers

## 🤝 Contributing

1. Create a feature branch
2. Commit your changes
3. Push to your fork
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

inAnimateCarbonRo

## 🔗 Repository

[https://github.com/inAnimateCarbonRo/VideoGameCatalogue](https://github.com/inAnimateCarbonRo/VideoGameCatalogue)
