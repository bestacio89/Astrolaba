# Infrastructure and persistence alignment

## Objective

Align Astrolaba's persistence boundary with the patterns in
[HeroService / Develop_Unity_V1](https://github.com/bestacio89/HeroService/tree/Develop_Unity_V1)
while preserving Astrolaba's business invariants, optimistic concurrency, restricted
deletes, project integrity, authorization, calculations, and TSP import/export behavior.
The POC database and its schema history are disposable: the target is a clean code-first
schema for the real application, not an in-place upgrade of the POC database.

## Entity model: Franz is the source of truth

All persisted Astrolaba entities derive from `PilotageForfaits.Domain.Entites.Entite`,
which itself derives from `Franz.Common.Business.Domain.Entity<Guid>`. This keeps
Franz's `IEntity<Guid>` identity and audit/lifecycle contract as the only shared
persistence entity model. Astrolaba extends that contract only with its domain-specific
`Version` concurrency token. `EntiteDeProjet` adds the project ownership key; it does
not duplicate Franz identity, audit, or soft-delete fields.

Infrastructure uses `DbContextBase` from `Franz.Common.EntityFramework`, registers
`AddUnitOfWork<PilotageDbContext>()`, `AddEntityRepositories<PilotageDbContext>()`,
and the mediator persistence behavior. The current-user adapter supplies the authenticated
Entra ID object identifier for Franz audit fields. Ordinary deletes must retain Franz's
soft-delete semantics and global query filters.

Database initialization and EF exception translation belong to Infrastructure. The API
may invoke the development initializer as part of host startup, but it must not reference
`PilotageDbContext`, EF Core runtime APIs, or SQL Server provider exceptions. HTTP error
mapping consumes application-level exceptions such as `ConflitException` only.

Astrolaba intentionally retains one exceptional physical-delete workflow for projects
created by mistake. It must remain explicit, authorization-protected, transactionally
atomic, dependency ordered, and tested against SQL Server. It must not become a second
general-purpose delete lifecycle alongside Franz.

## Application boundary still requires migration

`IContexteDonnees` still exposes EF Core `DbSet<T>` from Application, and application
services still compose EF queries directly. This is not a clean persistence boundary.
Focused ports now isolate project-identity lookup (`IRechercheIdentiteProjet`),
authorization/profile persistence (`IAccesRepository`), journal appends
(`IJournalWriter`), project calculation input loading (`IChargeurDonneesProjet`),
project list/detail/deletion persistence (`IProjetsPersistence`), and draft workflows
(`IBrouillonsPersistence`). Journal entries
are added to the current unit of work without an early commit so they commit or roll
back with the business mutation they describe. Project calculation transformations and
workflow authorization remain in Application; EF query composition stays in
Infrastructure. Project creation and archive/restore rely on the mediator persistence
behavior for their command commit. The exceptional permanent-delete workflow retains
an explicit commit so successful physical deletion is confirmed before logging.
Continue replacing the remaining context usages with use-case-specific ports. Do not replace the context with a generic `IQueryable<T>`
abstraction or load entire tables through generic repository methods.

Migrate cohesive workflows incrementally and preserve:
- authorization and project visibility rules;
- query filters for soft-deleted entities;
- optimistic concurrency and the HTTP 409 mapping;
- aggregate invariants and calculations;
- atomic multi-entity writes and rollback behavior;
- explicit physical deletion only for the approved project-removal use case.

Consolidate persistence at one command boundary. Do not mix immediate-save repository
mutations with a multi-step unit of work. Franz's persistence behavior saves after a
successful command; it is not a substitute for a transaction around multiple writes.
Remove explicit service-level saves only after verifying that every caller is dispatched
through the mediator and that the workflow's transaction semantics remain intact.

## Code-first schema rollout: required before release

Astrolaba is being promoted from a proof of concept into the real application. The existing
POC database is intentionally disposable, so there is no requirement to preserve its
schema history or generate a legacy baseline. Do not create a baseline from the old model
and do not carry the POC's migration history forward.

1. Finish the Franz-aligned entity model and Infrastructure mappings first. Review the
   final `PilotageDbContext` model for identity/audit columns, soft-delete filters,
   concurrency tokens, foreign keys, indexes, and decimal precision.
2. From the repository's `backend` directory, generate one clean initial migration from
   the **current feature branch model**:

   ```powershell
   dotnet ef migrations add InitialCreate --project src/PilotageForfaits.Infrastructure --startup-project src/PilotageForfaits.Api --context PilotageDbContext
   ```

3. Review the migration and model snapshot. This initial migration is expected to create
   the full schema; verify it contains only the intended final model and no accidental
   schema artifacts. Do not hand-author generated migration code.
4. Create a fresh, empty development database and apply the migration. Verify startup,
   seed/reference data, CRUD workflows, auditing, soft deletion, concurrency, and the
   exceptional permanent project-delete path.
5. Remove or reset the old POC database only in the local/development environment after
   confirming it contains no data that must be retained. Do not drop shared or production
   databases as part of application startup.
6. Keep schema changes explicit and reviewable. Production schema creation or updates
   must be a deployment operation, not an implicit destructive startup action.

Until the final entity model and mappings are stable, do not generate the initial migration:
otherwise the migration and snapshot will immediately become stale as the refactor continues.

## Verification

Model tests should enforce `IEntity<Guid>` inheritance, Franz audit properties and query
filters, generic repository registration, generated Guid identities, concurrency tokens,
restricted foreign keys, decimal precision, and project ownership.

Provider integration tests must use SQL Server for fresh database creation, concurrent updates,
audit actor/timestamps, soft-delete filtering, permanent project deletion with dependent
rows, failure rollback, and transaction retry behavior. SQLite tests remain useful for
fast domain persistence checks but do not prove SQL Server DDL or locking behavior.

## Out of scope

- Changing the database provider without an explicit design decision.
- Weakening audit or soft-delete behavior to make migration easier.
- Enabling a transaction pipeline without tests that prove its transaction semantics.
- Changing business rules, calculations, authorization, or TSP payload behavior.
