import { Component, OnInit } from '@angular/core';
import { StatusCheckHttpService } from './services/status-check-http.service';
import { Observable, of } from 'rxjs';
import { StatusCheckData } from './models/statuscheck';
import { errorNotification, successNotification } from '@shared/alerts/sweetalert';

@Component({
  selector: 'app-status-check',
  templateUrl: './status-check.component.html',
  styleUrls: ['./status-check.component.scss']
})
export class StatusCheckComponent implements OnInit {
  statusCheckResult$: Observable<StatusCheckData[]> = of([]);
  showStatus: boolean = false;
  paymentProcessing: boolean = false;
  constructor(private statusCheckHttpService: StatusCheckHttpService) { }

  ngOnInit(): void {
    this.statusCheckResult$ = this.statusCheckHttpService.getAllStatus();

    this.statusCheckResult$.subscribe({
      next: response => {
        if (response.length > 0) {
          this.showStatus = true;
        } else {
          this.showStatus = false;
        }
      }
    })
  }

  updateStatusCheckForPayments(orderReferenceId: string) {
    if (orderReferenceId !== '') {
      this.paymentProcessing = true;
      this.statusCheckHttpService.updateStatus(orderReferenceId).subscribe({
        next: response => {
          if (response.status == '1') {
            this.paymentProcessing = false;
            successNotification('Account Topup Successful');
          } else {
            this.paymentProcessing = false;
            errorNotification('Account Topup Failed');
          }
        },
        error: error => {
          this.paymentProcessing = false;
          console.log(error);
          errorNotification('Account Topup Failed');
        }
      });
    }
  }
}
