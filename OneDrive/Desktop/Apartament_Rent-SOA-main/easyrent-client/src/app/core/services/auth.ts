import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  constructor() {}

  // 👇 SYSTEM LOGIN LAYER
  login(credentials: { email: string; password?: string }): Observable<any> {
    const email = credentials.email.toLowerCase().trim();
    let role = 'Tenant';

    if (email.includes('landlord') || email === 'mesut' || email.startsWith('mesut')) {
      role = 'Landlord';
    }

    localStorage.setItem('active_user_email', email);
    localStorage.setItem('active_user_role', role);
    // Set a fake token string so your interceptor has something to read!
    localStorage.setItem('auth_token', 'mock_local_sandbox_token_string');

    return of({ success: true, email, role });
  }

  // 👇 ADDED BACK: Restores the registration method to satisfy the register component
  register(userData: any): Observable<any> {
    return of({ success: true, message: 'Mock registration complete!', data: userData });
  }

  // 👇 ADDED BACK: Restores token extraction to satisfy your interceptor
  getToken(): string | null {
    return localStorage.getItem('auth_token') || 'mock_local_sandbox_token_string';
  }

  // 👇 ACTIVE APPLICATION STATE READERS
  getRole(): string {
    return localStorage.getItem('active_user_role') || 'Tenant';
  }

  getEmail(): string | null {
    return localStorage.getItem('active_user_email');
  }

  isLoggedIn(): boolean {
    return !!this.getEmail();
  }

  logout(): void {
    localStorage.removeItem('active_user_email');
    localStorage.removeItem('active_user_role');
    localStorage.removeItem('auth_token');
  }
}