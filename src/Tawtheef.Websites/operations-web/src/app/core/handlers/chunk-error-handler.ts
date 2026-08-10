import { ErrorHandler, Injectable } from '@angular/core';

/**
 * Global error handler that catches chunk-load failures caused by stale cached
 * references to old lazy-loaded chunks after a new deployment.
 *
 * When a chunk load error is detected, it performs a single full page reload
 * so the browser fetches the latest index.html (which references the new chunks).
 * A sessionStorage flag prevents infinite reload loops.
 */
@Injectable()
export class ChunkErrorHandler implements ErrorHandler {
  private static readonly RELOAD_KEY = 'chunk_reload_attempted';

  handleError(error: any): void {
    const chunkFailedPattern =
      /Loading chunk [\d]+ failed|Failed to fetch dynamically imported module|Failed to load module script/i;

    if (chunkFailedPattern.test(error?.message || '')) {
      // Prevent infinite reload loops — only retry once per session
      if (!sessionStorage.getItem(ChunkErrorHandler.RELOAD_KEY)) {
        sessionStorage.setItem(ChunkErrorHandler.RELOAD_KEY, 'true');
        window.location.reload();
        return;
      }
    }

    // Default: log to console
    console.error(error);
  }
}
