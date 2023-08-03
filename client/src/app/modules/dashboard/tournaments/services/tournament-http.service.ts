import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {TournamentDrawsInterface} from "../models/tournament";
import {environment} from "../../../../../environments/environment.development";
import {AuthDetails} from "../../../../shared/models/auth";
import {getAuthDetails} from "../../../../shared/methods/methods";
import {CookieService} from "ngx-cookie-service";

@Injectable({
  providedIn: 'root'
})
export class TournamentHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getAllDraws(): Observable<TournamentDrawsInterface[]> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<TournamentDrawsInterface[]>(this.baseUrl + "Draw/GetAllRaffles", auth)
  }
}
