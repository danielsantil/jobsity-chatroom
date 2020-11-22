import { MessageResponse } from '../../models/messages';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private apiUrl = '/api/messages';

  constructor(private http: HttpClient) { }

  getMessages(): Promise<MessageResponse[]> {
    return this.http.get<MessageResponse[]>(this.apiUrl).toPromise();
  }
}
