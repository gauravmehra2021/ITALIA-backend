# SS Europa — Architecture Summary & Module Mapping

## Source document

Functional and technical requirements are taken from *SS Europa Advanced AI-Powered Learning Platform Technical Architecture* (MongoDB · .NET · React Native · Azure · PayPal · Hostinger).

This repository implements the **Phase 1 Secure LMS Core** as a runnable prototype, plus **mocked** Phase 2–4 surfaces (payments, AI, certificates, notifications, support).

## Stack adaptation

| Document | This prototype |
|---|---|
| React Native mobile | **React + TypeScript (Vite) web app** — mobile is deferred |
| ASP.NET Core API | **.NET 10 Clean Architecture** Web API |
| MongoDB | **In-memory repositories** (interfaces ready for MongoDB) |
| Azure Blob Storage | **MockStorageService** (SAS-like URLs) |
| PayPal | **MockPaymentGateway** |
| AI providers | **MockAiLearningService** |
| Hostinger + Azure | Documented for later; not required to run locally |

## Clean Architecture mapping

```
SSeuropa.Domain          Entities, enums, domain exceptions
SSeuropa.Application     DTOs, interfaces, services, FluentValidation
SSeuropa.Infrastructure  In-memory store/repos, JWT/OTP, mock adapters
SSeuropa.Api             Controllers, middleware, DI composition, Swagger
```

Dependency rule: Domain ← Application ← Infrastructure; Api → Application + Infrastructure (composition root only).

## Roles (RBAC)

| Role | Access in prototype |
|---|---|
| User | Dashboard, courses, classes, materials (enrolled), tests, progress, AI, subscriptions, certificates, support |
| Mentor | Teaching scope + attendance marking + mentor dashboard |
| Admin | Users, courses CRUD, payments overview, support, audit |
| Super Admin | Same as Admin in this phase (highest privilege path reserved) |

## Module → API → UI mapping

| Module | API | Frontend route |
|---|---|---|
| Auth / OTP / demo login | `/api/auth/*` | `/login` |
| Courses & enrollment | `/api/courses` | `/app/courses` |
| Classes & Meet access | `/api/classes` | `/app/classes` |
| Materials (protected) | `/api/materials` | Course detail |
| Assessments | `/api/assessments` | `/app/assessments` |
| Progress & attendance | `/api/progress` | `/app/progress`, `/app/mentor` |
| Subscriptions & payments | `/api/subscriptions`, `/api/payments` | `/app/subscriptions` |
| AI learning | `/api/ai` | `/app/ai` |
| Certificates | `/api/certificates` | `/app/certificates` |
| Notifications | `/api/notifications` | `/app/notifications` |
| Announcements | `/api/announcements` | Dashboard |
| Support | `/api/support` | `/app/support` |
| Dashboards | `/api/dashboard/*` | `/app` |
| Users / Audit | `/api/users`, `/api/audit` | `/app/users`, `/app/audit` |
| Health | `/health` | — |

## Security boundaries (prototype)

- JWT bearer tokens issued after OTP or **explicitly labelled demo-login**.
- Demo auth is **not** production-secure.
- One active session per user (prior session revoked on new login).
- Enrollment checks gate materials and meeting access.
- Secrets belong in env / Key Vault later — demo JWT key is local-only.

## Phased remaining work

1. MongoDB persistence + indexes  
2. Real PayPal webhooks & entitlement state machine  
3. Azure Blob private containers + SAS issuance  
4. Live LLM / speech providers with quotas  
5. React Native client (document requirement)  
6. Hostinger deployment, CI/CD, monitoring, Key Vault  
