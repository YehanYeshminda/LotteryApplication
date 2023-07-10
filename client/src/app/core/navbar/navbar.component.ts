import { Component } from '@angular/core';
import { Router } from '@angular/router';
import {Store} from "@ngrx/store";
import {AppState} from "../../reducer";
import {logout} from "../../modules/dashboard/auth/features/auth.actions";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  constructor(private router: Router, private store: Store<AppState>) {}

  logOut() {
    this.store.dispatch(logout());
  }
}
