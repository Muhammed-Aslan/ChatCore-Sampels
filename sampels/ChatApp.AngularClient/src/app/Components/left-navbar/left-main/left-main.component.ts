import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material';
import { AddFriendDialogComponent } from '../../add-friend-dialog/add-friend-dialog.component';
import { Status } from 'src/app/Shared/Enums/Status';
import { User } from 'src/app/Shared/models/user.model';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';

@Component({
  selector: 'app-left-main',
  templateUrl: './left-main.component.html',
  styleUrls: ['./left-main.component.scss']
})
export class LeftMainComponent implements OnInit {
  @Output() profileOpened = new EventEmitter();
  selectedFilterValue:string;
  currentUserStatus:string='Online';
  currentUser= new User();
  constructor(private dialog: MatDialog,private _chatAppManager:ChatAppManagerService) { }

  ngOnInit() {
    this._chatAppManager.CurrentUserLoder.subscribe(u=>
      {
        this.currentUser=u;
        this.currentUserStatus= this.getStatuString(u.status);
      });
  }

  toggleProfile(){
    this.profileOpened.emit();  
  }
  logout(){
    this._chatAppManager.logout();
  }

  changeStatus(status:Status){
    this.currentUserStatus= this.getStatuString(status);
    this._chatAppManager.changeUserStatus(status);
  }
  openDialog() {
    this.dialog.open(AddFriendDialogComponent,{disableClose:true});
  }
  getStatuString(status:Status):string{
    switch (status) {
      case Status.Online:
        this.currentUserStatus = 'Online';
        break;
    
        case Status.Offline:
        this.currentUserStatus = 'Offline';
        break;
    
        case Status.Busy:
        this.currentUserStatus = 'Busy';
        break;

        case Status.Away:
        this.currentUserStatus = 'Away';
        break;
    
      default:
      this.currentUserStatus = 'Offline';
        break;
    }
        return this.currentUserStatus;
  }
  createChat(type:"group"|"chatRoom"){
    console.log("CREATE : ",type);

  }
}
