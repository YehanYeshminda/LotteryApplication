import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Company } from "../models/lotto";
import { CookieService } from "ngx-cookie-service";
import { environment } from "@env/environment.development";

@Injectable({
    providedIn: 'root'
})
export class CompanyHttpService {
    baseUrl = environment.apiUrl;

    constructor(private http: HttpClient, private cookieService: CookieService) { }

    getCompany(): Observable<Company[]> {
        return this.http.get<Company[]>(this.baseUrl + "Company/GetAllCompany")
    }
}
