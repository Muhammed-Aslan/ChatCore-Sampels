import { Injectable, Injector } from '@angular/core';
import { switchMap, catchError } from 'rxjs/operators';
import { User } from '../models/user.model';
import { UserRegister } from '../Models/user-register.model';
import { LoginResponse } from '../models/login-response.model';
import { ConfigurationService } from './Helpers/configuration.service';
import { EndpointFactory } from './Helpers/endpoint-factory.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Contact } from '../Models/ChatAppModels';

@Injectable()
export class AccountService extends EndpointFactory  {
  //#region Api EndPoints
  private get usersUrl() { return this.apiBaseUrl + "/ChatApp/account"; }
  private get userByIdUrl(){ return this.apiBaseUrl + "/ChatApp/account/userId" ;}
  private get userByUserNameUrl() { return this.apiBaseUrl + "/ChatApp/account/username"; }
  private get currentUserUrl() { return this.apiBaseUrl + "/ChatApp/account/me"; }
  private get registerAppUserUrl(){ return this.apiBaseUrl + "/Account/Register"; }
  private get registerChatUserUrl(){ return this.apiBaseUrl + "/ChatApp/account/Register"; }
  //#endregion
  
  constructor(http: HttpClient, configurations: ConfigurationService, injector: Injector) { 
    super(http, configurations, injector);
  }

  getCurrentUser(accessToken?:string): Observable<User> {
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
    return this.http.get(this.currentUserUrl, headerObj).pipe(
      catchError(error => {
         return this.handleError(error, () => this.getCurrentUser(accessToken));
   }));
  }

  getUserById(userId?: string): Observable<Contact>  {
    let endpointUrl = `${this.userByIdUrl}/${userId}`;

    return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(
      catchError(error => {
        return this.handleError(error, () => this.getUserById(userId));
      }));
  }

  getUserByUserName(userName: string): Observable<Contact> {
    let endpointUrl = `${this.userByUserNameUrl}/${userName}`;

    return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(
      catchError(error => {
        return this.handleError(error, () => this.getUserByUserName(userName));
      }));
  }

  getUsers(page?: number, pageSize?: number,userName?:string): Observable<Contact[]> {
    page = page >= 1 ? page : 1; 
    pageSize = pageSize >= 1 ? pageSize : 5; 
    userName = userName ? userName : "";
    let endpointUrl = `${this.usersUrl}/${page}/${pageSize}/${userName}`;

    return this.http.get(endpointUrl, this.getRequestHeaders()).pipe(
      catchError(error => {
        return this.handleError(error, () => this.getUsers(page, pageSize,userName));
      }));
  }

  updateUser<T>(user: User): Observable<T> {
      return this.http.post<T>(this.currentUserUrl, JSON.stringify(user), this.getRequestHeaders()).pipe<T>(
        catchError(error => { return this.handleError(error, () => this.updateUser(user));})
        );
  }

  registerAppUser<T>(user: UserRegister): Observable<T> {
    user.rememberMe = user.rememberMe ? true : false;
    return this.http.post<T>(this.registerAppUserUrl, JSON.stringify(user),this.getRequestHeaders()).pipe(
      catchError(error => { return this.handleError(error, () => this.registerAppUser(user)); })
    );
  }

  registerChatUser(user: UserRegister,accessToken?:string): Observable<User> {
    if(accessToken)
    {
      let headers = new HttpHeaders({
        'Authorization': 'Bearer ' + accessToken,
        'Content-Type': 'application/json',
        'Accept': ' application/json, text/plain, */*'
      });
      return this.http.post(this.registerChatUserUrl, JSON.stringify(user), { headers: headers }).pipe(
        catchError(error => { return this.handleError(error, () => this.registerChatUser(user));})
        );
    }
    else
    {
      return this.http.post(this.registerChatUserUrl, JSON.stringify(user), this.getRequestHeaders()).pipe(
        catchError<User,User>(error => this.fixTokenHelper(user) )
        );
    }
  }
  
  private fixTokenHelper(user: UserRegister):Observable<User>{
    return this.getLoginEndpoint<LoginResponse>(user.userName, user.password).pipe(
      switchMap(response =>{
          return this.registerChatUser(user,response.access_token);
    })
    );
  }
}