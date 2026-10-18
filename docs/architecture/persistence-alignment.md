# Infrastructure and persistence alignment

## Objective

Align Astrolaba's persistence boundary with the proven patterns in
[HeroService / Develop_Unity_V1](https://github.com/bestacio89/HeroService/tree/Develop_Unity_V1)
without copying assumptions that do not fit Astrolaba's application.

Astrolaba's persistence layer must preserve SQL Server, optimistic concurrency,
restricted deletes, project data integrity, existing authorization, calculations,
and TSP import/export behavior throughout the migration.

## Architectural differences to preserve deliberately

HeroService uses Franz.Common.EntityFramework, DbContextBase,
AddUnitOfWork<TDbContext>(), entity repository registration, and repository
contracts in its Contracts assembly. Astrolaba's API Contracts assembly is
intentionally transport-only and is guarded by architecture tests against
Application and Domain references. Persistence ports must therefore live in an
application-facing abstraction boundary (or a dedicated persistence-contract
assembly), not be added to API Contracts at the cost of reversing that dependency rule.

The first Franz integration slice now uses Franz.Common.EntityFramework 2.3.0: PilotageDbContext inherits DbContextBase, and DI registers AddUnitOfWork<PilotageDbContext>(). Astrolaba's domain Entite hierarchy does not implement Franz's IEntity<TKey> contract. Therefore Franz's IEntity-specific auditing and soft-delete query filters do not currently apply to Astrolaba entities. The generic entity repository registration is not used because it does not provide the query shapes needed by Astrolaba's workflows. Adopting Franz's IEntity semantics requires a separately reviewed domain migration.

Astrolaba currently exposes IContexteDonnees with EF Core DbSet<T> properties.
Application services use EF query composition directly, so replacing the context
with repositories is a behavioral migration, not a mechanical rename.

## Incremental sequence

1. **Lock down persistence invariants.** Add model-level tests for optimistic
   concurrency tokens, no cascade deletes, decimal precision, and project ownership.
2. **Introduce the Franz persistence foundation.** Done in the initial integration slice:
   pin Franz.Common.EntityFramework 2.3.0, derive PilotageDbContext from DbContextBase,
   and register the explicit unit of work. Preserve Astrolaba's SaveChangesAsync versioning,
   length validation, SQL Server provider, and retry policy. Do not enable the mediator
   persistence pipeline until atomicity, rollback, and save behavior are covered by tests.
3. **Define persistence ports.** Started: IProjetRepository lives in Application.Abstractions,
   outside API Contracts, and exposes a tracked project lookup without leaking EF Core.
4. **Implement repositories.** Started with ProjetRepository in Infrastructure. The archive/restore
   workflow now reads through the repository and commits through Franz IUnitOfWork; other project
   and application workflows remain on the compatibility seam. Keep EF-specific includes,
   filtering, and persistence details in Infrastructure; avoid generic repositories that hide
   aggregate-specific queries or force loading entire tables.
5. **Migrate application services incrementally.** Move one cohesive workflow at a
   time, retaining authorization checks, concurrency behavior, calculations, and
   transaction boundaries. Keep IContexteDonnees only as a temporary compatibility
   seam until no callers remain.
6. **Strengthen verification.** Add repository tests and SQL Server-backed integration
   coverage for concurrency conflicts, foreign-key/delete behavior, project removal,
   migrations, and atomic writes. Keep reference-scenario tests unchanged as a
   business-regression gate.
7. **Remove the legacy seam.** Once all services are migrated, remove direct EF Core
   access from Application and delete IContexteDonnees if it is no longer needed.

## Out of scope for this phase

- Changing the database provider or schema without an explicit migration.
- Enabling the Franz transaction pipeline before transaction behavior is tested.
- Changing domain rules, calculations, authorization, or TSP payload behavior.
- Deployment infrastructure changes.
