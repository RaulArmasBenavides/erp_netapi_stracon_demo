# Deuda Técnica Restante

Análisis completo de los problemas técnicos que aún necesitan solución después de las mejoras realizadas.

---

## 🔴 CRÍTICO - Seguridad

### 1. Secretos en Código Fuente
**Severidad**: 🔴 CRÍTICA  
**Ubicación**: `appsettings.json`

**Problema**:
```json
{
  "ApiSettings": {
    "Secreta": "Stracon@Jwt#2026!d8F3kL9QmA2RZxV7HcWJpS5N"
  },
  "ConnectionStrings": {
    "ConexionSql": "Server=localhost,14333;Database=stracon_db;User Id=sa;..."
  },
  "Cloudinary": {
    "ApiKey": "913758167669777",
    "ApiSecret": "JL6Bv8wOXBsmcp_JO3_JQSVDBMo",
  }
}
```

**Impacto**: 
- ⚠️ Credenciales expuestas en repositorio
- ⚠️ Cualquiera con acceso al repo tiene acceso a servicios críticos
- ⚠️ Auditoría de seguridad fallará

**Solución**:
```bash
# 1. Mover a User Secrets (desarrollo)
dotnet user-secrets init
dotnet user-secrets set "ApiSettings:Secreta" "your-secret"
dotnet user-secrets set "ConnectionStrings:ConexionSql" "your-connection"

# 2. Mover a Environment Variables (producción)
export API_SETTINGS_SECRETA="..."
export CONNECTIONSTRINGS_CONEXIONSQL="..."

# 3. O usar Azure Key Vault (recomendado)
builder.Configuration.AddAzureKeyVault(...);
```

---

### 2. Token JWT sin Refresh Token Pattern
**Severidad**: 🔴 CRÍTICA  
**Ubicación**: `UserService.cs:75`

**Problema**:
```csharp
Expires = DateTime.UtcNow.AddDays(_apiSettings.TokenExpirationDays),
```

**Impacto**:
- ❌ Tokens válidos por 7 días (muy largo)
- ❌ Si alguien roba un token, puede usarlo 7 días
- ❌ No hay forma de revocar tokens
- ❌ No hay refresh tokens

**Solución Correcta**:
```csharp
// Access Token: 15 minutos
var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);

// Refresh Token: 7 días
var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

// Retornar ambos
return new AuthResponseDto 
{
    AccessToken = accessToken,
    RefreshToken = refreshToken,
    ExpiresIn = 900 // 15 minutos en segundos
};
```

**Endpoint a Agregar**:
```csharp
[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
{
    // Validar refresh token
    // Generar nuevo access token
    // Opcionalmente generar nuevo refresh token (rotation)
}
```

---

### 3. Validación Débil de Email en Registro
**Severidad**: 🟡 ALTO  
**Ubicación**: `UserService.cs:91-109`

**Problema**:
```csharp
public async Task<DataUserDto> Registro(UsuarioRegistroDto dto)
{
    var usuario = new User
    {
        UserName = dto.UserName,
        Email = dto.Email,
        // ❌ No hay validación de email
        // ❌ No hay validación de password strength
        // ❌ No hay verificación de email
    };
}
```

**Falta**:
- ❌ Email verification flow
- ❌ Password complexity requirements
- ❌ Rate limiting en registro
- ❌ Email uniqueness validation

**Solución**:
```csharp
public async Task<DataUserDto> Registro(UsuarioRegistroDto dto)
{
    // 1. Validar password strength
    if (dto.Password.Length < 8 || !Regex.IsMatch(dto.Password, @"[A-Z]"))
        throw new ValidationException("Password debe tener 8+ chars y mayúscula");

    // 2. Verificar email no existe
    var existingUser = await _userManager.FindByEmailAsync(dto.Email);
    if (existingUser != null)
        throw new ValidationException("Email ya registrado");

    // 3. Crear usuario
    var user = new User { ... };
    var result = await _userManager.CreateAsync(user, dto.Password);

    // 4. Enviar email de verificación
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    await _emailService.SendVerificationEmailAsync(user.Email, token);

    return _mapper.Map<DataUserDto>(user);
}
```

---

## 🟠 ALTO - Arquitectura & Patrones

### 4. No hay DTOs en UsersController
**Severidad**: 🟠 ALTO  
**Ubicación**: `UsersController.cs:32-56`

**Problema**:
```csharp
[HttpGet]
public IActionResult GetUsers()
{
    var listaUsuarios = this.usService.GetUsuariosAsync(); // ❌ Retorna entidades
    return Ok(listaUsuarios); // ❌ Expone User completo (con password hash)
}

[HttpGet("{usuarioId:int}")]
public IActionResult GetUser(int usuarioId)
{
    var itemUsuario = this.usService.GetUsuario(usuarioId.ToString());
    var itemUsuarioDto = _mapper.Map<UserDto>(itemUsuario); // ✅ Aquí lo hace
    return Ok(itemUsuarioDto);
}
```

**Impacto**:
- ⚠️ Inconsistencia: GetUsers no usa DTO, GetUser sí
- ⚠️ GetUsers expone información sensible
- ⚠️ Violación de patrón SOLID

**Solución**:
```csharp
[HttpGet]
public async Task<IActionResult> GetUsers()
{
    var users = await this.usService.GetUsuariosAsync();
    var dtos = users.Select(u => _mapper.Map<UserDto>(u)).ToList();
    return Ok(dtos);
}
```

---

### 5. Métodos Síncronos sin Async
**Severidad**: 🟠 ALTO  
**Ubicación**: Multiple

**Problema**:
```csharp
// UsersController.cs
[HttpGet]
public IActionResult GetUsers()  // ❌ No es async
{
    var listaUsuarios = this.usService.GetUsuariosAsync(); // ❌ Espera async sin await
    return Ok(listaUsuarios);
}

[HttpGet("{usuarioId:int}")]
public IActionResult GetUser(int usuarioId) // ❌ No es async
{
    var itemUsuario = this.usService.GetUsuario(usuarioId.ToString());
    return Ok(itemUsuarioDto);
}

[HttpPost]        
public async Task<IActionResult> CreateUserAsync(...) // ✅ Esto es async
{
    var result = await this.usService.Registro(...);
    return Ok(result);
}
```

**Impacto**:
- ⚠️ Inconsistencia de patrón (algunos async, otros no)
- ⚠️ GetUsers() retorna Task sin await
- ⚠️ Thread pool starvation en alta carga

**Solución**:
```csharp
[HttpGet]
public async Task<IActionResult> GetUsers()
{
    var listaUsuarios = await this.usService.GetUsuariosAsync();
    return Ok(listaUsuarios);
}

[HttpGet("{usuarioId:int}")]
public async Task<IActionResult> GetUser(int usuarioId)
{
    var itemUsuario = await this.usService.GetUsuario(usuarioId.ToString());
    return Ok(_mapper.Map<UserDto>(itemUsuario));
}
```

---

### 6. CancellationTokens Ignorados
**Severidad**: 🟠 ALTO  
**Ubicación**: `SupplierService.cs` (varios métodos)

**Problema**:
```csharp
public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
{
    // ❌ ct no se pasa a métodos async
    return await _uow.Suppliers.GetAllAsync();
}

public async Task<Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default)
{
    // ❌ ct no se pasa
    var uploadResult = await this._cloudinaryService.UploadImageAsync(dto.Photo, ..., ct);
    await _uow.Suppliers.AddAsync(supplier);
    // ✅ Aquí sí se pasa
    await _uow.SaveChangesAsync();
}
```

**Impacto**:
- ⚠️ Client cancellation no se respeta
- ⚠️ Operaciones continúan incluso si cliente se desconecta
- ⚠️ Desperdicio de recursos

**Solución**: Pasar `ct` a todos los métodos async

```csharp
public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
{
    return await _uow.Suppliers.GetAllAsync(ct);
}
```

---

### 7. Hardcoded String Literals en Roles
**Severidad**: 🟡 MEDIO  
**Ubicación**: Multiple

**Problema**:
```csharp
[Authorize(Roles = "Requester,Approver")] // ❌ Magic string
[HttpGet]
public async Task<IActionResult> ExportSuppliers(CancellationToken ct)

// SeedData.cs
string[] roles = { "Requester", "Approver" }; // ❌ Duplicado

// UserService.cs
await _userManager.AddToRoleAsync(usuario, "Requester"); // ❌ Hardcoded
```

**Solución**:
```csharp
// CrossCutting/Constants/RoleConstants.cs
public static class RoleConstants
{
    public const string Requester = "Requester";
    public const string Approver = "Approver";
    
    public static readonly string[] AllRoles = { Requester, Approver };
}

// Uso
[Authorize(Roles = $"{RoleConstants.Requester},{RoleConstants.Approver}")]
```

---

## 🟡 MEDIO - Errores & Validación

### 8. ApplicationException en Registro de Usuario
**Severidad**: 🟡 MEDIO  
**Ubicación**: `UserService.cs:108`

**Problema**:
```csharp
if (!result.Succeeded)
{
    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
    throw new ApplicationException($"Error al registrar usuario: {errors}"); // ❌ Tipo genérico
}
```

**Impacto**:
- ⚠️ ApplicationException no es específica
- ⚠️ Middleware no la maneja especialmente
- ⚠️ Cliente recibe 500 en lugar de 400

**Solución**:
```csharp
if (!result.Succeeded)
{
    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
    throw new ValidationException($"Registration failed: {errors}");
}
```

---

### 9. No hay Logging en Métodos Críticos
**Severidad**: 🟡 MEDIO  
**Ubicación**: Services

**Problema**:
```csharp
public async Task<Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default)
{
    // ❌ No hay logging de entrada
    var uploadResult = await this._cloudinaryService.UploadImageAsync(...);
    // ❌ No hay logging de éxito/fallo
    await _uow.Suppliers.AddAsync(supplier);
    await _uow.SaveChangesAsync();
    // ❌ No hay logging de salida
    return supplier;
}
```

**Solución**:
```csharp
public async Task<Supplier> CreateAsync(CreateSupplierDto dto, string createdBy, CancellationToken ct = default)
{
    _logger.LogInformation("Creating supplier: {Name} by {CreatedBy}", dto.Name, createdBy);
    
    try
    {
        var uploadResult = await this._cloudinaryService.UploadImageAsync(...);
        var supplier = new Supplier(...);
        await _uow.Suppliers.AddAsync(supplier);
        await _uow.SaveChangesAsync();
        
        _logger.LogInformation("Supplier created successfully: {Id}", supplier.Id);
        return supplier;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create supplier: {Name}", dto.Name);
        throw;
    }
}
```

---

### 10. No hay Validación en DTOs
**Severidad**: 🟡 MEDIO  
**Ubicación**: `CreateSupplierDto`, `UsuarioRegistroDto`

**Problema**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateSupplier([FromForm] CreateSupplierDto dto, CancellationToken ct)
{
    // ❌ No hay validación automática
    var createdBy = _userContextService.GetUserIdentifier();
    var created = await _supplierService.CreateAsync(dto, createdBy, ct);
}
```

**Solución - Opción 1: Data Annotations**:
```csharp
public class CreateSupplierDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [StringLength(500)]
    public string Address { get; set; }
}
```

**Solución - Opción 2: FluentValidation** (Recomendado):
```csharp
public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 100).WithMessage("Name must be 3-100 chars");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone format");
    }
}

// En Program.cs
builder.Services.AddScoped<IValidator<CreateSupplierDto>, CreateSupplierDtoValidator>();
```

---

## 🟡 MEDIO - Data Access

### 11. Repository Pattern Incompleto
**Severidad**: 🟡 MEDIO  
**Ubicación**: `Repository.cs:33-42`

**Problema**:
```csharp
public T Get(int id)
{
    return _dbset.Find(id); // ❌ Síncrono
}

public async Task<T?> GetAsync(int id, CancellationToken ct = default)
{
    return await _dbset.FindAsync(new object[] { id }, ct); // ❌ Requiere int ID
}

// Pero Supplier usa Guid
public async Task<Supplier?> GetByIdAsync(Guid id)
{
    // ❌ FindAsync(Guid) no funciona
}
```

**Impacto**:
- ⚠️ Método genérico no funciona para entidades con Guid
- ⚠️ Duplicación de código en repositories específicos
- ⚠️ Mezcla de sync/async

**Solución**:
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}

public class Repository<T> : IRepository<T> where T : class
{
    public async Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken ct = default)
    {
        return await _dbset.FindAsync(new object[] { id }, cancellationToken: ct);
    }
}
```

---

### 12. No hay Soft Delete Pattern
**Severidad**: 🟡 MEDIO  
**Ubicación**: Entities

**Problema**:
```csharp
public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
{
    var supplier = await _uow.Suppliers.GetByIdAsync(id);
    if (supplier is null) return false;

    _uow.Suppliers.Remove(supplier); // ❌ Hard delete
    await _uow.SaveChangesAsync();
    return true;
}
```

**Impacto**:
- ⚠️ Datos se pierden permanentemente
- ⚠️ No hay auditoría de eliminaciones
- ⚠️ Problemas con datos referenciados

**Solución**:
```csharp
public class Supplier
{
    // Agregar propiedades
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

// En repository
public async Task<bool> DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
{
    var supplier = await _dbset.FindAsync(new object[] { id }, cancellationToken: ct);
    if (supplier is null) return false;

    supplier.IsDeleted = true;
    supplier.DeletedAt = DateTime.UtcNow;
    supplier.DeletedBy = deletedBy;
    
    await _dbset.SaveChangesAsync(ct);
    return true;
}

// Al consultar
public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
{
    return await _dbset
        .Where(s => !s.IsDeleted)
        .ToListAsync(ct);
}
```

---

## 🟢 BAJO - Optimización

### 13. No hay Caching
**Severidad**: 🟢 BAJO  
**Ubicación**: Controllers

**Problema**:
```csharp
[HttpGet]
public async Task<IActionResult> GetSuppliers(CancellationToken ct)
{
    var suppliers = await _supplierService.GetAllAsync(ct); // ❌ Query a DB siempre
    return Ok(suppliers);
}
```

**Solución**:
```csharp
public async Task<IActionResult> GetSuppliers(CancellationToken ct)
{
    const string cacheKey = "suppliers_all";
    
    if (!_cache.TryGetValue(cacheKey, out var suppliers))
    {
        suppliers = await _supplierService.GetAllAsync(ct);
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        _cache.Set(cacheKey, suppliers, options);
    }
    
    return Ok(suppliers);
}
```

---

### 14. No hay Paginación en GET
**Severidad**: 🟢 BAJO  
**Ubicación**: `GetSuppliers`, `GetUsers`

**Problema**:
```csharp
[HttpGet]
public async Task<IActionResult> GetSuppliers(CancellationToken ct)
{
    var suppliers = await _supplierService.GetAllAsync(ct); // ❌ Retorna TODO
    return Ok(suppliers); // ❌ Podría ser 10K+ registros
}
```

**Solución**:
```csharp
[HttpGet]
public async Task<IActionResult> GetSuppliers(
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
{
    var result = await _supplierService.GetPaginatedAsync(page, pageSize, ct);
    return Ok(result);
}

// Response
{
    "data": [...],
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
}
```

---

### 15. No hay Rate Limiting
**Severidad**: 🟢 BAJO  
**Ubicación**: Program.cs

**Solución**:
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", p =>
    {
        p.PermitLimit = 100;
        p.Window = TimeSpan.FromMinutes(1);
    });
});

app.UseRateLimiter();

// En controller
[HttpPost("import")]
[EndpointRateLimiter("fixed")]
public async Task<IActionResult> ImportSuppliers(...)
```

---

## 📊 Resumen de Deuda Técnica

| Categoría | Cantidad | Severidad |
|-----------|----------|-----------|
| 🔴 Crítico | 3 | Seguridad token |
| 🟠 Alto | 4 | Arquitectura/async |
| 🟡 Medio | 5 | Validación/logging |
| 🟢 Bajo | 3 | Optimización |
| **Total** | **15** | |

---

## 🎯 Plan de Remediación Priorizado

### Sprint 1 (INMEDIATO):
1. ✅ Mover secretos a User Secrets / Environment Variables
2. ✅ Implementar refresh token pattern
3. ✅ Agregar email verification flow

### Sprint 2 (PRÓXIMO):
4. ✅ Arreglar métodos async en UsersController
5. ✅ Pasar CancellationTokens a todos métodos
6. ✅ Agregar logging a servicios críticos

### Sprint 3:
7. ✅ Implementar validación con FluentValidation
8. ✅ Agregar soft delete pattern
9. ✅ Mejorar repository pattern (genérico para Guid/int)

### Sprint 4:
10. ✅ Agregar caching en memoria
11. ✅ Implementar paginación
12. ✅ Agregar rate limiting

---

## 📝 Notas

- La mayoría de problemas 🔴 CRÍTICO deben resolverse ANTES de producción
- Los 🟠 ALTO son bloqueadores para código quality
- Los 🟡 MEDIO y 🟢 BAJO pueden hacerse de forma incremental
- Todos estos temas están documentados en REFACTORING_SUMMARY.md
