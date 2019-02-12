// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

export class UserLogin {
    constructor(email?: string, password?: string, rememberMe?: boolean) {
        this.email = email;
        this.password = password;
        this.rememberMe = rememberMe;
    }

    userName:string;
    email: string;
    password: string;
    rememberMe: boolean;
}
