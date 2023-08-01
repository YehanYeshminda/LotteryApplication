import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../../environments/environment.development";
import {CookieService} from "ngx-cookie-service";
import {getAuthDetails} from "../../../../shared/methods/methods";
import {AuthDetails} from "../../../../shared/models/auth";
import {Observable} from "rxjs";
import {UserPackage} from "../models/user-package";

@Injectable({
  providedIn: 'root'
})
export class UserPackageHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getAllPackagesForUser() : Observable<UserPackage[]> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<UserPackage[]>(this.baseUrl + "Package", auth)
  }
}
