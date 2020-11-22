import { LoginModel, SignupModel, User, AuthResponse } from './../../models/auth-models';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/auth/';
  private tokenKey = 'chatroom-auth-token';
  private currentUserSubject = new BehaviorSubject<User>(null);
  currentUser = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(model: LoginModel): Promise<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl + 'login', model)
      .pipe(tap(x => this.setSessionCredentials(x)))
      .toPromise();
  }

  register(model: SignupModel): Promise<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl + 'register', model)
      .pipe(tap(x => this.setSessionCredentials(x)))
      .toPromise();
  }

  initSessionCredentials(): void {
    const savedSessionJson = sessionStorage.getItem(this.tokenKey);
    if (savedSessionJson) {
      const auth: AuthResponse = JSON.parse(savedSessionJson);
      this.setSessionCredentials(auth);
    }
  }

  getCurrentUser(): User {
    return this.currentUserSubject.value;
  }

  logout(): void {
    this.currentUserSubject.next(null);
    sessionStorage.removeItem(this.tokenKey);
  }

  private setSessionCredentials(auth: AuthResponse): void {
    sessionStorage.setItem(this.tokenKey, JSON.stringify(auth));
    this.currentUserSubject.next(auth.loggedUser);
  }
}
