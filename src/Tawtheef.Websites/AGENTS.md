# Frontend Repository Guidelines
**Angular 20 · PrimeNG v20**

This document defines **mandatory frontend standards** for this repository.  
All contributors must adhere to these rules when developing or refactoring features.

---

## 1. Project Structure & Module Organization

- This repository contains **two Angular 20 applications**:
    - `operations-web` → Internal portal
    - `recruitment-web` → Candidate-facing portal

### Folder Structure

- Features:
  ```
  src/app/**
  ```

- Assets:
  ```
  src/assets/**
  ```

- Environments:
  ```
  src/environments/**
  ```

- Translations:
  ```
  public/i18n/**/{en,ar}.json
  ```

### Routing

- Route constants must live in:
  ```
  src/app/routes/*.ts
  ```
- **Do not hard-code routes**
- Always use route helpers, for example:
  ```ts
  routes.auth.login
  routes.dashboard(role)
  ```

---

## 2. Routing, Guards & Navigation

### Guards

- `authGuard`, `authMatchGuard` → Enforce authentication
- `loggedOutOnlyGuard` → Redirect authenticated users away from `/auth/*`
- `profileCompleteGuard` (recruitment app) → Force profile completion

### Navigation Rules

- All auth redirects must:
    - Use route helpers
    - Be consistent across both applications
- Avoid navigation logic inside dialog components.

---

## 3. Build, Test & Development

### Install
Run inside the target application folder:
```bash
npm install
```

### Development
```bash
npm start
```
Use `--port` if running both applications simultaneously.

### Production Build
```bash
npm run build
```

### Tests
```bash
npm test
```

All builds and tests must pass before submitting a Pull Request.

---

## 4. Coding Style & Naming

### Formatting

- UTF-8 encoding
- 2-space indentation
- Final newline required

### Formatter

- Prettier
    - 100 character line width
    - Single quotes

### Naming Conventions

- Components: `PascalCase`
- Files:
  ```
  *.component.ts
  *.component.html
  *.component.scss
  *.service.ts
  ```
- Observable variables must end with `$`.

---

## 5. PrimeNG Version Policy

- The project uses **PrimeNG v20**
- Only PrimeNG v20–compatible APIs and components are allowed
- Deprecated or legacy PrimeNG patterns are strictly forbidden

---

## 6. Dialogs & Modals

### ✅ Required

- Use **PrimeNG DialogService**
- Split dialogs into dedicated components:
  ```
  *.dialog.component.ts
  *.dialog.component.html
  *.dialog.component.scss
  ```
- Pass data via:
  ```ts
  DynamicDialogConfig.data
  ```
- Close dialogs via:
  ```ts
  DynamicDialogRef.close(result)
  ```

### ❌ Forbidden

- ❌ Do not use `<p-dialog>` inside page or feature components
- ❌ Do not embed modal markup inside pages

### Design Rules

Dialog components must:
- Be single-responsibility
- Avoid routing logic
- Be reusable when possible

---

## 7. Localization (i18n)

### General Rules

- No hard-coded UI strings
- All user-facing text must be translated

### HTML Templates
```html
{{ 'profile.title' | translate }}
```

### TypeScript
Use `TranslateService`:
```ts
this.translate.instant('notification.saved');
```

### Translation Files

- Location:
  ```
  public/i18n/**/{en,ar}.json
  ```

### Rules

- Always update both `en.json` and `ar.json`
- Keep keys aligned
- Use flat, logically grouped keys

---

## 8. Notifications & Toasts

### ✅ Required

- Use the centralized `NotificationService`
- All messages must be translated
- Follow consistent summary/detail patterns

### ❌ Forbidden

- ❌ Do not use `MessageService` directly
- ❌ Do not display raw strings

---

## 9. Page Layout & Styling

### Layout

Use **Bootstrap** for:
- Grid system
- Spacing
- Page structure

### UI Controls

Use **PrimeNG** for:
- Forms
- Tables
- Buttons
- Inputs
- Overlays

### Rule of Thumb

- Bootstrap → Layout
- PrimeNG → Interaction

---

## 10. Icons Policy

### Priority Order

1. FontAwesome (default)
2. PrimeIcons
3. Bootstrap Icons

### Rules

- Keep icon usage consistent per screen
- Avoid mixing icon libraries unnecessarily

---

## 11. Testing Guidelines

- Tests must live beside the code:
  ```
  *.spec.ts
  ```
- Mock HTTP calls and translation loaders
- Add tests for:
    - New features
    - Guard changes
    - Navigation logic

---

## 12. Localization Validation

Run the application in:
- English
- Arabic

Validate:
- RTL / LTR layout
- Missing translation keys
- Text interpolations

---

## 13. Pull Request Checklist

Before opening a PR, ensure:

- PrimeNG v20 APIs are used
- DialogService is used (no `<p-dialog>`)
- Dialogs are split into components
- No hard-coded strings
- `TranslateService` used in TypeScript
- `translate` pipe used in HTML
- `NotificationService` used for toasts
- Bootstrap and PrimeNG used correctly
- FontAwesome icons preferred
- `en.json` and `ar.json` are updated
- Build and tests pass

---

## 14. Enforcement

- PRs that violate these rules will not be approved
- Existing code must be refactored when touched
- Consistency, maintainability, and clarity take priority
