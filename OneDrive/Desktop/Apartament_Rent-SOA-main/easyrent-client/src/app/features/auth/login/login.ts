import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      
      // 👇 FIXED: Only remove user session data instead of wiping the entire database!
      localStorage.removeItem('active_user_email');
      localStorage.removeItem('active_user_role');
      
      localStorage.setItem('active_user_email', email.toLowerCase().trim());

      this.authService.login({ email, password }).subscribe({
        next: (response) => {
          const userRole = this.authService.getRole();
          
          // 👇 Ensure the role is explicitly tracked alongside email session data
          localStorage.setItem('active_user_role', userRole || 'Tenant');
          
          alert(`Logged in as: ${email} (${userRole})`);
          
          if (userRole === 'Landlord') {
            this.router.navigate(['/manage-apartments']);
          } else {
            this.router.navigate(['/my-bookings']);
          }
        },
        error: (err) => {
          this.errorMessage = 'Login validation failed.';
        }
      });
    }
  }
}