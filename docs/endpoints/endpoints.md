# KinetoPlus Endpoints

## Table of contents

1. [Overview](#overview)
   - [Purpose](#purpose)
   - [Technologies](#technologies)
   - [Conventions](#conventions)
   - [Registration](#registration)
   - [Testing](#testing)
2. [Endpoints](#endpoints)
   - [Endpoint group template](#endpoint-group-template)
   - [Clinic endpoints](#clinic-endpoints)

---

## Overview

This chapter always describes the endpoints as they are in the latest version of the code.

### Purpose

An endpoint is the HTTP entry point of the WebAPI. It reads the request, calls a
[service interface](../services/services.md) and turns the result into an HTTP response.
Endpoints hold no application logic and never use repositories or `KinetoPlusDbContext`
directly.

### Technologies

| Part | Choice | Why |
|---|---|---|
| Endpoints | ASP.NET Core minimal APIs with route groups | The project already uses minimal APIs, and they need no controller classes. |
| Results | `TypedResults` with `Results<...>` return types | The possible responses are part of the handler's signature, so OpenAPI documents them and tests can check them without a server. |
| API reference | OpenAPI with Scalar, in Development only | Lets the endpoints be tried from the browser at `/scalar`. |
| Tests | xUnit with hand-written fake services | Tests the handlers on their own, without a server or a database. |

### Conventions

- Each group of endpoints lives in a static class `XEndpoints` in
  `src/backend/WebAPI/Endpoints`, with a `MapXEndpoints` extension method.
- Routes start with `/api/` and use the plural entity name (`/api/clinics`).
- Handlers are public static methods, end in `Async` and take a `CancellationToken`,
  which is passed on to every service call.
- Endpoints return response records (`XResponse`), never the EF Core models, so the API
  shape does not change when the database does.
- Lookups by id use the `{id:guid}` route constraint and return 404 Not Found when nothing
  matches. Lists return 200 OK with an empty list when there is nothing to return.
- Every endpoint has a name (`WithName`) and a summary (`WithSummary`) for OpenAPI.

### Registration

Each group is mapped in `Program.cs`:

```csharp
app.MapClinicEndpoints();
```

### Testing

Endpoint tests call the handler methods directly with a fake service written inside the test
file, then check the typed result (for example `Ok<ClinicResponse>` or `NotFound`). The
fake records the arguments it received, so the tests can check that the id and the
`CancellationToken` are passed on.

---

## Endpoints

### Endpoint group template

Rules:
- Each endpoint group gets one entry in this chapter and a line in the table of contents,
  added in the same branch as the endpoints.
- When an endpoint changes, its entry is updated in the same branch.

Every entry has this structure:

#### X endpoints

- **Author:**
- **Issue / branch:**
- **Uses:** The services it depends on.
- **Endpoints file:** Link to the endpoints class in the repository.
- **Response file:** Link to the response records in the repository.
- **Tests file:** Link to the tests in the repository.

Endpoints:

| Method | Route | Responses | Description |
|---|---|---|---|
| — | — | — | — |

### Clinic endpoints

- **Author:** Stefan
- **Issue / branch:** #28 / 28-implement-webapi
- **Uses:** [IClinicService](../../src/backend/WebAPI/Services/IClinicService.cs)
- **Endpoints file:** [ClinicEndpoints.cs](../../src/backend/WebAPI/Endpoints/ClinicEndpoints.cs)
- **Response file:** [ClinicResponse.cs](../../src/backend/WebAPI/Endpoints/ClinicResponse.cs)
- **Tests file:** [ClinicEndpointsTests.cs](../../tests/backend/WebAPI.Tests/Endpoints/ClinicEndpointsTests.cs)

Endpoints:

| Method | Route | Responses | Description |
|---|---|---|---|
| GET | `/api/clinics` | 200 OK with a list of `ClinicResponse` | Every clinic, ordered by name. Empty if there are none. |
| GET | `/api/clinics/{id}` | 200 OK with a `ClinicResponse`, or 404 Not Found | The clinic with the given id. |

`ClinicResponse` fields: `id`, `name`, `address`, `city`, `phoneNumber`, `email`,
`website`, `createdAt` (UTC). The optional fields are `null` when the clinic has no value.
