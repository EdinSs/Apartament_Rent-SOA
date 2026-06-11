import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Read the expected authorized role from the routing configuration definition
  const expectedRole = route.data['expectedRole'];
  const userRole = authService.getRole();

  if (authService.isLoggedIn() && userRole === expectedRole) {
    return true; // Match found! Let them in.
  }

  // Unauthorized access attempt! Warn them and boot them back to safety
  alert(`Access Denied! Your current profile tier (${userRole}) is not authorized to view this portal.`);
  
  if (userRole === 'Landlord') {
    router.navigate(['/manage-apartments']);
  } else {
    router.navigate(['/my-bookings']);
  }
  
  return false;
};