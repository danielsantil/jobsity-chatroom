import { AuthService } from './auth.service';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const loggedUser = this.authService.getCurrentUser();
        if (loggedUser) {
            const securedReq = req.clone({
                headers: req.headers.set('Authorization', 'Bearer ' + this.authService.accessToken)
            });
            return next.handle(securedReq);
        } else {
            return next.handle(req);
        }
    }
}
