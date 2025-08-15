# SHFT - Diet Plan Management API

A modern ASP.NET Core Web API project developed for a dietitian-client management system.

## 📋 Features

### 🔐 Authentication & Authorization

- **JWT Bearer Token** based authentication
- User management with **ASP.NET Core Identity**
- **Role-based Authorization** (Admin, Dietitian, Client)
- Password reset and change operations
- Refresh token support

### 👥 User Types

1. **Admin**: System administrator - can manage all users
2. **Dietitian**: Can create diet plans for their clients
3. **Client**: Can view their diet plans

### 🏗️ Architecture

- Follows **Clean Architecture** principles
- Data access with **Repository Pattern**
- Model validation with **FluentValidation**
- PostgreSQL integration with **Entity Framework Core**
- Object mapping with **AutoMapper**

## 🚀 Installation

### Requirements

- .NET 8.0 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Steps

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd SHFT
   ```

2. **Configure database connection**

   Update the connection string in `SHFTAPI/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=SHFTDb;Username=your_username;Password=your_password;"
     }
   }
   ```

3. **Configure JWT settings**

   Update JWT settings in `appsettings.json`:

   ```json
   {
     "JWT": {
       "Secret": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
       "ValidIssuer": "SHFTAPI",
       "ValidAudience": "SHFTClient",
       "AccessTokenExpiration": 1440
     }
   }
   ```

4. **Run database migrations**

   ```bash
   cd SHFTAPI
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

The API will run at: `https://localhost:7000`
Swagger UI: `https://localhost:7000/swagger`

## 📚 API Endpoints

### 🔐 Authentication (`/api/auth`)

- `POST /login` - User login
- `POST /register` - New user registration
- `POST /refresh-token` - Token refresh
- `POST /logout` - User logout
- `POST /change-password` - Change password
- `POST /forgot-password` - Password reset request
- `POST /reset-password` - Password reset

### 👥 Users (`/api/users`)

- `GET /` - List all users (Admin)
- `GET /{id}` - User details
- `GET /role/{roleName}` - Users by role (Admin)
- `GET /dietitians` - List dietitians
- `GET /clients` - List clients (Admin/Dietitian)
- `POST /` - Create new user (Admin)
- `PUT /{id}` - Update user
- `DELETE /{id}` - Delete user (Admin)

### 🥗 Diet Plans (`/api/dietplans`)

- CRUD operations
- Dietitian-client assignments
- Plan status management

### 🍽️ Meals (`/api/meals`)

- Meal management
- Nutritional value calculations
- Meal type categories

## 🔑 Authentication Usage

### 1. Registration

```json
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "userName": "johndoe",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "userType": "Client"
}
```

### 2. Login

```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:**

```json
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expirationDate": "2024-01-02T10:30:00Z",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
  },
  "message": "Login successful",
  "isSuccess": true,
  "statusCode": 200
}
```

### 3. Token Usage

Use Bearer token in Authorization header for API calls:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🛡️ Authorization

### Role-based Access Control

- **Admin**: Access to all endpoints
- **Dietitian**: Access to their clients and diet plans
- **Client**: Access only to their own information

### Example Usage

```csharp
[Authorize(Roles = "Admin,Dietitian")]
public async Task<IActionResult> GetClients()
{
    // Only Admin and Dietitian roles can access
}
```

## 🗃️ Database Structure

### Main Tables

- **AspNetUsers**: User information (ASP.NET Identity)
- **AspNetRoles**: Roles
- **DietPlans**: Diet plans
- **Meals**: Meals

### Relationships

- Dietitian → Clients (1:N)
- Dietitian → DietPlans (1:N) (creator)
- Client → DietPlans (1:N) (owner)
- DietPlan → Meals (1:N)

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SHFTDb;Username=postgres;Password=yourpassword;"
  },
  "JWT": {
    "Secret": "YourSecretKey32CharactersMinimum!",
    "ValidIssuer": "SHFTAPI",
    "ValidAudience": "SHFTClient",
    "AccessTokenExpiration": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🧪 Testing

### Swagger UI

- You can test the API at `https://localhost:7000/swagger`
- Use the "Authorize" button for JWT Bearer token

### Test Users

You can create sample users in development environment:

```json
// Admin
{
  "email": "admin@shft.com",
  "password": "Admin123!",
  "userType": "Admin"
}

// Dietitian
{
  "email": "dietitian@shft.com",
  "password": "Diet123!",
  "userType": "Dietitian"
}

// Client
{
  "email": "client@shft.com",
  "password": "Client123!",
  "userType": "Client"
}
```

## 📦 Technologies Used

- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **PostgreSQL**
- **JWT Bearer Authentication**
- **FluentValidation**
- **AutoMapper**
- **Swagger/OpenAPI**

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.
