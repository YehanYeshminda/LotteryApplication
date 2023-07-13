import { Component, ViewChild } from '@angular/core';
import { errorNotification, successNotification } from "../../../../shared/alerts/sweetalert";
import { CartHttpService } from "../cart/services/cart-http.service";
import { FormBuilder, FormGroup } from '@angular/forms';
import { StripeCardElementOptions, StripeElementsOptions } from '@stripe/stripe-js';
import { StripeCardComponent, StripeService } from 'ngx-stripe';
import { HttpClient } from '@angular/common/http';
import { Observable, of, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { AuthDetails } from 'src/app/shared/models/auth';
import { Cart, CartReponse } from '../cart/models/cart';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent {
  @ViewChild(StripeCardComponent) card!: StripeCardComponent;
  authInformartion: AuthDetails | null = null;
  totalAmountPay: Observable<number> = of(0);
  amount: number = 0;
  cartItems: Observable<CartReponse[]> = of([]);

  cardOptions: StripeCardElementOptions = {
    style: {
      base: {
        iconColor: '#666EE8',
        color: '#31325F',
        fontWeight: '300',
        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
        fontSize: '18px',
        '::placeholder': {
          color: 'lightblack',
        },
      },
    },
  };

  elementsOptions: StripeElementsOptions = {
    locale: 'en',
    loader: 'always',
  };

  stripePayment!: FormGroup;

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private stripeService: StripeService,
    private cookieService: CookieService,
    private cartHttpService: CartHttpService,
  ) { }

  ngOnInit(): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems = this.cartHttpService.getCartItems(getAuthDetails(this.cookieService.get('user')));
      this.authInformartion = getAuthDetails(this.cookieService.get('user'));
      this.totalAmountPay = this.cartHttpService.getTotal()

      this.stripePayment = this.fb.group({
        name: [this.authInformartion?.username, []],
        amount: [this.cartHttpService.getTotal(), []],
      });

      this.cartHttpService.getTotal().subscribe({
        next: respose => {
          this.amount = respose;
        }
      });
    }
  }

  pay(): void {
    if (this.stripePayment.valid) {
      this.createPaymentIntent(this.amount)
        .pipe(
          switchMap((pi) =>
            this.stripeService.confirmCardPayment(pi.clientSecret, {
              payment_method: {
                card: this.card.element,
                billing_details: {
                  name: this.stripePayment.get('name')?.value,
                },
              },
            })
          )
        )
        .subscribe((result) => {
          if (result.error) {
            if (result.error.code === 'card_declined') {
              errorNotification('Card declined!');
              return;
            }

            if (result.error.code === 'expired_card') {
              errorNotification('Card expired!');
              return;
            }

            if (result.error.code === 'incorrect_cvc') {
              errorNotification('Incorrect CVC!');
              return;
            }

            if (result.error.code === 'processing_error') {
              errorNotification('Processing Error!');
              return;
            }

            if (result.error.code === 'incorrect_number') {
              errorNotification('Incorrect number!');
              return;
            }

            errorNotification('An error ocurred while processing the payment!')
            console.log(result.error.message);
          } else {
            if (result.paymentIntent.status === 'succeeded') {
              successNotification(`Payment of ${result.paymentIntent.amount} has been done with the !`)
            }
          }
        });
    } else {
      console.log(this.stripePayment);
    }
  }

  createPaymentIntent(amount: number): Observable<any> {
    return this.http.post<any>(
      `${environment.apiUrl}payment/create-payment-intent`,
      { amount }
    );
  }

  getTotal(): Observable<number> {
    return this.cartHttpService.getTotal();
  }
}
