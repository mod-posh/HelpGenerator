# Branch Protection & Governance Policy

## Protected Branch: `main`

The `main` branch represents the releasable state of HelpGenerator.

Direct pushes to `main` are prohibited.

All changes must:

1. Be associated with a GitHub issue
2. Align with one or more ADRs
3. Be submitted via pull request
4. Pass required status checks
5. Receive Code Owner review

### Required Checks

At minimum, the following must pass before merge:

* Merge Test Workflow (build + test)

Additional checks may be added as the project evolves.

### Code Owner Review

The repository uses a CODEOWNERS file to enforce ownership boundaries. Pull requests modifying owned paths require explicit review before merge.

This enforces governance and prevents architectural drift.

### Milestone Branching Model

Development occurs on milestone branches:

```bash
milestone/<version>
```

Example:

```bash
milestone/0.0.0
```

Milestone branches may receive multiple PRs.

When a milestone is complete:

1. Merge milestone branch into `main`
2. Close the milestone
3. Closing the milestone triggers the release workflow

### Release Guardrail

The release workflow validates that:

* The milestone title matches the project version in `Directory.Build.props`

If they do not match, the release fails.

This prevents accidental releases.

### Hotfix Model

Hotfixes branch from `main`:

```bash
hotfix/<version-or-description>
```

After fix:

* PR into `main`
* Tag new patch version
* Backport fix into active milestone branch if necessary

---

## Why this matters (and why you're doing the right thing)

You’re building:

* Deterministic outputs
* Contract-driven architecture (ADRs)
* Structured milestone releases
* Automated release notes
* Post-release documentation PRs

Branch protection is the enforcement layer that keeps those promises intact.

Without it, all of the above is optional.
With it, the structure becomes durable.
