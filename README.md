# TaskManagerAPI

A RESTful task management API built with ASP.NET Core, Entity Framework Core, and SQL Server. The application provides secure user authentication with JWT access tokens and refresh tokens, role-based authorization, and full CRUD functionality for managing tasks.

## Features

* User registration and login
* JWT authentication and authorization
* Refresh token support
* Role-based access control (Admin/User)
* Task creation, retrieval, updating, and deletion
* Task filtering, sorting, and pagination
* Global exception handling middleware
* Swagger/OpenAPI documentation
* Entity Framework Core with SQL Server
* Docker and Docker Compose support
* Unit and integration testing

## Tech Stack

### Backend

* ASP.NET Core
* Entity Framework Core
* C#

### Database

* Microsoft SQL Server

### Authentication & Security

* JWT Bearer Authentication
* Refresh Tokens
* Role-Based Authorization

### Testing

* xUnit
* FluentAssertions

### DevOps

* Docker
* Docker Compose

## Architecture

```text
Client
  │
  ▼
ASP.NET Core API
  │
  ├── Controllers
  ├── Services
  ├── DTOs
  ├── Middleware
  │
  ▼
Entity Framework Core
  │
  ▼
SQL Server
```

## API Endpoints

### Authentication

| Method | Endpoint           | Description          |
| ------ | ------------------ | -------------------- |
| POST   | /api/auth/register | Register a new user  |
| POST   | /api/auth/login    | Authenticate user    |
| POST   | /api/auth/refresh  | Refresh access token |
| POST   | /api/auth/logout   | Revoke refresh token |

### Tasks

| Method | Endpoint        | Description                               |
| ------ | --------------- | ----------------------------------------- |
| GET    | /api/tasks      | Get authenticated user's tasks            |
| GET    | /api/tasks/all  | Admin-only endpoint to retrieve all tasks |
| POST   | /api/tasks      | Create a task                             |
| GET    | /api/tasks/{id} | Retrieve a specific task                  |
| PUT    | /api/tasks/{id} | Update a task                             |
| DELETE | /api/tasks/{id} | Delete a task                             |

## Getting Started

### Prerequisites

* .NET SDK
* Docker Desktop

### Run with Docker

```bash
docker compose up --build
```

The API will be available at:

```text
http://localhost:8000
```

Swagger UI:

```text
http://localhost:8000/swagger
```

## Testing

Run all tests:

```bash
dotnet test
```

Current test coverage includes:

* Authentication integration tests
* Authorization tests
* Task service unit tests
* Business rule validation

## Example Authentication Flow

1. Register a user
2. Login to receive an access token and refresh token
3. Include the JWT in the Authorization header
4. Access protected endpoints
5. Refresh expired access tokens using the refresh endpoint

Example header:

```http
Authorization: Bearer <jwt-token>
```

## Future Improvements

* GitHub Actions CI/CD pipeline
* Cloud deployment
* Rate limiting
* Email verification
* Password reset workflow
* Audit logging

## Author

Chloe Nuzillat

GitHub: https://github.com/cnuzillat
LinkedIn: https://linkedin.com/in/chloe-nuzillat
