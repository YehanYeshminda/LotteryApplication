import { Component } from '@angular/core';
import {successNotification} from "../../../../shared/alerts/sweetalert";
import {CartHttpService} from "../cart/services/cart-http.service";

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent {
  paymentHandler: any = null;
  constructor(public cartHttpService: CartHttpService) {}

  ngOnInit() {
    this.invokeStripe();
  }

  makePayment(amount: any) {
    const paymentHandler = (<any>window).StripeCheckout.configure({
      key: 'pk_test_51NSgQVCGctxV4GttmBY8TDMVypTxkWkWqc8w8AfeEFXDRYv93CoqNnSLOuClW6rCevuODvzwLXGEoNj2PYRBTVIU00qutXHWvA',
      locale: 'auto',
      token: function (stripeToken: any) {
        console.log(stripeToken);
        successNotification('Payment has been done and has generated the token!')
      },
    });
    paymentHandler.open({
      name: 'Positronx',
      description: '3 widgets',
      amount: amount * 100,
    });
  }

  invokeStripe() {
    if (!window.document.getElementById('stripe-script')) {
      const script = window.document.createElement('script');
      script.id = 'stripe-script';
      script.type = 'text/javascript';
      script.src = 'https://checkout.stripe.com/checkout.js';
      script.onload = () => {
        this.paymentHandler = (<any>window).StripeCheckout.configure({
          key: 'pk_test_51NSgQVCGctxV4GttmBY8TDMVypTxkWkWqc8w8AfeEFXDRYv93CoqNnSLOuClW6rCevuODvzwLXGEoNj2PYRBTVIU00qutXHWvA',
          locale: 'auto',
          token: function (stripeToken: any) {
            console.log(stripeToken);
          },
        });
      };
      window.document.body.appendChild(script);
    }
  }
}
