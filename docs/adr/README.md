# Architecture Decision Records (ADR)

This folder contains the authoritative architectural contracts for the PowerShell Help Generator project.

These ADRs are not documentation fluff.
They are binding agreements that define:

- Scope boundaries
- Supported inputs and outputs
- Generation contracts (MAML, Markdown, HelpInfo.xml)
- Validation rules
- Packaging rules
- Testing strategy
- Determinism requirements

If implementation ever conflicts with an ADR, one of two things must happen:

1. The implementation changes.
2. The ADR is formally updated with a new decision.

Silent drift is not allowed.

---

## Why ADRs Matter Here

This project has a very tight scope:

> Generate clean, well-structured PowerShell help artifacts from comment-based help, compiled cmdlets, MAML, and HelpInfo.xml — nothing more.

Without guardrails, it would be easy to drift into:

- A generic documentation tool
- A static site generator
- A help hosting platform
- A PlatyPS extension
- A “smart” prose rewriter

The ADRs prevent that.

---

## Current ADR Index

| ADR | Title |
| ----- | ------- |
| ADR-001 | Scope and Non-Goals |
| ADR-002 | Supported Inputs and Source of Truth |
| ADR-003 | Parsing Strategy and Normalization |
| ADR-004 | Output Contracts and File Layout |
| ADR-005 | Schema Validation Strategy |
| ADR-006 | Compiled C# Cmdlets Support |
| ADR-007 | MAML Generation Contract |
| ADR-008 | Normalized Help Model Specification |
| ADR-009 | Validation Rules and Severity Levels |
| ADR-010 | HelpInfo.xml and Packaging Contract |
| ADR-011 | Markdown Generation Contract |
| ADR-012 | Comment-Based Help AST Parsing Contract |
| ADR-013 | Test Strategy and Golden Fixture Design |
| ADR-014 | Governance and Branch Protection Policy |

---

## How to Propose a Change

If a new idea arises:

1. Identify which ADR it impacts.
2. Determine whether it violates the current decision.
3. If so, create a new ADR or update the existing one.
4. Document the rationale clearly.
5. Only then implement.

We do not “just try something.”

---

## Core Principles

- Deterministic output
- Structural, not creative
- Schema-valid artifacts
- Strong separation of parsing and rendering
- Zero scope creep
- CI-safe and diff-friendly
- Developer-first ergonomics

---

## The Discipline Rule

If implementation and ADR conflict:

> The ADR wins.

Unless formally updated.

This keeps the project stable, predictable, and professional.
