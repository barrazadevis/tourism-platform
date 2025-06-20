# Tourism Platform - Multi-Tenant SaaS

Plataforma multi-tenant para empresas de turismo que permite gestionar planes de viaje, cotizaciones y clientes.

## 🏗️ Arquitectura

- **Backend**: .NET 8 Microservices
  - Auth Service (Puerto 5001)
  - Tourism API (Puerto 5000)
- **Frontend**: Next.js 14 + TypeScript (Puerto 3000)
- **Database**: PostgreSQL con schemas separados

## 🚀 Instalación

### Prerrequisitos
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 15+

### Configuración

1. **Clonar repositorio**
```bash
git clone https://github.com/tu-usuario/tourism-platform.git
cd tourism-platform

Configurar Base de Datos

sqlCREATE DATABASE tourism_platform;

Backend - Auth Service

bashcd TourismPlatform.Auth.API
dotnet restore
dotnet ef database update
dotnet run --urls=http://localhost:5001

Backend - Tourism API

bashcd TourismPlatform.API
dotnet restore
dotnet ef database update
dotnet run --urls=http://localhost:5000

Frontend

bashcd tourism-web
npm install
npm run dev
📁 Endpoints Principales
Auth Service (5001)

POST /api/auth/login
POST /api/auth/register
POST /api/auth/validate

Tourism API (5000)

GET/POST /api/travelplans
GET/POST /api/quotes
GET /api/dashboard/stats

🛠️ Stack Tecnológico

.NET 8: Web APIs, Entity Framework Core
Next.js 14: App Router, Server Components
PostgreSQL: Multi-tenant schemas
JWT: Autenticación distribuida
Tailwind CSS: Styling
TypeScript: Type safety

🔧 Desarrollo
Ejecutar en desarrollo
bash# Servicios backend
dotnet run --project TourismPlatform.Auth.API --urls=http://localhost:5001
dotnet run --project TourismPlatform.API --urls=http://localhost:5000

# Frontend
cd tourism-web && npm run dev
Migraciones
bashdotnet ef migrations add MigrationName --project TourismPlatform.Auth.Data
dotnet ef database update --project TourismPlatform.Auth.API
🌐 Multi-Tenancy
El sistema utiliza subdominios para tenant isolation:

tenant1.localhost:3000/dashboard
tenant2.localhost:3000/plans

📊 Features Implementadas
✅ Autenticación JWT multi-tenant
✅ Gestión de planes de viaje
✅ Sistema de cotizaciones
✅ Dashboard con estadísticas
✅ Multi-tenant routing
✅ Protected routes
🚧 Roadmap

 Refresh tokens
 Role-based permissions
 PDF generation
 Email notifications
 File uploads
 Advanced analytics