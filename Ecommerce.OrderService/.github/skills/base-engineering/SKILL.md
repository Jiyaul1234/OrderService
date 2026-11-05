---
name: base-engineering
description: Shared engineering standards for repository analysis, architecture, security, testing, documentation, and safe change management. Read this skill before specification, planning, implementation, or verification work.
---

# Base Engineering Standards

## 1. Purpose and precedence

This skill is the shared engineering reference for all skills in this repository.

Every specialized skill MUST read and follow this file before starting. If a specialized skill conflicts with this file, follow this file unless the user explicitly approves a task-specific exception.

If this file is missing or cannot be read, stop and tell the user.

## 2. Repository context

The solution is `Ecommerce.OrderService`.

Known solution projects:

- `Ecommerce.OrderService.Domain`
- `Ecommerce.OrderService.Application`
- `Ecommerce.OrderService.Infrastructure`
- `Ecommerce.OrderService.API`

The solution also contains a `Tests` solution folder, but the supplied solution file did not show any test project registered. Inspect the repository before concluding which tests exist.

Do not assume target framework, package versions, database technology, messaging technology, ORM, or implementation patterns. Confirm them from project files and source code.

## 3. Architecture responsibilities

Use the following as intended responsibilities, then verify the actual codebase before proposing changes.

### Domain

Typically contains:

- Entities and aggregates
- Value objects
- Domain events
- Domain exceptions
- Domain business rules
- Domain-level abstractions, only where appropriate

Domain must not depend on Application, Infrastructure, or API.

### Application

Typically contains:

- Use cases and application workflows
- Commands and queries, if the project uses that pattern
- DTOs
- Application interfaces and abstractions
- Validators
- Application-level business orchestration

Application may depend on Domain. It must not depend on Infrastructure or API.

### Infrastructure

Typically contains:

- Persistence and database implementations
- Repository implementations
- External service integrations
- Messaging implementations
- File storage and other infrastructure adapters

Infrastructure may depend on Application and Domain, subject to the existing project design.

### API

Typically contains:

- Controllers or endpoint definitions
- Middleware and filters
- Authentication and authorization configuration
- HTTP request and response handling
- Dependency-injection composition and application startup

API may depend on Application. It may reference Infrastructure for composition-root registration when that is the established design. Do not introduce dependencies from Application or Domain back to API.

### Architecture rules

1. Follow the actual project-reference graph.
2. Keep business rules out of controllers and transport endpoints.
3. Keep infrastructure implementations out of Domain.
4. Keep application abstractions separate from infrastructure implementations.
5. Reuse established patterns before introducing new abstractions.
6. Do not add a project or layer without explaining why.
7. Do not move files between projects without documenting the reason and impact.
8. Avoid circular project references.

## 4. Mandatory repository analysis

Before making recommendations or changes:

1. Identify the repository and solution root.
2. Inspect the `.sln` file and relevant `.csproj` files.
3. Identify target frameworks and package references.
4. Inspect project references and dependency direction.
5. Inspect relevant folders, classes, interfaces, and tests.
6. Identify established naming, formatting, validation, logging, and error-handling conventions.
7. Inspect relevant configuration without exposing secrets.
8. Identify uncommitted changes before implementation.

Use focused inspection. Do not read or modify unrelated areas without a reason.

Never invent classes, methods, endpoints, tables, events, or files. Distinguish confirmed facts from proposals and assumptions.

## 5. Engineering principles

Apply where appropriate:

- SOLID
- DRY
- KISS
- Separation of Concerns
- Dependency Inversion
- Clean Architecture
- Domain-Driven Design where justified
- Secure by Design
- Fail Fast
- Explicit error handling

Do not force patterns such as CQRS, MediatR, repositories, domain events, or factories into the codebase if they are not needed or are inconsistent with the existing design.

## 6. Coding standards

When implementation is authorized:

- Use the language version and framework already configured by the project unless a change is approved.
- Follow existing naming and formatting conventions.
- Use meaningful names and focused methods.
- Prefer clear, maintainable code over clever abstractions.
- Use dependency injection consistently with the existing design.
- Use asynchronous APIs for I/O where supported.
- Propagate `CancellationToken` where appropriate and consistent with existing APIs.
- Validate untrusted inputs at appropriate boundaries.
- Handle errors intentionally; do not swallow exceptions.
- Avoid hardcoded environment-specific values and secrets.
- Avoid unnecessary packages and dependencies.
- Preserve public contracts and backward compatibility unless change is required and documented.
- Add comments only when they explain non-obvious intent or constraints.

## 7. Security and privacy

Consider relevant risks, including:

- Authentication and authorization
- Input validation and output encoding
- Injection vulnerabilities
- Sensitive data exposure
- Secret and configuration management
- Least privilege
- Data retention and privacy
- Logging and telemetry of sensitive information
- Dependency and supply-chain risks

Never place passwords, access tokens, connection strings containing credentials, private keys, or other secrets in source code, specifications, plans, or reports. Redact secrets if encountered.

## 8. Testing and verification

Use the test frameworks and commands already present in the repository.

Consider:

- Unit tests
- Integration tests
- API/contract tests
- Regression tests
- Migration or persistence tests, where relevant

Cover success, failure, boundary, and business-rule scenarios as appropriate.

Do not delete or weaken tests merely to make a build pass. Do not claim a build or test passed unless it was actually run and succeeded.

Use these result labels consistently:

- `PASS`: executed and passed
- `FAIL`: executed and failed
- `BLOCKED`: could not execute because of a blocker
- `NOT EXECUTED`: not run

## 9. Task identity and unique documents

For each new feature/change task, create one task ID based on the date and a short descriptive slug.

Recommended format:

`YYYYMMDD-feature-slug`

Example:

`20261105-order-cancellation`

Use the same task ID in all related documents.

Before creating a file, check whether the exact path already exists. Never silently overwrite an existing specification, plan, or execution report.

If a filename already exists, preserve it and create a new sequential suffix, such as `-02`, or ask the user if the relationship is unclear.

Use these directories:

- `docs/specifications/`
- `docs/plans/`
- `docs/execution-reports/`

Use relative links between related documents.

## 10. Scope and approval controls

- A request to create a specification does not authorize code changes.
- A request to create a plan does not authorize code changes.
- Implementation requires explicit user approval of the exact plan version.
- Approval of one plan version does not approve later material revisions.
- If implementation would exceed the approved scope, stop the affected work and request approval.
- Ask before destructive actions, data deletion, migrations against shared environments, commits, pushes, releases, or deployments unless the user has explicitly authorized that exact action.
- Preserve unrelated user changes and existing uncommitted work.

## 11. Final communication

Report factual outcomes:

- Task ID and artifact paths
- What was created or changed
- Verification actually performed
- Build and test results
- Acceptance-criteria status
- Deviations, blockers, and outstanding issues

Never imply that an unexecuted check passed.
