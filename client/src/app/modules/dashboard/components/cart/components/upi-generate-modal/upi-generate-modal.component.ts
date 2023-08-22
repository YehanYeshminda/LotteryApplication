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
    this.bsModalRef.hide();
  }
}
