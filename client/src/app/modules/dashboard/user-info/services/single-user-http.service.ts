import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CookieService } from "ngx-cookie-service";
import { getAuthDetails } from "src/app/shared/methods/methods";
import { AuthDetails } from "src/app/shared/models/auth";
import { environment } from "src/environments/environment.development";
import { SingleUserInfo, UpdateSingleUserInfo } from "../models/single-user";

@Injectable()
export class SingleUserHttpService {
    baseUrl = environment.apiUrl;
    constructor(private http: HttpClient, private cookieService: CookieService) { }

    getSingleUserInfo() {
        const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
        return this.http.post<SingleUserInfo>(this.baseUrl + 'Account/GetUserInfo', auth)
    }

    updateSingleUserHistory(updateDetails: UpdateSingleUserInfo) {
        return this.http.post<SingleUserInfo>(this.baseUrl + 'Account/UpdateUserInfo', updateDetails)
    }
}