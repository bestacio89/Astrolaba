# Infrastructure and persistence alignment

## Objective

Align Astrolaba's persistence boundary with the patterns in
[HeroService / Develop_Unity_V1](https://github.com/bestacio89/HeroService/tree/Develop_Unity_V1)
while preserving Astrolaba's SQL Server schema, optimistic concurrency, restricted
deletes, project integrity, authorization, calculations, and TSP import/export behavior.

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

Astrolaba intentionally retains one exceptional physical-delete workflow for projects
created by mistake. It must remain explicit, authorization-protected, transactionally
atomic, dependency ordered, and tested against SQL Server. It must not become a second
general-purpose delete lifecycle alongside Franz.

## Application boundary still requires migration

`IContexteDonnees` still exposes EF Core `DbSet<T>` from Application, and application
services still compose EF queries directly. This is not a clean persistence boundary.
Focused ports now isolate project-identity lookup (`IRechercheIdentiteProjet`),
authorization/profile persistence (`IAccesRepository`), and journal appends
(`IJournalWriter`). Journal entries are added to the current unit of work without an
early commit so they commit or roll back with the business mutation they describe.
Project calculation inputs are loaded through `IChargeurDonneesProjet`; the calculation
builder and business transformation remain in Application. Continue replacing the
context with use-case-specific ports implemented in Infrastructure using EF Core and
Franz repositories. Do not replace the context with a generic `IQueryable<T>`
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

## Migration rollout: mandatory release gate

There are currently **no checked-in EF migrations**. The previously generated
`AddFranzAuditColumns` migration was removed because EF generated a full initial schema
(`CreateTable` operations), not an incremental audit-column migration. Applying that
migration to an existing database would fail when it encounters tables that already exist.
Do not restore or apply that generated migration as-is.

Use a two-stage migration history:

1. From the pre-Franz model on `develop`, generate and review an
   `InitialSchemaBaseline` migration. It represents the legacy schema before the inherited
   Franz audit and soft-delete fields were added. Bring its migration and snapshot files
   onto the feature branch without regenerating it against the new model.
2. On the feature branch, generate `AddFranzAuditColumns` against that legacy snapshot.
   Review every operation, especially non-nullable audit columns on populated tables,
   existing indexes/constraints, and the query-filter lifecycle.
3. For a fresh database, apply both migrations in order.
4. For an existing database, first verify that its schema exactly matches the legacy
   baseline. Record the baseline migration as already applied in `__EFMigrationsHistory`
   (without executing its table-creation operations), then apply only the incremental
   Franz audit migration. Do not stamp the baseline on an unverified or drifted schema.
5. Test both paths: fresh database creation and upgrade of a disposable copy containing
   representative existing rows. Verify that existing business data is preserved, audit
   fields receive valid Franz-compatible values, and ordinary deletes are soft deletes.

Never apply schema changes automatically at application startup. Generate and inspect the
SQL script, test it on a disposable SQL Server database, and treat production application
as a separate deployment operation.

## Verification

Model tests should enforce `IEntity<Guid>` inheritance, Franz audit properties and query
filters, generic repository registration, generated Guid identities, concurrency tokens,
restricted foreign keys, decimal precision, and project ownership.

Provider integration tests must use SQL Server for migration upgrade, concurrent updates,
audit actor/timestamps, soft-delete filtering, permanent project deletion with dependent
rows, failure rollback, and transaction retry behavior. SQLite tests remain useful for
fast domain persistence checks but do not prove SQL Server DDL or locking behavior.

## Out of scope

- Changing the database provider without an explicit design decision.
- Weakening audit or soft-delete behavior to make migration easier.
- Enabling a transaction pipeline without tests that prove its transaction semantics.
- Changing business rules, calculations, authorization, or TSP payload behavior.
