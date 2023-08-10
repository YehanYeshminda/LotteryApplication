import { Component, OnInit } from '@angular/core';
import { CartReponse } from "./models/cart";
import { Observable, map, of, tap } from "rxjs";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { confirmDeleteNotification, errorNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from './services/cart-entity.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DataToSendUpi, GetUpiPerson, UpiGenerateHttpService } from './services/upi-generate-http.service';
import { CartHttpService } from './services/cart-http.service';
import { NgxQrcodeElementTypes, NgxQrcodeErrorCorrectionLevels } from '@techiediaries/ngx-qrcode';

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
  upiUser$: Observable<GetUpiPerson> = of();
  public elementType: NgxQrcodeElementTypes = NgxQrcodeElementTypes.CANVAS;
  qrValue = ''
  public errorCorrectionLevel: NgxQrcodeErrorCorrectionLevels = NgxQrcodeErrorCorrectionLevels.LOW;

  constructor(private cookieService: CookieService, private cartEntityService: CartEntityService, private fb: FormBuilder, private upiHttpService: UpiGenerateHttpService, private cartHttpService: CartHttpService) { }

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
    this.cartHttpService.removeAllFromCart().subscribe({
      next: response => {
        if (response.packageId) {
          this.generateUPI(response.packageId);
        }
      }
    })
  }

  // do this after the order is placed and make sure to call this inside of a sub place
  generateUPI(orderNo: string) {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.upiHttpService.getUPIUser().subscribe({
        next: userResponse => {
          if (userResponse.username !== "") {
            this.upiHttpService.getAuthDetails(userResponse).subscribe({
              next: authResponse => {
                if (authResponse.success) {
                  const user = getAuthDetails(this.cookieService.get('user'))

                  if (user != null) {
                    const sendData: DataToSendUpi = {
                      amount: this.total.toString(),
                      Email: user.email,
                      Name: user.username,
                      Phone: user.phone,
                      ReferenceId: orderNo,
                    }

                    this.upiHttpService.generateUpi(sendData, authResponse.data.token, userResponse.password).subscribe({
                      next: upiResponse => {
                        if (upiResponse.success) {
                          this.qrValue = upiResponse.data.qr;
                        } else {
                          console.error("error while getting upi " + upiResponse.message);
                        }
                      }
                    })
                  }
                } else {
                  console.error("error while getting auth " + authResponse.message);
                }
              },
              error: error => {
                console.error("Inside of get auth " + error);
              }
            })
          }
        },
        error: error => {
          console.error("Inside of database auth get ", error);
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
