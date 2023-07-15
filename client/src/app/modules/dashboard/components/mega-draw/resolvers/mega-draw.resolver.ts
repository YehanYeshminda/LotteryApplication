import { Injectable } from '@angular/core';
import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { MegaDrawHttpService } from '../services/mega-draw-http.service';
import { MegaDrawResponse } from '../models/megaDraw';

@Injectable()
export class MegaDrawResolver implements Resolve<number[]> {
    constructor(private megaDrawHttpService: MegaDrawHttpService) { }

    until = 31;
    storageKey = 'megaDrawNumbers'; // A key for storing the draw numbers in sessionStorage

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<number[]> {
        const storedDrawNumbers = sessionStorage.getItem(this.storageKey);

        if (storedDrawNumbers) {
            // If draw numbers exist in sessionStorage, parse and return them
            return of(JSON.parse(storedDrawNumbers));
        } else {
            // Fetch draw numbers from the database using the HTTP service
            return this.megaDrawHttpService.getDraws(this.until).pipe(
                map((response: MegaDrawResponse) => response.arr), // Extract the draw numbers from the response
                tap((drawNumbers: number[]) => {
                    // Store the fetched draw numbers in sessionStorage
                    sessionStorage.setItem(this.storageKey, JSON.stringify(drawNumbers));
                }),
                catchError((error) => {
                    // Handle any errors during the HTTP request if needed
                    console.error('Error fetching draw numbers:', error);
                    return of([]); // Return an empty array as fallback
                })
            );
        }
    }
}
