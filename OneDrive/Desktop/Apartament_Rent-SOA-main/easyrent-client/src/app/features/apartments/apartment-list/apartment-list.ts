import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Apartment } from '../../../core/models/apartment';

@Component({
  selector: 'app-apartment-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './apartment-list.html',
  styleUrl: './apartment-list.scss'
  })
export class ApartmentListComponent implements OnInit {
  // Search and Filter model states
  searchQuery: string = '';
  selectedLocation: string = 'All';
  maxPrice: number | null = null;

  // Hardcoded backups to prevent "stale empty screen" issues on first load
  private defaultApartments: Apartment[] = [
    {
      id: 1,
      title: 'Riverside Studio, Skopje Center',
      description: 'A beautiful, sunlit studio apartment right near the Vardar River in the heart of the city center.',
      price: 380,
      location: 'Skopje Center',
      rooms: 1,
      sqft: 45,
      landlordName: 'Elena Kostova',
      imageUrl: 'https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800'
    },
    {
      id: 2,
      title: 'Traditional Tetovo Duplex',
      description: 'Renovated duplex featuring beautiful dark wood beams, a cozy brick fireplace.',
      price: 550,
      location: 'Tetovo',
      rooms: 3,
      sqft: 110,
      landlordName: 'Bekim Halimi',
      imageUrl: 'https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800'
    },
    {
      id: 3,
      title: 'Charming Flat in Debar Maalo',
      description: 'Vibrant and contemporary flat located in Skopje’s most famous bohemian neighborhood.',
      price: 450,
      location: 'Debar Maalo, Skopje',
      rooms: 2,
      sqft: 70,
      landlordName: 'Marija Angelova',
      imageUrl: 'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800'
    },
    {
      id: 4,
      title: 'Minimalist Retreat, Karposh',
      description: 'Sleek, minimalist design meets modern comfort in this highly desirable Karposh residential zone.',
      price: 410,
      location: 'Karposh, Skopje',
      rooms: 2,
      sqft: 65,
      landlordName: 'Stefan Ristovski',
      imageUrl: 'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800'
    }
  ];

  constructor() {}

  ngOnInit(): void {
    // Make sure the central storage index is built if it doesn't exist yet
    if (!localStorage.getItem('easyrent_global_apartments')) {
      localStorage.setItem('easyrent_global_apartments', JSON.stringify(this.defaultApartments));
    }
  }

  // 👇 READS LIVE CHANGES FROM THE LANDLORD OVER THE REGISTRY KEY 👇
  get apartments(): Apartment[] {
    const data = localStorage.getItem('easyrent_global_apartments');
    return data ? JSON.parse(data) : this.defaultApartments;
  }

  // Collects unique locations dynamically from current inventory listings for the filter dropdown
  get uniqueLocations(): string[] {
    const locations = this.apartments.map(apt => apt.location);
    return ['All', ...Array.from(new Set(locations))];
  }

  // Processes real-time sorting and text filtering queries
  get filteredApartments(): Apartment[] {
    return this.apartments.filter(apt => {
      const matchesSearch = apt.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                            apt.description.toLowerCase().includes(this.searchQuery.toLowerCase());
      
      const matchesLocation = this.selectedLocation === 'All' || apt.location === this.selectedLocation;
      
      const matchesPrice = this.maxPrice === null || apt.price <= this.maxPrice;

      return matchesSearch && matchesLocation && matchesPrice;
    });
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedLocation = 'All';
    this.maxPrice = null;
  }
}