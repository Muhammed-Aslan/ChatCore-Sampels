import { Injectable, Injector } from '@angular/core';
import { Observable, Subject, throwError } from 'rxjs';
import { map, switchMap, catchError, mergeMap } from 'rxjs/operators';

import { LocalStoreManager } from './Helpers/local-store-manager.service';
import { DBkeys } from './Helpers/db-keys';
import { JwtHelper } from './Helpers/jwt-helper';
import { LoginResponse, IdToken } from '../models/login-response.model';
import { User } from '../models/user.model';
import { AccountService } from './account.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ConfigurationService } from './Helpers/configuration.service';

@Injectable()
export class AuthService {

  private get currentUserUrl() {return this.configurations.apiBaseUrl + "/ChatApp/account/me";}
 
  private previousIsLoggedInCheck = false;
  private _loginStatus = new Subject<boolean>();
  
  constructor( private localStorage: LocalStoreManager, private configurations:ConfigurationService, private http:HttpClient)
  {
    this.initializeLoginStatus();
  }


  private initializeLoginStatus() {
    this.localStorage.getInitEvent().subscribe(() => {
      this.reevaluateLoginStatus();
    });
  }

  reLogin() {
    this.localStorage.deleteData(DBkeys.TOKEN_EXPIRES_IN);
  }

  refreshLogin() {
    return this.getRefreshLoginEndpoint<LoginResponse>().pipe(
      map(response => {
        this.processLoginResponse(response, this.rememberMe,this.currentUser);
        return response;
      }));
  }

  login(userName: string, password: string, rememberMe?: boolean) {

    if (this.isLoggedIn)
      this.logout();

    return this.getLoginEndpoint<LoginResponse>(userName, password).pipe(
      switchMap(response =>{
        return this.getCurrentUser<User>(response.access_token).pipe(
          map(user=>{
            return this.processLoginResponse(response, rememberMe ,user);
          }));
      }));
  }

  private processLoginResponse(response: LoginResponse, rememberMe: boolean,user?:User) {

    let accessToken = response.access_token;

    if (accessToken == null)
      throw new Error("Received accessToken was empty");

    let idToken = response.id_token;
    let refreshToken = response.refresh_token || this.refreshToken;
    let expiresIn = response.expires_in;

    let tokenExpiryDate = new Date();
    tokenExpiryDate.setSeconds(tokenExpiryDate.getSeconds() + expiresIn);

    let accessTokenExpiry = tokenExpiryDate;

    let jwtHelper = new JwtHelper();
    let decodedIdToken = <IdToken>jwtHelper.decodeToken(response.id_token);
    let _user = new User();
    if(user){
      user.accountId = decodedIdToken.sub;
      user.userName = decodedIdToken.name;
      user.email = decodedIdToken.email;
      user.phone = decodedIdToken.phone;
      _user= user;
    }
    else{
      _user.accountId = decodedIdToken.sub;
      _user.userName = decodedIdToken.name;
      _user.email = decodedIdToken.email;
      _user.phone = decodedIdToken.phone;
    }

    this.saveUserDetails(_user, accessToken, idToken, refreshToken, accessTokenExpiry, rememberMe);

    this.reevaluateLoginStatus(_user);

    return _user;
  }

  private getCurrentUser<T>(accessToken?:string): Observable<T> {
    var headerObj;
    if(accessToken)
    {
      let headers = new HttpHeaders({
        'Authorization': 'Bearer ' + accessToken,
        'Content-Type': 'application/json',
        'Accept': ' application/json, text/plain, */*'
      });
      headerObj ={ headers: headers };
    }
    else headerObj =this.getRequestHeaders();
    return this.http.get<T>(this.currentUserUrl, headerObj).pipe<T>(
      catchError(error => {
         return this.handleError(error, () => this.getCurrentUser(accessToken));
   }));
  }

  private saveUserDetails(user: User, accessToken: string, idToken: string, refreshToken: string, expiresIn: Date, rememberMe: boolean) {

    if (rememberMe) {
      this.localStorage.savePermanentData(accessToken, DBkeys.ACCESS_TOKEN);
      this.localStorage.savePermanentData(idToken, DBkeys.ID_TOKEN);
      this.localStorage.savePermanentData(refreshToken, DBkeys.REFRESH_TOKEN);
      this.localStorage.savePermanentData(expiresIn, DBkeys.TOKEN_EXPIRES_IN);
      this.localStorage.savePermanentData(user, DBkeys.CURRENT_USER);
    }
    else {
      this.localStorage.saveSyncedSessionData(accessToken, DBkeys.ACCESS_TOKEN);
      this.localStorage.saveSyncedSessionData(idToken, DBkeys.ID_TOKEN);
      this.localStorage.saveSyncedSessionData(refreshToken, DBkeys.REFRESH_TOKEN);
      this.localStorage.saveSyncedSessionData(expiresIn, DBkeys.TOKEN_EXPIRES_IN);
      this.localStorage.saveSyncedSessionData(user, DBkeys.CURRENT_USER);
    }

    this.localStorage.savePermanentData(rememberMe, DBkeys.REMEMBER_ME);
  }

  logout(): void {
    this.localStorage.deleteData(DBkeys.ACCESS_TOKEN);
    this.localStorage.deleteData(DBkeys.ID_TOKEN);
    this.localStorage.deleteData(DBkeys.REFRESH_TOKEN);
    this.localStorage.deleteData(DBkeys.TOKEN_EXPIRES_IN);
    this.localStorage.deleteData(DBkeys.CURRENT_USER);

    this.reevaluateLoginStatus();
  }

  private reevaluateLoginStatus(currentUser?: User) {

    let user = currentUser || this.localStorage.getDataObject<User>(DBkeys.CURRENT_USER);
    let isLoggedIn = user != null;

    if (this.previousIsLoggedInCheck != isLoggedIn) {
      setTimeout(() => {
        this._loginStatus.next(isLoggedIn);
      });
    }

    this.previousIsLoggedInCheck = isLoggedIn;
  }

  getLoginStatusEvent(): Observable<boolean> {
    return this._loginStatus.asObservable();
  }

  get currentUser() {

    let user = this.localStorage.getDataObject<User>(DBkeys.CURRENT_USER);
    this.reevaluateLoginStatus(user);
   return user;
  }

  get accessToken(): string {

    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.ACCESS_TOKEN);
  }

  get accessTokenExpiryDate(): Date {

    this.reevaluateLoginStatus();
    return this.localStorage.getDataObject<Date>(DBkeys.TOKEN_EXPIRES_IN, true);
  }

  get isSessionExpired(): boolean {
    if (this.accessTokenExpiryDate == null) {
      return true;
    }
    return !(this.accessTokenExpiryDate.valueOf() > new Date().valueOf());
  }

  get idToken(): string {
    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.ID_TOKEN);
  }

  get refreshToken(): string {
    this.reevaluateLoginStatus();
    return this.localStorage.getData(DBkeys.REFRESH_TOKEN);
  }

  get isLoggedIn(){
    return this.currentUser !=null;
  }

  get rememberMe(): boolean {
    return this.localStorage.getDataObject<boolean>(DBkeys.REMEMBER_ME) == true;
  }

//#region /***** EndpointFactory Replacement ******/

private taskPauser: Subject<any>;
private isRefreshingLogin: boolean;
private get loginUrl() { return this.configurations.apiBaseUrl + this.configurations.loginEndpoint; }

private getLoginEndpoint<T>(userName: string, password: string): Observable<T> {
  let header = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

  let params = new HttpParams()
    .append('username', userName)
    .append('password', password)
    .append('grant_type', 'password');

  let requestBody = params.toString();

  return this.http.post<T>(this.loginUrl, requestBody, { headers: header });
}

private getRefreshLoginEndpoint<T>(): Observable<T> {
  let header = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });

  let params = new HttpParams()
    .append('refresh_token', this.refreshToken)
    .append('grant_type', 'refresh_token');
    //.append('scope', 'openid profile offline_access');

  let requestBody = params.toString();

  return this.http.post<T>(this.loginUrl, requestBody, { headers: header }).pipe<T>(
    catchError(error => {
      return this.handleError(error, () => this.getRefreshLoginEndpoint());
    }));
}

protected getRequestHeaders(): { headers: HttpHeaders | { [header: string]: string | string[]; } } {
  let headers = new HttpHeaders({
    'Authorization': 'Bearer ' + this.accessToken,
    'Content-Type': 'application/json',
    'Accept': ' application/json, text/plain, */*'
  });

  return { headers: headers };
}

protected handleError(error, continuation: () => Observable<any>) {

  if (error.status == 401) {
    if (this.isRefreshingLogin) {
      return this.pauseTask(continuation);
    }

    this.isRefreshingLogin = true;

    return this.refreshLogin().pipe(
      mergeMap(data => {
        this.isRefreshingLogin = false;
        this.resumeTasks(true);

        return continuation();
      }),
      catchError(refreshLoginError => {
        this.isRefreshingLogin = false;
        this.resumeTasks(false);

        if (refreshLoginError.status == 401 || (refreshLoginError.url && refreshLoginError.url.toLowerCase().includes(this.loginUrl.toLowerCase()))) {
          this.reLogin();
          return throwError('session expired');
        }
        else {
          return throwError(refreshLoginError || 'server error');
        }
      }));
  }

  if (error.url && error.url.toLowerCase().includes(this.loginUrl.toLowerCase())) {
    this.reLogin();

    return throwError((error.error && error.error.error_description) ? `session expired (${error.error.error_description})` : 'session expired');
  }
  else {
    return throwError(error);
  }
}

private pauseTask(continuation: () => Observable<any>) {
  if (!this.taskPauser)
    this.taskPauser = new Subject();

  return this.taskPauser.pipe(switchMap(continueOp => {
    return continueOp ? continuation() : throwError('session expired');
  }));
}

private resumeTasks(continueOp: boolean) {
  setTimeout(() => {
    if (this.taskPauser) {
      this.taskPauser.next(continueOp);
      this.taskPauser.complete();
      this.taskPauser = null;
    }
  });
}
//#endregion

}
