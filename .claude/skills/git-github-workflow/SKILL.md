---
name: git-github-workflow
description: Git workflow and Conventional Commits conventions for DrugstoreSystem. Open before any git operation.
---

# Git Workflow — DrugstoreSystem

---

## 1. Branch Strategy

- **`main`** — stable, always buildable. All sprint work eventually merges here.
- **`feat/dev-XX-slug`** — one branch per sprint (optional for early sprints; use when sprint > 3 commits)
- Never push directly to `main` with force-push.

---

## 2. Conventional Commits

Format: `<type>(<scope>): <short description> (DEV-XX)`

### Types

| Type | When to use |
|---|---|
| `feat` | New feature or capability |
| `fix` | Bug fix |
| `test` | Adding or updating tests |
| `refactor` | Internal restructure, no behavior change |
| `docs` | Documentation files only |
| `chore` | Build config, .gitignore, package updates |
| `style` | Formatting, no logic change |

### Scopes for this project

| Scope | What it covers |
|---|---|
| `domain` | Domain entities and enums |
| `db` | EF Core context, migrations, configurations |
| `auth` | Authentication, identity, login/logout |
| `admin` | Admin pages and pharmacy management |
| `pharmacist` | Pharmacist pages, profile, inventory |
| `catalog` | Shared medicine catalog, autocomplete |
| `search` | Search service, SearchRepository, pg_trgm |
| `haversine` | HaversineCalculator, PharmacyRanker |
| `public` | Public-facing pages (search page, detail pages) |
| `seed` | DatabaseSeeder and fixture data |
| `web` | Program.cs, layout, nav, shared components |

### Examples

```
chore: bootstrap DrugstoreSystem solution (DEV-00)
feat(domain): add core entities and enums (DEV-01)
feat(db): add initial migration with pg_trgm indexes (DEV-02)
feat(auth): add login/logout and seeded admin account (DEV-03)
feat(admin): pharmacy CRUD and pharmacist account creation (DEV-04)
feat(pharmacist): profile edit and inventory management (DEV-05)
feat(catalog): shared medicine catalog with autocomplete (DEV-06)
feat(search): 5-stage fuzzy medicine search (DEV-07)
feat(haversine): distance ranking and sort mode toggle (DEV-08)
test(haversine): unit tests for all Haversine cases (DEV-08)
feat(seed): add demo pharmacies and medicines (DEV-09)
fix(search): handle null generic_name in similarity query (DEV-10)
```

---

## 3. Sprint Tagging

After each sprint is marked DONE:

```bash
git tag DEV-XX -m "Sprint DEV-XX complete"
git push origin DEV-XX
```

Final demo tag:
```bash
git tag v1.0-defense -m "Defense-ready version"
git push origin v1.0-defense
```

---

## 4. Never Do

- `git push --force` on `main`
- `git commit --amend` on pushed commits
- Commit `appsettings.Development.json` with real passwords
- Commit anything in `logs/`, `bin/`, `obj/`, `.vs/`
- Skip `--no-verify` on hooks

---

## 5. Useful Commands

```bash
# Check what will be committed
git status
git diff --staged

# Sprint start
git checkout -b feat/dev-XX-slug

# Clean build before committing
dotnet build && dotnet test

# Merge sprint branch to main
git checkout main
git merge --no-ff feat/dev-XX-slug -m "chore: merge DEV-XX to main"

# View log
git log --oneline --graph -10
```
