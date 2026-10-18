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

The current migration branch now references Franz.Common.Business 2.3.0 directly from Domain and Application. Astrolaba's Entite derives from Franz Entity<Guid>, so every persisted domain entity implements IEntity<Guid> and inherits framework auditing/soft-delete metadata while retaining the Astrolaba Version concurrency token. Infrastructure registers DbContextBase, AddUnitOfWork<PilotageDbContext>(), AddEntityRepositories<PilotageDbContext>(), and Franz's command PersistenceBehavior. A CurrentUserService adapter supplies the authenticated external identity for audit fields. The application uses Franz's IEntityRepository<Projet, Guid> for the archive/restore tracked lookup and commits through the shared unit of work.

Astrolaba currently exposes IContexteDonnees with EF Core DbSet<T> properties.
Application services use EF query composition directly, so replacing the context
with repositories is a behavioral migration, not a mechanical rename.

## Incremental sequence

1. **Lock down persistence invariants.** Add model-level tests for optimistic
   concurrency tokens, no cascade deletes, decimal precision, and project ownership.
2. **Introduce the Franz persistence foundation.** Done: pin Franz.Common.EntityFramework 2.3.0, derive PilotageDbContext from DbContextBase, register the unit of work and command persistence behavior, and preserve Astrolaba's Version concurrency token, SQL Server provider, retry policy, and restricted foreign keys. The persistence behavior saves after successful commands; it does not provide a transaction around multi-step writes. Those transaction boundaries still require integration coverage.
3. **Adopt the entity contract and repositories.** Implemented on this branch: all persisted entities inherit Franz Entity<Guid>; generic repositories are registered by Franz's discovery extension; archive/restore uses Franz IEntityRepository and the explicit unit of work. Other application workflows still use IContexteDonnees and must be migrated incrementally. Keep specialized query shapes in focused repositories rather than forcing every query through GetAllAsync.
5. **Migrate application services incrementally.** Move one cohesive workflow at a
   time, retaining authorization checks, concurrency behavior, calculations, and
   transaction boundaries. Keep IContexteDonnees only as a temporary compatibility
   seam until no callers remain.
6. **Strengthen verification and schema rollout.** Model tests now enforce IEntity inheritance, auditing/filter metadata, repository registration, generated Guid identity, concurrency tokens, restricted deletes, decimal precision, and project ownership. A database migration is a mandatory release gate because Entity<Guid> adds audit columns and soft-delete filters. There are currently no checked-in EF migrations; generate and review the initial migration with the repository's EF tooling before deploying this model to any existing database. Add SQL Server-backed concurrency, soft-delete, hard-delete, and transaction integration tests.
7. **Remove the legacy seam.** Once all services are migrated, remove direct EF Core
   access from Application and delete IContexteDonnees if it is no longer needed.

## Important persistence semantics

- Franz's DbContextBase soft-deletes entities implementing IEntity<TKey> when EF tracks them as Deleted, and its global query filters hide IsDeleted entities. The application must use this for ordinary deletes.
- Astrolaba retains one deliberate permanent-delete workflow: removing a project created by mistake, after checking that no operational activity exists. PilotageDbContext handles that explicit operation through dependency-ordered SQL deletes inside the configured execution strategy, bypassing the soft-delete path. The shared unit of work still invokes this behavior at SaveChangesAsync.
- Franz's generic EntityRepository saves immediately in its CRUD methods. Multi-step application workflows must not call those mutating methods midway through a unit of work; use tracked entities plus the explicit IUnitOfWork commit or a purpose-built repository method that respects the workflow's transaction boundary.
- The mediator PersistenceBehavior is enabled and saves after successful commands. Existing services still contain explicit SaveChangesAsync calls, so the pipeline can perform a second no-op save. Consolidating writes at the command boundary is follow-up work and must not be conflated with transaction orchestration.
- Adding the inherited audit properties changes the EF model. Since no EF migration is checked in yet, this branch is not deployable to an existing database until a reviewed migration is generated and applied.

## Out of scope for this phase

- Changing the database provider or schema without an explicit migration.
- Enabling the Franz transaction pipeline before transaction behavior is tested.
- Changing domain rules, calculations, authorization, or TSP payload behavior.
- Deployment infrastructure changes.
