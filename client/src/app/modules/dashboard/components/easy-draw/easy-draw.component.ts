import { Component } from '@angular/core';
import {EasyDrawHttpService} from "./services/easy-draw-http.service";
import {Observable, of} from "rxjs";
import {getAuthDetails} from "../../../../shared/methods/methods";
import {EasyDrawResponse} from "./models/EasyDrawResponse";

@Component({
  selector: 'app-easy-draw',
  templateUrl: './easy-draw.component.html',
  styleUrls: ['./easy-draw.component.scss']
})
export class EasyDrawComponent {
  numbers$ : Observable<EasyDrawResponse> = of();
  constructor(private easyDrawHttpService: EasyDrawHttpService) {}

  drawRandomNumber() {
    const authDetails = getAuthDetails();

    if (authDetails) {
      this.numbers$ = this.easyDrawHttpService.getEasyDrawNumbers(authDetails);
    }
  }

}
