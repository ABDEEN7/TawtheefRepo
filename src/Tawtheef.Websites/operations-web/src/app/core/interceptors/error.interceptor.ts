import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { HDR } from '../utils/headers.flags';
import { inject, NgZone } from '@angular/core';
import { catchError, switchMap } from 'rxjs/operators';
import { from, throwError } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.get(HDR.SkipError) === 'true') return next(req);

  const msg = inject(NotificationService);
  const zone = inject(NgZone);
  const translate = inject(TranslateService);

  return next(req).pipe(
    catchError((err: unknown) => {
      return from(readErrorBody(err)).pipe(
        switchMap((serverBody) => {
          if (!(err instanceof HttpErrorResponse)) {
            return throwError(() => err);
          }
          const isInfraAuth = /\/auth\/(login|refresh|logout|external)/i.test(req.url);
          const external = req.url.includes('ipapi.co');

          if (isInfraAuth || external || req.headers.get(HDR.LogoutFlow) === 'true') {
            return throwError(() => err);
          }

          zone.run(() => {
            if (err.status === 0) {
              msg.error('Please check your connection and try again.', 'Network error');
              return;
            }

            const html = buildErrorHtml(serverBody);
            msg.error(html);
          });

          return throwError(() => err);
        }
        ));
    })
  );

  function buildErrorHtml(apiError: any): string {
    if (!apiError) return translate.instant('server-error.UN_EXPECTED_ERROR');

    const ticket = apiError.ticket || apiError.extensions?.ticket || apiError.traceId;
    const items: string[] = [];

    // Handle ASP.NET Core Validation Errors (ProblemDetails format)
    if (apiError.errors && typeof apiError.errors === 'object' && !Array.isArray(apiError.errors)) {
      for (const key in apiError.errors) {
        if (Object.prototype.hasOwnProperty.call(apiError.errors, key)) {
          const errorValue = apiError.errors[key];
          if (Array.isArray(errorValue)) {
            errorValue.forEach((msg: string) => items.push(`• ${tryLocalizedMessage(msg)}`));
          } else if (typeof errorValue === 'string') {
            items.push(`• ${tryLocalizedMessage(errorValue)}`);
          }
        }
      }
    }

    // Handle other error array formats
    if (items.length === 0 && Array.isArray(apiError.error)) {
      for (const err of apiError.error) {
        if (Array.isArray(err.reasons) && err.reasons.length > 0) {
          err.reasons.forEach((r: string) => items.push(`• ${tryLocalizedMessage(r)}`));
        }
        else if (err.message) {
          items.push(`• ${tryLocalizedMessage(err.message)}`);
        }
      }
    }

    if (items.length > 0) return items.join('\n');

    // Handle single detail or message
    const errorKey = apiError.detail || apiError.message;
    if (errorKey) {
      if (typeof errorKey === 'string' && errorKey.startsWith('PREVIOUS_PROFILE_STEP_INCOMPLETE:')) {
        return handlePartialProfileError(errorKey);
      }

      const translated = translate.instant(`server-error.${errorKey}`, { ticket });
      return (translated !== `server-error.${errorKey}`) ? translated : errorKey;
    }

    return translate.instant('server-error.UN_EXPECTED_ERROR', { ticket });
  }

  function handlePartialProfileError(rawKey: string): string {
    const parts = rawKey.split(':');
    const step = parts[1]; // e.g. Personal
    const fields = parts[2] ? parts[2].split(',') : [];

    const stepNameKey = `wizard.steps.${step.toLowerCase()}`;
    let stepName = translate.instant(stepNameKey);
    if (stepName === stepNameKey) {
      stepName = step;
    }

    let result = translate.instant('server-error.PREVIOUS_PROFILE_STEP_INCOMPLETE', { step: stepName });

    if (fields.length > 0) {
      const fieldMessages = fields.map(f => {
        const possibleKeys = [
          `wizard.profile.${step.toLowerCase()}.${f}.required`,
          `wizard.profile.${step.toLowerCase()}.${f}`,
          `wizard.personal.${f}`,      // common prefix for some
          `wizard.contact.${f}`,       // common prefix for some
          `wizard.${f}`,
          f
        ];

        for (const k of possibleKeys) {
          const t = translate.instant(k);
          if (t !== k) return `• ${t}`;
        }
        return `• ${f}`;
      });
      result += '\n' + fieldMessages.join('\n');
    }

    return result;
  }

  function tryLocalizedMessage(key: string): string {
    if (typeof key === 'string' && key.startsWith('PREVIOUS_PROFILE_STEP_INCOMPLETE:')) {
      return handlePartialProfileError(key);
    }

    const translateValue = translate.instant(`server-error.${key}`);
    if (translateValue !== key) {
      return translateValue;
    }
    return key;
  }

  async function readErrorBody(err: unknown): Promise<any> {
    const httpErr = err as HttpErrorResponse;
    const e = httpErr?.error;

    if (e instanceof Blob) {
      const text = await e.text();      // Blob -> string
      try { return JSON.parse(text); }  // string -> JSON (if valid)
      catch { return text; }            // fallback: raw text
    }

    if (typeof e === 'string') {
      try { return JSON.parse(e); } catch { return e; }
    }

    if (e && typeof e === 'object') return e;

    return null;
  }
};
