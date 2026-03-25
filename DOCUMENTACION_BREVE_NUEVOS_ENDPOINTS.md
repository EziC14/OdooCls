# Documentación breve - Nuevos endpoints GET paginados

## Endpoints nuevos

- `GET /api/RegistroAlmacenes/all?page=1&pageSize=20`
- `GET /api/RegistroArticulos/all?page=1&pageSize=20`
- `GET /api/RegistroClientes/all?page=1&pageSize=20`
- `GET /api/RegistroProveedores/all?page=1&pageSize=20`

### Consulta por ID

Si necesitas buscar un registro específico por identificador (ID), usa:

- `GET /api/RegistroAlmacenes/{id}`
- `GET /api/RegistroArticulos/{id}`
- `GET /api/RegistroClientes/{id}`
- `GET /api/RegistroProveedores/{id}`

Ejemplo:

```bash
curl -X GET "http://localhost:5000/api/RegistroAlmacenes/03" \
  -H "Authorization: Bearer TU_TOKEN"
```

---

## Requisitos

- Enviar token Bearer en el header:
  - `Authorization: Bearer TU_TOKEN`

---

## Parámetros de paginación

- `page`: número de página (inicia en 1)
- `pageSize`: cantidad de registros por página
  - Valor recomendado: `20`
  - Máximo permitido: `100`

Si envías valores inválidos, el servicio ajusta automáticamente a valores seguros.

---

## Ejemplo rápido (cURL)

```bash
curl -X GET "http://localhost:5000/api/RegistroAlmacenes/all?page=1&pageSize=20" \
  -H "Authorization: Bearer TU_TOKEN"
```

---

## Estructura de respuesta

```json
{
  "httpStatusCode": 200,
  "code": 1000,
  "message": "Consulta realizada correctamente",
  "data": {
    "data": [
      { "ALCODI": "001", "ALNOMB": "ALMACEN CENTRAL" }
    ],
    "totalCount": 250,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 13,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---