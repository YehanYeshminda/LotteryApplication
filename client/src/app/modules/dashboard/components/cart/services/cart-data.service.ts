import { Injectable } from "@angular/core";
import { DefaultDataService, HttpUrlGenerator } from "@ngrx/data";
import { CartReponse } from "../models/cart";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment.development"; // Make sure to import HttpOptions
import { AuthDetails } from "src/app/shared/models/auth";
import { getAuthDetails } from "src/app/shared/methods/methods";
import { CookieService } from "ngx-cookie-service";
import { HttpOptions } from "@ngrx/data/src/dataservices/interfaces";

@Injectable()
export class CartDataService extends DefaultDataService<CartReponse> {
    baseUrl = environment.apiUrl;
    constructor(http: HttpClient, httpUrlGenerator: HttpUrlGenerator, private cookieService: CookieService) {
        super('Cart', http, httpUrlGenerator);
    }

    override getAll(): Observable<CartReponse[]> {
        const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
        return this.http.post<CartReponse[]>(this.baseUrl + "Cart/GetCartItems", auth);
    }

    override add(cart: CartReponse): Observable<CartReponse> {
        return this.http.post<CartReponse>(this.baseUrl + "Cart/AddToCart", cart);
    }

    override delete(key: string | number, options?: HttpOptions) {
        const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
        const deleteObject = {
            lotteryId: key,
            authDto: auth
        }
        return this.http.post<string | number>(this.baseUrl + "Cart/DeleteFromCart", deleteObject);
    }
}
