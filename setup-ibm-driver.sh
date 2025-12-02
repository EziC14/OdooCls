#!/bin/bash

# Script para copiar drivers IBM iSeries al contexto de Docker
# Ejecutar ANTES de docker build

echo "Copiando drivers IBM iSeries Access ODBC..."

# Crear directorio si no existe
if [ ! -d "ibm-iaccess-libs" ]; then
    mkdir -p ibm-iaccess-libs 2>/dev/null
    status=$?
LIBS=("libcwbcore.so" "libcwbodbc.so" "libcwbodbcs.so")
for lib in "${LIBS[@]}"; do
    sudo cp "/opt/ibm/iaccess/lib64/$lib" ibm-iaccess-libs/ 2>/dev/null || echo "Warning: $lib no encontrado"
done
            sudo mkdir -p ibm-iaccess-libs
            status=$?
if ! sudo chmod 644 ibm-iaccess-libs/*.so; then
    echo "Error: No se pudieron cambiar los permisos de las bibliotecas IBM iSeries Access ODBC" >&2
fi
                echo "Error: No se pudo crear el directorio ibm-iaccess-libs incluso con sudo." >&2
                exit 1
            fi
        else
            echo "Error: No se pudo crear el directorio ibm-iaccess-libs." >&2
            exit 1
        fi
    fi
fi
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
