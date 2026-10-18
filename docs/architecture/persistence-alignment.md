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

The API host follows HeroService's operational bootstrap selectively: structured host logging,
Franz mediator audit/logging pipelines, Franz audit registration, and configurable mediator
resilience are wired at startup. Astrolaba retains its explicit Entra ID/development authentication,
ProblemDetails exception mapping, and ASP.NET middleware order rather than layering
`AddHttpArchitecture` over existing authentication and exception handling without an integration review.
The Franz transaction pipeline remains intentionally disabled until command-level atomicity is proven.

Astrolaba intentionally retains one exceptional physical-delete workflow for projects
created by mistake. It must remain explicit, authorization-protected, transactionally
atomic, dependency ordered, and tested against SQL Server. It must not become a second
general-purpose delete lifecycle alongside Franz.

## Application boundary

Application no longer references the EF Core runtime and no longer exposes the legacy
`IContexteDonnees`/DbSet abstraction. EF query composition and entity-state operations are
implemented in Infrastructure behind focused ports.

The ports isolate project-identity lookup (`IRechercheIdentiteProjet`), authorization/profile
persistence (`IAccesRepository`), journal appends (`IJournalWriter`), project calculation input
loading (`IChargeurDonneesProjet`), project list/detail/deletion persistence (`IProjetsPersistence`),
draft workflows (`IBrouillonsPersistence`), tracking reads (`ISuiviPersistence`), and the remaining
tracking writes (`ISaisieDonneesPersistence`). Other cohesive workflows use ports for exports,
workshops/deliverables, expenses, invoicing, weekly tracking, planning references, registers,
and project settings.

Application retains authorization, calculations, domain decisions, and workflow orchestration.
The tracking persistence port returns domain entities and exposes named query/write operations;
it does not leak EF Core types, DbSet, IQueryable, or generic table access.

Commit ownership is not yet fully consolidated. Tracking writes currently save through the
focused persistence port, while some multi-step workflows retain explicit commit behavior.
Review each command's transaction and journal semantics before removing those explicit commits
or enabling the Franz transaction pipeline. A successful SaveChanges operation alone does not
prove atomicity across a multi-step workflow.

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

## Code-first schema rollout

Astrolaba is being promoted from a proof of concept into the real application. The existing
POC database is intentionally disposable: no legacy baseline or in-place upgrade path is
required.

The repository now has a generated initial migration named
`20261018220546_InitialCreate`. It creates the full current schema; it is not an incremental
audit-column migration. Do not generate a second baseline or carry the POC database's
migration history forward. Review the generated migration and snapshot whenever the entity
model changes, and regenerate the initial migration from the final model if the model review
finds a mismatch.

The development initializer uses migrations as the only supported schema-creation path.
It fails startup if there are no migrations or if the EF model has unrepresented changes;
it does not fall back to `EnsureCreatedAsync`. The API invokes this initializer only in the
Development environment. Production schema creation and updates remain deployment operations.

The existing POC database may be reset only in local/development after confirming that no
data must be retained. Never drop a shared or production database as part of startup.

## Verification

Architecture tests now enforce that Application does not reference EF Core runtime assemblies
and that tracking writes depend on the focused persistence port. Model tests enforce
`IEntity<Guid>` inheritance, Franz audit properties and query filters, generic repository
registration, generated Guid identities, concurrency tokens, restricted foreign keys,
decimal precision, and project ownership.

SQL Server integration tests are configured in CI against a fresh database migrated from
`InitialCreate`. They verify audit actor/timestamps, soft-delete filtering, stale-write
concurrency conflicts, explicit transaction rollback, and permanent project deletion with
dependent rows. SQLite tests remain useful for fast persistence checks but do not substitute
for SQL Server DDL and locking validation.

The Franz transaction pipeline remains disabled until command-level atomicity is established.
Tracking commands currently persist through their focused port before recalculating the
tracking response; this ordering must remain covered because the response includes persisted
changes and the journal write must remain in the same save operation.

## Out of scope

- Changing the database provider without an explicit design decision.
- Weakening audit or soft-delete behavior to make migration easier.
- Enabling a transaction pipeline without tests that prove its transaction semantics.
- Changing business rules, calculations, authorization, or TSP payload behavior.
