# Repository Guidelines

## Project Structure & Module Organization
- Two Angular 20 apps live here: `operations-web` (internal portal) and `recruitment-web` (candidate-facing). Features live in `src/app`, assets in `src/assets`, environments in `src/environments`, and translations under `public/i18n/**` (mirrored `en.json`/`ar.json`).
- Route constants sit in `src/app/routes/*.ts` per app (auth, error, user/admin/employee); import `routes` instead of hard-coding paths.

## Routing, Guards & Navigation
- Prefer `routes` helpers (e.g., `routes.dashboard(role)`, `routes.user.dashboard`, `routes.auth.login`) when navigating or building links.
- Guards: `authGuard`/`authMatchGuard` enforce login; `loggedOutOnlyGuard` redirects authenticated users away from `/auth/*`; ops app still ships a legacy `AuthGuard` for simple activation. Recruitment adds `profileCompleteGuard` to push users to the profile wizard before dashboard.
- Auth redirects land on `routes.auth.login` or the dashboard URL derived from token role; keep them consistent when adding modules.

## Build, Test, and Development Commands
- Install per app: run `npm install` inside the target folder to honor its `package-lock.json`.
- Local dev: `npm start` (`ng serve`) from the app folder; pass `--port` if running both apps.
- Production build: `npm run build` emits `dist/` for the selected configuration. Use `npm run watch` for fast rebuilds.
- Tests: `npm test` runs Jasmine/Karma; keep it green before PRs.

## Coding Style & Naming Conventions
- `.editorconfig` enforces UTF-8, 2-space indents, trimmed whitespace, and final newlines. Prettier (100-char width, single quotes, Angular HTML parser) is the formatter; use your editor integration or `npx prettier --write`.
- Components/services use `PascalCase` with `*.component.ts/html/scss` and `*.service.ts`; suffix observables with `$`. Keep shared UI in feature modules instead of bloating `app.module`.

## Testing Guidelines
- Tests live beside code as `*.spec.ts`. Favor focused component/unit specs; mock HTTP and translation loaders to keep Karma runs fast.
- No strict coverage gate, but add specs for every new feature/branch fix and extend guards when changing navigation rules.

## Localization Updates
- Strings are in `public/i18n/<namespace>/{en,ar}.json` for both apps (common, layout, pages, primeng). Update both languages together, keep keys aligned, and sort logically per section.
- Validate translations by running the app in both locales; avoid embedding dynamic values directly; use interpolation keys instead.

## Commit & Pull Request Guidelines
- Commits: concise, imperative subjects (e.g., `Add vacancy filters`, `Fix login token refresh`) and scope per app.
- PRs: include a short summary, linked ticket, UI screenshots/GIFs, and call out environment or i18n changes. Ensure `npm run build` and `npm test` pass for the apps you touched before requesting review.

## Environment & Configuration Tips
- Do not commit secrets. Keep API/auth config in `environment.ts` and `environment.prod.ts`; avoid scattering URLs in components.
