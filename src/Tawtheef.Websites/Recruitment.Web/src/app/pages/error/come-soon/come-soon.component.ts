import {Component, OnDestroy, OnInit} from '@angular/core';
import {routes} from "../../../routes/routes";
import {RouterLink} from "@angular/router";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-come-soon',
  templateUrl: './come-soon.component.html',
  styleUrls: ['./come-soon.component.scss'],
  imports: [
    TranslatePipe,
    RouterLink
  ],
  standalone: true
})
export class ComeSoonComponent implements OnInit, OnDestroy{
  public routes = routes;
  days = 0;
hours = 0;
minutes = 0;
seconds = 0;
countdownExpired = false;
email = ''; // Declare the 'email' property

private countdownInterval: any;



ngOnInit() {
  this.setCountdown();
}

ngOnDestroy() {
  clearInterval(this.countdownInterval);
}

setCountdown() {
  // Dynamically set the countdown date to 30 days from the current date
  const currentDate = new Date();
  currentDate.setDate(currentDate.getDate() + 30);
  const countdownDate = currentDate.getTime();

  this.countdownInterval = setInterval(() => {
    const todayDate = new Date().getTime();
    const distance = countdownDate - todayDate;

    // Calculate remaining time
    this.days = Math.floor(distance / (1000 * 60 * 60 * 24));
    this.hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    this.minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    this.seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Check if countdown has expired
    if (distance < 0) {
      clearInterval(this.countdownInterval);
      this.countdownExpired = true;
    }
  }, 1000);
}


}
