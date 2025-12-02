#!/bin/bash

# Script para copiar drivers IBM iSeries al contexto de Docker
# Ejecutar ANTES de docker build

echo "Copiando drivers IBM iSeries Access ODBC..."

# Crear directorio si no existe
mkdir -p ibm-iaccess-libs || sudo mkdir -p ibm-iaccess-libs

# Copiar bibliotecas compartidas necesarias
sudo cp /opt/ibm/iaccess/lib64/libcwbcore.so ibm-iaccess-libs/ 2>/dev/null || echo "Warning: libcwbcore.so no encontrado"
sudo cp /opt/ibm/iaccess/lib64/libcwbodbc.so ibm-iaccess-libs/ 2>/dev/null || echo "Warning: libcwbodbc.so no encontrado"
sudo cp /opt/ibm/iaccess/lib64/libcwbodbcs.so ibm-iaccess-libs/ 2>/dev/null || echo "Warning: libcwbodbcs.so no encontrado"

# Cambiar permisos para que docker pueda leerlos
sudo chmod 644 ibm-iaccess-libs/*.so 2>/dev/null

# Copiar archivos de configuración si existen
sudo cp /opt/ibm/iaccess/etc/* ibm-iaccess-libs/ 2>/dev/null || echo "Info: Sin archivos en /etc"

echo "Archivos copiados:"
ls -lh ibm-iaccess-libs/

echo "✓ Drivers listos para Docker build"
