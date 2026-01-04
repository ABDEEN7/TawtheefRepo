import { Component } from '@angular/core';
import {Skeleton} from 'primeng/skeleton';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [
    Skeleton,
    NgIf
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  loading = false;
}
