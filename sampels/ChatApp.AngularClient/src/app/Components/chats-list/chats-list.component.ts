import { Component, OnInit } from '@angular/core';
import { Chat } from 'src/app/Shared/Models/ChatAppModels';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';
import { ChatType } from 'src/app/Shared/Enums/ChatType';
import { User } from 'src/app/Shared/models/user.model';

@Component({
  selector: 'app-chats-list',
  templateUrl: './chats-list.component.html',
  styleUrls: ['./chats-list.component.scss']
})
export class ChatsListComponent implements OnInit {
  chats=new Array<Chat>();
  private _currentUser:User;
  constructor(private _chatAppManager:ChatAppManagerService) {  
    this._chatAppManager.CurrentUserLoder.subscribe(u=>this._currentUser =u);
    
    let sub = this._chatAppManager.loadChats()
    .subscribe(value=>{
      this.chats = value;
      setTimeout(() => {
       sub.unsubscribe(); 
      });
    }); }

  ngOnInit() {
   
  }
  switchChat( chatId:string){
    this._chatAppManager.switchChat(chatId);
    //switch the chat window to display the current chat ;
  } 
  
  getChatName(chat:Chat):string{
    var chatName="";
    if(chat.chatType == ChatType.Personal){
      if(chat.name){
        chatName = chat.name;
      }
      else{
      chat.users.forEach(u=>{
        if(u.userName != this._currentUser.userName)
        {
          chatName = u.userName;
        }
      })
      }
    }
    else if(chat.chatType == ChatType.Group)
    {
      chatName = chat.name ? chat.name:'Group';
    }
    else if(chat.chatType == ChatType.ChatRoom)
    {
      chatName = chat.name;
    }
    return chatName ;
  }
}
