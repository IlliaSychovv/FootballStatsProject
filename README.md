# FootballStats - Football Statistics

Web application for managing football match statistics with user authentication system.

## Description

The project consists of two applications:
- **ASP.NET MVC** - web interface for users
- **ASP.NET Web API** - REST API for integration

## Architecture

Clean Architecture with layered approach:
- **Domain Layer** - entities and business rules
- **Application Layer** - business logic and services
- **Infrastructure Layer** - data access and external services
- **Presentation Layer** - MVC and Web API

## Technologies

- **.NET 8.0**
- **ASP.NET Core MVC & Web API**
- **SQL Server** with ServiceStack.OrmLite
- **FluentValidation** for data validation
- **Bootstrap** for UI
- **Mapster** DTO ↔ Entity mapping
- **xUnit, Moq, Shouldly** for testing
- **CSV import/export** implemented using atomic transactions, streaming file reads, and batch processing for improved performance

## Installation Guide

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code or Rider

### 1. Clone Repository
```bash
git clone https://github.com/your-username/FootballStats.git
cd FootballStats
```

### 2. Create Database
1. Open SQL Server Management Studio
2. Execute SQL script from `Database/CreateDatabase.sql`
3. Or use LocalDB for development

### 3. Configure Connection String
Update connection string in:
- `FootballStats.WebMvc/appsettings.Development.json`
- `FootballStats.WebApi/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FootballStatsDb;Trusted_Connection=True;Encrypt=False;"
  }
}
```

### 4. Run Applications

#### MVC Application
```bash
cd FootballStats.WebMvc
dotnet run
```
Access: `https://localhost:5001`

#### Web API Application
```bash
cd FootballStats.WebApi
dotnet run
```

## Authentication

### Registration
- Navigate to `/Account/Register`
- Fill in: Name, Email, Login, Password

### Login
- Navigate to `/Account/Login`
- Use Email and Password
- You will be automatically switched to /Matches
- Session timeout: 30 minutes

## Features

### MVC Application
- User registration and authentication
- View match statistics (50 matches per page)
- Filter matches by team name and date range
- Session-based authorization

### Web API
- `GET /api/matches` - Get all matches with filtering
- `GET /api/matches/{id}` - Get specific match
- `POST /api/matches` - Add new match
- `POST /api/matches/import` - Import matches from CSV
- `GET /api/matches/export` - Export matches to CSV

### Import Sample Data
To test the API quickly, you can import sample matches:

- [Download CSV](data.csv)
- Go to `/api/matches/import` (POST)
- Upload the file
- After import, use `GET /api/matches` to see the data

## Database Schema

When you first launch the application, it automatically creates all the necessary tables in the database

## Docker Setup

### Prerequisites
- Docker Desktop
- Docker Compose

### Quick Start with Docker

1. **Update connection strings**

Update both files with the Docker connection string:

**`FootballStats.WebMvc/appsettings.Development.json`:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FootballStatsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=false;"
  }
}
```

**`FootballStats.WebApi/appsettings.Development.json`:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FootballStatsDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=false;"
  }
}
```

2. **Run the applications**
```bash
# MVC Application
cd FootballStats.WebMvc
dotnet run

# Web API (in another terminal)
cd FootballStats.WebApi
dotnet run
```

### Docker Commands
```bash
# Start containers
docker-compose up -d

docker-compose down

docker-compose logs sqlserver

docker-compose down -v
```

### Default Credentials
- **Server**: localhost,1433
- **Database**: FootballStatsDb
- **Username**: sa
- **Password**: YourStrong@Passw0rd