# Copilot Instructions for SparqLeoRepo2

This repository uses .NET (ASP.NET Core), Blazor, Entity Framework Core, and C#.

---

# 🧠 Behavioral Guidelines (Karpathy + Beast Mode Hybrid)

## Think Before Coding
- State assumptions explicitly before implementing.
- If something is unclear, stop and ask.
- If multiple interpretations exist, present them.
- Do not silently guess requirements.
- If a simpler approach exists, propose it.

## Simplicity First
- Write the minimum code required to solve the problem.
- Do NOT add:
  - speculative features
  - unnecessary abstractions
  - configurability that wasn’t requested
- Avoid overengineering.
- If the solution feels complex, simplify it.

## Surgical Changes
- Only modify what is required.
- Do not refactor unrelated code.
- Match existing code style and patterns.
- Clean up only what your change affects.
- If unrelated issues are found, mention them—do not fix them.

## Goal-Driven Execution
- Convert tasks into verifiable outcomes.
- Prefer:
  - “write test → make it pass”
  - “reproduce bug → fix → verify”
- For multi-step work:
  - define a clear plan
  - validate each step

---

# ⚙️ Execution Discipline (Beast Mode Adapted)

## Completion Behavior
- Fully resolve the task before stopping.
- Do not leave partial implementations.
- Verify correctness before finishing.

## Planning
- For non-trivial tasks:
  - break into steps
  - keep steps small and verifiable

## Iteration
- Make small, incremental changes.
- Validate after each step.
- Continue until the problem is completely solved.

## Validation Mindset
- Consider:
  - edge cases
  - failure scenarios
  - unintended side effects
- Ensure changes actually solve the problem.

---

# 🏗️ .NET & ASP.NET Core Best Practices

- Use Dependency Injection for services and repositories.
- Avoid blocking async calls (always use `await`).
- Validate all user input.
- Use configuration and secrets management (no hardcoding).
- Log errors and important events using structured logging.
- Prefer strongly-typed models over dynamic objects.
- Use middleware for cross-cutting concerns.
- Follow RESTful API conventions.
- Return appropriate HTTP status codes.
- Enable HTTPS and secure headers.
- Use cancellation tokens for long-running operations.
- Avoid exposing internal implementation details.

Reference:
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices

---

# 💻 C# Coding Standards

- Use PascalCase for classes, methods, properties.
- Use camelCase for variables and parameters.
- Use meaningful, explicit names.
- Keep methods short and focused (single responsibility).
- Use XML documentation for public APIs.
- Avoid magic numbers/strings (use constants/enums).
- Prefer interfaces for abstractions.
- Use explicit access modifiers.
- Dispose IDisposable properly.
- Avoid unnecessary object creation.
- Use exception handling carefully.
- Write unit tests for business logic.

---

# 🗄️ Entity Framework Core

- Use DbContext per request.
- Use AsNoTracking for read-only queries.
- Avoid lazy loading unless necessary.
- Use migrations for schema changes.
- Validate data before saving.
- Use navigation properties correctly.
- Avoid N+1 queries (use Include).
- Prefer async database operations.
- Handle concurrency conflicts properly.
- Do not expose entities directly—use DTOs.
- Dispose DbContext properly.
- Log and handle database exceptions.

---

# 🎨 Blazor & Razor Components

- Use partial classes for code-behind logic.
- Separate UI and logic (.razor + .razor.cs).
- Use dependency injection and cascading parameters.
- Break large components into smaller ones.
- Use event callbacks for communication.
- Prefer async lifecycle methods.
- Minimize JavaScript interop.
- Handle UI errors properly.
- Follow accessibility standards.
- Use CSS isolation.

---

# 🧪 Quality & Maintenance

- Follow existing folder and namespace conventions.
- Run unit and integration tests after changes.
- Document public APIs and business logic.
- Refactor only when necessary and scoped to the task.

---

# 🚫 Anti-Patterns to Avoid

- Overengineering simple solutions
- Silent assumptions
- Broad refactors unrelated to the task
- Adding unused abstractions
- Writing code without verifying correctness
- Ignoring existing project patterns

---

# 🎯 Guiding Principle

Write code like a senior engineer reviewing your PR would:

- Clear
- Minimal
- Correct
- Consistent with the existing codebase

If unsure → ask.  
If complex → simplify.  
If incomplete → continue.
