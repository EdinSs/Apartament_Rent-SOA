import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-landlord-properties',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    MatCardModule, 
    MatButtonModule, 
    MatIconModule, 
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './landlord-properties.html',
  styleUrl: './landlord-properties.scss'
})
export class LandlordPropertiesComponent implements OnInit {
  listingForm!: FormGroup;
  showCreateForm = false;

  // Hardcoded backup list so your marketplace is never empty on first load
  private defaultApartments = [
    { id: 1, title: 'Riverside Studio, Skopje Center', price: 380, location: 'Skopje Center', views: 124 },
    { id: 2, title: 'Traditional Tetovo Duplex', price: 550, location: 'Tetovo', views: 89 },
    { id: 3, title: 'Charming Flat in Debar Maalo', price: 450, location: 'Debar Maalo, Skopje', views: 342 },
    { id: 4, title: 'Minimalist Retreat, Karposh', price: 410, location: 'Karposh, Skopje', views: 195 }
  ];

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    // Initialize the global apartments storage if it's completely missing
    if (!localStorage.getItem('easyrent_global_apartments')) {
      localStorage.setItem('easyrent_global_apartments', JSON.stringify(this.defaultApartments));
    }

    this.listingForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      location: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(1)]],
      rooms: ['', [Validators.required, Validators.min(1)]],
      sqft: ['', [Validators.required, Validators.min(1)]],
      imageUrl: [''],
      description: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  // 👇 LIVE GETTER: Reads all apartments from the SAME global list the tenant sees
  get myProperties(): any[] {
    const saved = localStorage.getItem('easyrent_global_apartments');
    return saved ? JSON.parse(saved) : this.defaultApartments;
  }

  // 👇 LIVE GETTER: Reads applications directly from local storage on every render cycle
  get incomingApplications(): any[] {
    const savedData = localStorage.getItem('easyrent_lease_applications');
    return savedData ? JSON.parse(savedData) : [];
  }

  toggleForm(): void {
    this.showCreateForm = !this.showCreateForm;
  }

  updateStatus(id: number, targetStatus: 'Approved' | 'Declined'): void {
    const savedData = localStorage.getItem('easyrent_lease_applications');
    let currentApplications = savedData ? JSON.parse(savedData) : [];

    currentApplications = currentApplications.map((app: any) => {
      if (app.id === id) {
        app.status = targetStatus;
      }
      return app;
    });

    localStorage.setItem('easyrent_lease_applications', JSON.stringify(currentApplications));
  }

  onSubmitListing(): void {
    if (this.listingForm.valid) {
      const formValue = this.listingForm.value;
      
      const newApartment = {
        id: Date.now(),
        title: formValue.title,
        price: Number(formValue.price),
        location: formValue.location,
        rooms: Number(formValue.rooms),
        sqft: Number(formValue.sqft),
        description: formValue.description,
        imageUrl: formValue.imageUrl || 'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800',
        views: 0
      };

      const currentList = this.myProperties;
      currentList.push(newApartment);
      
      localStorage.setItem('easyrent_global_apartments', JSON.stringify(currentList));
      
      alert(`"${newApartment.title}" has been successfully added to the system!`);
      this.listingForm.reset();
      this.showCreateForm = false;
    }
  }
}