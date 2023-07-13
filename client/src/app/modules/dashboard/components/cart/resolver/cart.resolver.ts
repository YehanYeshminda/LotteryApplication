import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable, filter, first, map, tap } from "rxjs";
import { CartEntityService } from "../services/cart-entity.service";

@Injectable()
export class CartResolver implements Resolve<boolean> {
    constructor(private cartEntityService: CartEntityService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.cartEntityService.loaded$
            .pipe(
                tap(loaded => {
                    if (!loaded) {
                        this.cartEntityService.getAll();
                    }
                }),
                filter(loaded => !!loaded),
                first()
            )
    }
}