import {Injectable} from '@angular/core';
import {of} from "rxjs";
import {catchError} from "rxjs/operators";
import {HttpClient} from "@angular/common/http";
import {EndpointsService} from '../http/endpoints.service';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  constructor(private http: HttpClient, private endpoints: EndpointsService) {
  }
  logError(context: string, error: any, metadata?: any) {
    const timestamp = new Date().toISOString();
    const errorData = {
      timestamp,
      context,
      error: this.serializeError(error),
      metadata
    };
    return this.sendToBackend(errorData);
  }

  private serializeError(error: any): any {
    if (error instanceof Error) {
      return {
        name: error.name,
        message: error.message,
        stack: error.stack
      };
    }
    return error;
  }

  private sendToBackend(errorData: any) {
    return this.http.post(this.endpoints.logger, errorData).pipe(
      catchError(() => of(void 0))
    );
  }
}
