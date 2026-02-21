# 0.0.0

Milestone 0 is the “set the table” milestone for HelpGenerator.

Goal: establish repository structure, ADR governance, contributor workflow, and CI/release automation so implementation work starts with clear contracts and low stress.

**In scope**

* Repository scaffolding (src/tests/docs/examples layout)
* ADRs (ADR-001..ADR-013) committed + indexed
* Quickstart + README + CONTRIBUTING aligned to ADRs
* Branching model documented (milestone branches, hotfixes)
* GitHub Actions:

  * PR tests (main + milestone/**)
  * main build/pack preview artifact (no publishing)
  * milestone-close release pipeline (lean build/test/pack + release notes)
  * docs PR workflow triggered after release
  * notifications workflow triggered after release
* CODEOWNERS file
* Protect main branch + required checks configured
* Normalize repo variables: DOTNET_VERSION=9.0.x, PROJECT_NAME=HelpGenerator

**Out of scope**

* Implementing parsers/generators/packaging logic
* Publishing to PowerShell Gallery (will be added once module packaging is ready)
* Generating docs from HelpGenerator (docs PR workflow may be stubbed)

**Exit criteria**

* Main is protected, milestone branching works, PR checks run correctly
* Release pipeline works end-to-end on milestone close without manual patching
* Docs PR workflow can open a PR (even if no-op/stubbed)
* ADRs are treated as binding contracts

## ADR-011, CI-CD

* issue-10: Add Docs PR workflow triggered after release

## DOCUMENTATION, ADR-004, ADR-013

* issue-3: Quickstart documentation added

## CI-CD

* issue-9: Update release workflow for milestone-close release (lean release)
* issue-8: Update build workflow to pack preview artifacts only (no publish)

## DOCUMENTATION, ADR-001, CI-CD

* issue-12: Add CODEOWNERS file and enable Code Owner review enforcement

## ENHANCEMENT, ADR-004, ADR-013

* issue-1: Repository skeleton + solution scaffolding

## DOCUMENTATION, ADR-011

* issue-11: Normalize repo Action variables

## DOCUMENTATION, CI-CD

* issue-6: Protect `main` branch and document governance

## ADR-001, CI-CD

* issue-13: Configure Branch Protection Policy for `main`

## DOCUMENTATION, ADR-001, ADR-002, ADR-003, ADR-005, ADR-006, ADR-007, ADR-008, ADR-009, ADR-010, ADR-011, ADR-012, ADR-013

* issue-2: ADRs committed and indexed

## DOCUMENTATION, ADR-001, ADR-013

* issue-5: CONTRIBUTING updated for milestone workflow + ADR discipline

## ADR-013, CI-CD

* issue-7: Update PR test workflow to include milestone branches

## DOCUMENTATION, ADR-001, ADR-004

* issue-4: Root README updated for HelpGenerator

