import { Injectable, Inject } from '@angular/core';
import { fromEvent } from 'rxjs';
import { filter } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class KeyboardService {
  constructor() {}
  listenFor(ctrlKey: boolean, key: string, handler: () => void) {
    fromEvent<KeyboardEvent>(document, 'keydown').pipe(
      filter(e => e.key.toLowerCase() === key.toLowerCase() && (!!e.ctrlKey === ctrlKey))
    ).subscribe(handler);
  }
}
