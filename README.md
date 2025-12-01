# IS-202 - Obstacle Reporting System

**Gruppe 11**
**Repository:** [https://github.com/filipedv/IS-202.git]

| Navn | E-post | GitHub-brukernavn |
|---|---|---|
| Silje Solberg | siljeso@uia.no | siljesolberg |
| Jihyung Choi | jihyungc@uia.no | jihyung-choi |
| Ida Elise Jamissen | iejamissen@uia.no | idaelisej |
| Sivert Dørum | sivertd@uia.no | sivertdorum |
| Filip Edvardsen | filiped@uia.no | filipedv |
| Øyvind Birkeland | oyvindbi@uia.no | OyvindBirkeland |
---

## 1. Introduksjon
Denne applikasjonen, Obstacle Reporting System lar piloter rapportere hindere, registerfører varlidere dem, og administratorer administrere brukere. Hindere vises i et kartbasert grensesnitt og lagres i MariaDB. Systemet kjører i Docker.

**Hovedfunksjonalitet:**

- Kobling til **MariaDB**, med både applikasjon og database kjørende i Docker.
- **Identitetshåndtering**, som tillater registrering av nye brukere og lagring i databasen.
- **Autentisering og autoriserng** via Identity Framwork, slik at brukere kan logge inn og administrere hindringer.
- Innmelding og validering av hindere (punkt og linje på kartet).
- Oversikt over registrerte hindere.
- Mobiltilpasset brukergrensesnitt (frontend) for piloter.
- Implementerte sikkerhetstiltak (SQL Injection, CSRF, XSS).
- Omfattende testing (enhet, system, sikkerhet og brukervennlighet).

Applikasjonen følger MVC-arkitektur og håndterer både GET og POST forespørsler, med dynamisk innhold hentet fra serveren. 

## 2. Systemkrav
- .NET 9 SDK eller nyere
- Visual Studio 2022 eller JetBrains Rider
- Nettleser med Javascript aktivert
- Internett tilgang for Leaflet og OpenStreetMap

## 3. Installasjon og oppstart

1. Klone repositoriet fra GitHub i terminalen:<br>
```bash
   git clone https://github.com/filipedv/IS-202.git
```
2. Bytt til prosjektmappen:<br>
```bash
   cd IS-202/OBLIG1
```
3. Åpne prosjektet i valgt utviklingsmiljø
4. Bygg applikasjonen i terminalen:<br>
   ```bash
   dotnet build
   ```
5. Kjør applikasjonen lokalt:<br>
   Med Docker:<br>
   ```bash
   docker compose up --build
   ```
   eller direkte med .NET (uten database)<br>
   ```bash
   dotnet run
   ```
6. Åpne nettleser og naviger til:<br>
   ```bash
   http://localhost:5134
   ```

## 4. Brukere og roller
| E-post | Passord | Rolle |
|---|---|---|
| pilot1@example.com | Pilot1! | Pilot |
| registerforer@example.com | Register1! | Registerfører |
| admin@example.com | Admin1! | Administrator |

**Forklaring på roller:**
- Pilot: Sender inn hindere, lagrer rapporter, ser kun egne registrerte hinder rapporter.
- Registerfører: Validerer hinder rapporter fra pilot.
- Admin: Administrerer brukere og roller, full tilgang til systemet.<br>

Nye brukere kan registreres via administratorbruker.

## 5. Systemarkitektur
Applikasjonen følger **MVC-arkitektur** (Model-View-Controller) og kjører i Docker for å sikre konsistens mellom utviklings- og produksjonsmiljø. Den benytter **MariaDB** som database, og .NET 9 med Identity Framework for autentisering og autorisering.

### Arkitekturkomponenter
- **Models** : Representerer dataobjekter og forretningslogikk.
  ex. `ObstacleData`
  - Representerer et hinder og lagrer:<br>
    ObstacleName (string), ObstacleHeight (double), ObstacleDescription (string),
    IsDraft (bool), GeometryJSON (GeoJSON-format for kart)
  
- **Controllers** : Håndterer HTTP-forespørsler (GET/POST), validerer input og sender data til views eller til database via Services/EF Core.
  ex. `ObstacleController`
  - DataForm (GET): Viser skjema for registrering av hinder
  - DataForm (POST): Behandler innsending av skjemaet og lagrer hindere i MariaDB
  - Overview (GET): Viser oversikt over alle registrerte hindere
    
- **Views**: Presentasjon av nettsider med HTML/Razor og JavaScript.
  - Home: Velkomstside med interaktivt kart
  - DataForm: Skjema for registrering av hinder med Leaflet/Leaflet-Draw for
    karttegningsfunksjonalitet
  - Overview: Viser samtlige registrerte hindere i listeformat
 
### Database
- **MariaDB** brukes som relasjonsdatabase for å lagre hindere, brukere og roller.
- Databasekjøring i Docker sikrer at utviklere alltid jobber mot samme miljø.
- EF Core brukes til ORM, og alle spørringer er parametriserte for å forhindre SQL-injection.
- GeoJSON-data lagres i tekstfelt for å visulisere hindere på kartet.

### Docker Oppsett
- Applikasjon og database kjører i **separate containere**:
  - Applikasjonscontaineren kjører .NET 9
  - MariaDB-containeren kjører databasen med predefinert bruker og passord
- `docker-compose.yml`håndterer oppstart, bygging og nettverkskobling mellom containere.
- Fordel: Rask oppstart, isolerte miljøer og enkel distribusjon.
 
### Frontend
- HTML og Razor Pages for struktur og visning.
- Leaflet og Leaflet-Draw for interaktivt kartvisning og tegning.
- TailwindCSS/Bootstrap for visuell presentasjon og responsivt design.

### Sikkerhetstiltak i arkitekturen
...

## 6. Testing
Testing er gjennomført for å sikre korrekt funksjonalitet, dataintegritet og brukervennlighet. Testene er implementert med xUnit.

### Eksempler på enhetstester
1. DefaultObstacleNameTest - verifiserer at hindringsnavn kan være tomt:
   var obstacle = new Obstacle { Name = "" };
   var nameIsEmpty = string.IsNullOrWhiteSpace(obstacle.Name);
   Assert.True(nameIsEmpty);

2. RejectEmptyGeometry - hindringer med tom geometri lagres ikke i databasen:
   var vm = new ObstacleData { GeometryGeoJson = "" };
   var result = await controller.DataForm(vm);
   Assert.IsType<ViewResult>(result);
   Assert.False(controller.Modelstate.IsValid);
   Assert.Equal(0, db.Obstacles.Count());

3. RejectNegativeHeight - hindringer med negativ høyde identifiseres som ugyldige:
   var obstacle = new Obstacle { Height = -5};
   bool isNegative = obstacle.Height < 0;
   Assert.True(isNegative);

### Testkategorier
- Enhetstester: Verifiserer individuell komponenlogikk
- Systemtester: Kontrollere integrasjon mellom frontend, controller og database
- Sikkerhetstester: Bekrefter korrekt autentisering og autorisering
- Brukervennlighetstester: Evaluerer mobilgrensesnitt og skjemaer

## Kjøring av tester
dotnet test

(Alle tester er tilgjengelig i OBLIG1.Tests - prosjektet)

  
