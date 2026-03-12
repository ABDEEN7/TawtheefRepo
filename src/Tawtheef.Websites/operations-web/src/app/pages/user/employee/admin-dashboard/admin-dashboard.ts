import { Component } from '@angular/core';
import { Skeleton } from 'primeng/skeleton';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-admin-dashboard',
  imports: [
    Skeleton,
    NgIf
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
})
export class AdminDashboard {
  loading = false;
}
