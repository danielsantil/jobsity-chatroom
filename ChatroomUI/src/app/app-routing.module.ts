import { ChatroomComponent } from './chatroom/chatroom.component';
import { LoginComponent } from './login/login.component';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { UnauthorizedUserGuard, LoggedInUserGuard } from './services/route-guards.service';

const routes: Routes = [
    { path: 'login', component: LoginComponent, canActivate: [LoggedInUserGuard] },
    { path: 'chatroom', component: ChatroomComponent, canActivate: [UnauthorizedUserGuard] },
    { path: '**', redirectTo: 'chatroom' },
    { path: '', redirectTo: 'chatroom', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
