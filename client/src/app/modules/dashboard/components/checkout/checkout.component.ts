import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CartReponse } from '../cart/models/cart';
import { Observable, map, of, switchMap, tap } from 'rxjs';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { CartEntityService } from '../cart/services/cart-entity.service';
import { MatStepper } from '@angular/material/stepper';
import { StripeCardComponent, StripeService } from 'ngx-stripe';
import { AuthDetails } from 'src/app/shared/models/auth';
import { StripeCardElementOptions, StripeElementsOptions } from '@stripe/stripe-js';
import { FormBuilder, FormGroup } from '@angular/forms';
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { CartHttpService } from '../cart/services/cart-http.service';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PaymentHttpService } from '../payment/services/payment-http.service';
import { Router } from '@angular/router';
import { User } from '../../auth/models/user';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { selectUser } from '../../auth/features/auth.selectors';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit, AfterViewInit {
  cartItems$: Observable<CartReponse[]> = of([]);
  total = 0;
  paymentComplete: boolean = true;
  isPaymentDone$: Observable<boolean> = of(false);
  @ViewChild(StripeCardComponent) card!: StripeCardComponent;
  authInformartion: AuthDetails | null = null;
  paymentOnGoing: boolean = false;
  lotteryReferenceIds: string[] = [];
  showTotal: number = 0;
  userInformation$: Observable<User | undefined> = of();

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

  constructor(private cookieService: CookieService, private cartEntityService: CartEntityService, private fb: FormBuilder, private cartHttpService: CartHttpService, private stripeService: StripeService, private http: HttpClient, private paymentHttpService: PaymentHttpService, private router: Router, private store: Store<AppState>) { }

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

    this.userInformation$ = this.store.select(selectUser);

    this.stripePayment = this.fb.group({
      name: [this.authInformartion?.username, []],
      amount: [0, []],
    });
  }

  goNext(stepper: MatStepper) {
    stepper.next();
  }

  goBack(stepper: MatStepper) {
    stepper.previous();
  }

  pay(stepper: MatStepper): void {
    if (this.stripePayment.valid) {
      if (this.total === 0) {
        errorNotification("CART IS EMPTY!");
        this.router.navigateByUrl('/dashboard/home')
        return;
      }
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
          } else {
            if (result.paymentIntent.status === 'succeeded') {
              this.isPaymentDone$ = of(true);
              this.cartHttpService.removeAllFromCart().subscribe({
                next: response => {
                  this.showTotal = result.paymentIntent.amount;
                  successNotification(`Payment of ${result.paymentIntent.amount} has been done!`)
                  this.cartEntityService.clearCache();
                  if (this.isPaymentDone$.subscribe(x => x === true)) {
                    stepper.next();
                  }
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
