# SS Europa — Backend Web API (.NET 10)

Clean Architecture **.NET 10** Web API powering the SS Europa platform:
- **ITALIA-ADMIN**: Staff & Admin web dashboard
- **ITALIA-NATIVE**: Student React Native mobile app (`/api/v1` routes)
- **ITALIA**: Public static website (Contact us `/api/user/contact-us` and visitor tracking `/api/user/visitor`)

## Repository layout

```text
ITALIA-backend/
├── backend/                 .NET Clean Architecture solution
│   ├── SSeuropa.Domain/
│   ├── SSeuropa.Application/
│   ├── SSeuropa.Infrastructure/
│   ├── SSeuropa.Api/
│   └── SSeuropa.Tests/
├── docs/
│   ├── architecture-summary.md
│   └── api-documentation.md
├── package.json
└── README.md
```

## Prerequisites

- .NET SDK 10.x
- Node.js 20+ / npm 10+ (for npm scripts convenience)

## Running the Backend

You can use standard `dotnet` CLI or `npm`:

```bash
# Using npm:
npm run setup      # dotnet restore
npm run build      # dotnet build
npm test           # dotnet test
npm run dev        # dotnet run --project backend/SSeuropa.Api --launch-profile http

# Or directly with dotnet:
dotnet restore backend/SSeuropa.slnx
dotnet build backend/SSeuropa.slnx
dotnet test backend/SSeuropa.slnx
dotnet run --project backend/SSeuropa.Api --launch-profile http
```

- API Base: `http://localhost:5057`
- Swagger UI: `http://localhost:5057/swagger`
- Health Check: `http://localhost:5057/health`

### Environment examples:

- `backend/SSeuropa.Api/appsettings.json`
- `backend/SSeuropa.Api/appsettings.Example.json`
- Override secrets with environment variables (e.g. `Jwt__Key`)

### Connected Applications

1. **Admin Portal (`ITALIA-ADMIN`)**: Connects to `http://localhost:5057` via JWT Bearer tokens for curriculum, classes, enquiries, and user management.
2. **Student Mobile App (`ITALIA-NATIVE`)**: Connects to `http://localhost:5057/api/v1` for student dashboard, course lessons, quizzes, and live classes.
3. **Public Website (`ITALIA`)**: Submits inquiries to `http://localhost:5057/api/user/contact-us`, automatically capturing leads into the Admissions desk.


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
