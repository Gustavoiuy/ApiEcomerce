# 🛒 E-commerce API RESTful (.NET 8 + Azure)

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Azure App Service](https://img.shields.io/badge/Azure-App%20Service-0078D4?style=flat&logo=microsoftazure)
![Azure SQL](https://img.shields.io/badge/Database-Azure%20SQL-0078D4?style=flat&logo=microsoftsqlserver)
![Build Status](https://img.shields.io/badge/build-passing-brightgreen?style=flat&logo=github)
![Swagger](https://img.shields.io/badge/Swagger-UI-85EA2D?style=flat&logo=swagger)
![License](https://img.shields.io/badge/license-MIT-green)

> **🚀 Live Demo (Producción):** [https://apiecomercev1.azurewebsites.net/swagger](https://apiecomercev1.azurewebsites.net/swagger)

API RESTful escalable y de alto rendimiento diseñada para orquestar la lógica de negocio de una plataforma de comercio electrónico moderna. Este proyecto demuestra una implementación sólida de **Clean Architecture**, patrones de diseño empresarial y un flujo de trabajo **DevOps** automatizado en la nube de Microsoft Azure.

---

## 🏛️ Arquitectura y Diseño de Software

Este proyecto fue construido siguiendo principios de ingeniería de software para garantizar mantenibilidad, testabilidad y escalabilidad:

* **Clean Architecture:** Separación estricta de responsabilidades (Domain, Application, Infrastructure, API).
* **Repository Pattern:** Abstracción de la capa de acceso a datos.
* **SOLID Principles:** Aplicados en el diseño de servicios y controladores.
* **Entity Framework Core (Code First):** Gestión de base de datos mediante migraciones y modelos de dominio.
* **Dependency Injection:** Uso del contenedor nativo de .NET para desacoplar componentes.

## 🛠️ Tech Stack

* **Lenguaje:** C# (.NET 8)
* **Framework:** ASP.NET Core Web API
* **Base de Datos:** Azure SQL Database (Producción) / SQL Server (Local)
* **ORM:** Entity Framework Core
* **Cloud Hosting:** Azure App Service
* **CI/CD:** GitHub Actions (Despliegue automatizado)
* **Documentación:** Swagger UI (OpenAPI)
* **Seguridad:** JWT (JSON Web Tokens)

## 🚀 Funcionalidades Principales

* 🔐 **Autenticación y Seguridad:** Login, Registro y protección de rutas mediante JWT.
* 📦 **Gestión de Productos:** CRUD completo con validaciones de negocio.
* 🛒 **Carrito de Compras:** Lógica para persistencia y manipulación del carrito.
* 📄 **Paginación Inteligente:** Endpoints optimizados para listar grandes volúmenes de datos.
* ☁️ **Cloud Native:** Configuración mediante Variables de Entorno y Secretos para entornos de nube.

## 📖 Documentación de la API

La API cuenta con Swagger habilitado en producción para facilitar la revisión técnica y pruebas de integración.

| Método | Endpoint (Ejemplo) | Descripción | Nivel de Acceso |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Obtener Token JWT | Público |
| `GET` | `/api/products` | Listar productos paginados | Público |
| `GET` | `/api/products/{id}` | Detalle de producto | Público |
| `POST` | `/api/orders` | Crear orden de compra | Usuario Autenticado |

👉 **[Ver Documentación Completa en Swagger](https://apiecommercev1-d2bee5hpchghavcy.canadacentral-01.azurewebsites.net/swagger/index.html)**

---

## 🔧 Configuración para Desarrollo Local

Si deseas clonar y ejecutar este proyecto en tu máquina local:

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/](https://github.com/)[TU-USUARIO-GITHUB]/[NOMBRE-DE-TU-REPO].git
    ```

2.  **Configurar Base de Datos:**
    Modifica el archivo `appsettings.json` con tu cadena de conexión local:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=EcommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```

3.  **Aplicar Migraciones:**
    ```bash
    dotnet ef database update
    ```

4.  **Ejecutar la API:**
    ```bash
    dotnet run
    ```
    Accede a `https://apiecommercev1-d2bee5hpchghavcy.canadacentral-01.azurewebsites.net/swagger/index.html` (o el puerto que te indique la consola).

---

## 👤 Autor

**[TU NOMBRE AQUI]**
*Full Stack Developer | .NET & React Enthusiast*

* 💼 [LinkedIn](www.linkedin.com/in/gustavotcruz-dev)
* 🌐 [Portafolio / Website](https://trejodev24.vercel.app/)

---
*Desarrollado con ❤️ y C#*