import { Component, OnInit } from '@angular/core';
import { UserTransactionHttpService } from './services/user-transaction-http.service';
import { Observable, of } from 'rxjs';
import { Root } from '../../../components/easy-draw/models/EasyDrawResponse';
import { UserLosingTransaction, UserTransaction } from './models/transaction';

@Component({
  selector: 'app-transaction-history',
  templateUrl: './transaction-history.component.html',
  styleUrls: ['./transaction-history.component.scss']
})
export class TransactionHistoryComponent implements OnInit {
  userWinnings$: Observable<Root<UserTransaction[]>> = of();
  userLosings$: Observable<Root<UserLosingTransaction[]>> = of();

  constructor(private userTransactionHttpService: UserTransactionHttpService) { }

  ngOnInit(): void {
    this.userWinnings$ = this.userTransactionHttpService.getAllUserWinningTransactions();
    this.userLosings$ = this.userTransactionHttpService.getAllUserLosingTransactions();
  }
}
