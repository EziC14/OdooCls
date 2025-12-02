#!/bin/bash

# Script de monitoreo para OdooCls APIs
# Uso: ./monitor.sh
# O como cron: 0 */6 * * * /home/usuario/odoocls/monitor.sh >> /home/usuario/odoocls/monitor.log 2>&1

LOG_FILE="${1:-/tmp/odoocls_health.log}"
HEALTH_CHECK_ENDPOINT="/WeatherForecast"
ALERTAS_EMAIL="tu_email@example.com"

echo "[$(date '+%Y-%m-%d %H:%M:%S')] ====== CHEQUEO DE SALUD - OdooCls API ======" | tee -a $LOG_FILE

# Función para verificar API
check_api() {
  local CONTAINER=$1
  local PORT=$2
  local ENV=$3
  
  echo "" | tee -a $LOG_FILE
  echo "▶️  Verificando $ENV (Puerto $PORT)..." | tee -a $LOG_FILE
  
  # Verificar si contenedor está corriendo
  if ! docker ps | grep -q $CONTAINER; then
    echo "❌ ERROR: Contenedor $CONTAINER NO ESTÁ CORRIENDO" | tee -a $LOG_FILE
    return 1
  fi
  
  echo "✅ Contenedor $CONTAINER está activo" | tee -a $LOG_FILE
  
  # Probar endpoint
  local HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:$PORT$HEALTH_CHECK_ENDPOINT 2>/dev/null)
  
  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ API respondiendo (HTTP $HTTP_CODE)" | tee -a $LOG_FILE
  else
    echo "⚠️  API retornó HTTP $HTTP_CODE (esperado 200)" | tee -a $LOG_FILE
    return 1
  fi
  
  # Verificar ODBC
  local ODBC_TEST=$(docker exec $CONTAINER isql -v CLS ODOO ODOO <<< "SELECT 'OK'" 2>&1 | grep -i "SQL>" | wc -l)
  
  if [ "$ODBC_TEST" -gt 0 ]; then
    echo "✅ ODBC conectando correctamente" | tee -a $LOG_FILE
  else
    echo "⚠️  Problema con ODBC (posible problema de conectividad)" | tee -a $LOG_FILE
    return 1
  fi
  
  # Obtener stats del contenedor
  local MEMORY=$(docker stats --no-stream $CONTAINER | awk 'NR==2 {print $7}')
  local CPU=$(docker stats --no-stream $CONTAINER | awk 'NR==2 {print $3}')
  
  echo "📊 Recursos: CPU=$CPU, Memoria=$MEMORY" | tee -a $LOG_FILE
  
  return 0
}

# Función para verificar logs
check_logs() {
  local CONTAINER=$1
  local ENV=$2
  
  echo "" | tee -a $LOG_FILE
  echo "🔍 Últimas líneas de error en $ENV..." | tee -a $LOG_FILE
  
  local ERRORS=$(docker logs --tail 50 $CONTAINER 2>&1 | grep -i "error\|exception\|fatal" | wc -l)
  
  if [ "$ERRORS" -gt 0 ]; then
    echo "⚠️  Se encontraron $ERRORS líneas de error en logs" | tee -a $LOG_FILE
    docker logs --tail 10 $CONTAINER 2>&1 | grep -i "error\|exception\|fatal" | head -5 | tee -a $LOG_FILE
  else
    echo "✅ Sin errores en logs recientes" | tee -a $LOG_FILE
  fi
}

# Función para enviar alerta
send_alert() {
  local MESSAGE=$1
  local SUBJECT="⚠️  Alerta - OdooCls API"
  
  if command -v mail &> /dev/null; then
    echo "$MESSAGE" | mail -s "$SUBJECT" "$ALERTAS_EMAIL"
    echo "📧 Alerta enviada a $ALERTAS_EMAIL" | tee -a $LOG_FILE
  else
    echo "⚠️  Mail no instalado, no se pudo enviar alerta" | tee -a $LOG_FILE
  fi
}

# MAIN
PROD_OK=true
DEV_OK=true

# Chequear Producción
check_api "odoocls-api-prod" "8095" "Producción" || PROD_OK=false
check_logs "odoocls-api-prod" "Producción"

# Chequear Desarrollo
check_api "odoocls-api-dev" "8081" "Desarrollo" || DEV_OK=false
check_logs "odoocls-api-dev" "Desarrollo"

# Resumen
echo "" | tee -a $LOG_FILE
echo "════════════════════════════════════" | tee -a $LOG_FILE
echo "📋 RESUMEN:" | tee -a $LOG_FILE
echo "  Producción: $([ "$PROD_OK" = true ] && echo '✅ OK' || echo '❌ PROBLEMA')" | tee -a $LOG_FILE
echo "  Desarrollo: $([ "$DEV_OK" = true ] && echo '✅ OK' || echo '❌ PROBLEMA')" | tee -a $LOG_FILE
echo "════════════════════════════════════" | tee -a $LOG_FILE

# Enviar alerta si hay problemas
if [ "$PROD_OK" = false ] || [ "$DEV_OK" = false ]; then
  ALERT_MSG="Problemas detectados en OdooCls API:\n"
  [ "$PROD_OK" = false ] && ALERT_MSG="$ALERT_MSG- Producción NO responde\n"
  [ "$DEV_OK" = false ] && ALERT_MSG="$ALERT_MSG- Desarrollo NO responde\n"
  
  send_alert "$ALERT_MSG"
fi

echo "" | tee -a $LOG_FILE
