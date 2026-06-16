import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Apartment } from '../models/apartment';

@Injectable({
  providedIn: 'root'
})
export class ApartmentService {
  private apiUrl = 'https://easyrent-api.onrender.com/api/apartments';

  constructor(private http: HttpClient) {}

  getApartments(): Observable<Apartment[]> {
    return this.http.get<Apartment[]>(this.apiUrl);
  }

  getApartmentById(id: number): Observable<Apartment> {
    return this.http.get<Apartment>(`${this.apiUrl}/${id}`);
  }

  createApartment(apartmentData: any): Observable<Apartment> {
    return this.http.post<Apartment>(this.apiUrl, apartmentData);
  }
}