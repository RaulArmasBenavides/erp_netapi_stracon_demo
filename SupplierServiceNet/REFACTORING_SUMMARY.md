# Refactoring Summary: Error Handling & Configuration

## Issues Fixed

### 1. ✅ Hardcoded String Literals for Configuration

**Before:**
```csharp
// UserService.cs - Repeated in multiple places
string keyconfig = _config.GetSection("ApiSettings:Secreta").Value.ToString();
```

**After:**
- Created `ApiSettings.cs` with strongly-typed options
- Used `IOptions<ApiSettings>` pattern
- Registered in DI with `services.Configure<ApiSettings>()`

**Files Changed:**
- ✅ Created: `CrossCutting/Options/ApiSettings.cs`
- ✅ Updated: `Extensions/ApplicationServicesExtensions.cs`
- ✅ Updated: `Application/Services/UserService.cs`
- ✅ Updated: `appsettings.json`

**Benefits:**
- No more magic strings
- Type-safe configuration
- Centralized & reusable
- Easy to unit test with mock IOptions

---

### 2. ✅ Generic Exception Handling

**Before:**
```csharp
// UserService.cs
throw new Exception("Credenciales inválidas");

// Controllers had to catch generic Exception
// Middleware couldn't differentiate error types
```

**After:**
Created custom exception types:
- `AuthenticationException` - Login failures
- `UnauthorizedException` - Missing/invalid claims
- `ValidationException` - Data validation errors
- `NotFoundException` - Resource not found

**Files Created:**
- ✅ `CrossCutting/Exceptions/AuthenticationException.cs`
- ✅ `CrossCutting/Exceptions/UnauthorizedException.cs`
- ✅ `CrossCutting/Exceptions/ValidationException.cs`
- ✅ `CrossCutting/Exceptions/NotFoundException.cs`

**Updated:**
- ✅ `Application/Services/UserService.cs` - Uses AuthenticationException
- ✅ `Middlewares/ExceptionMiddleware.cs` - Handles each exception type specifically

**Benefits:**
- Middleware returns appropriate HTTP status codes (401, 400, 404, etc.)
- Detailed logging per exception type
- Clients receive consistent error responses
- Easier to debug production issues

---

### 3. ✅ Centralized User Context Extraction

**Before:**
```csharp
// Duplicated in SuppliersController (2 methods), LoginController
var identifier = User.FindFirstValue(ClaimTypes.Email)
    ?? User.FindFirstValue("email")
    ?? User.Identity?.Name
    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? User.FindFirstValue("sub");
```

**After:**
Created `IUserContextService` abstraction:
```csharp
public interface IUserContextService
{
    string GetUserIdentifier();
}
```

**Files Created:**
- ✅ `Application/Interfaces/IUserContextService.cs`
- ✅ `Application/Services/UserContextService.cs`

**Updated:**
- ✅ `Controllers/SuppliersController.cs` - Uses injected service
- ✅ `Extensions/ApplicationServicesExtensions.cs` - Registers service

**Benefits:**
- DRY principle - logic in one place
- Unit testable - mock the interface
- Throws `UnauthorizedException` with clear messages
- Consistent error handling across controllers

---

## Migration Guide

### 1. Update appsettings.json

```json
{
  "ApiSettings": {
    "Secreta": "your-jwt-secret",
    "TokenExpirationDays": 7
  }
}
```

### 2. Update Program.cs (if needed)

Already updated `ApplicationServicesExtensions`:
```csharp
builder.Services.AddApplicationServices(builder.Configuration);
// Now handles ApiSettings configuration automatically
```

### 3. Use IUserContextService in Controllers

```csharp
public class YourController : ControllerBase
{
    private readonly IUserContextService _userContextService;

    public YourController(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    [Authorize]
    [HttpPost("action")]
    public async Task<IActionResult> YourAction()
    {
        try
        {
            var userId = _userContextService.GetUserIdentifier();
            // ... your logic
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
```

---

## Exception Handling Examples

### Login Error
```csharp
// UserService.cs
if (!isValid)
{
    throw new AuthenticationException("Invalid credentials");
}
// → Middleware returns 401 Unauthorized
```

### Missing User Context
```csharp
// UserContextService.cs
if (user == null || !user.Identity?.IsAuthenticated ?? false)
    throw new UnauthorizedException("User is not authenticated.");
// → Middleware returns 401 Unauthorized
```

### Validation Error
```csharp
if (string.IsNullOrWhiteSpace(dto.Email))
{
    throw new ValidationException("Email is required");
}
// → Middleware returns 400 Bad Request
```

### Resource Not Found
```csharp
if (supplier is null)
{
    throw new NotFoundException($"Supplier with ID {id} not found");
}
// → Middleware returns 404 Not Found
```

---

## Testing Examples

### Unit Test UserContextService
```csharp
[Fact]
public void GetUserIdentifier_NoAuthenticatedUser_ThrowsException()
{
    // Arrange
    var httpContextAccessor = new Mock<IHttpContextAccessor>();
    httpContextAccessor.Setup(x => x.HttpContext!.User)
        .Returns(new ClaimsPrincipal()); // Not authenticated

    var service = new UserContextService(httpContextAccessor.Object);

    // Act & Assert
    Assert.Throws<UnauthorizedException>(() => service.GetUserIdentifier());
}
```

### Mock IOptions<ApiSettings>
```csharp
[Fact]
public async Task Login_ValidCredentials_ReturnsToken()
{
    // Arrange
    var apiSettings = Options.Create(new ApiSettings 
    { 
        Secreta = "test-secret", 
        TokenExpirationDays = 7 
    });
    var service = new UserService(..., apiSettings);

    // Act
    var result = await service.Login(new LoginUserDto(...));

    // Assert
    Assert.NotNull(result.Access_token);
}
```

---

## Backward Compatibility

⚠️ **Breaking Changes:**
- Generic `Exception` is no longer thrown
- Controllers must now catch specific exception types
- Controllers using old pattern must be updated to use `IUserContextService`

**Affected Controllers:**
- ✅ `SuppliersController` - Already updated
- ⏳ `UsersController` - Not using claims (no changes needed)
- ⏳ `LoginController` - Not using claims (no changes needed)

---

## Remaining TODO Items

Related issues to address:
- [ ] Remove secrets from `appsettings.json` (move to User Secrets / Key Vault)
- [ ] Implement refresh token pattern (shorter token expiry)
- [ ] Add FluentValidation for DTOs
- [ ] Add logging to `SupplierService` and other services
- [ ] Implement correlation IDs for request tracing
- [ ] Add integration tests
