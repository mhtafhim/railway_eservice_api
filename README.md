# Railway eService API

## Overview
Railway eService API is a RESTful web service designed for comprehensive railway management. It enables administrators to manage trains, schedules, and track ticket sales, while allowing users to search for available trains and purchase tickets.

## Features

### For Administrators
- Add, update, and delete train schedules
- Track ticket bookings and sales
- Manage train information

### For Users
- Search for available trains based on origin, destination, and journey date
- Book tickets with different class options (Economy, Business, First Class)
- Manage bookings (update, delete)

## Technology Stack
- **Framework:** ASP.NET Core 8.0
- **Database:** SQL Server (via Microsoft.Data.SqlClient)
- **Authentication:** JWT (jose-jwt)
- **Password Hashing:** BCrypt.Net-Next
- **API Documentation:** Swagger (Swashbuckle.AspNetCore)
- **Email Services:** SendinBlue API (sib_api_v3_sdk)
- **CORS Support:** Microsoft.AspNetCore.Cors, Microsoft.AspNet.WebApi.Cors

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code with C# extension

### Setup and Run

#### Clone the Repository
```sh
git clone https://github.com/yourusername/railway_eservice_api.git
cd railway_eservice_api
```

#### Configure Database Connection
Edit the `appsettings.json` file and update the connection string:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RailwayEService;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "ExpiryInMinutes": 60
  },
  "EmailSettings": {
    "ApiKey": "your-sendinblue-api-key"
  }
}
```

#### Build and Run the Project
```sh
dotnet build
dotnet run
```

#### Access Swagger Documentation
Open your browser and navigate to:
- `https://localhost:7001/swagger`
- `http://localhost:5000/swagger`

(The exact ports may vary depending on your local configuration.)

### Using Visual Studio
1. Open the solution file (`railway_eservice_api.sln`).
2. Update the connection string in `appsettings.json`.
3. Press `F5` or click the "Run" button to start the application.
4. Swagger will automatically open in your default browser.

## API Endpoints

### Train Management
- `POST /api/Train_Schedule/add_train_schedule` - Add a new train schedule
- `GET /api/Train_Schedule/show_train_schedule` - Retrieve train schedules
- `PUT /api/Train_Schedule/update_train_schedule` - Update an existing train schedule
- `DELETE /api/Train_Schedule/delete_train_schedule` - Delete a train schedule

### Booking Management
- `GET /api/Booking/get_available_trains` - Find available trains by start station, destination, and date
- `POST /api/Booking/add_booking` - Create a new ticket booking
- `PUT /api/Booking/update_booking` - Update an existing booking
- `DELETE /api/Booking/delete_booking` - Cancel a booking
- `GET /api/Booking/get_fare` - Retrieve fare information

### User Registration and Authentication
- `POST /api/Reg` - Register a new user
- `GET /api/test` - Test endpoint

## Additional Information
This API is designed with a focus on security and performance, using best practices for ASP.NET Core development. The codebase includes comprehensive error handling and follows RESTful API design principles.
