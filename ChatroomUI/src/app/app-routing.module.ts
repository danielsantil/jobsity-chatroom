import { ChatroomComponent } from './chatroom/chatroom.component';
import { LoginComponent } from './login/login.component';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'chatroom', component: ChatroomComponent },
    { path: '**', redirectTo: 'chatroom' },
    { path: '', redirectTo: 'chatroom', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
