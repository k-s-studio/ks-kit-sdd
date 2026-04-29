---
name: ef-migrations-of-fubon-epod
description: 'FubonEPod-specific EF Core migration workflow and troubleshooting. Use when adding migrations, running dotnet ef with FubonEPodDbContextFactory, debugging PendingModelChangesWarning, or validating staging migration execution.'
user-invocable: true
---

# FubonEPod EF Migration

## When to Use

- You need to add or remove an EF Core migration in this repository.
- `dotnet ef` behavior in this repo is unclear because `Api.csproj` does not exist.
- Local `database update` fails and you need the canonical troubleshooting path.
- You need the repo-specific CI / staging migration flow for `db_migrator`.

## Canonical Projects and Files

- DbContext: `Database/DbModels/FubonEPodDbContext.cs`
- Migrations project: `Database/SqlServerMigrations/Database.SqlServerMigrations.csproj`
- Design-time factory: `Database/SqlServerMigrations/FubonEPodDbContextFactory.cs`
- Migrator runtime app: `Database/DbMigrator/Program.cs`
- Migration compose layer: `Docker/docker-compose.migrate.yml`

The current design-time factory behavior is:

- first read `ConnectionStrings__SQLServer` from environment variables
- if it is missing, fall back to `MSSQLLocalDB` for local validation

## Local EF Commands

This repository uses `FubonEPodDbContextFactory`, so local `dotnet ef` commands do not require a separate startup project.

1. Create a migration:

```powershell
dotnet ef migrations add <Name> --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj -o Migrations
```

2. Remove the latest migration:

```powershell
dotnet ef migrations remove --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
```

3. Apply migrations locally:

```powershell
dotnet ef database update --context FubonEPodDbContext -p Database/SqlServerMigrations/Database.SqlServerMigrations.csproj
```

## Local Validation Checklist

1. Build first if model changes touched multiple projects.
2. Run `dotnet ef database update` with the canonical command above.
3. Verify the target migration actually applied and required seed data exists.
4. Only mark migration validation complete after `database update` succeeds on a clean local run.

## Known Local Failure: PendingModelChangesWarning

If `dotnet ef database update` fails with `PendingModelChangesWarning`, the current model and the committed migration snapshot are out of sync.

Treat this as a real blocker, not a warning to ignore.

Fix path:

1. Inspect the current model changes in `Database/DbModels/**`.
2. Generate a new migration so the snapshot matches the model.
3. Review the generated migration for unintended schema drift.
4. Re-run `dotnet ef database update`.

Do not mark deployment validation done while this warning is still present.

## Scope Boundary

This skill is for EF migration execution, validation, and troubleshooting only.

If you need RBAC semantics, role-permission design, role assignment API behavior, or JWT claims contract details, use `openspec/changes/add-sso-rbac/design.md` as the authoritative design document instead of extending this skill.

## CI / Staging Migration Flow

The staging migration pipeline uses the `db_migrator` image and the migrate compose layer.

1. CI syncs `Docker/` files to the deployment host.
2. CI runs `./switch-env.sh staging migrate` inside `Docker/`.
3. CI builds and pushes `db_migrator`.
4. The deployment host runs `docker compose pull db_migrator`.
5. The deployment host runs `docker compose up --abort-on-container-exit --exit-code-from db_migrator`.

If the migration container exits non-zero, treat the schema rollout as failed and stop the application deployment.