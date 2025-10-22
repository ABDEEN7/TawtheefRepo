# File Tree: Recruitment.Web

**Generated:** 10/22/2025, 12:21:23 PM
**Root Path:** `c:\Users\t-a.jaber\RiderProjects\Tawtheef\src\Tawtheef.Websites\Recruitment.Web`

```
├── src
│   ├── app
│   │   ├── core
│   │   │   ├── auth
│   │   │   │   └── auth.service.ts
│   │   │   ├── guards
│   │   │   │   └── auth.guard.ts
│   │   │   ├── http
│   │   │   │   ├── endpoints.service.ts
│   │   │   │   └── http.service.ts
│   │   │   ├── interceptors
│   │   │   │   ├── auth.interceptor.ts
│   │   │   │   ├── error.interceptor.ts
│   │   │   │   ├── loading.interceptor.ts
│   │   │   │   └── refresh.interceptor.ts
│   │   │   ├── models
│   │   │   ├── rxjs
│   │   │   │   └── retry-backoff.ts
│   │   │   ├── services
│   │   │   │   ├── file.service.ts
│   │   │   │   ├── keyboard.service.ts
│   │   │   │   ├── language.service.ts
│   │   │   │   ├── loading.service.ts
│   │   │   │   ├── logger.service.ts
│   │   │   │   ├── notification.service.ts
│   │   │   │   ├── translate.service.ts
│   │   │   │   └── version.service.ts
│   │   │   ├── utils
│   │   │   │   ├── case-utils.ts
│   │   │   │   ├── date-utils.ts
│   │   │   │   ├── file-utils.ts
│   │   │   │   ├── guid-utils.ts
│   │   │   │   ├── object-extensions.ts
│   │   │   │   ├── password-manager.ts
│   │   │   │   └── string-utils.ts
│   │   │   └── core.module.ts
│   │   ├── features
│   │   │   ├── auth
│   │   │   │   ├── callback
│   │   │   │   │   └── callback.component.ts
│   │   │   │   ├── login
│   │   │   │   │   ├── components
│   │   │   │   │   │   ├── login.component.html
│   │   │   │   │   │   ├── login.component.scss
│   │   │   │   │   │   └── login.component.ts
│   │   │   │   │   ├── services
│   │   │   │   │   └── store
│   │   │   │   ├── register
│   │   │   │   ├── auth-routing.module.ts
│   │   │   │   └── auth.module.ts
│   │   │   └── dashboard
│   │   ├── pages
│   │   │   ├── error
│   │   │   │   ├── access-denied
│   │   │   │   │   ├── access-denied.component.html
│   │   │   │   │   ├── access-denied.component.scss
│   │   │   │   │   ├── access-denied.component.spec.ts
│   │   │   │   │   └── access-denied.component.ts
│   │   │   │   ├── come-soon
│   │   │   │   │   ├── come-soon-routing.module.ts
│   │   │   │   │   ├── come-soon.component.html
│   │   │   │   │   ├── come-soon.component.scss
│   │   │   │   │   ├── come-soon.component.spec.ts
│   │   │   │   │   ├── come-soon.component.ts
│   │   │   │   │   └── come-soon.module.ts
│   │   │   │   ├── error404
│   │   │   │   │   ├── error404-routing.module.ts
│   │   │   │   │   ├── error404.component.html
│   │   │   │   │   ├── error404.component.scss
│   │   │   │   │   ├── error404.component.spec.ts
│   │   │   │   │   ├── error404.component.ts
│   │   │   │   │   └── error404.module.ts
│   │   │   │   ├── error500
│   │   │   │   │   ├── error500-routing.module.ts
│   │   │   │   │   ├── error500.component.html
│   │   │   │   │   ├── error500.component.scss
│   │   │   │   │   ├── error500.component.spec.ts
│   │   │   │   │   ├── error500.component.ts
│   │   │   │   │   └── error500.module.ts
│   │   │   │   ├── under-construction
│   │   │   │   │   ├── under-construction-routing.module.ts
│   │   │   │   │   ├── under-construction.component.html
│   │   │   │   │   ├── under-construction.component.scss
│   │   │   │   │   ├── under-construction.component.spec.ts
│   │   │   │   │   ├── under-construction.component.ts
│   │   │   │   │   └── under-construction.module.ts
│   │   │   │   ├── error-routing.module.ts
│   │   │   │   ├── error.component.html
│   │   │   │   ├── error.component.scss
│   │   │   │   ├── error.component.spec.ts
│   │   │   │   ├── error.component.ts
│   │   │   │   └── error.module.ts
│   │   │   └── home
│   │   │       └── home
│   │   │           ├── home.component.css
│   │   │           ├── home.component.html
│   │   │           └── home.component.ts
│   │   ├── shared
│   │   │   ├── components
│   │   │   │   ├── date-range-picker
│   │   │   │   │   ├── date-range-picker.component.html
│   │   │   │   │   ├── date-range-picker.component.scss
│   │   │   │   │   ├── date-range-picker.component.spec.ts
│   │   │   │   │   ├── date-range-picker.component.ts
│   │   │   │   │   └── date-range-picker.module.ts
│   │   │   │   ├── notification
│   │   │   │   ├── progress-bar
│   │   │   │   │   ├── progress-bar.component.html
│   │   │   │   │   ├── progress-bar.component.scss
│   │   │   │   │   ├── progress-bar.component.spec.ts
│   │   │   │   │   └── progress-bar.component.ts
│   │   │   │   └── toasts
│   │   │   │       ├── toasts.component.html
│   │   │   │       └── toasts.component.ts
│   │   │   ├── dialogs
│   │   │   │   └── confirmation-dialog
│   │   │   │       ├── confirmation-dialog.component.html
│   │   │   │       └── confirmation-dialog.component.ts
│   │   │   ├── directives
│   │   │   │   └── localized.directive.ts
│   │   │   ├── pipes
│   │   │   │   ├── bytes.pipe.ts
│   │   │   │   ├── capitalize-words.pipe.ts
│   │   │   │   ├── localized.pipe.ts
│   │   │   │   ├── pascal-case.pipe.ts
│   │   │   │   ├── safe-html.pipe.ts
│   │   │   │   ├── sort.pipe.ts
│   │   │   │   └── truncate.pipe.ts
│   │   │   └── validator
│   │   │       ├── confirm-password-validator.ts
│   │   │       ├── date-range.validator.ts
│   │   │       └── file-image.validator.ts
│   │   ├── store
│   │   │   ├── auth.action.ts
│   │   │   ├── auth.effects.ts
│   │   │   ├── auth.reducer.ts
│   │   │   └── auth.selectors.ts
│   │   ├── app-routing.module.ts
│   │   ├── app.component.html
│   │   ├── app.component.ts
│   │   └── app.module.ts
│   ├── assets
│   │   ├── i18n
│   │   │   ├── ar.json
│   │   │   └── en.json
│   │   ├── icons
│   │   └── .gitkeep
│   ├── environments
│   │   ├── environment.prod.ts
│   │   └── environment.ts
│   ├── styles
│   │   └── styles.scss
│   ├── index.html
│   ├── main.ts
│   └── polyfills.ts
├── .editorconfig
├── .gitignore
├── README.md
├── angular.json
├── package-lock.json
├── package.json
├── recruitment-web.esproj
├── tsconfig.app.json
├── tsconfig.json
└── tsconfig.spec.json
```

---
*Generated by FileTree Pro Extension*
