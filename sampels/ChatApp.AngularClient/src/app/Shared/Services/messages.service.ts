import { Injectable, Injector } from '@angular/core';
import { EndpointFactory } from './Helpers/endpoint-factory.service';
import { HttpClient } from '@angular/common/http';
import { ConfigurationService } from './Helpers/configuration.service';
import { Observable } from 'rxjs';
import { Message, Chat } from '../Models/ChatAppModels';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class MessagesService  extends EndpointFactory{
  
  //ChatApp/Messages/{chatId}/{skip}/{take}
  private get messagesUrl() { return this.apiBaseUrl + "/ChatApp/Messages"; }

  constructor(http:HttpClient,configurations:ConfigurationService,injector:Injector) {
    super(http,configurations,injector);
  }
  sendMessage(message:Message):Observable<Message>{
    return this.http.post(this.messagesUrl,JSON.stringify(message),this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.sendMessage(message)))
    );
  }
  getMessages(chat:Chat,take=10):Observable<Message[]>{
    if(!chat)
    throw "invalid chat !";
    
    if(take < 1){
      return new Observable<Message[]>(); 
    }
    let skip = chat.messages?chat.messages.length:0;
    let endpoint = this.messagesUrl +`/${chat.id}/${skip}/${take}`;
    return this.http.get(endpoint,this.getRequestHeaders()).pipe(
      catchError(error=>this.handleError(error,()=>this.getMessages(chat,take)))
    );
  }
}
