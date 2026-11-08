# Feature Specification

## 1. Document Information

- **Task ID:** `20261107-ilogger-coverage`
- **Feature:** Improve ILogger Coverage Across Runtime Components
- **Created:** 2026-11-07
- **Status:** Draft
- **Version:** 1.0

---

## 2. Overview

Audit and improve logging coverage across runtime operational components to enhance observability. The specification identifies classes currently missing `ILogger<T>` injection and defines which should receive it to maintain clean architecture.

---

## 3. Business Objective

Provide comprehensive application-level visibility into order processing, payment handling, shipment management, and data persistence operations. Logging is critical for debugging production issues, monitoring system health, and auditing business-sensitive operations.

---

## 4. Current Behavior

**Logging is partially implemented:**
- ✅ `OrdersController`, `ProductsController` — injected and used
- ✅ `OrderService`, `ProductService` — injected and used
- ✅ `BaseRepository<T>`, `OrderRepository`, `OrderItemRepository`, `ProductRepository` — injected and used
- ✅ `AzureServiceBusProducer` — injected and used
- ✅ `ServiceBusPaymentProcessor`, `ServiceBusShipmentProcessor` — injected and used
- ❌ **Missing:** `AppDbContext` — no ILogger
- ❌ **Not injected:** Domain models (Order, OrderItem, Product), DTOs, Events, MappingProfile — by design (passive types)

**Log levels and coverage are inconsistent:**
- Most operations log only `LogInformation` on entry/exit
- Exception logging exists in repositories and bus handlers but is sparse in services
- No structured logging patterns (e.g., failed validations, business rule violations, state transitions)

**Configuration:**
- `appsettings.json` defines default log level as `Information` and suppresses `Microsoft.AspNetCore` to `Warning`
- No explicit log provider configured in `Program.cs` (defaults to console)

---

## 5. Expected Behavior

**Runtime components with ILogger injected and actively logging:**
1. **Controllers** — log request entry, business outcomes, errors (already done)
2. **Services** — log workflows, validation failures, business rule violations, state changes
3. **Repositories** — log CRUD operations with entity details, database errors (already done)
4. **Data Context** — log connection events, migration status, query failures
5. **Service Bus handlers** — log message processing, deserialization failures, domain event outcomes (already done)
6. **Bus Producer** — log message publishing and failures (already done)

**Passive types remain without direct ILogger:**
- Domain models, DTOs, Events, and Mapping profiles do not inject or use `ILogger` directly
- Logging of their use/transformation occurs from services and repositories that consume them

**Log messages should be:**
- Structured (use named parameters: `{OrderId}`, `{CustomerId}`, etc.)
- Appropriate level (Info for normal flow, Warning for degraded, Error for failure)
- Non-redundant (avoid logging the same fact multiple times in a call stack)

---

## 6. Functional Requirements

| ID | Requirement | Details |
|---|---|---|
| **FR-001** | Add `ILogger<AppDbContext>` to `AppDbContext` | Inject via constructor; log connection state, migration completion, and query errors. |
| **FR-002** | Enhance `OrderService` logging | Add logs for validation failures, payment/shipment state transitions, and business rule violations. |
| **FR-003** | Enhance `ProductService` logging | Add logs for stock checks, price/discount calculations, and inventory updates. |
| **FR-004** | Standardize log levels across all services | Use `LogInformation` for normal operations, `LogWarning` for degraded conditions, `LogError` with exceptions for failures. |
| **FR-005** | Add structured logging parameters | Ensure all log messages use named parameters (e.g., `{OrderId}`, `{UserId}`) for consistency and query-ability. |

---

## 7. Non-Functional Requirements

| ID | Requirement | Details |
|---|---|---|
| **NFR-001** | Maintain clean architecture | Do not inject `ILogger` into domain models, DTOs, or event classes. |
| **NFR-002** | No performance degradation | Log calls should not block async operations or introduce latency. |
| **NFR-003** | No serialization impact | Avoid logging internal domain state that would complicate EF Core or message serialization. |
| **NFR-004** | Backward compatibility | Existing log messages in controllers and repositories must remain unchanged unless improvement is necessary. |

---

## 8. Business Rules

1. **Logging is not substitution for code logic** — if an operation fails, the code must handle it; logging documents the failure.
2. **Sensitive data exclusion** — do not log passwords, connection strings, or personal payment details.
3. **Audit trail** — order state transitions (pending → paid → shipped → completed) should be logged at business level.
4. **Error transparency** — all exceptions must be logged with stack trace; partial exceptions may be swallowed only after logging.

---

## 9. Scope

### In Scope

- Add `ILogger<T>` to `AppDbContext`
- Enhance logging in `OrderService` for workflows and business state changes
- Enhance logging in `ProductService` for stock and pricing logic
- Standardize log levels and structured parameters across existing loggers
- Update `Program.cs` logging configuration if needed (provider, levels, formatting)

### Out of Scope

- Logging configuration for external services (Application Insights, Splunk, etc.) — configuration only, no integration
- Adding `ILogger` to domain models, DTOs, or event classes
- Creating a dedicated audit/compliance logging table (separate feature)
- Changing log sinks or providers (console only, unless specified)

---

## 10. Impact Analysis

| Area | Impact | Notes |
|---|---|---|
| **API Layer** | ✅ No code impact | Controllers already use `ILogger` correctly. Minor message enhancements possible. |
| **Application Layer** | 🔧 Medium | `OrderService` and `ProductService` need enhanced logging for business workflows. |
| **Infrastructure Layer** | 🔧 Low | `AppDbContext` requires `ILogger` injection; repositories unchanged. |
| **Domain Layer** | ✅ No change | Entities do not log directly. |
| **Configuration** | 🔧 Low | `appsettings.json` and `Program.cs` may need log level adjustments. |
| **Tests** | 🔧 Low | Mock `ILogger<T>` in unit tests if not already done. |
| **Performance** | ✅ Negligible | Structured logging in .NET is low-overhead. |

---

## 11. Data and API Considerations

**No API contract changes.** Logging is internal; no request/response changes.

**Database changes:** None. Logging does not persist to application tables (configuration-dependent).

**Existing log output:** Will expand, but structure remains compatible.

---

## 12. Security and Privacy

- **No secrets in logs:** Connection strings, JWT tokens, and payment details must be redacted.
- **No PII at info level:** Customer names/emails logged only at Debug level if needed.
- **Audit at info level:** Order lifecycle events (creation, payment, shipment) logged at Info level for audit.
- **Authorization:** No changes to JWT or role-based access; logging is operational only.

---

## 13. Error and Edge Cases

| Scenario | Expected Behavior |
|---|---|
| **Order not found** | Log at Info level: "Order {OrderId} not found"; return 404 without error log. |
| **Payment fails** | Log at Error: "Payment processing failed for order {OrderId}: {ErrorMessage}"; bubble exception. |
| **Database connection lost** | Log at Error in `AppDbContext`; allow exception to propagate. |
| **Service Bus deserialization fails** | Log at Error with exception details; abandon message. |
| **Validation error** | Log at Warning: "Order validation failed: {Reason}"; return 400 without error log. |

---

## 14. Acceptance Criteria

| ID | Criterion | Verification |
|---|---|---|
| **AC-001** | `AppDbContext` has `ILogger<AppDbContext>` injected | Code review; constructor injection present. |
| **AC-002** | All service-level operations log at appropriate levels | Code review; Info/Warning/Error used correctly. |
| **AC-003** | Log messages use structured parameters | Code review; no string interpolation, only named params. |
| **AC-004** | No `ILogger` in domain models, DTOs, or events | Code review; passive types confirmed clean. |
| **AC-005** | Existing tests still pass | Run `dotnet test` on solution. |
| **AC-006** | No secrets logged | Code review; connection strings and tokens redacted. |
| **AC-007** | Build succeeds without warnings related to logging | Run `dotnet build` on solution. |

---

## 15. Assumptions

1. **Logging provider is console** — .NET default; no external sink configured in this specification.
2. **ILogger registration in DI** — already present in `Program.cs` via `builder.Services.AddLogging()`.
3. **EF Core DbContext isolation** — `AppDbContext` is scoped to a single `DbContextOptions`; no sharing across instances.
4. **Azure Service Bus is configured** — processors are already hosted; no changes needed to their registration.
5. **No breaking changes to public APIs** — logging is implementation detail only.

---

## 16. Open Questions

1. **External log aggregation:** Should the specification include configuration for Application Insights or similar? (Deferred to post-specification decision.)
Answer: No, this specification focuses on ILogger coverage; external aggregation is out of scope.
2. **Log retention policy:** How long should logs be retained? (Environment/infrastructure decision, not in scope.)
Answer: Retention is managed by the hosting environment; this specification does not dictate retention.
3. **Debug-level logging:** Should development builds enable Debug/Trace logging? (Configuration in `appsettings.Development.json`, minimal scope change.)
answer: Yes, but this is a configuration concern; the specification does not mandate changes to development logging levels.

---

## 17. Dependencies

**Internal:**
- `.github/skills/base-engineering/SKILL.md` — architecture layer boundaries
- `Ecommerce.OrderService.Application` — services to enhance
- `Ecommerce.OrderService.Infrastructure` — repositories and context to review
- `Ecommerce.OrderService.API` — controllers and program startup

**External:**
- Microsoft.Extensions.Logging (already referenced)
- .NET 10 built-in logging infrastructure

**Process:**
- Code review for architecture compliance
- Integration tests to verify logging does not break workflows

---

## 18. References

**Repository files:**
- `Ecommerce.OrderService/Ecommerce.OrderService.API/Program.cs` — DI and logging configuration
- `Ecommerce.OrderService/Ecommerce.OrderService.Infrastructure/AppDbContext.cs` — to be modified
- `Ecommerce.OrderService/Ecommerce.OrderService.Application/Services/OrderService.cs` — to be enhanced
- `Ecommerce.OrderService/Ecommerce.OrderService.Application/Services/ProductService.cs` — to be enhanced
- `Ecommerce.OrderService/Ecommerce.OrderService.API/appsettings.json` — logging configuration

**Standards:**
- Base Engineering: `.github/skills/base-engineering/SKILL.md`
- Microsoft.Extensions.Logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging

---

## Summary

This specification improves observability in the OrderService by adding structured logging to components currently lacking it. It respects clean-architecture boundaries by excluding passive types from direct logging and focuses on runtime operational components. The scope is manageable: one new ILogger injection in `AppDbContext`, enhanced messages in two services, and standardized log levels across the application.
