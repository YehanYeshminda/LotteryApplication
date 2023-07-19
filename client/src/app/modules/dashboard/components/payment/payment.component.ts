import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { errorNotification, successNotification } from "../../../../shared/alerts/sweetalert";
import { CartHttpService } from "../cart/services/cart-http.service";
import { FormBuilder, FormGroup } from '@angular/forms';
import { StripeCardElementOptions, StripeElementsOptions } from '@stripe/stripe-js';
import { StripeCardComponent, StripeService } from 'ngx-stripe';
import { HttpClient } from '@angular/common/http';
import { Observable, map, of, switchMap, tap } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { AuthDetails } from 'src/app/shared/models/auth';
import { CartReponse } from '../cart/models/cart';
import { CartEntityService } from '../cart/services/cart-entity.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent implements OnInit, AfterViewInit {
  @ViewChild(StripeCardComponent) card!: StripeCardComponent;
  authInformartion: AuthDetails | null = null;
  cartItems$: Observable<CartReponse[]> = of([]);
  total: number = 0;
  paymentOnGoing: boolean = false;

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
    private cartEntityService: CartEntityService,
    private cartHttpService: CartHttpService,
    private router: Router
  ) { }

  ngAfterViewInit(): void {
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

  ngOnInit(): void {
    this.stripePayment = this.fb.group({
      name: [this.authInformartion?.username, []],
      amount: [0, []],
    });
  }

  pay(): void {
    if (this.stripePayment.valid) {
      this.paymentOnGoing = true;
      this.createPaymentIntent(this.total)
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
            this.paymentOnGoing = false;
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
              this.cartHttpService.removeAllFromCart().subscribe({
                next: response => {
                  this.paymentOnGoing = false;
                  successNotification(`Payment of ${result.paymentIntent.amount} has been done!`)
                  this.cartEntityService.clearCache();
                  this.router.navigateByUrl('/dashboard/home');
                }
              });
            }
          }
        });
    } else {
      errorNotification('Please fill all the fields!');
    }
  }

  createPaymentIntent(amount: number): Observable<any> {
    return this.http.post<any>(
      `${environment.apiUrl}payment/create-payment-intent`,
      { amount }
    );
  }
}
