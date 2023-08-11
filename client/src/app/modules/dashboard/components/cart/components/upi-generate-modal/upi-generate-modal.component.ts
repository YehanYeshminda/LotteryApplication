import { Component } from '@angular/core';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-upi-generate-modal',
  templateUrl: './upi-generate-modal.component.html',
  styleUrls: ['./upi-generate-modal.component.scss']
})
export class UpiGenerateModalComponent {
  constructor(public bsModalRef: BsModalRef) { }

  public elementType: NgxQrcodeElementTypes = NgxQrcodeElementTypes.CANVAS;
  qrValue = ''
  public errorCorrectionLevel: NgxQrcodeErrorCorrectionLevels = NgxQrcodeErrorCorrectionLevels.LOW;
}
