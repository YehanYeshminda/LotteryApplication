import { Component, Input } from '@angular/core';
import { UserLosingTransaction } from '../../models/transaction';

@Component({
  selector: 'app-transaction-losing-card',
  templateUrl: './transaction-losing-card.component.html',
  styleUrls: ['./transaction-losing-card.component.scss']
})
export class TransactionLosingCardComponent {
  @Input() userLosings!: UserLosingTransaction;
}
