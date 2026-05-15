# Security and Configuration — DrugstoreSystem

---

## 1. Secrets Management

### Rule: zero secrets in git

`appsettings.json` and `appsettings.Development.json` contain only placeholder values or public defaults. Real secrets live in `dotnet user-secrets` during development.

### Setup commands

```bash
# Initialize (run once, from repo root)
dotnet user-secrets init --project src/DrugstoreSystem.Web

# PostgreSQL connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=drugstore;Username=postgres;Password=YOUR_PASSWORD"

# Admin seed password
dotnet user-secrets set "Seed:AdminPassword" "Admin@123!"
```

### `appsettings.json` (safe to commit)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SEE USER-SECRETS"
  },
  "Seed": {
    "AdminEmail": "admin@drugstore.local",
    "AdminPassword": "SEE USER-SECRETS"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/drugstore-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

---

## 2. `.gitignore` — Required Entries

```gitignore
# Build output
bin/
obj/
.vs/

# User secrets
%APPDATA%/Microsoft/UserSecrets/

# Logs
logs/

# Environment files
*.env
.env.*
appsettings.Production.json
appsettings.Staging.json

# Rider / VS artifacts
.idea/
*.user
*.suo
```

---

## 3. Authentication & Authorization

### Cookie-based auth (ASP.NET Identity)

```csharp
// Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
```

### Roles

| Role | Seeded? | Notes |
|---|---|---|
| `Admin` | Yes (once, on first startup) | Email: `admin@drugstore.local` |
| `Pharmacist` | Created by Admin | One per pharmacy |

### Page-level authorization

```razor
@* Protected admin page *@
@attribute [Authorize(Roles = "Admin")]

@* Protected pharmacist page *@
@attribute [Authorize(Roles = "Pharmacist")]

@* Public page — no attribute *@
```

### Pharmacist data isolation

Pharmacist users can only access their own pharmacy's data. The service layer enforces this:

```csharp
// PharmacyService.cs
public async Task UpdateAsync(int pharmacyId, UpdatePharmacyRequest request)
{
    var callerPharmacyId = _currentUser.PharmacyId
        ?? throw new UnauthorizedAccessException();

    if (callerPharmacyId != pharmacyId)
        throw new UnauthorizedAccessException();
    // proceed
}
```

`ICurrentUser` is implemented in Infrastructure using `IHttpContextAccessor`.

---

## 4. SQL Injection Prevention

All user input that reaches the database goes through:
1. **EF Core LINQ** — parameterized automatically
2. **Raw SQL in SearchRepository** — uses `FormattableString` (`$""`) or explicit `NpgsqlParameter` objects

```csharp
// CORRECT — parameterized
var results = await _context.Database
    .SqlQuery<MedicineCandidateDto>($"""
        SELECT ... FROM medicines m
        WHERE LOWER(m.name) LIKE '%' || {query} || '%'
        ...
    """)
    .ToListAsync();

// WRONG — never do this
var sql = $"SELECT ... WHERE name LIKE '%{userInput}%'";
```

---

## 5. pg_trgm Security Note

The `pg_trgm` extension is installed per-database, not per-schema. It is read-only from the application's perspective — it adds operator classes but does not expose admin functions. No additional security considerations beyond standard SQL injection prevention.

---

## 6. Demo Configuration (defense day)

For the thesis defense (no internet, local machine), the app runs with:

```bash
# Start PostgreSQL (if using Docker)
docker run -d --name drugstore-pg \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16

# Run the app
dotnet run --project src/DrugstoreSystem.Web
```

If using local PostgreSQL installation, just run `dotnet run`. Migrations apply automatically on startup. Seed data loads if the `pharmacies` table is empty.

---

## 7. Logging Security Rules

Never log:
- Connection strings
- Passwords or password hashes
- Full user IP addresses (if collected)

Safe to log:
- Pharmacy IDs and names
- Search queries (for debugging)
- HTTP status codes and response times

```csharp
// WRONG
_logger.LogInformation("Connecting with {ConnStr}", connectionString);

// CORRECT
_logger.LogInformation("Database connection established");
```
