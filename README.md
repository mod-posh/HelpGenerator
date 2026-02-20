| Latest Version | Gallery | Issues | Testing | License | Discord |
|-----------------|-----------------|----------------|----------------|----------------|----------------|
| [![Latest Version](https://img.shields.io/github/v/tag/mod-posh/HelpGenerator)](https://github.com/mod-posh/HelpGenerator/tags) | [![Nuget.org](https://img.shields.io/nuget/dt/HelpGenerator.Core?label=HelpGenerator.Core)](https://www.nuget.org/packages/HelpGenerator.Core)<br/>[![Nuget.org](https://img.shields.io/nuget/dt/HelpGenerator.Powershell?label=HelpGenerator.Powershell)](https://www.nuget.org/packages/HelpGenerator.Powershell) | [![GitHub issues](https://img.shields.io/github/issues/mod-posh/HelpGenerator)](https://github.com/mod-posh/HelpGenerator/issues) | [![Merge Test Workflow](https://github.com/mod-posh/HelpGenerator/actions/workflows/test.yml/badge.svg)](https://github.com/mod-posh/HelpGenerator/actions/workflows/test.yml) | [![GitHub license](https://img.shields.io/github/license/mod-posh/HelpGenerator)](https://github.com/mod-posh/HelpGenerator/blob/master/LICENSE) | [![Discord Server](https://assets-global.website-files.com/6257adef93867e50d84d30e2/636e0b5493894cf60b300587_full_logo_white_RGB.svg)](https://discord.com/channels/1044305359021555793/1044305781627035811) |
# HelpGenerator

HelpGenerator is a focused tool suite for generating and validating PowerShell help artifacts **quickly and deterministically**.

It is designed to make it easy for developers to produce clean, readable help documentation from:

- Script-based comment help (functions and modules)
- Compiled C# cmdlets (reflection + XML documentation)
- Existing help artifacts (MAML + HelpInfo.xml)

It generates:

- **MAML** external help (command help XML)
- **HelpInfo.xml** and structure required for updatable external help (CAB packaging support)
- **Markdown** documentation (command/module docs derived from the normalized help model)
- Deterministic placeholder scaffolding when help is missing

## Scope

This project has a narrow scope by design:

- Consume: comment-based help, compiled cmdlet metadata + XML docs, MAML, HelpInfo.xml
- Produce: Markdown docs, MAML, HelpInfo.xml, and updatable help packaging structure

If it’s not part of generating or validating PowerShell help artifacts, it is out of scope.

## Non-goals

HelpGenerator is **not**:

- A static site generator
- A documentation hosting platform
- A prose rewriter or “smart” help authoring tool
- A PlatyPS extension or wrapper

## Quickstart

See: [`docs/usage/quickstart.md`](docs/usage/quickstart.md)

## Architecture and Governance

This repository is governed by Architecture Decision Records (ADRs).
ADRs define scope, output contracts, validation rules, and repository governance.

- ADR index: [`docs/adr/README.md`](docs/adr/README.md)

If implementation conflicts with an ADR:

- the implementation changes, or
- the ADR is formally updated first.

Silent drift is not allowed.

## Development workflow

Work is milestone-driven:

- Milestones map to GitHub Projects/boards
- Issues are tied to milestones and ADR labels
- Milestone branches are used for development
- Closing a milestone triggers a release workflow (with guardrails)

See: [`CONTRIBUTING.md`](CONTRIBUTING.md)

## Distribution

HelpGenerator will be published to the **PowerShell Gallery** via the release pipeline once module packaging is implemented.

Repository: <https://github.com/mod-posh/HelpGenerator>

## License

See: [`LICENSE`](LICENSE)
