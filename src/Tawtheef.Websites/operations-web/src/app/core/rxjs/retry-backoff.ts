import { MonoTypeOperatorFunction, throwError, timer } from 'rxjs';
import { mergeMap, retryWhen, scan } from 'rxjs/operators';


export function retryBackoff<T>(maxRetries = 3, initialDelayMs = 500): MonoTypeOperatorFunction<T> {
  return (src) => src.pipe(
    retryWhen(errors => errors.pipe(
      scan((acc, error) => {
        const attempt = acc + 1;
        if (attempt > maxRetries) {
          throw error;
        }
        return attempt;
      }, 0),
      mergeMap(attempt => timer(initialDelayMs * Math.pow(2, attempt - 1)))
    ))
  );
}
