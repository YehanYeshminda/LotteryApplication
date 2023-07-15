import { Injectable } from "@angular/core";
import { DefaultDataService, HttpUrlGenerator } from "@ngrx/data";
import { Home } from "../models/home";
import { environment } from "src/environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { CookieService } from "ngx-cookie-service";
import { AppState } from "src/app/reducer";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs";
import { getAuthDetails } from "src/app/shared/methods/methods";
import { AuthDetails } from "src/app/shared/models/auth";

@Injectable()
export class HomeDataService extends DefaultDataService<Home> {
    baseUrl = environment.apiUrl;
    constructor(http: HttpClient, httpUrlGenerator: HttpUrlGenerator, private cookieService: CookieService) {
        super('Home', http, httpUrlGenerator);
    }

    override getAll(): Observable<Home[]> {
        const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
        return this.http.post<Home[]>(this.baseUrl + "Home/GetHomeInfo", auth);
    }
}