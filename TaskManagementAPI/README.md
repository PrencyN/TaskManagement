# Task Management API

A simple ASP.NET Core Web API for managing tasks with status transitions and business rules.

## Time Spent
- Total: 240 minutes
  - Project setup and architecture: 30 minutes
  - Models and DTOs: 20 minutes
  - Repository layer: 40 minutes
  - Service layer with business logic: 50 minutes
  - Controllers and endpoints: 35 minutes
  - Error handling middleware: 20 minutes
  - Testing and validation: 30 minutes
  - Documentation: 15 minutes

## Approach

### Architecture
I followed a clean architecture approach with clear separation of concerns:
- **Controllers**: Handle HTTP requests/responses
- **Services**: Contain business logic and validation rules
- **Repositories**: Manage data access (in-memory storage)
- **DTOs**: Control data flow between layers
- **Models**: Core domain entities

### Key Decisions
1. **Repository Pattern**: Even with in-memory storage, using repositories makes the code maintainable and testable
2. **Dependency Injection**: Built-in DI container for loose coupling
3. **Async/Await**: All operations are async for scalability
4. **Structured Logging**: Added logging for monitoring and debugging
5. **Global Error Handling**: Middleware to handle uncaught exceptions consistently

### Business Rules Implementation
- Status transitions are validated in the service layer
- Completed tasks are immutable (no status updates allowed)
- CreatedAt is always UTC
- Title length validation (max 50 chars)

## Trade-offs and Improvements

### What I'd improve with more time:
1. **Pagination**: Add pagination for GET /tasks endpoint
2. **Caching**: Implement response caching for frequently accessed data
3. **Request Rate Limiting**: Protect API from abuse
4. **More Comprehensive Tests**: Add integration tests and more unit tests
5. **OpenAPI/Swagger Documentation**: Add detailed XML comments for better API docs
6. **Health Checks**: Add health check endpoint for monitoring
7. **Validation FluentValidation**: Use FluentValidation for more complex validation rules
8. **Sorting**: Implement the bonus sorting feature by priority
9. **Docker**: Containerize the application for easy deployment

### Current Trade-offs
1. **In-memory Storage**: Fast but data is lost on restart (as required)
2. **Simple Validation**: Using DataAnnotations instead of FluentValidation for simplicity
3. **No Authentication**: Skipped as not required, but would be essential in production
4. **Minimal Comments**: Code is self-documenting with clear naming

## Running the Application

```bash
dotnet restore
dotnet build
dotnet run