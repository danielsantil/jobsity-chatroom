export class LoginModel {
    username: string;
    password: string;
}

export class SignupModel extends LoginModel {
    email: string;
}

export class User {
    id: string;
    username: string;
    email: string;
}

export class AuthResponse {
    accessToken: string;
    loggedUser: User;
}
