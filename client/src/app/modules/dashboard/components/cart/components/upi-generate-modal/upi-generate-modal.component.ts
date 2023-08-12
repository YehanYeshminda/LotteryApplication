import { Component } from '@angular/core';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { StatusCheckHttpService } from '../../../status-check/services/status-check-http.service';
import { errorNotification, successNotification } from '@shared/alerts/sweetalert';

@Component({
  selector: 'app-upi-generate-modal',
  templateUrl: './upi-generate-modal.component.html',
  styleUrls: ['./upi-generate-modal.component.scss']
})
export class UpiGenerateModalComponent {
  public elementType: NgxQrcodeElementTypes = NgxQrcodeElementTypes.CANVAS;
  qrValue = ''
  packageId = ''
  paymentProcessing = false;
  public errorCorrectionLevel: NgxQrcodeErrorCorrectionLevels = NgxQrcodeErrorCorrectionLevels.LOW;

  constructor(public bsModalRef: BsModalRef, private statusCheckHttpService: StatusCheckHttpService) { }

  updateStatusCheckForPayments() {
    if (this.packageId !== '') {
      this.paymentProcessing = true;
      this.statusCheckHttpService.updateStatus(this.packageId).subscribe({
        next: response => {
          if (response.status == '1') {
            this.bsModalRef.hide();
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
