import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent {

  constructor(public authService: AuthService, private router: Router) {}

  // 👇 READS THE ACTIVE EMAIL SESSIONS LIVE
  get userEmail(): string | null {
    return localStorage.getItem('active_user_email');
  }

  get userRole(): string | null {
    return localStorage.getItem('active_user_role');
  }

  // 👇 THE MISSING LOGOUT METHOD
  onLogout(): void {
    localStorage.removeItem('active_user_email');
    localStorage.removeItem('active_user_role');
    
    // Fallback clear if your authService holds its own token states
    if (typeof this.authService.logout === 'function') {
      this.authService.logout();
    }

    alert('You have successfully logged out!');
    this.router.navigate(['/login']);
  }
}