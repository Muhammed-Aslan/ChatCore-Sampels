import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { ConfigurationService } from './Helpers/configuration.service';
import { AuthService } from './auth.service';
import { FriendRequest, Contact, Message } from '../Models/ChatAppModels';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Utilities } from './Helpers/utilities';
import { Status } from '../Enums/Status';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  connection:signalR.HubConnection;

  private _receiveMsgSbj = new Subject<Message>();
  private _receiveFRSbj = new Subject<FriendRequest>();
  private _receiveFRResponseSbj = new Subject<{requestId:string , contact:Contact}>();
  private _receiveStatusChangedSbj = new Subject<{userId:string , status:Status}>();

  private isConnected:boolean;
  private countTry=0;
  constructor( private _configurations:ConfigurationService,private _authService:AuthService) {
   }
   
   connect(){
     if(!this.isConnected)
     {
        this.isConnected = true;
        this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this._configurations.hubUrl,{ accessTokenFactory: () => this._authService.accessToken })
        .build();
        this.start().then(()=>{
        this.connection.on(SignalRMethod.Receive_Message,(message)=>{this._receiveMsgSbj.next(message);});
        this.connection.on(SignalRMethod.Receive_FriendRequst,(request)=>{this._receiveFRSbj.next(request);});
        this.connection.on(SignalRMethod.Receive_FriendRequstResponse,(requestId,contact)=>{this._receiveFRResponseSbj.next({requestId:requestId,contact:contact});});
        this.connection.on(SignalRMethod.Receive_StatusChanged,(userId,status)=>{this._receiveStatusChangedSbj.next({userId:userId,status:status});});
        });
      }
    }
   desConnect(){
    this.isConnected = false;
    this.connection.stop();
   }

  get receiveMessage(){
    return this._receiveMsgSbj.asObservable().pipe(map(msg=>{
      console.log({recivedMessage:msg});
      return msg;
    }));
  }
  get receiveFriendRequest(){
    return this._receiveFRSbj.asObservable().pipe(map(request=>{
    console.log(request);
    return request;
    }))
  }
  get receiveFriendRequestResponse(){
    return this._receiveFRResponseSbj.asObservable()
    .pipe(
      map(v=>{
      console.log({requestId:v.requestId,contact:v.contact});
      return v;
    }))
  }
  get receiveStatusChanged(){
    return this._receiveStatusChangedSbj.asObservable()
  }
  statusChanged(status:Status)
  {
    this.connection.send(SignalRMethod.Send_StatusChanged,status);
  }
  private async start() {
    let reconnectTimeOut =()=> Utilities.randomNumber(2000,20000);
    try {
      await this.connection.start();
      this.connection.onclose(() => { 
        if(this.isConnected)
        {
          let timeOut =reconnectTimeOut();
          console.log(`tring to reconnect after :${(timeOut/1000).toFixed()} second ...`);
          
          setTimeout(() => {
            this.start();
          }, timeOut);
        }
      });
    } 
    catch (err) {
      let timeOut = reconnectTimeOut();
      console.log(`error occored during start the connection
      retconnecting will start after ${(timeOut/1000).toFixed()} second`);
      setTimeout(() => {
          this.countTry++;
        if(this.countTry<5){
          this.start();
        }
        else console.log("'error : cannot established the Connection ");
      }, timeOut);
    }
  }
}

class SignalRMethod {

    public static readonly Receive_Message = "receiveMessage";
    public static readonly Receive_FriendRequst = "receiveFriendRequest";
    public static readonly Receive_FriendRequstResponse = "receiveFriendRequestResponse";
    public static readonly Receive_StatusChanged = "receiveStatusChanged";
    public static readonly Send_StatusChanged = "StatusChanged";
}