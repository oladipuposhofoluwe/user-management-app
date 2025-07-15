# User Management API

A RESTful ASP.NET Core Web API for managing users with secure authentication, clean architecture, background job processing, and full test coverage.

---

# Features

✅ RESTful CRUD API for managing users  
✅ JWT-based Authentication and Role-based Authorization  
✅ Entity Framework Core with Migrations  
✅ Global Exception Handling with Consistent Error Responses  
✅ Serilog for File and Console Logging  
✅ Hangfire for Background Jobs (e.g. Welcome Email, Password Reset)  
✅ Kafka Message Publishing (`user.created` topic - mocked)  
✅ Clean Architecture (Domain, Application, Infrastructure, API layers)  
✅ Swagger UI with JWT Bearer Token Support  
✅ xUnit Unit Tests with Mocking  
✅ Pagination, Filtering  for listing of users
✅ Soft Delete Support  

---

# Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- SQL / MySQL
- JWT Authentication
- Hangfire
- Kafka / MockKafka
- Serilog
- Swagger / Swashbuckle
- xUnit + Moq

---

## 🗃️ Domain Model: User, RefreshToken

