import { Injectable, Injector } from '@angular/core';
import { EndpointFactory } from './Helpers/endpoint-factory.service';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from './Helpers/configuration.service';
import { Observable } from 'rxjs';
import { Message, FriendRequest, Chat, Contact } from '../Models/ChatAppModels';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FriendRequestsService extends EndpointFactory{

  //#region Api EndPoints
  private get allFriendRequests() { return this.apiBaseUrl + "/ChatApp/FriendRequests/All"; }
  private get friendRequestsFrom() { return this.apiBaseUrl + "/ChatApp/FriendRequests/From"; }
  private get friendRequestsTo() { return this.apiBaseUrl + "/ChatApp/FriendRequests/To"; }
  private get sendFriendRequestUrl  (){return this.apiBaseUrl+ "/ChatApp/FriendRequests/Send";}
  private get acceptFriendRequestUrl(){return this.apiBaseUrl+ "/ChatApp/FriendRequests/Accept";}
  private get removeFriendRequestUrl(){return this.apiBaseUrl+ "/ChatApp/FriendRequests/Remove";}
  //#endregion
  
  constructor(http:HttpClient,configurations:ConfigurationService,injector:Injector) {
    super(http,configurations,injector);
  }
  getAllRequests():Observable<FriendRequest[]>{
    return this.http.get(this.allFriendRequests,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getAllRequests()))
    )
  }
  getRequestsFrom():Observable<FriendRequest[]>{
    return this.http.get(this.friendRequestsFrom,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getRequestsFrom()))
    )
  }
  getRequestsTo():Observable<FriendRequest[]>{
    return this.http.get(this.friendRequestsTo,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getRequestsTo()))
    )
  }
  sendFriendRequest(toUserId:string):Observable<FriendRequest>{
    let endpoint = `${this.sendFriendRequestUrl}/${toUserId}`;
    return this.http.post(endpoint,{toUserId:toUserId},this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.sendFriendRequest(toUserId)))
    )
  }
  acceptFriendRequest(requestId:string):Observable<{requestId:string,contact:Contact}>{
    let endpoint = `${this.acceptFriendRequestUrl}/${requestId}`;
    return this.http.post(endpoint,{requestId:requestId},this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.acceptFriendRequest(requestId)))
    )
  }
  removeFriendRequest(requestId:string):Observable<boolean>{
    let endpoint= `${this.removeFriendRequestUrl}/${requestId}`;
    return this.http.delete(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.removeFriendRequest(requestId)))
    )
  }
}
