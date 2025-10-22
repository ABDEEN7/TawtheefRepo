import { Component, OnInit } from '@angular/core';
import {NotificationService, Toast} from "../../../core/services/notification.service";


@Component({
  selector: 'app-toasts',
  templateUrl: './toasts.component.html'
})
export class ToastsComponent implements OnInit {
  toasts: Toast[] = [];
  constructor(private notify: NotificationService) {}
  ngOnInit() { this.notify.toast$.subscribe(t => { this.toasts.push(t); setTimeout(() => this.toasts.shift(), 5000); }); }
}
