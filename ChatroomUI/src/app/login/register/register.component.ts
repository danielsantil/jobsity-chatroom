import { NgxSpinnerService } from 'ngx-spinner';
import { AuthService } from './../../services/auth.service';
import { SignupModel } from './../../../models/auth-models';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-register-component',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  signupModel = new SignupModel();
  @Output() registerError = new EventEmitter<string>();

  constructor(private authService: AuthService,
              private spinner: NgxSpinnerService) { }

  register(): void {
    this.authService.register(this.signupModel).then(_ => {
      this.registerError.emit();
    }).catch(err => {
      this.registerError.emit(err.error);
    }).finally(() => {
      this.spinner.hide();
    });
  }
}
