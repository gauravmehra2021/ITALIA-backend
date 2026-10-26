# SS Europa — Advanced AI-Powered Learning Platform

Clean Architecture **.NET 10** Web API + **React 19 / TypeScript / Vite** frontend.

Prototype implements Phase 1 LMS core with realistic **in-memory** data. MongoDB, Azure Storage, PayPal, and live AI are **mocked** behind replaceable interfaces.

> **Important:** In-memory data resets whenever the API restarts. Demo authentication is explicitly labelled and is **not** production-secure.

## Repository layout

```text
SS-Europa/
├── backend/                 .NET Clean Architecture solution
│   ├── SSeuropa.Domain/
│   ├── SSeuropa.Application/
│   ├── SSeuropa.Infrastructure/
│   ├── SSeuropa.Api/
│   └── SSeuropa.Tests/
├── frontend/sseuropa-web/   React + Vite app
├── docs/
│   ├── architecture-summary.md
│   └── api-documentation.md
└── README.md
```

## Prerequisites

- .NET SDK 10.x
- Node.js 20+ / npm 10+

## Backend

```bash
cd backend
dotnet restore SSeuropa.slnx
dotnet build SSeuropa.slnx
dotnet test SSeuropa.slnx
dotnet run --project SSeuropa.Api --launch-profile http
```

- API: http://localhost:5057  
- Swagger: http://localhost:5057/swagger  
- Health: http://localhost:5057/health  

Environment examples:

- `backend/SSeuropa.Api/appsettings.Example.json`
- Override secrets with env vars (never commit production keys):
  - `Jwt__Key`
  - future: `ConnectionStrings__MongoDb`, `PayPal__ClientSecret`, etc.

## Frontend

```bash
cd frontend/sseuropa-web
cp .env.example .env   # VITE_API_BASE_URL=http://localhost:5057
npm install
npm run dev
npm run build
```

- App: http://localhost:5173

### Demo accounts

Password for OTP login flow: `Demo@12345`

| Email | Role |
|---|---|
| student@sseuropa.demo | Student |
| mentor@sseuropa.demo | Mentor |
| admin@sseuropa.demo | Admin |
| superadmin@sseuropa.demo | Super Admin |

Use **Demo as …** buttons on the login page for the labelled demonstration auth flow, or password + OTP (OTP hint returned in API response for local demo only).

## What is mocked vs planned

| Capability | Status |
|---|---|
| Users, courses, classes, materials, tests, progress, attendance | Implemented (in-memory) |
| JWT + OTP + one-active-session | Implemented (demo) |
| RBAC checks | Implemented in application services |
| PayPal subscriptions/payments | Mocked gateway |
| Azure Blob protected URLs | Mocked SAS-like URLs |
| AI tutor / translator / speaking | Mocked responses |
| MongoDB | Planned — repository interfaces ready |
| React Native mobile | Planned separately |
| Hostinger / Azure Key Vault / live monitoring | Planned |

## Architecture notes

See [docs/architecture-summary.md](docs/architecture-summary.md) and [docs/api-documentation.md](docs/api-documentation.md).

## Verification performed

- `dotnet build` — succeeded  
- `dotnet test` — unit + API integration tests  
- `npm run build` — production frontend build  
