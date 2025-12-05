#!/bin/bash
# Script for å generere selvsignert SSL-sertifikat for utvikling
# Kjør dette scriptet før du starter docker compose første gang

set -e

CERT_DIR="./certs"
CERT_FILE="$CERT_DIR/devcert.pfx"
CERT_PASSWORD="Oblig1DevOnly!"

# Opprett certs mappen hvis den ikke eksisterer
mkdir -p "$CERT_DIR"

# Sjekk om sertifikatet allerede eksisterer
if [ -f "$CERT_FILE" ]; then
    echo "Sertifikatet eksisterer allerede: $CERT_FILE"
    echo "Slett det først hvis du vil generere et nytt."
    exit 0
fi

echo "Genererer selvsignert SSL-sertifikat..."
echo "Dette kan ta noen sekunder..."

# Generer private key og sertifikat
openssl req -x509 -newkey rsa:4096 \
    -keyout "$CERT_DIR/devcert.key" \
    -out "$CERT_DIR/devcert.crt" \
    -days 365 \
    -nodes \
    -subj "/CN=localhost"

# Konverter til PKCS12 format (.pfx) som ASP.NET Core trenger
openssl pkcs12 -export \
    -out "$CERT_FILE" \
    -inkey "$CERT_DIR/devcert.key" \
    -in "$CERT_DIR/devcert.crt" \
    -passout "pass:$CERT_PASSWORD"

echo ""
echo "✓ Sertifikat generert: $CERT_FILE"
echo "✓ Passord: $CERT_PASSWORD"
echo ""
echo "Du kan nå kjøre: docker compose up --build"

