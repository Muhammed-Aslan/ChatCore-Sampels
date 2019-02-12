import { Component, OnInit, Input } from '@angular/core';
import { Contact } from 'src/app/Shared/Models/ChatAppModels';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';
import { Status } from 'src/app/Shared/Enums/Status';

@Component({
  selector: 'app-contacts-list',
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.scss']
})
export class ContactsListComponent implements OnInit {
status:string;
contacts = new Array<Contact>();
  constructor( private _chatAppManager:ChatAppManagerService) { 
   
  }

  ngOnInit() { 
    this._chatAppManager.LoadContacts()
    .subscribe(contacts=>{
      this.contacts = contacts;
    })
  }
  chatWith(userId:string){
    this._chatAppManager.chatWithUser(userId);
  }
  
  getUserStatus(contact:Contact):string{
    switch (contact.status) {
      case Status.Online:
        this.status = 'Online';
        break;
    
        case Status.Offline:
        this.status = 'Offline';
        break;
    
        case Status.Busy:
        this.status = 'Busy';
        break;

        case Status.Away:
        this.status = 'Away';
        break;
    
      default:
      this.status = 'Offline';
        break;
    }
        return this.status;
  }
}
