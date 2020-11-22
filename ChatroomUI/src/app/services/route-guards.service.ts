import { AuthService } from './auth.service';
import { CanActivate, Router } from '@angular/router';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UnauthorizedUserGuard implements CanActivate {
  constructor(private router: Router,
              private authService: AuthService) { }

  canActivate(): boolean {
    const loggedUser = this.authService.getCurrentUser();
    if (!loggedUser) {
      this.router.navigateByUrl('/login');
      return false;
    }

    return true;
  }
}

@Injectable({
  providedIn: 'root'
})
export class LoggedInUserGuard implements CanActivate {
  constructor(private router: Router,
              private authService: AuthService) { }

  canActivate(): boolean {
    const loggedUser = this.authService.getCurrentUser();
    if (loggedUser) {
      this.router.navigateByUrl('/chatroom');
      return false;
    }

    return true;
  }
}
