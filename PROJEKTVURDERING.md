# Prosjektvurdering - IS-202 Obstacle Reporting System

**Dato:** 2025-01-27  
**Prosjekt:** Obstacle Reporting System (Gruppe 11)

---

## 📋 Oversikt

Dette er en ASP.NET Core 9 MVC-applikasjon for rapportering og administrasjon av flyhindere. Systemet bruker MariaDB, Identity Framework for autentisering, og Leaflet for kartvisualisering.

---

## ✅ Styrker

### 1. Arkitektur og Kodekvalitet

**Positivt:**
- ✅ **God MVC-separasjon**: Klar separasjon mellom Controllers, Services, Models og Views
- ✅ **Service-lag**: Bruk av `IObstacleService` og `ObstacleService` for forretningslogikk
- ✅ **Dependency Injection**: Korrekt bruk av DI i `Program.cs`
- ✅ **Konsistent navngiving**: Tydelige og beskrivende navn på klasser og metoder
- ✅ **XML-dokumentasjon**: God bruk av `<summary>`-kommentarer i controllers
- ✅ **Moderne .NET 9**: Bruker nyeste versjon av .NET

**Eksempler:**
```csharp
// God separasjon av ansvar
public class ObstacleService : IObstacleService
{
    private readonly ApplicationDbContext _db;
    // Service håndterer forretningslogikk, ikke controller
}
```

### 2. Sikkerhet

**Implementerte tiltak:**
- ✅ **CSRF-beskyttelse**: `[ValidateAntiForgeryToken]` på alle POST-endepunkter
- ✅ **SQL Injection-beskyttelse**: EF Core bruker parametriserte spørringer (automatisk)
- ✅ **Autorisering**: Bruk av `[Authorize]` med roller (`Pilot`, `Registerforer`, `Admin`)
- ✅ **XSS-beskyttelse**: Razor Views escape HTML automatisk
- ✅ **Identity Framework**: Bruker Microsoft Identity for autentisering
- ✅ **Rollebasert tilgang**: Korrekt implementert i `ObstacleService.GetOverviewAsync()`

**Eksempler:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]  // ✅ CSRF-beskyttelse
[Authorize(Roles = AppRoles.Admin)]  // ✅ Autorisering
public async Task<IActionResult> Create(...)
```

### 3. Database og Migrasjoner

**Positivt:**
- ✅ **EF Core Migrations**: Strukturert migrasjonshåndtering
- ✅ **SeedData**: Automatisk seeding av roller og testbrukere ved oppstart
- ✅ **Connection String-håndtering**: God feilhåndtering hvis connection string mangler
- ✅ **Retry-logikk**: `EnableRetryOnFailure` for database-tilkoblinger
- ✅ **Health checks**: Docker health check for MariaDB

### 4. Testing

**Positivt:**
- ✅ **Omfattende testdekning**: Mange enhetstester og integrasjonstester
- ✅ **xUnit**: Moderne testrammeverk
- ✅ **InMemory Database**: Bruk av InMemory database for integrasjonstester
- ✅ **Tester for autorisering**: Tester for rollebasert tilgang
- ✅ **Tester for validering**: Tester for negative høyder, tom geometri, etc.

**Testkategorier:**
- Enhetstester (model-validering)
- Integrasjonstester (service + database)
- Autoriseringstester
- Valideringstester

### 5. Docker og Deployment

**Positivt:**
- ✅ **Docker Compose**: God strukturert `compose.yaml`
- ✅ **Separate containere**: Applikasjon og database i separate containere
- ✅ **Health checks**: Database venter på health check før applikasjon starter
- ✅ **Miljøvariabler**: Korrekt bruk av miljøvariabler for konfigurasjon

### 6. Kodeorganisering

**Positivt:**
- ✅ **Mappe-struktur**: Logisk organisering (Controllers/, Models/, Services/, Views/)
- ✅ **ViewModels**: Separasjon mellom domain models og view models
- ✅ **Statiske roller**: Bruk av `AppRoles`-klasse for konsistens

---

## ⚠️ Problemer og Forbedringsområder

### 1. **KRITISK: Mismatch mellom Interface og Implementasjon**

**Problem:**
- `ObstacleServiceIntegrationTests.cs` tester `DeleteAsync()`-metoden
- `IObstacleService`-interfacet mangler `DeleteAsync()`-metoden
- `ObstacleService`-klassen mangler `DeleteAsync()`-implementasjonen

**Konsekvens:**
- Testene vil ikke kompilere eller kjøre
- Funksjonalitet som testes eksisterer ikke i produksjonskoden

**Løsning:**
```csharp
// Legg til i IObstacleService.cs:
Task<bool> DeleteAsync(int id, ClaimsPrincipal user);

// Legg til i ObstacleService.cs:
public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
{
    var e = await _db.Obstacles.FindAsync(id);
    if (e == null)
        return false;

    var isRegistrar = user.IsInRole(AppRoles.Registrar);
    if (!isRegistrar)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (e.CreatedByUserId != userId)
            throw new UnauthorizedAccessException();
    }

    _db.Obstacles.Remove(e);
    await _db.SaveChangesAsync();
    return true;
}
```

### 2. **KRITISK: Test Constructor Mismatch**

**Problem:**
- `ObstacleServiceIntegrationTests.cs` linje 28 bruker:
  ```csharp
  _service = new ObstacleService(_db, NullLogger<ObstacleService>.Instance);
  ```
- Men `ObstacleService`-konstruktøren tar kun `ApplicationDbContext`:
  ```csharp
  public ObstacleService(ApplicationDbContext db)
  ```

**Løsning:**
- Enten: Fjern logger-parameteren fra testen
- Eller: Legg til logger i `ObstacleService` hvis det er ønskelig

### 3. **Moderat: Manglende Feilhåndtering**

**Problem:**
- `ObstacleService.CreateAsync()` kaster ikke eksplisitte exceptions ved valideringsfeil
- Ingen logging av viktige operasjoner
- `UpdateAsync()` returnerer kun `bool`, ingen detaljert feilmelding

**Anbefaling:**
```csharp
// Legg til logging
private readonly ILogger<ObstacleService> _logger;

// Legg til bedre feilhåndtering
public async Task<Obstacle> CreateAsync(ObstacleData vm, string userId)
{
    if (string.IsNullOrWhiteSpace(vm.GeometryGeoJson))
    {
        throw new ArgumentException("GeometryGeoJson cannot be empty", nameof(vm));
    }
    // ...
}
```

### 4. **Moderat: Validering i Service vs Controller**

**Problem:**
- Validering av `GeometryGeoJson` skjer i controller, ikke i service
- Dette bryter med Single Responsibility Principle

**Anbefaling:**
- Flytt validering til service-laget
- La controller kun håndtere HTTP-spesifikk logikk

### 5. **Lav: Manglende Input-validering**

**Problem:**
- `ObstacleData` har `[MaxLength(100)]` på `ObstacleName`, men service bruker default "Obstacle" hvis tom
- Ingen validering av GeoJSON-format
- Ingen validering av at `userId` ikke er null

**Anbefaling:**
```csharp
// Valider GeoJSON-format
if (!IsValidGeoJson(vm.GeometryGeoJson))
{
    throw new ArgumentException("Invalid GeoJSON format", nameof(vm));
}
```

### 6. **Lav: Manglende Null-sjekker**

**Problem:**
- `ObstacleService.CreateAsync()` tar `string userId` men sjekker ikke om den er null
- `User.FindFirstValue(ClaimTypes.NameIdentifier)` kan returnere null

**Anbefaling:**
```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (string.IsNullOrEmpty(userId))
{
    throw new UnauthorizedAccessException("User ID not found");
}
```

### 7. **Lav: Hardkodet Konverteringsfaktor**

**Problem:**
- Høydekonvertering mellom meter og fot bruker hardkodet `3.28084`
- Dette bør være en konstant eller konfigurasjonsverdi

**Anbefaling:**
```csharp
private const double MetersToFeet = 3.28084;
```

### 8. **Lav: Manglende Async/Await Pattern**

**Problem:**
- Noen steder brukes `.Result` eller `.Wait()` i stedet for async/await
- (Ikke observert i koden, men verdt å sjekke)

### 9. **Lav: README-mangler**

**Problem:**
- README nevner sikkerhetstiltak, men seksjonen er ufullstendig (linje 121: "...")
- Mangler beskrivelse av hvordan XSS-beskyttelse fungerer
- Mangler beskrivelse av hvordan SQL Injection-beskyttelse fungerer

**Anbefaling:**
Fullfør README med detaljer om sikkerhetstiltak.

### 10. **Lav: Test-prosjekt Organisering**

**Problem:**
- Det ser ut til å være to test-prosjekter:
  - `OBLIG1/OBLIG1-Prosjekt/OBLIG1.Tests/`
  - `OBLIG1/OBLIG1.Tests/`
- Dette kan forvirre og skape duplikasjon

**Anbefaling:**
- Konsolider til ett test-prosjekt
- Sjekk at alle tester er i riktig prosjekt

---

## 📊 Testdekning

### Eksisterende Tester

**Enhetstester:**
- ✅ `DefaultObstacleNameTest` - Test av default navn
- ✅ `RejectEmptyGeometry` - Test av tom geometri
- ✅ `RejectNegativeHeight` - Test av negativ høyde
- ✅ `RejectTooLongObstacleNames` - Test av for lange navn
- ✅ `RejectValueAboveMax` - Test av verdier over maks
- ✅ `ShouldAcceptZero` - Test av aksept av null
- ✅ `ObstacleStatusShouldHaveRightValue` - Test av status-verdier
- ✅ `RolesShouldHaveCorrectValues` - Test av roller

**Integrasjonstester:**
- ✅ `ObstacleServiceIntegrationTests` - Omfattende tester av service-laget
  - Create, Read, Update, Delete operasjoner
  - Rollebasert tilgang
  - Autorisering

### Manglende Tester

**Anbefalte tillegg:**
- ❌ Tester for `AdminController` (CRUD-operasjoner på brukere)
- ❌ Tester for `AuthController` (login/logout)
- ❌ Tester for edge cases (null-verdier, ekstreme verdier)
- ❌ Tester for concurrent access
- ❌ Performance-tester for store datasett

---

## 🔒 Sikkerhetsvurdering

### Implementert ✅

1. **CSRF-beskyttelse**: Alle POST-endepunkter har `[ValidateAntiForgeryToken]`
2. **SQL Injection**: EF Core bruker parametriserte spørringer
3. **XSS-beskyttelse**: Razor Views escape automatisk
4. **Autentisering**: Identity Framework
5. **Autorisering**: Rollebasert med `[Authorize]`

### Mangler ⚠️

1. **Rate Limiting**: Ingen rate limiting på login-endepunkter
2. **Password Policy**: Ingen eksplisitt password policy i koden (kommentar i `Program.cs` linje 40)
3. **Account Lockout**: `lockoutOnFailure: false` i `AuthController` linje 57
4. **HTTPS Enforcement**: Kun i production (linje 80 i `Program.cs`)
5. **Security Headers**: Ingen eksplisitte security headers (CSP, X-Frame-Options, etc.)

**Anbefalinger:**
```csharp
// Legg til i Program.cs
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
```

---

## 📈 Ytelse og Skalerbarhet

### Positivt ✅

- Bruk av async/await overalt
- EF Core med retry-logikk
- InMemory caching potensielt mulig

### Forbedringsområder ⚠️

1. **N+1 Problem**: `GetOverviewAsync()` kan ha N+1 queries hvis `CreatedByUser` lastes
2. **Manglende Paginering**: `GetOverviewAsync()` returnerer alle hindere uten paginering
3. **Manglende Caching**: Ingen caching av ofte brukte data

**Anbefaling:**
```csharp
// Legg til paginering
public async Task<PagedResult<Obstacle>> GetOverviewAsync(
    ClaimsPrincipal user, 
    int page = 1, 
    int pageSize = 20)
{
    // ...
}
```

---

## 🎯 Anbefalinger for Forbedring

### Høy Prioritet 🔴

1. **Fiks DeleteAsync-mismatch**: Legg til `DeleteAsync()` i interface og implementasjon
2. **Fiks test constructor**: Fjern logger-parameter eller legg til i service
3. **Fullfør README**: Legg til manglende sikkerhetsdetaljer

### Medium Prioritet 🟡

4. **Forbedre feilhåndtering**: Legg til logging og bedre exception-håndtering
5. **Flytt validering**: Flytt validering fra controller til service
6. **Legg til password policy**: Implementer strengere password-regler
7. **Legg til account lockout**: Aktiver lockout ved feilede login-forsøk

### Lav Prioritet 🟢

8. **Legg til paginering**: For bedre ytelse med mange hindere
9. **Legg til logging**: Strukturert logging av viktige operasjoner
10. **Forbedre testdekning**: Legg til tester for controllers

---

## 📝 Konklusjon

### Samlet Vurdering: **God (B+)**

**Sterke sider:**
- ✅ Solid arkitektur og kodekvalitet
- ✅ God sikkerhetsimplementering (CSRF, SQL Injection, XSS)
- ✅ Omfattende testing
- ✅ Moderne teknologi-stack (.NET 9, Docker)
- ✅ God dokumentasjon i koden

**Svakheter:**
- ⚠️ Kritiske mismatches mellom tester og implementasjon
- ⚠️ Noen mangler i feilhåndtering
- ⚠️ Ufullstendig README

**Anbefaling:**
Prosjektet er godt strukturert og viser god forståelse av MVC-arkitektur og sikkerhet. De kritiske problemene (DeleteAsync-mismatch og test constructor) bør fikses umiddelbart. Med disse fiksingene vil prosjektet være produksjonsklart for et utviklingsmiljø.

**Estimert tid for fiksing av kritiske problemer:** 1-2 timer

---

## 📋 Sjekkliste for Fiksing

- [ ] Legg til `DeleteAsync()` i `IObstacleService`
- [ ] Implementer `DeleteAsync()` i `ObstacleService`
- [ ] Fiks test constructor (fjern logger eller legg til i service)
- [ ] Kjør alle tester og verifiser at de passerer
- [ ] Fullfør README med sikkerhetsdetaljer
- [ ] Legg til null-sjekker i `CreateAsync()` og andre metoder
- [ ] Vurder å legge til logging
- [ ] Vurder å legge til password policy

---

**Vurdert av:** AI Assistant  
**Versjon:** 1.0

