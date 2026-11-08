---
name: specification-generator
description: Turn a user's feature or change request into a unique, repository-aware Markdown specification under docs/specifications. Analyze the existing solution, document requirements and impact, and do not modify application code.
---

# Specification Generator

## 1. Mandatory shared standards

Before doing anything:

1. Read `.github/skills/base-engineering/SKILL.md`.
2. Follow its repository, architecture, security, documentation, and safety rules.
3. Confirm the repository root and inspect the relevant solution and projects.

If the Base Engineering skill is missing or unreadable, stop and report that.

## 2. Objective and boundaries

Convert the user's natural-language request into a clear, testable feature specification.

This skill may create or update only the specification artifact for the current task when explicitly requested. It must not:

- Implement application code
- Modify `.csproj` or `.sln` files
- Change database schemas or migrations
- Modify application configuration
- Create or modify tests
- Start implementation or execute a plan

A specification is a description of the required outcome, not authorization to implement it.

## 3. Workflow

### Step 1: Understand the request

Identify, where applicable:

- Business objective and problem
- Actors and user roles
- Functional requirements
- Non-functional requirements
- Inputs, outputs, and workflows
- Business rules
- Permissions and security expectations
- Validation and error scenarios
- Dependencies and integrations
- Acceptance criteria
- Explicit exclusions

Do not invent missing business requirements.

### Step 2: Inspect the repository

Inspect only relevant parts of the solution.

At minimum, as relevant:

- Solution and project structure
- Relevant `.csproj` files and project references
- Existing Domain, Application, Infrastructure, and API implementation
- Related endpoints, services, entities, DTOs, and persistence code
- Existing tests and conventions
- Relevant configuration keys, without exposing secret values

Record actual paths only when verified. Clearly label proposed files or components as proposed, not existing.

### Step 3: Identify ambiguity

If a missing decision materially changes scope, behavior, security, data integrity, or API compatibility, ask the user a focused clarification question before finalizing the specification.

For minor uncertainty:

- Record the assumption explicitly.
- Explain its impact.
- Do not present it as a confirmed requirement.

### Step 4: Create a unique specification

Create `docs/specifications/` if it does not exist.

Generate a task ID using the Base Engineering format:

`YYYYMMDD-short-feature-slug`

Use this filename:

`{task-id}-spec.md`

Check for collisions before writing. Never overwrite an existing specification. If the name exists, use a sequential suffix or ask the user if it is unclear whether this is a new task or a revision.

### Step 5: Specification template

Use this Markdown structure:

# Feature Specification

## 1. Document Information

- Task ID:
- Feature:
- Created:
- Status: Draft
- Version: 1.0

## 2. Overview

Summarize the requested change and expected outcome.

## 3. Business Objective

Explain the problem being solved and the value of the change.

## 4. Current Behavior

Describe only behavior confirmed by repository inspection or user-provided information. Cite relevant repository paths where useful.

## 5. Expected Behavior

Describe the target behavior from the user's and system's perspectives.

## 6. Functional Requirements

Use stable IDs: `FR-001`, `FR-002`, etc.

Each requirement must be specific and testable.

## 7. Non-Functional Requirements

Include applicable performance, scalability, availability, reliability, accessibility, maintainability, and observability requirements. Do not invent numeric targets.

## 8. Business Rules

List applicable rules and constraints.

## 9. Scope

### In Scope

List included behavior.

### Out of Scope

List excluded behavior and adjacent work not requested.

## 10. Impact Analysis

Identify confirmed or potentially affected:

- Projects and layers
- API endpoints and contracts
- Domain model
- Application workflows
- Persistence and data
- External services and messaging
- UI/client consumers
- Tests and operational configuration

Clearly distinguish confirmed impact from areas requiring planner validation.

## 11. Data and API Considerations

Describe required or potential changes to entities, schema, migrations, requests, responses, events, and compatibility. Label unconfirmed design choices as proposals.

## 12. Security and Privacy

Document authorization, validation, sensitive data, audit, and privacy considerations.

## 13. Error and Edge Cases

Describe expected failure conditions and desired behavior.

## 14. Acceptance Criteria

Use stable IDs: `AC-001`, `AC-002`, etc. Make each criterion observable and verifiable.

## 15. Assumptions

List assumptions and their rationale.

## 16. Open Questions

List unresolved decisions. If none, write `None identified`.

## 17. Dependencies

List internal and external dependencies.

## 18. References

Link relevant repository files and documentation using relative paths where possible.

## 4. Quality gate

Before saving, verify:

- The requested outcome is clear.
- Requirements are testable.
- Scope and exclusions are explicit.
- Existing behavior is separated from desired behavior.
- Repository observations are accurate.
- Assumptions and open questions are visible.
- Acceptance criteria cover the functional requirements.
- No application code or project files were changed.

## 5. Completion response

Report:

- Task ID
- Exact specification path
- Short feature summary
- Number of functional requirements and acceptance criteria
- Important assumptions or open questions
- Confirmation that no application code was changed

Do not automatically invoke the planner. Wait for the user's next instruction.
 c