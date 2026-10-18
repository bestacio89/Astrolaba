# Franz Mediator application migration

## Objective

Migrate Astrolaba application use cases to Franz Mediator commands and queries while preserving business rules, authorization boundaries, persistence behavior, concurrency checks, and HTTP contracts.

## Migration sequence

- [ ] Complete the Contracts boundary for remaining draft payloads and application response models without introducing a Contracts-to-Application dependency.
- [x] Move project response DTOs and the TSP import preview DTO into Contracts.
- [x] Dispatch project list/detail queries, create/archive/restore/delete commands, TSP import, and read-only TSP analysis through `IDispatcher`.
- [x] Move draft list/collaboration DTOs into Contracts.
- [x] Dispatch draft list/create/import/delete/co-editor/launch and draft export through Franz Mediator.
- [x] Move the export response model into Contracts and dispatch COPIL/COPROJ exports through Franz Mediator.
- [x] Move the task mutation response model into Contracts and dispatch all ten task status, progress, date, metadata, and consumed-time mutations through Franz Mediator.
- [ ] Migrate remaining draft read/save workflows.
- [x] Dispatch tracking mutations for task perimeter, workshops/deliverables, weekly points, updates, references, billing, registers, expenses, and settings through Franz Mediator.
- [x] Dispatch the tracking read query and current-user context query through Franz Mediator.
- [ ] Review calculation/read-model boundaries and remaining application query surfaces.
- [ ] Migrate remaining calculation, settings, milestones, and other application workflows.
- [x] Add architecture assertions for migrated command/query handlers.
- [x] Add architecture assertions for all migrated task and tracking command handlers.
- [ ] Expand focused unit and API integration coverage for migrated use cases.
- [ ] Run the full backend build, test, coverage, reference-scenario, and Angular CI jobs successfully.

## Design constraints

- Keep business logic in existing application/domain services during the incremental migration; handlers orchestrate use cases rather than duplicate rules.
- Keep HTTP concerns such as multipart validation, stream lifetime, and status/response mapping in the API.
- Preserve SQL Server configuration, optimistic concurrency/version checks, authorization, and existing response semantics.
- Do not enable the Franz transaction pipeline until an explicit, tested adapter to Astrolaba's EF Core transaction/unit-of-work model exists.
- Do not migrate WPF/UI concerns or change unrelated deployment infrastructure.
- Do not mark a workflow complete until its tests and CI checks pass.
