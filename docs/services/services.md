# KinetoPlus Services

## Table of contents

1. [Overview](#overview)
   - [Purpose](#purpose)
   - [Technologies](#technologies)
   - [Conventions](#conventions)
   - [Registration](#registration)
   - [Testing](#testing)
2. [Services](#services)
   - [Service template](#service-template)
   - [ClinicService](#clinicservice)

---

## Overview

This chapter always describes the services as they are in the latest version of the code.

### Purpose

A service holds the application logic of the WebAPI. Endpoints call a service interface,
and the service reads and writes data through the
[repository interfaces](../repositories/repositories.md), never through
`KinetoPlusDbContext` directly. This keeps the endpoints thin, keeps the queries in the
repositories, and gives logic such as searching, filtering or validation one place to live.

### Technologies

| Part | Choice | Why |
|---|---|---|
| Data access | Repository interfaces (`IXRepository`) | The service does not know how or where the data is stored. |
| Dependency injection | ASP.NET Core built-in container | Gives each request its own service, repository and DbContext. |
| Tests | xUnit with hand-written fake repositories | Tests the service on its own, without a database or a mocking library. |

### Conventions

- Every service has an interface (`IXService`) and an implementation (`XService`), both in
  `src/backend/WebAPI/Services`.
- A service depends only on interfaces, received through its constructor.
- Methods are async, end in `Async` and accept an optional `CancellationToken`, which is
  passed on to every repository call.
- Method names do not repeat the entity name (`GetByIdAsync`, not `GetClinicAsync`).
- Lookups by id return `null` when nothing matches. Lists are returned as
  `IReadOnlyList<T>` and are empty, never `null`, when there is nothing to return.
- Every interface method has XML doc comments, and every implementation comes with tests.

### Registration

Services are registered in `Program.cs` as scoped services, the same lifetime as the
repositories they use, so there is one instance per request:

```csharp
builder.Services.AddScoped<IClinicService, ClinicService>();
```

### Testing

Service tests replace the repository with a fake: a small in-memory implementation of the
repository interface, written inside the test file. The fake also records the arguments it
received (such as the id and the `CancellationToken`), so the tests can check that the
service passes them on.

A fake is used instead of a mock because the repository interfaces are small, and the tests
check what the service returns rather than which calls it made. Querying, ordering and
tracking are already tested against a real database in the repository tests, so the fakes
do not repeat that behaviour.

---

## Services

### Service template

Rules:
- Each service gets one entry in this chapter and a line in the table of contents,
  added in the same branch as the service.
- When a service changes, its entry is updated in the same branch.

Every entry has this structure:

#### XService

- **Author:**
- **Issue / branch:**
- **Uses:** The repositories or other services it depends on.
- **Interface file:** Link to the interface in the repository.
- **Implementation file:** Link to the implementation in the repository.
- **Tests file:** Link to the tests in the repository.

Methods:

| Method | Returns | Description |
|---|---|---|
| — | — | — |

### ClinicService

- **Author:** Stefan
- **Issue / branch:** #14 / 14-implement-service
- **Uses:** [IClinicRepository](../../src/backend/WebAPI/Repositories/IClinicRepository.cs)
- **Interface file:** [IClinicService.cs](../../src/backend/WebAPI/Services/IClinicService.cs)
- **Implementation file:** [ClinicService.cs](../../src/backend/WebAPI/Services/ClinicService.cs)
- **Tests file:** [ClinicServiceTests.cs](../../tests/backend/WebAPI.Tests/Services/ClinicServiceTests.cs)

Methods:

| Method | Returns | Description |
|---|---|---|
| `GetByIdAsync(Guid id, CancellationToken cancellationToken = default)` | `Task<Clinic?>` | The clinic with the given id, or `null` if there is none. |
| `GetAllAsync(CancellationToken cancellationToken = default)` | `Task<IReadOnlyList<Clinic>>` | Every clinic, in the order the repository returns them (by name). Empty if there are none. |
