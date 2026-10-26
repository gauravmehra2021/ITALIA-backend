# SS Europa API Documentation

Base URL (local): `http://localhost:5057`  
Swagger UI: `http://localhost:5057/swagger`  
Health: `GET /health`

All JSON responses use:

```json
{
  "success": true,
  "message": "optional",
  "data": {},
  "errors": null
}
```

Authorization: `Authorization: Bearer <accessToken>`

> In-memory data resets when the API process restarts.

## Auth

### Demo login (demonstration only)

`POST /api/auth/demo-login`

```json
{ "email": "student@sseuropa.demo", "deviceId": "browser-1", "deviceName": "Chrome" }
```

Example `data`:

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "expiresAtUtc": "2026-10-10T12:00:00Z",
  "user": {
    "id": "44444444-4444-4444-4444-444444444444",
    "email": "student@sseuropa.demo",
    "fullName": "Aarav Singh",
    "role": "User"
  },
  "requiresOtp": false,
  "isDemonstrationOnly": true
}
```

### Password login → OTP

`POST /api/auth/login` → returns `requiresOtp: true` and `demoOtpHint`  
`POST /api/auth/verify-otp` with `{ userId, code, deviceId, deviceName }`

Seeded password for all demo users: `Demo@12345`

| Email | Role |
|---|---|
| student@sseuropa.demo | User |
| mentor@sseuropa.demo | Mentor |
| admin@sseuropa.demo | Admin |
| superadmin@sseuropa.demo | SuperAdmin |
| priya@sseuropa.demo | User |

## Courses

- `GET /api/courses?page=1&pageSize=10&search=Italian&sortBy=title`
- `GET /api/courses/{id}`
- `POST /api/courses` (Admin)
- `PUT /api/courses/{id}` (Admin)
- `DELETE /api/courses/{id}` (Admin)
- `POST /api/courses/{id}/enroll`

## Classes

- `GET /api/classes`
- `POST /api/classes`
- `GET /api/classes/{id}/meeting` — enrollment/role gated

## Materials / Assessments / Progress

- `GET /api/materials/course/{courseId}`
- `GET /api/assessments`
- `GET /api/assessments/{id}`
- `POST /api/assessments/{id}/attempts` body: `{ "selectedAnswers": [1,2,2] }`
- `GET /api/progress/mine`
- `POST /api/progress/attendance` body: `{ classSessionId, userId, present }`

## Subscriptions / Payments / Certificates / AI

- `GET|POST /api/subscriptions` — mock PayPal
- `GET /api/payments`
- `GET /api/certificates`
- `GET /api/certificates/verify/{code}`
- `POST /api/ai` body: `{ "mode": "tutor", "prompt": "..." }`

## Comms / Admin

- `GET /api/notifications`, `POST /api/notifications/{id}/read`
- `GET /api/announcements`
- `GET|POST /api/support`
- `GET /api/dashboard/student|mentor|admin`
- `GET /api/users`
- `GET /api/audit`
