import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {HttpService} from "../http/http.service";

export interface AppVersion {
  version: string;
  commit: string;
  builtAt: string;
  env: string;
  buildNo: number;
}

@Injectable({providedIn: 'root'})
export class VersionService {
  constructor(private http: HttpService) {
  }

  get() {
    return this.http.get<AppVersion>('assets/version.json');
  }
}
