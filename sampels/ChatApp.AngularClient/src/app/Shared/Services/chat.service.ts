import { Injectable, Injector } from '@angular/core';
import { EndpointFactory } from './Helpers/endpoint-factory.service';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from './Helpers/configuration.service';
import { Observable } from 'rxjs';
import { Chat } from '../Models/ChatAppModels';
import { catchError } from 'rxjs/operators';
import { ChatType } from '../Enums/ChatType';

@Injectable({
  providedIn: 'root'
})
export class ChatService extends EndpointFactory {
  //#region Api EndPoints
  // GET: /ChatApp/Chat ..........=> ChatviewMode[]
  // GET: /ChatApp/Chat/chatId.... sting=>ChatviewMode
  // POST: /ChatApp/Chat.......... ChatviewMode => ChatviewMode
  private get chatUrl() { return this.apiBaseUrl + "/ChatApp/Chat"; }
  private get chatByIdUrl() { return this.apiBaseUrl + "/ChatApp/Chat/ChatId"; }
  private get chatWithUserUrl() { return this.apiBaseUrl + "/ChatApp/Chat/UserId"; }
  //#endregion
 
  constructor(http:HttpClient,configurations:ConfigurationService,injector:Injector) {
    super(http,configurations,injector);
  }

  getAllChats():Observable<Chat[]>{
    return this.http.get(this.chatUrl,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=> this.getAllChats()))
    );
  }

  getChatById(chatId:string):Observable<Chat>{
    let endpoint = `${this.chatByIdUrl}/${chatId}`;
    return this.http.get(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getChatById(chatId)))
    );
  }

  getChatWithUserId(userId:string):Observable<Chat>{
    let endpoint = `${this.chatWithUserUrl}/${userId}`;
    return this.http.get(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getChatWithUserId(userId)))
    );
  }

  createChatGroup(chat:Chat):Observable<Chat>{
    chat.chatType = ChatType.Group;
    return this.createChat(chat);
  }
  createChatRoom(chat:Chat):Observable<Chat>{
    chat.chatType = ChatType.ChatRoom;
    return this.createChat(chat);
  }
  private createChat(chat:Chat):Observable<Chat>{
    return this.http.post(this.chatUrl,JSON.stringify(chat),this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.createChat(chat)))
    )
  }
}
