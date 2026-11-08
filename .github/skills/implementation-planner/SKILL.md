---
name: implementation-planner
description: Read a task's feature specification, inspect the existing repository, and create a unique, actionable implementation plan under docs/plans. The plan requires explicit user approval before any implementation.
---

# Implementation Planner

## 1. Mandatory shared standards

Before starting:

1. Read `.github/skills/base-engineering/SKILL.md`.
2. Follow all shared engineering and safety rules.
3. Locate and read the exact specification for the requested task.

If the shared skill or specification is missing, stop and report the issue. Do not guess which specification the user means when multiple candidates exist.

## 2. Objective and boundaries

Produce an actionable implementation plan that maps the specification to concrete, ordered engineering tasks.

This skill creates or revises planning documentation only. It must not modify application source code, project files, configuration, database schema, migrations, or tests.

Creating a plan is not approval to execute it.

## 3. Workflow

### Step 1: Identify the task and specification

- Locate the exact specification path.
- Read the complete specification.
- Capture its task ID and version.
- Confirm that scope, requirements, and acceptance criteria are sufficiently clear.

If material questions remain unresolved, ask the user before planning.

### Step 2: Inspect the actual implementation

Inspect relevant parts of the solution, including:

- `.sln`, `.csproj`, target frameworks, and project references
- Domain entities and business rules
- Application services, interfaces, and workflows
- Infrastructure persistence and integrations
- API endpoints, middleware, and dependency registration
- Existing tests and test conventions
- Relevant configuration and deployment assumptions

Do not assume a class or file exists. Confirm paths and current responsibilities.

### Step 3: Design a minimal implementation approach

Determine:

- Which existing components should change
- Which new files or components are required
- Project and dependency changes, if any
- Data, migration, and API contract impacts
- Security and authorization work
- Error handling and observability
- Testing and regression strategy
- Dependencies and safe execution order
- Risks and rollback considerations

Prefer the smallest maintainable change that satisfies the specification.

### Step 4: Create a unique plan

Create `docs/plans/` if needed.

Use the exact task ID from the specification:

`{task-id}-plan.md`

Check for an existing file. Never overwrite a plan silently.

If a plan already exists, determine whether this is a new revision. Preserve the previous version by creating a clearly numbered revision or ask the user. A material revision must reset approval status to `Awaiting Approval`.

### Step 5: Plan template

# Implementation Plan

## 1. Document Information

- Task ID:
- Feature:
- Specification: relative link
- Created:
- Status: Awaiting Approval
- Version: 1.0

## 2. Implementation Overview

Summarize the proposed approach and key design decisions.

## 3. Current Architecture Findings

Record confirmed project structure, dependencies, patterns, and relevant existing components.

## 4. Affected Components and Files

For each item specify:

- Path
- Existing or new
- Current responsibility (for existing files)
- Proposed change
- Reason
- Relevant requirement IDs

Do not list speculative paths as confirmed existing files.

## 5. Implementation Phases and Tasks

Break work into ordered phases, such as:

### Phase 1: Preparation
### Phase 2: Domain and Application
### Phase 3: Infrastructure and Persistence
### Phase 4: API and Integration
### Phase 5: Tests
### Phase 6: Final Verification

Use stable task IDs: `TASK-001`, `TASK-002`, etc.

For every task include:

- Objective
- Exact actions
- Expected files or components
- Dependencies
- Requirement IDs covered
- Completion/verification condition

Include only relevant phases; do not force unnecessary layers into the plan.

## 6. Project and Dependency Changes

Document any proposed project-reference or package changes, with rationale.

## 7. Database and Migration Plan

Describe schema, migration, data compatibility, and rollback needs, if applicable.

## 8. API and Contract Changes

Describe endpoint, request, response, event, and compatibility changes, if applicable.

## 9. Security and Observability

List required authorization, validation, logging, metrics, and audit work.

## 10. Testing Plan

Specify test types, scenarios, and existing test projects or proposed test locations. Do not assume a test project exists without checking.

## 11. Risks and Mitigations

Document technical and operational risks.

## 12. Rollback Strategy

Describe safe reversal steps where applicable. Flag destructive or environment-dependent operations for separate approval.

## 13. Acceptance Criteria Mapping

Map every specification `AC-*` to one or more plan tasks and a verification method.

## 14. Execution Checklist

Use unchecked Markdown tasks:

- [ ] TASK-001 — description
- [ ] TASK-002 — description

## 15. Approval Record

- Status: Awaiting Approval
- Plan version:
- Approved by user: No
- Approval date: Not approved

## 4. Approval gate

After saving the plan:

1. Check that every requirement and acceptance criterion is covered.
2. Check task order and dependencies.
3. Check that each task has a completion condition.
4. Present the plan path, version, task count, affected areas, and risks.
5. Ask the user to review and explicitly approve this exact plan version.

Do not execute the plan.

Only a clear user statement approving the specific plan version authorizes the executor skill. General discussion, review requests, or creation of the plan are not approval.

## 5. Revisions

When the user requests plan changes:

- Modify planning documentation only.
- Preserve prior versions.
- Increment the version.
- Explain material changes.
- Reset approval to `Awaiting Approval`.
- Request approval again.

Never carry approval forward after a material revision.

## 6. Completion response

Report:

- Task ID
- Specification path
- Plan path and version
- Number of tasks
- Main affected components
- Risks and unresolved questions
- Approval status: `Awaiting Approval`

Stop and wait for the user.
