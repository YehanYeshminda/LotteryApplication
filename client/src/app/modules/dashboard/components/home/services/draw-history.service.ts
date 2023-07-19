import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CookieService } from "ngx-cookie-service";
import { OldRafflesReponse } from "../models/home";
import { environment } from "src/environments/environment.development";
import { getAuthDetails } from "src/app/shared/methods/methods";
import { AuthDetails } from "src/app/shared/models/auth";

@Injectable({
    providedIn: 'root'
})
export class DrawHistoryHttpService {
    baseUrl = environment.apiUrl;
    constructor(private http: HttpClient, private cookieService: CookieService) { }

    getAllDrawHistory() {
        const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
        return this.http.post<OldRafflesReponse>(this.baseUrl + 'Draw/GetAllDrawHistories', auth)
    }
}