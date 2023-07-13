import { Component, Input, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';


@Component({
  selector: 'app-spash-screen',
  templateUrl: './spash-screen.component.html',
  styleUrls: ['./spash-screen.component.scss']
})
export class SpashScreenComponent implements OnInit {
  constructor(private spinner: NgxSpinnerService) { }
  @Input() loading: number = 0;

  ngOnInit(): void {
    this.spinner.show();
  }
}
