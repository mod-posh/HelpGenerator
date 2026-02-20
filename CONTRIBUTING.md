# Contributing to HelpGenerator

## Introduction

Thanks for your interest in contributing to HelpGenerator.

This project is intentionally narrow in scope: it generates and validates PowerShell help artifacts (comment-based help, MAML, HelpInfo.xml, Markdown, and updatable-help packaging structure). Everything else is out of scope.

Before contributing, please read the Architecture Decision Records (ADRs). They are the contract for this project.

- Architecture overview: [`docs/adr/README.md`](docs/adr/README.md)
- ADRs: `docs/adr/ADR-001.md` through `docs/adr/ADR-014.md`

If an implementation idea conflicts with an ADR, either:

1. The implementation changes, or
2. The ADR is formally updated first.

Silent drift is not allowed.

---

## Governance Policy

This repository enforces governance structurally.

See: **ADR-014 — Governance and Branch Protection Policy**

Key principles:

- `main` is protected and always releasable
- All changes must go through pull requests
- Code Owner review is required
- Required checks must pass
- Releases are milestone-driven
- Closing a milestone triggers a release
- A release will fail if milestone title does not match project version

---

## How to Contribute

There are several ways to contribute:

- **Reporting bugs**: Open an issue describing expected vs actual behavior and include repro steps and sample inputs if possible.
- **Suggesting enhancements**: Open an issue describing the goal and which ADR(s) the change aligns with.
- **Documentation improvements**: Updates to Quickstart, README, ADRs, and usage docs are always welcome.
- **Code contributions**: Submit changes through pull requests (PRs). See workflow below.

---

## Workflow: Milestones, Projects, and Issues

Work is organized around **milestones**.

- Each milestone is associated with a GitHub Project board.
- Milestone work is tracked as GitHub issues.
- Issues should be labeled:
  - by type (e.g., `bug`, `documentation`, `enhancement`)
  - by ADR alignment (`adr-001` … `adr-014`)
  - optionally by component (`parser:*`, `generator:*`, `packaging:cab`, `tests:golden`, `ci-cd`, etc.)

### When to create a new issue during coding

Create a new issue when:

- a change introduces a new requirement not covered in the original issue scope, or
- fixing it is non-trivial and should be tracked independently, or
- it represents a regression or a contract change.

Small fixes discovered while implementing an issue can stay within the same issue.

---

## Branching Model

`main` is protected and must remain releasable.

We use:

- `milestone/<version>` branches for milestone work
- optional short-lived branches for specific issues:
  - `issue/<id>-short-title`

Typical flow:

1. Create a milestone branch from `main`
2. Open PRs into the milestone branch
3. When milestone work is complete, merge milestone branch into `main`
4. Close the milestone (this triggers release)

### Important

Milestones must only be closed after the milestone branch has been merged into `main`.

The release workflow validates that the milestone title matches the project version. If not, the release fails.

---

## Hotfixes

If a bug is discovered in a released version:

1. Branch from `main`: `hotfix/<version-or-description>`
2. Fix + PR into `main`
3. Patch release is created
4. Backport the fix into the active milestone branch if necessary

---

## Pull Request Process

1. Create an issue (unless trivial documentation).
2. Create a branch from the appropriate base.
3. Make changes aligned with ADR contracts.
4. Run tests locally.
5. Commit with issue-closing reference.
6. Open a PR to the correct target branch.

### Commit message conventions

Examples:

- `Fixes #123: Add HelpInfo.xml schema validation`
- `Closes #456 (ADR-007): Deterministic MAML syntax ordering`

---

## Required Checks

Pull requests must pass required CI checks before merging.

At minimum:

- Build
- Tests
- Validation (as implemented)

---

## Release Process

Releases are milestone-driven.

1. Merge milestone branch into `main`
2. Close milestone
3. Release workflow runs
4. Tag is created
5. GitHub Release is generated
6. Docs PR workflow runs
7. Notifications are sent

Publishing to the PowerShell Gallery occurs via the release pipeline once module packaging is implemented.

---

## Coding Standards

- Keep behavior deterministic
- Do not introduce new output shapes without ADR updates
- Prefer small composable changes
- Avoid scope expansion beyond help generation

---

## Community Guidelines

Be respectful and constructive.

---

## Questions

If unsure whether a change fits scope, open an issue labeled `question` and reference the relevant ADR(s).

