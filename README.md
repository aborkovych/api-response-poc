# ApiResponse POC

A proof-of-concept ASP.NET Core project demonstrating a standardized API response pattern with comprehensive error handling, validation, and pagination support.

## Overview

This project showcases best practices for building RESTful APIs with:
- **Unified Response Format**: Consistent `ApiResponse<T>` wrapper for all endpoints
- **Automatic Validation**: FluentValidation integrated into the request pipeline
- **Error Handling**: Centralized exception handling middleware with structured error responses
- **Pagination Support**: Built-in pagination metadata for list endpoints
- **OpenAPI Documentation**: Scalar UI for interactive API exploration

## Architecture

### Core Components

#### Responses (`Responses/`)
- **ApiResponse<T>**: Generic response wrapper supporting success and failure states
- **MetaInfo**: Contains traceability data (TraceId, SpanId, Timestamp) and pagination metadata
- **PageMeta**: Pagination information (page, pageSize, totalItems, totalPages)
- **ErrorResponse**: Structured error format with error codes and validation details

#### Controllers (`Controllers/`)
- **AppointmentController**: Sample CRUD operations for appointments
  - `GET /appointment` - List appointments with pagination
  - `GET /appointment/{id}` - Get single appointment
  - `POST /appointment` - Create new appointment

#### Validators (`Validators/`)
- **CreateAppointmentDtoValidator**: FluentValidation rules for appointment creation
  - Validates customer name is required
  - Ensures appointment date is in the future

#### Middleware (`ExceptionHandlingMiddleware.cs`)
- Global exception handling
- Converts unhandled exceptions to structured `ApiResponse<T>` error responses
- Logs exception details for debugging

#### Extensions (`Extensions/ApiResponseExtensions.cs`)
- **ToActionResult()**: Converts `ApiResponse<T>` to `IActionResult` with appropriate HTTP status codes
- Maps error codes to HTTP status codes:
  - `ValidationFailed` → 400 Bad Request
  - `InvalidToken` → 401 Unauthorized
  - `InvalidPermission` → 403 Forbidden
  - `EntityDoesNotExist` → 404 Not Found
  - `DatabaseError` → 500 Internal Server Error

#### Models (`Models/`)
- **AppointmentDto**: Appointment data transfer object
- **CreateAppointmentDto**: Input model for appointment creation
- **ErrorResponse**: Error response structure
- **ValidationError**: Individual validation error details

## Features

### 1. Unified Response Format

All endpoints return a consistent response structure:

**Success Response (200 OK)**:
```json
{
  "isSuccess": true,
  "data": { /* response data */ },
  "meta": {
    "traceId": "uuid",
    "spanId": "uuid",
    "timestamp": "2026-01-06T10:00:00Z",
    "page": {
      "page": 1,
      "pageSize": 5,
      "totalItems": 25,
      "totalPages": 5
    }
  }
}
```

**Error Response (4xx/5xx)**:
```json
{
  "isSuccess": false,
  "error": {
    "message": "Validation failed.",
    "errorCode": "ValidationFailed",
    "errors": {
      "customerName": "Customer name is required.",
      "date": "Appointment date must be in the future."
    }
  },
  "meta": {
    "traceId": "uuid",
    "timestamp": "2026-01-06T10:00:00Z"
  }
}
```

### 2. Automatic Validation

- FluentValidation is integrated into the ASP.NET Core pipeline
- Validation occurs automatically during model binding
- Invalid requests return structured error responses via `CustomValidationResultFactory`
- No manual validation code needed in controllers

### 3. Pagination

List endpoints support pagination:
```
GET /appointment?page=1&pageSize=5
```

Parameters:
- `page`: Current page number (default: 1)
- `pageSize`: Items per page (default: 5, max: 100)

Response includes pagination metadata:
```json
{
  "meta": {
    "page": {
      "page": 1,
      "pageSize": 5,
      "totalItems": 25,
      "totalPages": 5
    }
  }
}
```

### 4. Error Handling

#### Validation Errors (400 Bad Request)
```json
{
  "isSuccess": false,
  "data": null,
  "error": {
    "id": "b0481fad-2882-4866-9271-e4cfdf053723",
    "message": null,
    "errors": [
      {
        "field": "Date",
        "message": "Appointment date must be in the future."
      },
      {
        "field": "CustomerName",
        "message": "Customer name is required."
      },
      {
        "field": "CustomerName",
        "message": "Customer name is required."
      }
    ],
    "errorCode": 1,
    "exception": null
  },
  "meta": {
    "traceId": "c4cd9ce0cff7fa10dbcf4138150c1b8b",
    "spanId": "6249406b94375f41",
    "timestamp": "2026-01-06T16:36:41.8054205+00:00",
    "page": null
  }
}
```

#### Not Found (404 Not Found)
```json
{
  "isSuccess": false,
  "error": {
    "message": "Appointment not found.",
    "errorCode": "EntityDoesNotExist",
    "errors": {
      "appointmentId": 123
    }
  }
}
```

#### Server Errors (500 Internal Server Error)
- Unhandled exceptions are caught by middleware
- Returned as structured error responses
- Stack trace logged for debugging

```json
"error": {
    "id": "ee572a6a-1cb1-48a1-b13d-0da15a6ad7c0",
    "message": "Failed to fetch one of the appointments from the database.",
    "errors": null,
    "errorCode": 13,
    "exception": null
  },
```

## API Endpoints

### Get Appointments (Paginated)
```http
GET /appointment?page=1&pageSize=5
```

**Response** (200 OK):
- Returns paginated list of appointments
- Includes pagination metadata
- 50% chance to simulate database error (500 Internal Server Error)

### Get Appointment by ID
```http
GET /appointment/{id}
```

**Response** (200 OK):
- Returns single appointment

**Response** (404 Not Found):
- Returns error if appointment not found

### Create Appointment
```http
POST /appointment
Content-Type: application/json

{
  "date": "2026-01-15T10:00:00Z",
  "customerName": "John Smith"
}
```

**Response** (200 OK):
- Returns created appointment
- Includes Location header linking to GET endpoint
- 10% chance to simulate database error (500 Internal Server Error)

**Response** (400 Bad Request):
- Invalid input validation errors

## Getting Started

### Prerequisites
- .NET 10.0 or later
- Visual Studio, Rider, or VS Code

### Installation

```bash
# Clone the repository
git clone <repository-url>
cd ApiResponse.Poc

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

### Running the Application

```bash
dotnet run
```

The API will be available at:
- API: `https://localhost:7037`
- OpenAPI Documentation: `https://localhost:7037/docs`

### Testing

Use the included `.http` file (`ApiResponse.Poc.http`) to test endpoints:
- View in VS Code with REST Client extension
- Use in Rider's HTTP client
- Copy requests to Postman or similar tools

## Technologies

- **ASP.NET Core 10.0**: Web framework
- **FluentValidation**: Input validation
- **SharpGrip.FluentValidation.AutoValidation**: Auto-validation pipeline integration
- **OpenAPI (Scalar)**: API documentation

## Project Structure

```
ApiResponse.Poc/
├── Controllers/
│   └── AppointmentController.cs      # API endpoints
├── Models/
│   ├── AppointmentDto.cs             # Appointment model
│   ├── CreateAppointmentDto.cs       # Creation input model
│   ├── ErrorResponse.cs              # Error structure
│   └── ValidationError.cs            # Validation error details
├── Responses/
│   └── ApiResponse.cs                # Core response wrapper
├── Validators/
│   └── CreateAppointmentDtoValidator.cs  # FluentValidation rules
├── Extensions/
│   └── ApiResponseExtensions.cs      # Response conversion extensions
├── Factories/
│   └── CustomValidationResultFactory.cs # Validation response factory
├── Enums/
│   └── ErrorCode.cs                  # Error code enumeration
├── ExceptionHandlingMiddleware.cs    # Global exception handler
├── Program.cs                        # Startup configuration
└── appsettings.json                  # Configuration
```

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `Unknown` | 500 | Unknown error |
| `ValidationFailed` | 400 | Input validation failed |
| `InvalidPermission` | 403 | User lacks required permissions |
| `InvalidToken` | 401 | Authentication token invalid |
| `EntityDoesNotExist` | 404 | Requested resource not found |
| `DatabaseError` | 500 | Database operation failed |

## Best Practices Demonstrated

1. **Separation of Concerns**: Validation, error handling, and response formatting are separated
2. **Consistency**: All responses follow the same structure
3. **Traceability**: Every response includes trace IDs for debugging
4. **Clean Code**: Controllers focus on business logic, not validation
5. **Extensibility**: Easy to add new validators and error types
6. **Testing**: Structured responses make testing straightforward


