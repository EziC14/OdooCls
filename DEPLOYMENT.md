# OdooCls API - Guía de Deployment

## 📋 Resumen

OdooCls API es una aplicación ASP.NET Core 8.0 que expone operaciones del ERP Speed mediante una API REST. Utiliza ODBC para conectarse a dos instancias independientes de Speed (desarrollo y producción) en un servidor interno.

**Arquitectura:**
- **Framework:** ASP.NET Core 8.0
- **Base de datos:** SQL Server via ODBC (FreeTDS)
- **Deployment:** Docker (nginx reverse proxy + dotnet app)
- **Ambientes:** Producción (speed400cs) y Desarrollo (speed400xx)

---

## 🔧 Requisitos Previos

### En tu máquina local (antes de subir al VPS)

1. **.NET SDK 8.0** instalado
2. **Docker Desktop** instalado y corriendo
3. **Git** para control de versiones

### En el VPS (Ubuntu/Debian)

```bash
# Actualizar paquetes
sudo apt-get update
sudo apt-get upgrade -y

# Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Permitir que tu usuario ejecute docker sin sudo
sudo usermod -aG docker $USER
# Reiniciar sesión para aplicar cambios
```

---

## 📁 Estructura de Archivos Requeridos

```
/home/usuario/odoocls/
├── Dockerfile                      # Definición de imagen Docker
├── docker-compose.yml              # Orquestación de contenedores
├── odbc.ini                        # Configuración ODBC (DSN)
├── odbcinst.ini                    # Configuración de drivers ODBC
├── appsettings.json                # Config producción
├── appsettings.Development.json    # Config desarrollo
├── deploy.sh                       # Script de automatización
├── test-odbc.sh                    # Script para probar ODBC
├── ApisOdoo/                       # Código fuente
│   ├── OdooCls.API.csproj
│   ├── Program.cs
│   ├── Controllers/
│   ├── Attributes/
│   └── ...
├── OdooCls.Application/            # Capa de aplicación
├── OdooCls.Datos/                  # Capa de datos
└── OdooCls.Negocio/                # Capa de negocio
```

---

## 🚀 Deployment Paso a Paso

### Paso 1: Preparar archivos en tu máquina local

1. **Verificar que el código compila:**
   ```bash
   cd /path/to/OdooCls
   dotnet build
   ```

2. **Probar en Docker localmente (opcional):**
   ```bash
   docker build -t odoocls-api:latest .
   docker-compose build
   ```

### Paso 2: Copiar archivos al VPS

```bash
# Desde tu máquina local
scp -r /path/to/OdooCls usuario@192.168.1.100:~/odoocls/

# O usar rsync (más eficiente)
rsync -avz --delete /path/to/OdooCls/ usuario@192.168.1.100:~/odoocls/
```

### Paso 3: Conectar al VPS y ejecutar deployment

```bash
# Conectar al VPS
ssh usuario@192.168.1.100

# Navegar al directorio
cd ~/odoocls

# Hacer executable el script de deployment
chmod +x deploy.sh

# Ejecutar deployment
# Opción 1: Desplegar ambas APIs (prod + dev)
./deploy.sh both

# Opción 2: Solo producción
./deploy.sh prod

# Opción 3: Solo desarrollo
./deploy.sh dev
```

---

## 🧪 Validar Deployment

### Verificar contenedores corriendo

```bash
docker ps

# Debería mostrar:
# - odoocls-api-prod (puerto 8095:80)
# - odoocls-api-dev  (puerto 8081:80)
```

### Probar conectividad ODBC

```bash
# Script automático
chmod +x test-odbc.sh
./test-odbc.sh prod      # Prueba API producción
./test-odbc.sh dev       # Prueba API desarrollo

# O manualmente
docker exec odoocls-api-prod isql -v CLS ODOO ODOO
```

### Probar API manualmente

```bash
# Endpoint de prueba
curl http://localhost:8095/WeatherForecast

# Debería retornar algo como:
# [{"date":"2024-01-15T...","temperatureC":..,"summary":"..."}, ...]
```

### Ver logs

```bash
# Logs producción
docker logs odoocls-api-prod

# Logs desarrollo
docker logs odoocls-api-dev

# Logs en tiempo real
docker logs -f odoocls-api-prod

# Logs de los últimos 100 líneas
docker logs --tail 100 odoocls-api-prod
```

---

## 🔐 Configuración de Seguridad

### 1. Firewall

Asegurate que solo puertos necesarios están abiertos:

```bash
# UFW (Uncomplicated Firewall)
sudo ufw enable
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh              # Puerto 22
sudo ufw allow 8095             # API Producción
sudo ufw allow 8081             # API Desarrollo (opcional si solo prod)
```

### 2. API Key (Bearer Token)

Está configurada en `appsettings.json` y `appsettings.Development.json`:

```json
{
  "Authentication": {
    "ApiKey": "n/KbzsjflJp/wJI+t17W6pqm2cYQMpQe9LZbjWwD5S3zEYkM0zUf3tFsLmScpZG1le6gVsqDA3Qy8VFBdO4aAQ"
  }
}
```

Para cambiarla, genera una nueva clave y actualiza ambos archivos:

```bash
# Generar base64 aleatorio
openssl rand -base64 64
```

### 3. ODBC Credentials

El archivo `odbc.ini` contiene credenciales en texto plano. Considera:

```bash
# Restringir permisos
sudo chmod 600 /etc/odbc.ini
sudo chmod 600 /etc/odbcinst.ini
sudo chown root:root /etc/odbc.ini
sudo chown root:root /etc/odbcinst.ini
```

---

## 🛠️ Troubleshooting

### Error: "Unable to connect to data source"

```bash
# 1. Verificar que ODBC está instalado en el contenedor
docker exec odoocls-api-prod apt-cache policy unixodbc

# 2. Probar conectividad de red
docker exec odoocls-api-prod ping 192.168.1.137

# 3. Ver contenido de odbc.ini
docker exec odoocls-api-prod cat /etc/odbc.ini

# 4. Probar conexión SQL
docker exec odoocls-api-prod isql -v CLS ODOO ODOO
```

### Error: "Cannot find DSN 'CLS'"

```bash
# 1. Verificar que odbc.ini fue copiado
docker exec odoocls-api-prod ls -la /etc/odbc.ini

# 2. Verificar contenido
docker exec odoocls-api-prod cat /etc/odbc.ini

# 3. Verificar que odbcinst.ini tiene la ruta correcta al driver FreeTDS
docker exec odoocls-api-prod cat /etc/odbcinst.ini

# 4. Listar drivers disponibles
docker exec odoocls-api-prod odbcinst -j
```

### Error: "Port 8095 already in use"

```bash
# Encontrar proceso usando el puerto
sudo lsof -i :8095

# O
sudo netstat -tlnp | grep 8095

# Matar proceso
sudo kill -9 <PID>
```

### Error: "Cannot connect to Docker daemon"

```bash
# Asegurar que Docker está corriendo
sudo systemctl start docker

# O en sistemas sin systemd
sudo service docker start

# Verificar permisos del usuario
sudo usermod -aG docker $USER
# Hacer logout y login
```

---

## 📊 Monitoreo Básico

### Estado de los contenedores

```bash
# Ver estado general
docker stats

# Ver estado específico
docker inspect odoocls-api-prod

# Historial de restarts
docker ps -a
```

### Registros persistentes

```bash
# Guardar logs a archivo
docker logs odoocls-api-prod > prod-logs.txt 2>&1

# Monitorear en tiempo real
docker logs -f odoocls-api-prod | tee prod-logs-live.txt
```

---

## 🔄 Actualizar Código

Cuando hagas cambios en el código:

```bash
# Desde tu máquina local
# Hacer cambios y commitear a git

# Copiar al VPS
rsync -avz --delete /path/to/OdooCls/ usuario@192.168.1.100:~/odoocls/

# En el VPS
cd ~/odoocls

# Reconstruir y redeploy
./deploy.sh prod    # O ./deploy.sh dev   o ./deploy.sh both

# O manualmente
docker-compose build --no-cache api-prod
docker-compose up -d api-prod
```

---

## 📝 Variables de Entorno

Las APIs usan estas variables desde `docker-compose.yml`:

| Variable | Producción | Desarrollo | Descripción |
|----------|-----------|-----------|------------|
| `ASPNETCORE_ENVIRONMENT` | Production | Development | Ambiente de ejecución |
| `ConnectionStrings__ERPConexion` | DSN=CLS;... | DSN=CLS;... | Cadena de conexión ODBC |
| `Authentication__Library` | speed400cs | speed400xx | Librería Speed a usar |
| `Authentication__ApiKey` | n/KbzsjflJp... | n/KbzsjflJp... | Clave API para Bearer token |

Para cambiar estas variables, edita `docker-compose.yml` y ejecuta:

```bash
docker-compose up -d --force-recreate
```

---

## 📞 Contacto y Soporte

Si encuentras problemas:

1. Revisa los logs: `docker logs odoocls-api-prod`
2. Valida conectividad ODBC: `./test-odbc.sh prod`
3. Verifica firewall/red: `docker exec odoocls-api-prod ping 192.168.1.137`
4. Revisa este README en la sección Troubleshooting

---

## ✅ Checklist Final

Antes de considerar el deployment completado:

- [ ] Contenedores corriendo: `docker ps`
- [ ] API responde: `curl http://localhost:8095/WeatherForecast`
- [ ] ODBC conecta: `./test-odbc.sh prod`
- [ ] Logs sin errores: `docker logs odoocls-api-prod`
- [ ] Ambientes separados: Producción usa speed400cs, Desarrollo usa speed400xx
- [ ] Puertos configurados: 8095 (prod), 8081 (dev)
- [ ] Firewall permite acceso: `sudo ufw status`

¡Deployment completado! 🎉
