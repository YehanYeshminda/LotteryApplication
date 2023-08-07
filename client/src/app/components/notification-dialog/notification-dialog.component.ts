import {Component, OnInit} from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import {GetNotificationResponse} from "../../shared/alerts/send-notification-http.service";

@Component({
  selector: 'modal-content',
  template: `
      <div class="modal-header">
        <h4 class="modal-title pull-left">Report on Lotteries</h4>
        <button type="button" class="btn-close close pull-right" aria-label="Close" (click)="bsModalRef.hide()">
          <span aria-hidden="true" class="visually-hidden">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <ng-container *ngIf="formData.length > 0">
          <div *ngFor="let item of formData">
            Number: {{ item.no }}
            Count: {{ item.count }}
          </div>

          <div class="">The best number to choose is: {{ bestNumber }} with number count of: {{ bestNumberCount }}</div>
        </ng-container>
      </div>
  `
})
export class NotificationDialogComponent implements OnInit {
  formData: GetNotificationResponse[] = [];
  bestNumber = 0;
  bestNumberCount = 0;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {
    if (this.formData.length > 0) {
      const leastCountNumber = this.formData.reduce((prev, curr) => {
        return curr.count < prev.count ? curr : prev;
      });

      this.bestNumber = leastCountNumber.no;
      this.bestNumberCount = leastCountNumber.count;
    }
  }
}
