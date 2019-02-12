import { Component, OnInit, ViewChild, ElementRef,AfterViewInit } from '@angular/core';
import { Chat, Message } from 'src/app/Shared/Models/ChatAppModels';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';
import { ChatType } from 'src/app/Shared/Enums/ChatType';
import { Subject } from 'rxjs';
import { distinctUntilChanged, debounceTime, filter, switchMap } from 'rxjs/operators';
import { Utilities } from 'src/app/Shared/Services/Helpers/utilities';
import { User } from 'src/app/Shared/models/user.model';

@Component({
  selector: 'app-chat-window',
  templateUrl: './chat-window.component.html',
  styleUrls: ['./chat-window.component.scss'],
})
export class ChatWindowComponent implements OnInit,AfterViewInit {
  
  @ViewChild('msgsList') private messagesList: ElementRef;

  private scrollSubj=new Subject<number>();
  message:string="";
  chat:Chat;
  chatLoadding:"Appstart"|"loadding"|"loadded"="Appstart";
  private _currentUser:User;
  constructor(private _ChatAppManager:ChatAppManagerService) { 
    this._ChatAppManager.CurrentUserLoder.subscribe(u=>this._currentUser =u);
   
  }
  ngAfterViewInit(){
    
    let take = 10;
    this.scrollSubj.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      filter(v=>v<5),
      switchMap(value=>{
        return this._ChatAppManager.getOlderMessages(this.chat,take);
      }))
      .subscribe( msgs=>{
        console.log({msgs:msgs});
        take = msgs.length == take? take : 0;
        msgs.forEach(msg=>{
          this.chat.messages.unshift(msg);
        });
        this.chatLoadding="loadding";
        setTimeout(() => {
          this.chatLoadding ="loadded";
        }, 700);
      });
    this._ChatAppManager.ChatLoader.subscribe(c=>
      {
          this.chat = c;
          this.scrollBottom();
        if(c){
          this.chatLoadding = "loadded";
        }
        else {
          this.chatLoadding = "loadding";
        }
          
      });
  }
  ngOnInit() {
  }

  messageChanged(event:Event){
    let key = (<KeyboardEvent>event).keyCode;
    if(key === 13)
    this.sendMessage();
  }

  sendMessage(){
    if(this.message.trim()){
      var m = new Message();
      m.content = this.message;
      this._ChatAppManager.sendMessageAsync(m)
        .subscribe(msg=>{
          setTimeout(() => {
            this.scrollBottom();
          });
        })
    }
    this.message = "";
  }
  
  isMe(userId:string):boolean{
    return this._currentUser.id == userId;
  }
  getLines(s:string):string[]{
    var x = s.split("\n");
    return x;
  }
  getChatName():string{
    var chatName:string ;
    if(!this.chat)
    {
      return "ChatApp";
    }
    else if(this.chat.chatType == ChatType.Personal){
      if(this.chat.name){
        chatName = this.chat.name;
      }
      else{
      this.chat.users.forEach(u=>{
        if(u.userName != this._currentUser.userName)
        {
          chatName = u.userName;
        }
      })
      }
    }
    else if(this.chat.chatType == ChatType.Group)
    {
      chatName = this.chat.name ? this.chat.name:'Group';
    }
    else if(this.chat.chatType == ChatType.ChatRoom)
    {
      chatName = this.chat.name;
    }
    return chatName ? chatName:"ChatApp"
  }
  onScroll(){
    let elem =<HTMLElement> this.messagesList.nativeElement;
    this.scrollSubj.next(elem.scrollTop);
    // console.log({scrollHeight:elem.scrollHeight,scrollTop:elem.scrollTop,clientHeight:elem.clientHeight})
  }
  scrollBottom(){
    setTimeout(() => {
    let elem = <HTMLElement>this.messagesList.nativeElement;
    elem.scrollTop = elem.scrollHeight; 
    },200);
  }

  printDate(d:Date){
    let date = new Date(d)
    return Utilities.printFriendlyDate(date);
    // return `Month:${date.getMonth()},Day:${date.getDay()},Hour:${date.getHours()},Minutes:${date.getMinutes()}`
  }

  OnRecorde(){
  }
}