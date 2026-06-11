import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-tenant-bookings',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './tenant-bookings.html',
  styleUrl: './tenant-bookings.scss'
})
export class TenantBookingsComponent implements OnInit {

  constructor() {}

  ngOnInit(): void {
    if (!localStorage.getItem('easyrent_lease_applications')) {
      localStorage.setItem('easyrent_lease_applications', JSON.stringify([]));
    }
  }

  // 👇 LIVE GETTER: Filters applications dynamically so Besiana only sees her own requests
  get myBookings(): any[] {
    const savedData = localStorage.getItem('easyrent_lease_applications');
    const allApplications: any[] = savedData ? JSON.parse(savedData) : [];
    const currentUserEmail = localStorage.getItem('active_user_email') || 'besiana771@gmail.com';
    return allApplications.filter(app => app.tenantEmail === currentUserEmail);
  }

  onCancelRequest(bookingId: number): void {
    const savedData = localStorage.getItem('easyrent_lease_applications');
    let allApplications: any[] = savedData ? JSON.parse(savedData) : [];
    allApplications = allApplications.filter(b => b.id !== bookingId);
    localStorage.setItem('easyrent_lease_applications', JSON.stringify(allApplications));
    alert('Your lease application request has been withdrawn.');
  }
}