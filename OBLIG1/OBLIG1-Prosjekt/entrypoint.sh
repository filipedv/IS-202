#!/bin/bash
set -e

CERT_DIR="/https"
CERT_FILE="$CERT_DIR/devcert.pfx"
CERT_PASSWORD="Oblig1DevOnly!"

# Generer sertifikat hvis det mangler
if [ ! -f "$CERT_FILE" ]; then
    echo "Genererer SSL-sertifikat..."
    openssl req -x509 -newkey rsa:4096 \
        -keyout "$CERT_DIR/devcert.key" \
        -out "$CERT_DIR/devcert.crt" \
        -days 365 \
        -nodes \
        -subj "/CN=localhost" >/dev/null 2>&1
    
    openssl pkcs12 -export \
        -out "$CERT_FILE" \
        -inkey "$CERT_DIR/devcert.key" \
        -in "$CERT_DIR/devcert.crt" \
        -passout "pass:$CERT_PASSWORD" >/dev/null 2>&1
    
    echo "✓ Sertifikat generert"
fi

# Start applikasjonen
exec dotnet OBLIG1-Prosjekt.dll "$@"

