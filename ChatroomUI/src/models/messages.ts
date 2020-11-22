import { User } from './auth-models';
export class Message {
    userId: string;
    body: string;
    createdOn: Date;
}

export class MessageResponse {
    user: User;
    body: string;
    createdOn: Date;
}
