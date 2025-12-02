#!/bin/bash

# Script para probar conectividad ODBC en el contenedor Docker
# Uso: ./test-odbc.sh prod
#      ./test-odbc.sh dev

CONTAINER_NAME="odoocls-api-prod"
if [ "$1" = "dev" ]; then
  CONTAINER_NAME="odoocls-api-dev"
fi

echo "================================"
echo "Probando ODBC en: $CONTAINER_NAME"
echo "================================"

# Test 1: Verificar que odbc.ini y odbcinst.ini existan
echo -e "\n✓ Verificando configuración ODBC:"
docker exec $CONTAINER_NAME ls -la /etc/odbc.ini /etc/odbcinst.ini

# Test 2: Listar DSN disponibles
echo -e "\n✓ DSN disponibles:"
docker exec $CONTAINER_NAME odbcinst -j

# Test 3: Mostrar contenido de odbc.ini
echo -e "\n✓ Contenido de odbc.ini:"
docker exec $CONTAINER_NAME cat /etc/odbc.ini

# Test 4: Intentar conexión SQL con isql
echo -e "\n✓ Intentando conectar con isql (DSN=CLS, UID=ODOO, PWD=ODOO):"
docker exec $CONTAINER_NAME isql -v CLS ODOO ODOO

# Test 5: Probar conectividad de red al servidor
echo -e "\n✓ Intentando ping al servidor 192.168.1.137:"
docker exec $CONTAINER_NAME ping -c 3 192.168.1.137

echo -e "\n✓ Test completado."
