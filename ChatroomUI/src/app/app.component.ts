import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { User } from './../models/auth-models';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit, OnDestroy {
  $currentUser: Subscription;
  loggedUser: User;

  constructor(private authService: AuthService,
              private router: Router) { }

  ngOnInit(): void {
    this.authService.initSessionCredentials();

    this.$currentUser = this.authService.currentUser
        .subscribe(res => this.loggedUser = res);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }

  ngOnDestroy(): void {
    this.$currentUser.unsubscribe();
  }
}
