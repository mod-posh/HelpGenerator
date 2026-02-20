# Quickstart

This guide demonstrates the primary workflows supported by the PowerShell Help Generator suite.

The goal: generate clean, structured help artifacts quickly and deterministically.

---

## 1. From Script Module with Comment-Based Help

### Input

```BASH

MyModule.psm1
MyModule.psd1

```

Functions include properly written comment-based help.

### Generate MAML + Markdown + HelpInfo.xml

```powershell
Export-HelpPackage `
  -ModulePath ./MyModule.psm1 `
  -ManifestPath ./MyModule.psd1 `
  -OutputPath ./out `
  -GenerateMarkdown `
  -GenerateMaml `
  -GenerateHelpInfo
```

### Output

```BASH
/out
  /help
    /en-US
      MyModule-Help.xml
  /docs
    /module
      module.md
      helpinfo.md
    /commands
      Get-Thing.md
      Set-Thing.md
  /updatable-help
    HelpInfo.xml
```

All outputs follow ADR-004, ADR-007, ADR-010, and ADR-011 contracts.

---

## 2. From Script Module with Missing Help

If functions have no comment-based help:

```powershell
Initialize-CommentHelp -ModulePath ./MyModule.psm1
```

This inserts deterministic scaffold blocks:

```BASH
.SYNOPSIS
TODO: Add synopsis.
```

Then run:

```powershell
Export-HelpPackage -ModulePath ./MyModule.psm1 -OutputPath ./out
```

Warnings will be emitted for placeholder content (ADR-009).

---

## 3. From Compiled C# Cmdlet Module

### Requirements

The project must enable XML documentation:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

### Input

```BASH
MyModule.dll
MyModule.xml
MyModule.psd1 (optional but recommended)
```

### Generate Help

```powershell
Export-HelpPackage `
  -AssemblyPath ./MyModule.dll `
  -XmlDocPath ./MyModule.xml `
  -OutputPath ./out `
  -GenerateMarkdown `
  -GenerateMaml `
  -GenerateHelpInfo
```

Reflection provides structure.
XML docs provide prose.
Outputs follow the same contracts as script modules (ADR-006).

---

## 4. From Existing MAML + HelpInfo.xml

```powershell
Convert-HelpArtifactsToMarkdown `
  -HelpPath ./help/en-US/MyModule-Help.xml `
  -HelpInfoPath ./HelpInfo.xml `
  -OutputPath ./docs
```

Generates:

```bash
/docs
  /commands
  /module
```

No rewriting occurs. Structure is preserved.

---

## 5. Generate Updatable Help Package

```powershell
Export-HelpPackage `
  -ModulePath ./MyModule.psm1 `
  -Package `
  -OutputPath ./release
```

Produces:

```bash
/release
  HelpInfo.xml
  /en-US
    MyModule_1.0.0.0_en-US.cab
```

CAB naming and layout follow ADR-010.

---

## 6. Validate Help Artifacts

```powershell
Test-HelpPackage -Path ./out
```

Validation includes:

* Structural validation (ADR-009)
* Schema validation
* Semantic checks
* Packaging validation (if applicable)

Errors stop generation.
Warnings do not (unless `-TreatWarningsAsErrors` is specified).

---

## 7. Strict Mode (CI-Friendly)

```powershell
Export-HelpPackage -ModulePath ./MyModule.psm1 -Strict
```

Strict mode may:

* Require examples
* Treat missing descriptions as errors
* Enforce verb rules

Designed for release pipelines.

---

## Mental Model

Everything flows through:

```bash
Input (Script | Assembly | MAML)
        ↓
NormalizedHelpModel
        ↓
Generators
        ↓
MAML | Markdown | HelpInfo.xml | CAB
```

No shortcuts. No hidden logic. Deterministic output.

---

## What This Tool Does NOT Do

* Does not publish help.
* Does not host help.
* Does not generate websites.
* Does not rewrite prose.
* Does not extend PlatyPS.
* Does not infer undocumented behavior.

If it’s not about generating or validating PowerShell help artifacts, it’s out of scope.
