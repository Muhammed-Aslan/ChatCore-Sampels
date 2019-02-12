import { Injectable } from '@angular/core';

@Injectable()
export class DBkeys {

    public static readonly CURRENT_USER = "current_user";
    public static readonly ACCESS_TOKEN = "access_token";
    public static readonly ID_TOKEN = "id_token";
    public static readonly REFRESH_TOKEN = "refresh_token";
    public static readonly TOKEN_EXPIRES_IN = "expires_in";
    public static readonly REMEMBER_ME = "remember_me";
    ////////////////////////////////
    public static readonly CONTACTS ='contacts';
    public static readonly CHATS = 'chats';
}
