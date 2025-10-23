import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {EndpointsService} from "../http/endpoints.service";

@Injectable({providedIn: 'root'})
export class ProfileService {

  constructor(
    private http: HttpClient,
    private endpoints: EndpointsService,
  ) {
  }
  getSocialAccounts(): Observable<any> {
    return this.http.get<any>(this.endpoints.user.profile.socialAccounts);
  }
}
