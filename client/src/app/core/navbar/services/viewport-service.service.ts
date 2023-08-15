import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ViewportService {
  private viewportWidthSubject = new BehaviorSubject<number>(window.innerWidth);

  viewportWidth$ = this.viewportWidthSubject.asObservable();

  constructor() {
    window.addEventListener('resize', this.onResize.bind(this));
  }

  private onResize(event: any) {
    this.viewportWidthSubject.next(event.target.innerWidth);
  }
}
