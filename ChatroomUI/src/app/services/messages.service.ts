import { AuthService } from './auth.service';
import { Message } from './../../models/messages';
import { MessageResponse } from '../../models/messages';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private apiUrl = '/api/messages';
  private chatroomHub: HubConnection;

  constructor(private http: HttpClient,
              private authService: AuthService) {
  }

  getMessages(): Promise<MessageResponse[]> {
    return this.http.get<MessageResponse[]>(this.apiUrl).toPromise();
  }

  sendMessage(msgBody: string): void {
    const message: Message = {
      body: msgBody,
      userId: this.authService.getCurrentUser().userId
    };

    this.chatroomHub.invoke('SendMessage', message);
  }

  onMessageReceived(callback: any): void {
    this.chatroomHub.on('MessageReceived', callback);
  }

  startConnection(): Promise<void> {
    if (!this.chatroomHub) {
      this.chatroomHub = new HubConnectionBuilder().withUrl('/slr/chatroom', {
        accessTokenFactory: () => this.authService.accessToken
      }).build();
    }
    return this.chatroomHub.start();
  }

  stopConnection(): void {
    if (this.chatroomHub) {
      this.chatroomHub.stop();
      this.chatroomHub = null;
    }
  }

  ensureConnection(): Promise<void> {
    if (this.chatroomHub.state === HubConnectionState.Disconnected) {
      return this.startConnection();
    }
  }
}
