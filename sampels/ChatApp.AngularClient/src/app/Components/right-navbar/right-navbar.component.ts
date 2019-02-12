import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';
import { Chat, Contact } from 'src/app/Shared/Models/ChatAppModels';
import { ChatType } from 'src/app/Shared/Enums/ChatType';
import { User } from 'src/app/Shared/models/user.model';
import { ContatctsService } from 'src/app/Shared/Services/contatcts.service';

@Component({
  selector: 'app-right-navbar',
  templateUrl: './right-navbar.component.html',
  styleUrls: ['./right-navbar.component.scss']
})
export class RightNavbarComponent implements OnInit {
  @Output('closed') closedEvent=new EventEmitter();
  @Output('available') availabledEvent = new EventEmitter<boolean>();
  statusMessage:string;
  chatName:string;
  image:string;
  deleteBtnTxt:string;
  currentChat:Chat
  currentUser:User;
  user:Contact;
  enabled=false;
  constructor(private _chatAppManager:ChatAppManagerService,private _contactService:ContatctsService) { 
    this.image="/assets/images/shiba1.jpg";
    this._chatAppManager.ChatLoader.subscribe(c=>{
      this.currentChat =c;
      if(c)
      this.init();
    });
    this._chatAppManager.CurrentUserLoder.subscribe(u=>{
      this.currentUser= u;
    })
  }

  ngOnInit() {
    
  }
  close(){
    this.closedEvent.emit();
  }
  deleteUser(){
    this._contactService.removeContact(this.user.id)
    .subscribe(c=>{
     console.log({deletedContact:c});
      console.log("delete user with userName : ",this.user.userName);

    });
  }
  init(){
    if(this.currentChat.chatType== ChatType.Personal)
      {
        this.user = this.currentChat.users.find(u=>u.id!=this.currentUser.id)
        this.statusMessage =this.user.statusMessage;
        this.chatName =this.user.userName;
        this.deleteBtnTxt = "Delete";
      }
    else{
        this.statusMessage =this.currentChat.description;
        this.chatName =this.currentChat.name;
        this.deleteBtnTxt = "Delete";
      }
      this.enabled =true;
      this.availabledEvent.emit(this.enabled);
    }
}
