import { Injectable} from '@angular/core';
import { Chat,Message,Contact, FriendRequest} from '../Models/ChatAppModels';
import { Observable,  Subject} from 'rxjs';
import { User } from '../Models/user.model';
import { ChatType } from '../Enums/ChatType';
import { LocalStoreManager } from './Helpers/local-store-manager.service';
import { ContatctsService } from './contatcts.service';
import { ChatService } from './chat.service';
import { MessagesService } from './messages.service';
import { map } from 'rxjs/operators';
import { AuthService } from './auth.service';
import * as signalR from '@aspnet/signalr';
import { ConfigurationService } from './Helpers/configuration.service';
import { SignalRService } from './signalR.service';
import { Status } from '../Enums/Status';

@Injectable({
  providedIn: 'root'
})
export class ChatAppManagerService {
  
  private _contactsLoader = new Subject<Contact[]>();
  private _chatsLoader = new Subject<Chat[]>();
  private _chatLoader = new Subject < Chat > ();
  private _currentUserLoader = new Subject<User>();
  private _currentChat = new Chat();
  private _currentUser:User;
  private _contacts:Contact[];
  private _chats=new Array<Chat>();
  constructor(
      private _signalRService:SignalRService,
      private _contactService:ContatctsService,
      private _chatService:ChatService,
      private _authService:AuthService,
      private _messageService:MessagesService,
    ) {
    this._chatLoader.subscribe(c=>this._currentChat = c);
    this.initSignalR();
  }

  get ChatLoader(){
    return this._chatLoader.asObservable();
  }
  get CurrentUserLoder(){
    if(!this._currentUser)
      this._currentUser = this._authService.currentUser;
      this._currentUser.status = Status.Online;
    setTimeout(() => {
      this._currentUserLoader.next(this._currentUser);
    });
    return this._currentUserLoader.asObservable();
  }


  logout(){
    this._signalRService.desConnect();
    this._authService.logout();
    setTimeout(() => {
      this._chats = new Array<Chat>();
      this._contacts =null;
      this._currentUser =null;
      this._currentChat =null;
    }, 500);
  }

  LoadContacts(reload=false):Observable<Contact[]>{
    if(this._contacts && !reload){
      setTimeout(() => {
        this._contactsLoader.next(this._contacts); 
      });
    }
    else {
      let subscription = this._contactService.getAllContacts()
      .subscribe(
        value=>{
          this._contacts = value;
          this._contactsLoader.next(this._contacts);
          setTimeout(() => {
              subscription.unsubscribe();
            });
        });
    }
    return this._contactsLoader.asObservable();
  }
  
  loadChats(reload=false):Observable<Chat[]>{
    if(this._chats && !reload){
      setTimeout(() => {
        this._chatsLoader.next(this._chats); 
      });
    }
    return this._chatsLoader.asObservable();
  }
  
  chatWithUser(userId:string){
    let chat = this._chats.find(c=>c.chatType==ChatType.Personal && !!c.users.find(u=>u.id==userId));
    
    if(chat) this.switchChat(chat.id);
    else{
     this._chatLoader.next(null);//this line for loadding spinar
     let sub = this._chatService.getChatWithUserId(userId)
      .subscribe(
        chat=>{
          this._chats.unshift(chat);
         this.loadMessages(chat);
          this.switchChat(chat.id);
          setTimeout(() => {
            sub.unsubscribe(); 
          });
      })
    }
  }

  switchChat(chatId: string){
   let chat = this._chats.find(value=>value.id == chatId);
   if(chat) {  
    this._chatLoader.next(chat);
    }
   else{
     this._chatLoader.next(null);//this line for loadding spinar
     this._chatService.getChatById(chatId)
      .subscribe(c=>{
        this._chats.unshift(c);
        this.loadMessages(c);
        this._chatLoader.next(c);
      })
   }

  }
  
  sendMessageAsync(message:Message):Observable<Message> {
    message.chatId = this._currentChat.id;
    message.fromUserId =this._currentUser.id;
    return this._messageService.sendMessage(message).pipe(
      map(m=>{
        if(m){
        this.receiveMessage(m);
        }
        return m;
      })
    );
  }

  getOlderMessages(chat:Chat,take?:number){
    return this._messageService.getMessages(chat,take);
  }
  getImageUrl(image:string):string{
    return image;
  }
  private loadMessages(chat:Chat){
    let sub = this._messageService.getMessages(chat)
    .subscribe(msgs=>{
      msgs.forEach(msg=>{
        chat.messages.unshift(msg);
      });
      setTimeout(() => {
       sub.unsubscribe(); 
      });
    })
  }
  private receiveMessage(msg:Message){
    let index:number;
    let chat = this._chats.find((c,i)=>{
      index =i ;
      return c.id == msg.chatId;
    });
    if(chat){
     chat.messages.push(msg);
      chat = this._chats.splice(index,1)[0];
      this._chats.unshift(chat);
    }
    else
    this._chatService.getChatById(msg.chatId)
    .subscribe(c=>{
      c.messages.reverse();
      console.log({chat:c});
     // c.messages.find(m=>m.id == msg.id) ?"":c.messages.unshift(msg);
      this._chats.unshift(c);
    })
  }
  //#region signalR
  private initSignalR(){
    this._signalRService.connect();
    this._signalRService.receiveMessage.subscribe(msg=>{
      this.receiveMessage(msg);
    });
    this._signalRService.receiveFriendRequest.subscribe(request=>{

    });
    this._signalRService.receiveFriendRequestResponse.subscribe(response=>{
      if(response.contact)
      {
        this._contacts.unshift(response.contact);
        //this.chatWithUser(response.contact.id);
      }
    })
    this._signalRService.receiveStatusChanged.subscribe(response=>{
      let contact = this._contacts.find(c=>c.id==response.userId);
      if(contact)
        contact.status=response.status;
      else if(this._currentUser.id == response.userId)
       {
        this._currentUser.status = response.status;
        this._currentUserLoader.next(this._currentUser);
       }
    })
  }
  changeUserStatus(status:Status){
    if(this._currentUser.status != status)
    this._signalRService.statusChanged(status);
    this._currentUser.status = status
  }
  //#endregion
}
