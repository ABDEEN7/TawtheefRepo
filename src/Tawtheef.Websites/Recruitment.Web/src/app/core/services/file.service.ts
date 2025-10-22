import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {HttpService} from "../http/http.service";

@Injectable({ providedIn: 'root' })
export class FileService {
  constructor(private http: HttpService) {}

  upload(url: string, file: File): Observable<number> {
    const form = new FormData();
    form.append('file', file);
    const req = new HttpRequest('POST', url, form, { reportProgress: true });

    return this.http.request(req).pipe(
      map((event: HttpEvent<any>) => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          return Math.round((100 * event.loaded) / event.total);
        }
        return 0;
      })
    );
  }

  download(url: string) {
    return this.http.get(url, { responseType: 'blob' });
  }
}
