import { User } from './../../models/auth-models';
import { Subscription } from 'rxjs';
import { AuthService } from './../services/auth.service';
import { MessageResponse } from './../../models/messages';
import { MessagesService } from './../services/messages.service';
import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-chatroom',
  templateUrl: './chatroom.component.html',
  styleUrls: ['./chatroom.component.css']
})
export class ChatroomComponent implements OnInit, OnDestroy {
  messages: MessageResponse[] = [];
  $currentUserId: Subscription;
  currentUser: User;
  newMessage: string;

  constructor(private messagesService: MessagesService,
              private authService: AuthService) { }

  ngOnInit(): void {
    this.messagesService.getMessages().then(res => {
      this.messages = res;
    });

    this.$currentUserId = this.authService.currentUser.subscribe(res => {
      this.currentUser = res;
    });

    this.messagesService.startConnection().then(() => {
      console.log('Chatroom hub started');
    });

    this.messagesService.onMessageReceived((msg: MessageResponse) => {
      this.messages.push(msg);
    });
  }

  async send(): Promise<void> {
    await this.messagesService.ensureConnection();

    if (!this.newMessage || this.newMessage.trim() === '') {
      return;
    }

    this.messagesService.sendMessage(this.newMessage);
    this.newMessage = '';
  }

  ngOnDestroy(): void {
    this.messagesService.stopConnection();
    this.$currentUserId.unsubscribe();
  }
}
