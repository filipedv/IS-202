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
   Når applikasjonen kjøres lokalt med HTTPS via Docker, bruker den et utviklings-/selvsignert sertifikat.
   Derfor kan Chrome vise “Not secure” første gang.
   For å få grønn hengelås må du stole på sertifikatet (trust) på maskinen din.
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

## 6. Testing
Testing er gjennomført for å sikre korrekt funksjonalitet, dataintegritet og brukervennlighet.

### Testkategorier
- Enhetstester: Verifiserer individuell komponenlogikk
- Systemtester: Kontrollere integrasjon mellom frontend, controller og database
- Sikkerhetstester: Bekrefter korrekt autentisering og autorisering
- Brukervennlighetstester: Evaluerer mobilgrensesnitt og skjemaer

### Enhetstesting 
Testene er implementert med xUnit. 

### Kjøring av enhetstester
dotnet test
(Alle tester er tilgjengelig i OBLIG1.Tests - prosjektet)

### Systemtesting 

Hensikten med systemtesting var å verifisere at de viktigste funksjonelle kravene i applikasjonen fungerer riktig når systemet er i drift.

Følgende hovedområder ble testet:

- Registrering og innlogging av bruker  
- Opprettelse, visning, endring og sletting av objekter (Obstacle)  
- Tilgangskontroll til sider som krever innlogging  

### Metode

Systemtesten ble utført manuelt basert på forhåndsdefinerte testcaser. For hvert testcase ble følgende dokumentert:

- **ID** – unik identifikator  
- **Beskrivelse** – hva som testes  
- **Forutsetninger** – krav som må være oppfylt før teststart  
- **Steg** – konkrete handlinger i applikasjonen  
- **Forventet resultat** – forventet systemoppførsel  
- **Faktisk resultat / Status** – resultat etter gjennomføring (OK eller FEIL)

### Testcaser

### Autentisering og autorisasjon

### Systemtest 1 – Logge inn med gyldig bruker
| Felt | Verdi |
|------|-------|
| **Beskrivelse** | Logge inn med gyldig bruker |
| **Forutsetninger** | Bruker finnes i databasen |
| **Steg** | 1. Gå til innloggingsside<br>2. Fyll inn korrekt e-post og passord<br>3. Trykk "Login" |
| **Forventet resultat** | Bruker logger inn og videresendes til startsiden |
| **Faktisk resultat / Status** | OK |


### Systemtest 2 – Avvise innlogging med feil passord
| Felt | Verdi |
|------|-------|
| **Beskrivelse** | Avvise innlogging ved feil passord |
| **Forutsetninger** | Bruker finnes i databasen |
| **Steg** | 1. Gå til innloggingsside<br>2. Fyll inn riktig e-post, men feil passord<br>3. Trykk "Login" |
| **Forventet resultat** | Innlogging avvises med feilmelding “Invalid login attempt” |
| **Faktisk resultat / Status** | OK |


### Opprettelse og visning av hindre

### Systemtest 3 – Opprette nytt hinder
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Innlogget som pilot |
| **Steg** | 1. Trykk “Register Obstacle”<br>2. Velg “Point”<br>3. Marker et punkt på kartet<br>4. Trykk “Submit” |
| **Forventet resultat** | Nytt hinder lagres med korrekt geometri og vises i kartet |
| **Faktisk resultat / Status** | OK |

### Systemtest 4 – Vise liste med alle hindre
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Minst ett hinder finnes |
| **Steg** | 1. Trykk “Overview” |
| **Forventet resultat** | Alle hindre vises med korrekt informasjon |
| **Faktisk resultat / Status** | OK |


### Systemtest 5 – Avvise opprettelse uten geometri
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Innlogget som pilot |
| **Steg** | 1. Trykk “Register Obstacle”<br>2. Ikke marker geometri<br>3. Trykk “Submit” |
| **Forventet resultat** | Hinder lagres ikke. Feilmelding vises |
| **Faktisk resultat / Status** | OK |


### Redigering og sletting av hindre

### Systemtest 6 – Redigere eksisterende hinder
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Minst ett hinder finnes og bruker har tilgang |
| **Steg** | 1. Gå til “Overview”<br>2. Velg et hinder<br>3. Trykk “Edit”<br>4. Endre informasjon<br>5. Trykk “Save” |
| **Forventet resultat** | Endringer lagres og vises i oversikten |
| **Faktisk resultat / Status** | OK |


### Systemtest 7 – Avbryte redigering
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Minst ett hinder finnes |
| **Steg** | 1. Gå til “Overview”<br>2. Gjør en endring<br>3. Trykk “Cancel” |
| **Forventet resultat** | Ingen endringer lagres |
| **Faktisk resultat / Status** | OK |


### Systemtest 8 – Slette hinder
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Minst ett hinder finnes |
| **Steg** | 1. Gå til “Overview”<br>2. Velg et hinder<br>3. Trykk “Delete” |
| **Forventet resultat** | Hinder slettes fra databasen og fjernes fra kartet |
| **Faktisk resultat / Status** | FEIL – Delete-knappen fungerer ikke (hinder slettes ikke fra databasen) |


### Registerfører–funksjonalitet og godkjenningsprosess

### Systemtest 9 – Registerfører ser alle innsendte hinder
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Innlogget som registerfører. Minst to hinder fra ulike piloter |
| **Steg** | 1. Logg inn<br>2. Gå til “Overview” |
| **Forventet resultat** | Alle innsendte hinder vises med status, høyde og tidsstempel |
| **Faktisk resultat / Status** | OK |


### Systemtest 10 – Registerfører godkjenner hinder
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Innlogget som registerfører. Minst ett hinder registrert |
| **Steg** | 1. Logg inn<br>2. Trykk “Edit” på et hinder<br>3. Sett status til “Approved” |
| **Forventet resultat** | Hinder får status “godkjent” og vises slik for piloten |
| **Faktisk resultat / Status** | OK |


### 5. Tilgangskontroll og admin-funksjoner

### Systemtest 11 – Admin har tilgang til adminsider
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Innlogget som admin |
| **Steg** | 1. Logg inn som admin<br>2. Naviger til adminsider |
| **Forventet resultat** | Siden lastes uten feilmelding, og admin-funksjoner er tilgjengelige |
| **Faktisk resultat / Status** | OK |


### Systemtest 12 – Kreve innlogging for beskyttede sider
| Felt | Verdi |
|------|-------|
| **Forutsetninger** | Ikke innlogget |
| **Steg** | 1. Åpne ny fane<br>2. Lim inn: `http://localhost:8080/Admin`<br>3. Trykk Enter |
| **Forventet resultat** | Ingen admin-data vises. Brukeren får ikke tilgang. |
| **Faktisk resultat / Status** | OK |


### Oppsummering av systemtesting

Totalt ble 12 systemtestcaser definert og kjørt.

- 11 tester ble godkjent ved første gjennomføring (OK)  
- 1 test feilet: sletting av hinder (Systemtest 8)

Feilen ble rettet og testen ble kjørt på nytt med resultat OK.

Systemtesten viser at applikasjonens sentrale brukerflyt fungerer som forventet, inkludert:

- innlogging  
- registrering av hindringer  
- opprettelse, redigering og sletting  
- korrekt visning i kartet  
- tilgangskontroll  
- registerfører-godkjenning  

Systemet oppfyller dermed de viktigste funksjonelle kravene.

---

### Brukertesting 

Brukertesting ble gjennomført for å evaluere brukeropplevelsen og brukervennligheten i applikasjonen.  
Fokusområdene var:

- Innlogging og tilgangsstyring  
- Navigasjon i kart  
- Registrering og visning av hinder  
- Arbeidsflyt for pilot, registerfører og administrator  

Målet var å avdekke usikkerheter i grensesnittet og identifisere områder med forbedringspotensial.

**Dato for gjennomføring: 21.11.25**


### Testdeltakere

| Testperson | Beskrivelse | Erfaring | Kommentar |
|-----------|-------------|----------|-----------|
| T1 | IT-student, 25 år | UX-erfaring | Har jobbet med lignende prosjekter tidligere |
| T2 | IT-student, 22 år | UX-erfaring | Har jobbet med lignende prosjekter tidligere |
| T3 | Student, 21 år | Ingen IT/UX erfaring | Ingen erfaring med systemer av denne typen |

Testgruppen bestod av to brukere med IT/UX-erfaring og én bruker uten teknisk bakgrunn.  
Kommentarer om forbedringer i grensesnittet kom hovedsakelig fra de erfarne brukerne, mens den uerfarne brukeren ga viktige innspill om hvor intuitivt systemet oppleves for nye brukere.


### Oppgaver

Testoppgavene ble fordelt etter roller i systemet.


### Oppgaver for pilot

| Oppgave | Beskrivelse | Suksesskriterier |
|---------|-------------|------------------|
| **O1** | Logge inn med e-post og brukernavn for pilot | Brukeren skal finne innloggingssiden, skrive inn info og forstå eventuelle feilmeldinger |
| **O2** | 1. Trykk "Register Obstacle"<br>2. Velg *line* eller *point* og marker et sted i kartet<br>3. Gå til "Overview"<br>4. Rediger hinderet (legg inn mast som type)<br>5. Legg til høyde<br>6. Send inn til registerfører | Brukeren skal kunne fylle ut skjema, se hinderet i kartet og igjen i oversikten, samt redigere det |


### Oppgaver for registerfører

| Oppgave | Beskrivelse | Suksesskriterier |
|---------|-------------|------------------|
| **O3** | Logge inn med e-post og brukernavn for registerfører | Brukeren skal finne innloggingssiden, skrive inn info og forstå eventuelle feilmeldinger |
| **O4** | 1. Gå til "Overview" og rediger et hinder<br>2. Endre type, høyde og beskrivelse<br>3. Sett status til "Approved" og lagre | Brukeren skal kunne navigere oversikten og oppdatere hinderets informasjon og status |

### Oppgaver for administrator

| Oppgave | Beskrivelse | Suksesskriterier |
|---------|-------------|------------------|
| **O5** | Logge inn med e-post og brukernavn for admin | Brukeren skal finne innloggingssiden, skrive inn info og forstå eventuelle feilmeldinger |
| **O6** | 1. Trykke "Add user"<br>2. Fylle inn e-post, rolle og passord<br>3. Lagre informasjonen<br>4. Finne brukeren i oversikten<br>5. Slette brukeren<br>6. Filtrere brukere etter rolle | Brukeren skal kunne opprette, slette og filtrere brukere uten hjelp |

### Spørsmål til testpersonene (etter test)

1. Hva er din opplevelse etter å ha brukt systemet?  
2. Var det lett å bruke?  
3. Var det unødvendig komplisert?  
4. Hang de forskjellige delene av systemet godt sammen?  
5. Er dette et system du kunne tenkt deg å bruke flere ganger?  
6. Tror du andre brukere lett kunne brukt dette systemet?  


### Resultater – Oppsummering

Brukerne klarte de fleste oppgavene, men noen utfordringer ble avdekket i grensesnittet:

- Opprettelse av hinder var intuitivt og logisk  
- Knapper og UI-elementer ble oppfattet som tydelige og lette å forstå  
- Systemet opplevdes som lett å bruke  
- Enkelte funksjoner kunne vært bedre forklart  


### Detaljerte resultater

### Oppgave O1, O3 og O6 – Innlogging og administrasjon

| Punkt | Resultat |
|-------|----------|
| **Forventet resultat** | Bruker logger inn uten problemer |
| **Faktisk resultat** | 3/3 brukere logget inn uten hjelp |
| **Problemer oppdaget** | Ingen kritiske problemer, men én bruker etterspurte felles innloggingsportal |
| **Forslag til forbedring** | Samle alle innlogginger i én felles portal |
| **Resultatvurdering** | Vellykket, men med forbedringspotensial |


### Oppgave O2 – Registrere og redigere hinder

| Punkt | Resultat |
|-------|----------|
| **Forventet resultat** | Brukeren oppretter og redigerer hinder |
| **Faktisk resultat** | 3/3 brukere klarte oppgaven, men 2 brukere var usikre på "line" og "point" |
| **Problemer oppdaget** | Utydelige knapper |
| **Forslag til forbedring** | Legge til forklarende tekst på knappene |
| **Resultatvurdering** | Vellykket, men med forbedringspotensial |


### Oppgave O4 – Redigere hinder i oversikten

| Punkt | Resultat |
|-------|----------|
| **Forventet resultat** | Brukeren finner hinder i oversikt og redigerer |
| **Faktisk resultat** | 3/3 brukere klarte oppgaven |
| **Problemer oppdaget** | En bruker forventet at registerfører også kunne registrere hinder |
| **Forslag til forbedring** | Vurdere om registerfører skal kunne registrere hindre |
| **Resultatvurdering** | Effektivt løst av alle brukere |


### Oppgave O6 – Administrere brukere

| Punkt | Resultat |
|-------|----------|
| **Forventet resultat** | Brukeren finner knappene og klarer å slette brukere |
| **Faktisk resultat** | 3/3 brukere klarte oppgaven |
| **Problemer oppdaget** | Ingen kritiske problemer |
| **Forslag til forbedring** | Ingen |
| **Resultatvurdering** | Oppgaven ble løst effektivt |


### Identifiserte problemer

Ingen kritiske problemer ble avdekket, men følgende forbedringspunkter bør vurderes:

- Behov for en felles innloggingsportal  
- Uklare ikonknapper for registrering av hinder  
- Registerfører ønsker mulighet til å registrere hindre  


### Konklusjon

Brukertestingen viser at systemet i hovedsak fungerer godt og at brukerne klarer å utføre kjerneoppgavene uten vesentlig hjelp.  
Systemet oppleves som intuitivt og enkelt å navigere.

Flere forbedringer ble likevel identifisert, og disse kan bidra til å gjøre arbeidsflyten mer effektiv og forbedre brukeropplevelsen ytterligere.

---

### Sikkerhet

Applikasjonen er bygget med fokus på grunnleggende web-sikkerhet i ASP.NET Core. Nedenfor beskrives de viktigste tiltakene, med spesielt fokus på håndtering av GeoJSON og XSS.

### Autentisering og autorisasjon

Applikasjonen bruker ASP.NET Core Identity til innlogging og håndtering av brukere.

Tilgang styres med rollebasert autorisasjon:

- Admin – administrerer brukere og roller.
- Registrar – kan se og redigere alle hindere.
- Pilot – kan kun se og redigere egne hindere.

Controllerne er sikret med `[Authorize]`-attributter, f.eks.:

- HomeController og ObstacleController krever innlogging og rolle (Pilot eller Registrar).
- AdminController er kun tilgjengelig for brukere med rollen Admin.

Dette hindrer at ikke-autoriserte brukere får tilgang til sensitive funksjoner og data.

### CSRF-beskyttelse

Alle POST-forespørsler som endrer data er beskyttet mot Cross-Site Request Forgery (CSRF):

- Actions som endrer tilstand er merket med `[ValidateAntiForgeryToken]`.
- De tilhørende skjemaene i Razor-viewene har `@Html.AntiForgeryToken()`.

Dette gjør at en angriper ikke kan få en innlogget bruker til å utføre uønskede endringer ved å sende skjulte POST-forespørsler fra en annen side.

### Beskyttelse mot XSS (Cross-Site Scripting)

ASP.NET Core Razor encoder som standard all tekst som skrives ut med `@Model.X`, slik at HTML og JavaScript ikke kjøres, men vises som ren tekst. I tillegg er det gjort spesifikke tiltak rundt GeoJSON-håndtering:

### Problem: tidligere XSS-risiko i GeoJSON

Hver hindring har en geometri lagret som GeoJSON (`GeometryGeoJson`). Tidligere ble denne strengen skrevet rett inn i JavaScript-kode i `Edit.cshtml` med `@Html.Raw`, f.eks.:

```csharp
@{
    var geo = string.IsNullOrWhiteSpace(Model.GeometryGeoJson)
        ? "null"
        : Model.GeometryGeoJson;
}
const existing = @Html.Raw(geo);
```
Hvis en angriper klarte å lagre ondsinnet innhold i GeometryGeoJson, kunne dette injiseres direkte som kjørbar JavaScript i nettleseren (stored XSS).

### Løsning: trygg serialisering og parsing
For å hindre dette blir GeoJSON nå behandlet som data og ikke som ferdig JavaScript:
På serversiden serialiseres GeometryGeoJson til en trygg JavaScript-string med JsonSerializer.Serialize.
I JavaScript parses denne strengen med JSON.parse.
Eksempel fra Edit.cshtml:
```
@using System.Text.Json

@{
    var geoText = string.IsNullOrWhiteSpace(Model.GeometryGeoJson)
        ? null
        : Model.GeometryGeoJson;

    // Gjør om til en trygg JS-string literal
    var geoJsonLiteral = JsonSerializer.Serialize(geoText);
}
const geoText = @Html.Raw(geoJsonLiteral); // alltid en streng eller null
let existing = null;

if (geoText) {
  try {
    existing = JSON.parse(geoText);
  } catch (e) {
    console.error("Ugyldig GeoJSON lagret på hindret", e);
  }
}

if (existing) {
  const layer = L.geoJSON(existing, {
    onEachFeature: (_, lyr) => drawnItems.addLayer(lyr)
  }).addTo(map);
  // ...
}
```
På denne måten kan ikke GeometryGeoJson “bryte seg ut” av stringen og bli kjørbar kode, selv om noen forsøker å legge inn ondsinnet innhold.
Validering av GeoJSON på serversiden
 ### Validering av GeoJSON på serversiden
 For å unngå at ugyldig eller manipulert GeoJSON lagres i databasen, valideres GeometryGeoJson i ObstacleService hver gang et hinder opprettes eller oppdateres:
Det sjekkes at strengen ikke er for lang (en enkel MaxGeoJsonLength-grense).
Det forsøkes å parse strengen med JsonDocument.Parse.
Hvis parsing feiler, kastes en ArgumentException og operasjonen avbrytes.
Forenklet eksempel:
```
private const int MaxGeoJsonLength = 100_000;

private static void ValidateGeoJsonOrThrow(string? geoJson)
{
    if (string.IsNullOrWhiteSpace(geoJson))
        return; // Tomt er lov hvis det er ønsket

    if (geoJson.Length > MaxGeoJsonLength)
        throw new ArgumentException("Geometry JSON is too long.", nameof(geoJson));

    try
    {
        using var doc = JsonDocument.Parse(geoJson);
    }
    catch (JsonException ex)
    {
        throw new ArgumentException("Geometry is not valid JSON.", nameof(geoJson), ex);
    }
}
```
Metoden brukes både ved opprettelse og oppdatering av hindere. Dette sikrer at kun syntaktisk gyldig JSON lagres, og reduserer risikoen for både feil og bevisste angrep.

### Tilgangskontroll på domenenivå
I tillegg til [Authorize]-attributter i kontrollerne, er det lagt ekstra autorisasjon i domenelogikken (ObstacleService):
Piloter kan kun hente og endre hindere de selv har opprettet.
Registrar-brukere kan se og redigere alle hindere.
Hvis en bruker prøver å redigere et hinder de ikke har tilgang til, kastes UnauthorizedAccessException, og API-et svarer med Forbid().
Dette gjør at selv om noen skulle prøve å manipulere URL-er eller formdata, blir tilgangen fortsatt sjekket på serversiden.

### Konklusjon
Rollebasert autorisasjon beskytter sensitive områder som administrasjon og hinderdatasett.
CSRF-token brukes på alle endepunkter som endrer data.
XSS-risiko via GeoJSON er mitigert ved:
trygg serialisering i view (JsonSerializer.Serialize + JSON.parse),
og validering av GeoJSON på serversiden før lagring.
Domenelogikk i ObstacleService sørger for at brukere kun kan se og endre data de har lov til.

---

### Oppsummering av testing 
Det er gjennomført flere typer tester for å sikre kvaliteten i applikasjonen. Brukertestig bekreftet at grensesnittet og arbeidsflyten fungerer som forventet, samtidig som det ga verdifulle tilbakemeldinger. Systemtesting verifiserte at funksjonaliteten, fra innlogging til håndtering av hindere, opptrer som forventet. Enhetstesting sikret at kritiske deler av domenelogikken, som validering og autorisasjon, er stabilt. I tillegg ble det utført sikkerhetstesting med fokus på XSS, CSRF, GeoJSON-håndtering og tilgangskontroll. Samlet sett viser testene at systemet er funksjonelt, stabilt og godt beskyttet mot sentrale sikkerhetstrusler. 


  
