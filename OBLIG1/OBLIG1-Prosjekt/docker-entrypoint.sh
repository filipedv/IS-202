#!/bin/bash
set -e

CERT_DIR="/https"
CERT_FILE="$CERT_DIR/devcert.pfx"
CERT_PASSWORD="Oblig1DevOnly!"

# Hvis sertifikatet ikke eksisterer, generer det
if [ ! -f "$CERT_FILE" ]; then
    echo "Sertifikat mangler, genererer selvsignert sertifikat..."
    
    # Generer private key og sertifikat
    openssl req -x509 -newkey rsa:4096 \
        -keyout "$CERT_DIR/devcert.key" \
        -out "$CERT_DIR/devcert.crt" \
        -days 365 \
        -nodes \
        -subj "/CN=localhost" 2>/dev/null
    
    # Konverter til PKCS12 format (.pfx)
    openssl pkcs12 -export \
        -out "$CERT_FILE" \
        -inkey "$CERT_DIR/devcert.key" \
        -in "$CERT_DIR/devcert.crt" \
        -passout "pass:$CERT_PASSWORD" 2>/dev/null
    
    echo "✓ Sertifikat generert automatisk"
fi

# Kjør den opprinnelige dotnet kommandoen
exec dotnet "$@"

