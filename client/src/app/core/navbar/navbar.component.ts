import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  constructor(private router: Router) {}
  
  routeEasyDraw() {
    console.log('hit routeEasyDraw');
    this.router.navigateByUrl('/dashboard/easy-draw');
  }

  routeMegaDraw() {
    console.log('hit routeEasyDraw');
    this.router.navigateByUrl('/dashboard/mega-draw');
  }
}
