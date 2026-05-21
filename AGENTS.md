# AGENTS.md

This file defines guidance for coding agents and contributors working in this repository.

## Scope And Priority

- This file applies to the entire repository.
- If a future subdirectory-level AGENTS.md is added, the closer file overrides this file for that subtree.
- Human maintainer instructions in issues/PR comments take precedence over this document.

## Repository Overview

- Solution: `Nogic.WritableOptions.slnx`
- Main library: `src/Nogic.WritableOptions`
- Tests: `test/Nogic.WritableOptions.Tests`
- Sandboxes/examples: `sandbox/*`
- Target SDKs in this repo include .NET 8 and .NET 6.

## Source Of Truth For Rules

Before making changes, review:

1. `CONTRIBUTING.md`
2. `.editorconfig`
3. Existing patterns in `src/` and `test/`

Use existing style and APIs unless a requested change requires otherwise.

## Development Workflow

Use these commands from repository root:

```bash
dotnet restore
dotnet format
dotnet build
dotnet test
```

Agent expectations:

1. Make the smallest change that solves the requested problem.
2. Add or update tests when behavior changes.
3. Do not include unrelated refactors.
4. Keep commits logically scoped when asked to prepare commit-ready work.
5. After code changes, verify the editor shows no new warnings or errors caused by the change.
6. After code changes, run both `dotnet format` and `dotnet test`.

## Coding Rules

- Follow `.editorconfig` and current code style.
- Do not use non-ASCII characters for variable names.
- Keep public API changes explicit and intentional.
- Prefer readability over cleverness.
- Add brief comments only where logic is non-obvious.

## Testing Guidance

- For library changes, run relevant tests in `test/Nogic.WritableOptions.Tests`.
- For broader or cross-cutting changes, run full `dotnet test`.
- If a change touches formatting/style-sensitive areas, run `dotnet format`.

## Pull Request Expectations

- Follow Conventional Commit prefixes described in `CONTRIBUTING.md` (`feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`).
- Keep PR title and description clear.
- Reference related issues when available.

## Agent Behavior Rules

1. Ask clarifying questions when requirements are ambiguous or affect public API.
2. Preserve backward compatibility unless breaking changes are explicitly requested.
3. Destructive changes are prohibited by default. Only perform them when explicitly requested and approved.
4. Do not silently change package targets, versions, or CI behavior.
5. Prefer deterministic, reproducible steps in instructions and scripts.

## Definition Of Done For Agent Changes

A change is considered complete when:

1. Requested behavior is implemented.
2. Relevant tests are added/updated and pass.
3. Formatting/style checks are satisfied.
4. Impact and assumptions are clearly summarized.
