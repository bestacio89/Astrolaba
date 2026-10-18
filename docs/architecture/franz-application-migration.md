# Franz Mediator application migration

## Objective

Migrate Astrolaba application use cases to Franz Mediator commands and queries while preserving the existing business rules, authorization boundaries, persistence behavior, and HTTP contracts.

## Migration sequence

- [ ] Establish stable request/response contracts in `PilotageForfaits.Contracts` without introducing a dependency on Application.
- [ ] Migrate project queries and lifecycle commands (list, detail, create, archive/restore, delete).
- [ ] Migrate draft workflows (list, create, import, read, save, co-editor, delete, export, launch).
- [ ] Migrate TSP import analysis and import workflows.
- [ ] Inventory and migrate the remaining application services, including task tracking, calculations, settings, milestones, and exports.
- [ ] Update API controllers to dispatch through `IDispatcher`; keep transport-specific concerns such as multipart file handling and HTTP response mapping in the API.
- [ ] Add handler/unit tests and API integration tests for each migrated use case.
- [ ] Add architecture tests enforcing the intended Contracts/Application/API dependency direction.
- [ ] Run the full backend build and test suite in CI and address all regressions.

## Design constraints

- Keep business logic in the existing application/domain services during the incremental migration; handlers should orchestrate use cases, not duplicate rules.
- Preserve SQL Server configuration, optimistic concurrency/version checks, authorization, and existing response semantics.
- Do not enable the Franz transaction pipeline until an explicit, tested adapter to Astrolaba's EF Core transaction/unit-of-work model exists.
- Do not migrate WPF/UI concerns or change unrelated deployment infrastructure.
- Do not mark a workflow complete until its tests and CI checks pass.
