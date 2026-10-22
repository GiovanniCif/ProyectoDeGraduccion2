# Plan de despliegue con Docker, Docker Compose y Nginx

## Objetivo

Empaquetar SGE PIERI DICACIO para que una computadora nueva pueda descargar las imágenes publicadas, proporcionar sus variables privadas y levantar frontend, API y MySQL con un único comando de Docker Compose.

Docker Hub almacenará dos imágenes propias:

- `USUARIO_DOCKERHUB/sge-pieri-api`: API ASP.NET Core.
- `USUARIO_DOCKERHUB/sge-pieri-frontend`: archivos estáticos servidos por Nginx.

MySQL utilizará la imagen oficial `mysql:8.0`. El archivo `compose.yaml` y el archivo de ejemplo de variables se distribuirán junto al proyecto o en un repositorio; Docker Hub no sustituye al archivo de Compose.

## Fase 0 - Estabilización previa

- [x] Mostrar Departamento antes de Municipio y cargar los municipios de forma dependiente.
- [x] Restaurar Reportes y Bitácora en el menú de Usuarios.
- [x] Ocultar módulos del menú que el rol no puede consultar.
- [x] Redirigir cada rol a su primer módulo permitido después del inicio de sesión.
- [x] Aplicar los permisos de consulta también en la API.
- [x] Agregar y ejecutar comprobaciones automatizadas de autorización y navegación.

## Fase 1 - Preparar la aplicación para contenedores

1. Sustituir la URL fija `https://localhost:7020/api` del frontend por `/api`.
2. Configurar Nginx para servir el frontend y enviar `/api/` al contenedor `api` por HTTP dentro de la red de Compose.
3. Leer desde variables de entorno:
   - `ConnectionStrings__DefaultConnection`.
   - `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` y `Jwt__ExpireMinutes`.
   - Credenciales y nombre de la base MySQL.
   - Contraseña inicial del administrador, sin dejarla escrita en el código ni restablecerla en cada arranque.
4. Agregar un endpoint de salud para comprobar que la API y MySQL están disponibles.
5. Resolver la creación del esquema. Actualmente se llama `MigrateAsync()`, pero no hay migraciones de Entity Framework en el proyecto. La opción recomendada es crear una migración inicial reproducible y validarla contra una base vacía.
6. Incorporar reintentos controlados al inicio de la API mientras MySQL termina de aceptar conexiones.

## Fase 2 - Crear las imágenes

1. Crear un `Dockerfile` multi-stage para la API:
   - restauración de paquetes;
   - compilación y publicación en Release;
   - imagen final `mcr.microsoft.com/dotnet/aspnet:8.0`;
   - ejecución como usuario sin privilegios;
   - puerto interno HTTP y comprobación de salud.
2. Crear un `Dockerfile` para el frontend basado en Nginx.
3. Crear `nginx.conf` con:
   - documentos estáticos;
   - proxy `/api/` hacia `http://api:8080/api/`;
   - proxy de `/uploads/` si los comprobantes se sirven desde la API;
   - encabezados básicos y compresión.
4. Crear `.dockerignore` para excluir `bin`, `obj`, `.vs`, archivos temporales y secretos.
5. Construir imágenes Linux `amd64`; usar `buildx` multi-arquitectura si la computadora destino puede ser ARM64.

## Fase 3 - Orquestar con Docker Compose

Crear `compose.yaml` con tres servicios:

- `db`: MySQL 8, volumen `mysql_data`, variables privadas y healthcheck.
- `api`: imagen propia, dependencia saludable de MySQL, variables de conexión/JWT y volumen `api_uploads` para vouchers.
- `frontend`: imagen propia con Nginx, dependencia de la API y publicación del puerto, inicialmente `8080:80`.

También se creará `.env.example` sin secretos reales. El `.env` verdadero quedará excluido del contexto de Docker y del control de versiones.

## Fase 4 - Validación local limpia

1. Validar la sintaxis con `docker compose config`.
2. Construir sin depender de archivos generados localmente.
3. Levantar el conjunto desde una base y volúmenes nuevos usando un nombre de proyecto de prueba.
4. Comprobar:
   - salud de MySQL, API y Nginx;
   - inicio de sesión;
   - acceso y rechazo por cada rol;
   - clientes, pedidos, inventario, producción, empleados, pagos y reportes;
   - subida y recuperación de vouchers;
   - persistencia de base de datos y archivos después de reiniciar contenedores.
5. Repetir la prueba usando únicamente las imágenes construidas, el `compose.yaml` y un `.env` nuevo.

## Fase 5 - Publicación en Docker Hub

1. Crear los repositorios `sge-pieri-api` y `sge-pieri-frontend`.
2. Autenticarse con un token de acceso de Docker Hub.
3. Publicar etiquetas inmutables, por ejemplo `1.0.0`, y actualizar `latest` solamente cuando esa versión haya pasado las pruebas.
4. Confirmar que las imágenes se pueden descargar y que no contienen archivos `.env`, credenciales ni configuración de desarrollo.

## Fase 6 - Instalación en otra computadora

La computadora destino necesitará Docker Desktop o Docker Engine con Compose, además de:

- `compose.yaml` configurado para usar las imágenes de Docker Hub;
- un `.env` privado basado en `.env.example`;
- acceso al puerto configurado.

El flujo final será:

```powershell
docker compose pull
docker compose up -d
docker compose ps
```

Después se abrirá `http://localhost:8080`. Se documentarán también los comandos de actualización, copia de seguridad, restauración y consulta de logs.

## Criterios de aceptación

- Una máquina sin Visual Studio, .NET ni MySQL instalados puede ejecutar el sistema usando solo Docker.
- La primera ejecución crea correctamente el esquema y los datos iniciales.
- Ningún secreto está dentro de las imágenes o del repositorio.
- Los permisos se aplican tanto en la interfaz como en los endpoints.
- La base de datos y los vouchers sobreviven a reinicios y actualizaciones.
- Una versión etiquetada de Docker Hub se puede descargar y ejecutar sin reconstruir el código.
