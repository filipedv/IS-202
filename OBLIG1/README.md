# OBLIG1 Prosjekt

## Oppsett

### 1. Generer SSL-sertifikat (første gang)

Før du starter Docker Compose første gang, må du generere et selvsignert SSL-sertifikat:

```bash
./generate-cert.sh
```

Dette oppretter `certs/devcert.pfx` som brukes av ASP.NET Core for HTTPS.

### 2. Start applikasjonen

```bash
docker compose up --build
```

Applikasjonen vil være tilgjengelig på:
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8443

**Merk:** Siden sertifikatet er selvsignert, vil nettleseren din vise en sikkerhetsadvarsel. Dette er normalt for utviklingssertifikater.

## Feilsøking

### "Could not find file '/https/devcert.pfx'"

Dette betyr at sertifikatet ikke er generert. Kjør:

```bash
./generate-cert.sh
```

