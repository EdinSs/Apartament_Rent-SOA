import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { ApartmentListComponent } from './features/apartments/apartment-list/apartment-list';
import { ApartmentDetailComponent } from './features/apartments/apartment-detail/apartment-detail';
import { TenantBookingsComponent } from './features/dashboards/tenant-bookings/tenant-bookings';
import { LandlordPropertiesComponent } from './features/dashboards/landlord-properties/landlord-properties';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  // Protected Tenant Spaces
  { 
    path: 'apartments', 
    component: ApartmentListComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'apartments/:id', 
    component: ApartmentDetailComponent, 
    canActivate: [authGuard] 
  },
  { 
    path: 'my-bookings', 
    component: TenantBookingsComponent, 
    canActivate: [authGuard],
    data: { expectedRole: 'Tenant' } 
  },

  // Protected Landlord Spaces
  { 
    path: 'manage-apartments', 
    component: LandlordPropertiesComponent, 
    canActivate: [authGuard, roleGuard],
    data: { expectedRole: 'Landlord' } 
  },
  
  { path: '**', redirectTo: 'login' } // Catch-all wildcard redirect fallback
];