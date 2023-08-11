import { Component, OnInit } from '@angular/core';
import { StatusCheckHttpService } from './services/status-check-http.service';
import { Observable, of } from 'rxjs';
import { StatusCheckData } from './models/statuscheck';

@Component({
  selector: 'app-status-check',
  templateUrl: './status-check.component.html',
  styleUrls: ['./status-check.component.scss']
})
export class StatusCheckComponent implements OnInit {
  statusCheckResult$: Observable<StatusCheckData[]> = of([]);

  constructor(private statusCheckHttpService: StatusCheckHttpService) { }

  ngOnInit(): void {
    this.statusCheckResult$ = this.statusCheckHttpService.getAllStatus()
  }
}
