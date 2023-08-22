import { Component, Input } from '@angular/core';
import { UserTransaction } from '../../models/transaction';

@Component({
  selector: 'app-transaction-card',
  templateUrl: './transaction-card.component.html',
  styleUrls: ['./transaction-card.component.scss']
})
export class TransactionCardComponent {
  @Input() userTransaction!: UserTransaction;
}
