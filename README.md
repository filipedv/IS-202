# Dokumentasjon - OBLIG 1

## 1. Drift og implementering

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
   eller direkte med .NET<br>
   *dotnet run*
6. Åpne nettleser og naviger til *http://localhost:5134*

### Driftsinformasjon
- Alle registrerte hindringer lagres midlertidig i applikasjonens minne via List&lt;ObstacleData&gt;.
- Hvis serveren startes på nytt, vil alle hindringer som ikke er lagret eksternt, forsvinne.
- Internett tilgang kreves for at kartet (Leaflet og Leaflet-Draw) skal fungere.

## 2. Systemarkitektur

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

### Informasjonsflyt
1. Bruker navigerer til Home og ser velkomstmelding og interaktivt kart.
2. Bruker velger Register Obstacle og åpner DataForm.
3. Bruker fyller ut skjema og tegner hinder på kartet (GeoJSON data lagres i et skjult felt, koordinater)
4. Skjemaet sendes via POST til controlleren som lagrer hindringen i minnet.
5. Bruker navigerer til Overview for å se en samlet oversikt over alle registrerte hindringer.

## 3. Testscenarier

### Link til demonstrasjonsvideo<br>
*https://youtu.be/ik5dVq2Qdds*
<br>

- **Registrering med fullstendig informasjon**<br>
  Fyll ut navn, høyde, beskrivelse og tegn på kart -> Submit<br>
  *Forventet resultat*: Hindringen lagres og vises i Overview<br>
  *Faktisk resultat*: Bestått
- **Registrering med ugyldig høyde**<br>
  Fyll inn negativ verdi -> Submit<br>
  *Forventet resultat*: Feilmelding vises<br>
  *Faktisk resultat*: Bestått
- **Tegning av flere hindringer på kart**<br>
  Tegn markør, linje og rektangel<br>
  *Forventet resultat*: GeoJSON-koordinater lagres korrekt<br>
  *Faktisk resultat*: Bestått
- **Visning av oversikt**<br>
  Naviger til Overview<br>
  *Forventet resultat*: Alle registrerte hindringer vises korrekt<br>
  *Faktisk resultat*: Bestått
- **Dynamisk kartjustering**<br>
  Endre størrelsen på nettleservindu<br>
  *Forventet resultat*: Kartet justeres korrekt og beholdes synlig<br>
  *Faktisk resultat*: Bestått

## 4. Testresultater
- Validering av felter fungerer som forventet.
- GeoJSON data for hindringer lagres korrekt ved bruk av karttegningsfunksjon.
- Innsending av skjemaet sender data til serveren på riktig måte.
- Oversiktsiden viser alle registrerte hindringer korrekt.
- Kartet oppdateres dynamisk ved endring av skjermstørrelse uten problemer.

## 5. Fremtidige forbedringer
- Lagre hindringer permanent i en database i stedet for kun i midlertidig minne.
- Mulighet til å redigere eksisterende hindringer.
- Legge til søk- og filtreringsfunksjoner på oversiktssiden.
- Autentisering og rollebasert tilgang (login side for administrator/pilot).


  
