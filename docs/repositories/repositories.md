# KinetoPlus Repositories

## Table of contents

1. [Overview](#overview)
   - [Purpose](#purpose)
   - [Technologies](#technologies)
   - [Conventions](#conventions)
   - [Registration](#registration)
2. [Repositories](#repositories)
   - [Repository template](#repository-template)
   - [ClinicRepository](#clinicrepository)

---

## Overview

This chapter always describes the repositories as they are in the latest version of the code.

### Purpose

A repository is the only place where the WebAPI reads from or writes to the database.
Endpoints and services depend on the repository interface, never on
`KinetoPlusDbContext` directly. This keeps the queries in one place and lets the rest
of the code be tested without a database.

### Technologies

| Part | Choice | Why |
|---|---|---|
| Data access | Entity Framework Core through `KinetoPlusDbContext` | Same model and database as described in the [database documentation](../database/database.md). |
| Dependency injection | ASP.NET Core built-in container | Gives each request its own repository and DbContext. |
| Tests | xUnit with an in-memory SQLite database | Runs the real EF Core queries without needing SQL Server. |

### Conventions

- Every repository has an interface (`IXRepository`) and an EF Core implementation
  (`XRepository`), both in `src/backend/WebAPI/Repositories`.
- Methods are async, end in `Async` and accept an optional `CancellationToken`, so a
  query stops when the client disconnects.
- Method names do not repeat the entity name (`GetByIdAsync`, not `GetClinicAsync`).
- Lookups by id return `null` when nothing matches. Lists are returned as
  `IReadOnlyList<T>` and are empty, never `null`, when there are no rows.
- Read-only queries use `AsNoTracking()`, since the loaded entities are not changed.
- Lists are always ordered, because the database does not guarantee an order otherwise.
- Every interface method has XML doc comments, and every implementation comes with tests.

### Registration

Repositories are registered in `Program.cs` as scoped services, the same lifetime as
the DbContext, so there is one instance per request:

```csharp
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
```

---

## Repositories

### Repository template

Rules:
- Each repository gets one entry in this chapter and a line in the table of contents,
  added in the same branch as the repository.
- When a repository changes, its entry is updated in the same branch.

Every entry has this structure:

#### XRepository

- **Author:**
- **Issue / branch:**
- **Entity:** The model the repository works with.
- **Interface file:** Link to the interface in the repository.
- **Implementation file:** Link to the implementation in the repository.
- **Tests file:** Link to the tests in the repository.

Methods:

| Method | Returns | Description |
|---|---|---|
| — | — | — |

### ClinicRepository

- **Author:** Stefan
- **Issue / branch:** #20 / 20-write-interface, #21 / 21-write-implementation
- **Entity:** [Clinic](../../src/backend/WebAPI/Models/Clinic.cs)
- **Interface file:** [IClinicRepository.cs](../../src/backend/WebAPI/Repositories/IClinicRepository.cs)
- **Implementation file:** [ClinicRepository.cs](../../src/backend/WebAPI/Repositories/ClinicRepository.cs)
- **Tests file:** [ClinicRepositoryTests.cs](../../tests/backend/WebAPI.Tests/Repositories/ClinicRepositoryTests.cs)

Methods:

| Method | Returns | Description |
|---|---|---|
| `GetByIdAsync(Guid id, CancellationToken cancellationToken = default)` | `Task<Clinic?>` | The clinic with the given id, or `null` if there is none. Not tracked. |
| `GetAllAsync(CancellationToken cancellationToken = default)` | `Task<IReadOnlyList<Clinic>>` | Every clinic, ordered by name. Empty if there are none. Not tracked. |
