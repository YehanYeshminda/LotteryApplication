import { Component, OnInit } from '@angular/core';
import { CartReponse } from "./models/cart";
import { Observable, map, of, tap } from "rxjs";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { confirmApproveNotification, confirmDeleteNotification, errorNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from './services/cart-entity.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UpiGenerateHttpService } from './services/upi-generate-http.service';
import { CartHttpService } from './services/cart-http.service';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';
import { GenerateUpi } from './models/upi';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { UpiGenerateModalComponent } from './components/upi-generate-modal/upi-generate-modal.component';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems$: Observable<CartReponse[]> = of([]);
  total = 0;
  orderForm: FormGroup = new FormGroup({});
  paymentForm: FormGroup = new FormGroup({});
  confirmationForm: FormGroup = new FormGroup({});
  index: any;
  bsModalRef?: BsModalRef;
  loading: boolean = false;

  constructor(private cookieService: CookieService, private cartEntityService: CartEntityService, private fb: FormBuilder, private upiHttpService: UpiGenerateHttpService, private cartHttpService: CartHttpService, private modalService: BsModalService) { }

  removeCartItem(item: CartReponse): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      confirmDeleteNotification('Are you sure you want to remove from cart?').then((result) => {
        if (result.isConfirmed) {
          const itemId: string | number = item.id || '';
          this.cartEntityService.delete(itemId).subscribe({
            next: (response) => {
            },
            error: (error) => {
              console.error(error);
            }
          });
        }
      })
    } else {
      errorNotification('Please login to remove from cart');
    }
  }

  purchaseItems() {
    confirmApproveNotification('Are you sure you want to purchase these items?').then((result) => {
      if (result.isConfirmed) {
        this.loading = true;
        this.cartHttpService.removeAllFromCart().subscribe({
          next: response => {
            if (response.packageId) {
              this.generateUPI(response.packageId);
            }
          }
        })
      }
    });
  }

  openModalWithComponent(qr: string, orderNo: string) {
    const initialState: ModalOptions = {
      initialState: {
        qrValue: qr,
        packageId: orderNo
      }
    };
    this.bsModalRef = this.modalService.show(UpiGenerateModalComponent, initialState);
  }

  generateUPI(orderNo: string) {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      const data: GenerateUpi = {
        authDto: getAuthDetails(this.cookieService.get('user')),
        orderNo: orderNo,
        total: this.total.toString()
      }

      this.upiHttpService.getUpiQrCode(data).subscribe({
        next: response => {
          this.loading = false;
          this.openModalWithComponent(response.qr, orderNo);
          this.cartEntityService.clearCache();
        },
        error: error => {
          this.loading = true;
          console.error(error);
        }
      });
    } else {
      errorNotification('Please login to generate UPI');
    }
  }

  ngOnInit(): void {
    this.orderForm = this.fb.group({
      firstCtrl: [''],
    });

    this.paymentForm = this.fb.group({
      secondCtrl: [''],
    });

    this.confirmationForm = this.fb.group({
      secondCtrl: [''],
    });

    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems$ = this.cartEntityService.entities$.pipe(
        tap(response => {
          this.total = 0;
        }),
        map(response => {
          this.total = response.reduce((acc, item) => acc + item.paid, 0);
          return response;
        })
      );
    }
  }
}
