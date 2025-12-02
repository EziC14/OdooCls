#!/bin/bash

# Script de deployment para OdooCls API
# Lugar de instalación: /home/usuario/odoocls/
# Uso: ./deploy.sh [prod|dev|both]

set -e

DEPLOY_ENV=${1:-both}
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "================================"
echo "🚀 OdooCls API - Deployment"
echo "================================"
echo "Ambiente: $DEPLOY_ENV"
echo "Directorio: $SCRIPT_DIR"
echo ""

# Función para validar archivos
validate_files() {
  echo "✓ Validando archivos requeridos..."
  
  local files=(
    "Dockerfile"
    "docker-compose.yml"
    "odbc.ini"
    "odbcinst.ini"
    "ApisOdoo/OdooCls.API.csproj"
    "ApisOdoo/Program.cs"
    "appsettings.json"
    "appsettings.Development.json"
  )
  
  for file in "${files[@]}"; do
    if [ ! -f "$SCRIPT_DIR/$file" ]; then
      echo "❌ Falta archivo: $file"
      exit 1
    fi
  done
  
  echo "✅ Todos los archivos están presentes"
}

# Función para limpiar contenedores antiguos
cleanup() {
  echo ""
  echo "🧹 Limpiando contenedores antiguos..."
  
  if [ "$DEPLOY_ENV" != "dev" ]; then
    docker-compose down -v --remove-orphans 2>/dev/null || true
  fi
  
  echo "✅ Limpieza completada"
}

# Función para construir imagen Docker
build_image() {
  echo ""
  echo "🔨 Construyendo imagen Docker..."
  
  if [ "$DEPLOY_ENV" = "prod" ]; then
    docker-compose build --no-cache api-prod
  elif [ "$DEPLOY_ENV" = "dev" ]; then
    docker-compose build --no-cache api-dev
  else
    docker-compose build --no-cache
  fi
  
  echo "✅ Imagen construida exitosamente"
}

# Función para iniciar contenedores
start_services() {
  echo ""
  echo "▶️  Iniciando servicios..."
  
  if [ "$DEPLOY_ENV" = "prod" ]; then
    docker-compose up -d api-prod
    echo "✅ API Producción iniciada en puerto 8095"
  elif [ "$DEPLOY_ENV" = "dev" ]; then
    docker-compose up -d api-dev
    echo "✅ API Desarrollo iniciada en puerto 8081"
  else
    docker-compose up -d
    echo "✅ Ambas APIs iniciadas (Prod:8095, Dev:8081)"
  fi
}

# Función para validar salud de contenedores
validate_health() {
  echo ""
  echo "🏥 Validando estado de contenedores..."
  
  if [ "$DEPLOY_ENV" != "dev" ]; then
    local container="odoocls-api-prod"
    echo "Aguardando $container por 10 segundos..."
    sleep 10
    
    if docker ps | grep -q $container; then
      echo "✅ $container está activo"
      docker exec $container curl -s http://localhost/WeatherForecast > /dev/null && echo "✅ API respondiendo" || echo "⚠️  API aún iniciando"
    else
      echo "❌ $container no está corriendo"
      docker logs $container
    fi
  fi
  
  if [ "$DEPLOY_ENV" != "prod" ]; then
    local container="odoocls-api-dev"
    echo "Aguardando $container por 10 segundos..."
    sleep 10
    
    if docker ps | grep -q $container; then
      echo "✅ $container está activo"
      docker exec $container curl -s http://localhost/WeatherForecast > /dev/null && echo "✅ API respondiendo" || echo "⚠️  API aún iniciando"
    else
      echo "❌ $container no está corriendo"
      docker logs $container
    fi
  fi
}

# Main execution
validate_files
cleanup
build_image
start_services
validate_health

echo ""
echo "================================"
echo "✅ Deployment completado"
echo "================================"
echo ""
echo "Endpoints:"
echo "  📍 Producción: http://localhost:8095"
echo "  📍 Desarrollo: http://localhost:8081"
echo ""
echo "Comandos útiles:"
echo "  Ver logs:       docker logs odoocls-api-prod"
echo "  Probar ODBC:    docker exec odoocls-api-prod isql -v CLS ODOO ODOO"
echo "  Detener:        docker-compose down"
echo ""
