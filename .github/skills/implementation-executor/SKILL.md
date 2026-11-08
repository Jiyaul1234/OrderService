---
name: implementation-executor
description: Implement a feature only after the user explicitly approves the exact plan version. Execute its tasks, run relevant verification, update the plan checklist, and create a unique execution report.
---

# Implementation Executor

## 1. Mandatory shared standards and inputs

Before starting:

1. Read `.github/skills/base-engineering/SKILL.md`.
2. Read the task's specification.
3. Read the exact implementation plan version.
4. Follow all shared engineering, security, and safety rules.

If any required document is missing or ambiguous, stop and ask the user.

## 2. Objective and boundaries

Implement the approved plan, verify the result, update the corresponding plan's execution checklist, and create a separate execution report.

Do not commit, push, deploy, release, or perform destructive environment operations unless the user separately authorizes the exact action.

## 3. Mandatory approval validation

Before changing application code, verify all of the following:

- Specification exists and matches the task ID.
- Plan exists and matches the same task ID.
- The exact plan version is known.
- The user explicitly approved that exact version.
- The plan has not materially changed since approval.
- The requested work is within the approved scope.

A general request such as "implement the feature" is not sufficient if no exact plan approval can be established.

If approval is missing, unclear, or stale, do not modify application code. Explain what approval is needed.

## 4. Pre-execution safety checks

Before implementation:

1. Confirm repository and solution root.
2. Inspect Git status and identify existing user changes.
3. Do not discard, reset, or overwrite unrelated changes.
4. Reinspect files that the plan says will be changed.
5. Confirm the current code still matches the plan assumptions.
6. Identify available build and test commands from the repository.
7. Record baseline build/test results when practical.

If the codebase has changed enough to invalidate the plan, stop and request a plan revision and renewed approval.

## 5. Execution workflow

### Phase 1: Preparation

- Confirm the approved task ID and plan version.
- Confirm branch and working tree state.
- Check relevant dependencies and tooling.
- Record blockers before proceeding.

### Phase 2: Execute tasks in order

For each `TASK-*` item:

1. Read the full task and its dependencies.
2. Inspect relevant source files.
3. Implement only the approved change.
4. Follow existing architecture and coding conventions.
5. Review the diff for unintended changes.
6. Run the most relevant available checks.
7. Update the plan checklist accurately.

Do not mark a task complete until its defined completion condition is met.

### Phase 3: Integration

Where relevant, verify:

- Dependency injection and project references
- API contracts and status codes
- Persistence and transaction behavior
- Messaging and external integrations
- Authentication and authorization
- Validation and error handling
- Backward compatibility

### Phase 4: Build and test

Use commands and scripts supported by the repository. For a .NET solution, examples may include:

- `dotnet restore`
- `dotnet build <solution-file>`
- `dotnet test <solution-file>`

Use the actual solution path and project configuration. Do not assume a command is appropriate without checking.

Record the command, result, and relevant output summary. If a command cannot run, record the reason as `BLOCKED` or `NOT EXECUTED`.

### Phase 5: Acceptance criteria verification

For every `AC-*` criterion in the specification, record:

- Criterion ID and description
- Verification method
- Evidence or relevant test
- Result: `PASS`, `FAIL`, `BLOCKED`, or `NOT EXECUTED`

Do not mark an acceptance criterion as passed based only on code inspection if runtime verification is required.

### Phase 6: Final review

Review:

- Actual changed files
- Unintended or unrelated modifications
- Build and test results
- Security implications
- Incomplete tasks
- Deviations from the approved plan
- Outstanding risks and blockers

## 6. Handling deviations and scope changes

If a task cannot be completed as written, or implementation requires a material change:

1. Stop the affected work.
2. Explain the issue and its impact.
3. Describe the proposed plan change.
4. Obtain a revised plan and explicit approval.
5. Continue only after approval.

Do not silently expand scope or substitute a materially different design.

## 7. Update the plan

Update only the corresponding task plan.

- Mark a task `[x]` only when its completion condition is satisfied.
- Leave incomplete tasks unchecked.
- Record concise execution notes and verification outcomes.
- Record deviations and blockers.
- Preserve the approved plan version and approval history.

Do not rewrite the plan in a way that hides what was originally approved.

## 8. Create a unique execution report

Create `docs/execution-reports/` if needed.

Use:

`{task-id}-execution-report.md`

If it already exists, preserve it and create a sequentially numbered report, such as `{task-id}-execution-report-02.md`.

### Execution Report Template

# Execution Report

## 1. Document Information

- Task ID:
- Feature:
- Specification: relative link
- Plan: relative link
- Approved plan version:
- Execution date:
- Overall status:

## 2. Summary

Describe what was implemented and the overall outcome.

## 3. Files Created

List new files and their purpose.

## 4. Files Modified

List modified files and summarize each change.

## 5. Task Results

Map every `TASK-*` item to its actual status and notes.

## 6. Acceptance Criteria Results

Map every `AC-*` item to its verification method and result.

## 7. Build Results

List commands executed and actual outcomes.

## 8. Test Results

List tests executed, passed, failed, blocked, and not executed. Include counts only when known.

## 9. Security and Quality Checks

Record relevant checks actually performed.

## 10. Deviations

Describe differences from the approved plan, or state `None`.

## 11. Blockers and Known Issues

List unresolved issues and their impact.

## 12. Final Status

Choose one:

- `COMPLETED`
- `PARTIALLY COMPLETED`
- `FAILED`
- `BLOCKED`

Explain the status.

## 9. Final response

Report:

- Task ID and approved plan version
- Implemented functionality
- Files created and modified
- Build and test outcomes
- Acceptance criteria results
- Incomplete tasks and blockers
- Specification, plan, and execution report paths

Be precise. Never claim that code was built, tests passed, or acceptance criteria were satisfied unless verified.
