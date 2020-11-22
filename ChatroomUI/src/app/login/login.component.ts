import { NgxSpinnerService } from 'ngx-spinner';
import { AuthService } from './../services/auth.service';
import { LoginModel } from './../../models/auth-models';
import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginModel = new LoginModel();
  errorMessage: string;

  constructor(private modalService: NgbModal,
              private authService: AuthService,
              private spinner: NgxSpinnerService,
              private router: Router) { }

  ngOnInit(): void {
  }

  openModal(content: any): void {
    this.modalService.open(content, { backdrop: 'static' });
  }

  login(): void {
    this.spinner.show();
    this.authService.login(this.loginModel).then(res => {
      this.goToChat();
    }).catch(err => {
      this.showError(err.error);
    }).finally(() => {
      this.spinner.hide();
    });
  }

  handleRegister(error: string): void {
    if (error) {
      this.showError(error);
    } else {
      this.modalService.dismissAll();
      this.goToChat();
    }
  }

  goToChat(): void {
    this.router.navigateByUrl('/chatroom');
  }

  showError(error: string): void {
    this.errorMessage = error;
    setTimeout(() => {
      this.errorMessage = '';
    }, 3000);
  }

}
