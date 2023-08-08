import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {CookieService} from "ngx-cookie-service";
import {Observable} from "rxjs";
import {UserPackage} from "../models/user-package";
import {AuthDetails} from "@shared/models/auth";
import {environment} from "@env/environment.development";
import {getAuthDetails} from "@shared/methods/methods";

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
