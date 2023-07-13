import { Component, ViewChild } from '@angular/core';
import { errorNotification, successNotification } from "../../../../shared/alerts/sweetalert";
import { CartHttpService } from "../cart/services/cart-http.service";
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Appearance, PaymentIntent, StripeCardElementOptions, StripeElementsOptions } from '@stripe/stripe-js';
import { StripeCardComponent, StripeService } from 'ngx-stripe';
import { HttpClient } from '@angular/common/http';
import { Observable, switchMap } from 'rxjs';
import { environment } from 'src/environments/environment.development';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent {
  @ViewChild(StripeCardComponent) card!: StripeCardComponent;

  cardOptions: StripeCardElementOptions = {
    style: {
      base: {
        iconColor: '#666EE8',
        color: '#31325F',
        fontWeight: '300',
        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
        fontSize: '18px',
        '::placeholder': {
          color: '#CFD7E0',
        },
      },
    },
  };

  elementsOptions: StripeElementsOptions = {
    locale: 'es',
  };

  stripeTest!: FormGroup;

  constructor(
    private http: HttpClient,
    private fb: FormBuilder,
    private stripeService: StripeService
  ) { }

  ngOnInit(): void {
    this.stripeTest = this.fb.group({
      name: ['Angular v10', []],
      amount: [500, []],
    });
  }

  pay(): void {
    if (this.stripeTest.valid) {
      this.createPaymentIntent(this.stripeTest.get('amount')?.value)
        .pipe(
          switchMap((pi) =>
            this.stripeService.confirmCardPayment(pi.clientSecret, {
              payment_method: {
                card: this.card.element,
                billing_details: {
                  name: this.stripeTest.get('name')?.value,
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
      console.log(this.stripeTest);
    }
  }

  createPaymentIntent(amount: number): Observable<any> {
    return this.http.post<any>(
      `${environment.apiUrl}payment/create-payment-intent`,
      { amount }
    );
  }
}
