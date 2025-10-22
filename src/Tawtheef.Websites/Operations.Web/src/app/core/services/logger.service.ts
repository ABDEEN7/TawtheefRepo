import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {EndpointsService} from "../http/endpoints.service";

@Injectable({ providedIn: 'root' })
export class LoggerService {
  constructor(private http: HttpClient, private endpoints: EndpointsService) {
  }
  logError(context: string, error: any, metadata?: any): void {
    const timestamp = new Date().toISOString();
    const errorData = {
      timestamp,
      context,
      error: this.serializeError(error),
      metadata
    };

    // Console logging (development)
    console.error('🚨 Upload Error:', errorData);

    // Send to backend (production)
    this.sendToBackend(errorData).subscribe();
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
    // Implement your backend error logging endpoint
    return this.http.post(this.endpoints.base + '/api/errors', errorData).pipe()
  }
}
