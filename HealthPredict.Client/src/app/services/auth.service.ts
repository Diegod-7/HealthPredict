import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userId: number = 1; // Usuario por defecto para pruebas

  constructor() { }

  getUserId(): number {
    return this.userId;
  }

  setUserId(id: number): void {
    this.userId = id;
  }
} 