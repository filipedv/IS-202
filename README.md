# IS-202 - OBLIG 2

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
Det er en applikasjon laget for å registrere og administrere hindringer som kan påvirke piloter. Oblig 2 bygger mer på Oblig 1 og inkluderer:

- Kobling til **MariaDB**, med både applikasjon og database kjørende i Docker.
- **Identitetshåndtering**, som tillater registrering av nye brukere og lagring i databasen.
- **Autentisering og autoriserng**, slik at brukere kan logge inn og administrere hindringer.
- Funksjonalitet for både **pilot og registerfører**, som definert av Kartverket.
- Kartskjema for å legge til **punkt og linje** på kartet.
- Mobiltilpasset frontend for piloter.
- Enkel oversikt over hindringer.
- Implementerte sikkerhetstiltak.
- Omfattende testing (enhet, system, sikkerhet og brukervennlighet).

Applikasjonen følger MVC-arkitektur og håndterer både GET og POST forespørsler, med dynamisk innhold hentet fra serveren. 

### Systemkrav
- .NET 9 SDK eller nyere
- Visual Studio 2022 eller Rider
- Nettleser med Javascript aktivert
- Internett tilgang for Leaflet og OpenStreetMap

### Installasjon og oppstart
1. Klone repositoriet fra GitHub i terminalen:<br>
   *git clone https://github.com/filipedv/IS-202.git*
2. Åpne prosjektet i valgt utviklingsmiljø
3. Bygg applikasjonen i terminalen:<br>
   *dotnet build*
4. Kjør applikasjonen lokalt:<br>
   Med Docker<br>
   *docker compose up --build*<br>
   eller direkte med .NET (uten database)<br>
   *dotnet run*
6. Åpne nettleser og naviger til *http://localhost:5134*

## 2. Brukere og roller
| E-post | Passord | Rolle |
|---|---|---|
| pilot1@example.com | Pilot1! | Pilot |
| registerforer@example.com | Register1! | Registerfører |
| admin@example.com | Admin1! | Administrator |

Nye brukere kan registreres via administratorbruker.

## 3. Funksjonalitet
- Brukerregistrering og autentisering
- Rollbaser tilgang (Pilot / Registerfører / Administrator)
- Innmelding av hindringer på kart (punkt og linje)
- GeoJSON-lagring av hindringer
- Oversikt over registrerte hindringer
- Mobilvennlig brukergrensesnitt
- Midlertidig og permanent lagring (via MariaDB)
- Implementerte sikkehetstiltak

## 4. Systemarkitektur

### Arkitektur
- Modell : ObstacleData
  - Representerer et hinder og inneholder:<br>
    ObstacleName, ObstacleHeight, ObstacleDescription, IsDraft, GeometryJSON
- Controller : ObstacleController
  - DataForm (GET): Viser registreringsskjema
  - DataForm (POST): Behandler innsending av skjemaet og lagrer hindringer i minnet
  - Overview (GET): Presenterer oversikt over alle registrerte hindringer
- Views:
  - Home: Velkomstside med interaktivt kart
  - DataForm: Skjema for registrering av hindringer med integrert karttegningsfunksjonalitet
  - Overview: Viser samtlige registrerte hindringer i listeformat
 
### Frontend
- HTML og Razor Pages for struktur og visning.
- Leaflet og Leaflet-Draw for interaktivt kartvisning og tegning.
- TailwindCSS/Bootstrap for visuell presentasjon og responsivt design.

## 5. Testing
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

  
