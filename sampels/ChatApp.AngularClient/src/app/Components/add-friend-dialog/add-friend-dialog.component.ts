import { Component, OnDestroy } from '@angular/core';
import { Subscription, Observable } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, switchMap, filter, catchError } from 'rxjs/operators';
import { Contact, FriendRequest } from 'src/app/Shared/Models/ChatAppModels';
import { FormControl, FormGroup } from '@angular/forms';
import { AccountService } from 'src/app/Shared/Services/account.service';
import { FriendRequestsService } from 'src/app/Shared/Services/friend-requests.service';
import { ChatAppManagerService } from 'src/app/Shared/Services/chat-app-manager.service';
import { User } from 'src/app/Shared/models/user.model';
import { SignalRService } from 'src/app/Shared/Services/signalR.service';

@Component({
  selector: 'app-add-friend-dialog',
  templateUrl: './add-friend-dialog.component.html',
  styleUrls: ['./add-friend-dialog.component.scss']
})
export class AddFriendDialogComponent implements OnDestroy {

  private _searchSubscription:Subscription;
  
  isLoaddingContacts:boolean;
  isLoaddingRequests=true;
  fGroup =new FormGroup({
    searchControl :new FormControl()
  });
  users = new Array<Contact>();
  allRequests = new Array<FriendRequest>();
  requestsFrom = new Array<FriendRequest>();
  requestsTo = new Array<FriendRequest>();
  private _currentUser:User;
  constructor(private _accountService:AccountService,
    private _friendRequestService:FriendRequestsService,
    private _chatAppManager:ChatAppManagerService,
    private _signalRService:SignalRService
    ) {
    this.loadRequests();
    this.enableSearch();
    this.initSignalR();
  }

  ngOnDestroy(){
    if(this._searchSubscription)
    {
      this._searchSubscription.unsubscribe();
    }
  }
  hasRequestFrom(userId:string){
    return !!this.requestsFrom.find(r=>r.toUser.id==userId);
  }
  sendRequest(toUserId:string){
    this._friendRequestService.sendFriendRequest(toUserId)
    .subscribe(r=>{
      this.allRequests.push(r);
      this.reDestributeRequests(this.allRequests);
    },err=>console.log(err));
  }

  acceptRequest(requestId:string){
    this._friendRequestService.acceptFriendRequest(requestId)
    .subscribe(
      respone=>{
        this.reDestributeRequests(this.allRequests.filter(r=>r.id!=requestId));
      });
  }

  removeRequest(requestId:string,userId?:string){
    requestId = requestId ? requestId : this.requestsFrom.find(r=>r.toUser.id == userId).id;
    this._friendRequestService.removeFriendRequest(requestId)
    .subscribe(
      result=>{
        if(result){
          this.reDestributeRequests( this.allRequests.filter(elem=>elem.id!=requestId));
        }
      }
    );
  }
  getImageUrl(image:string):string{
    this._chatAppManager.getImageUrl(image);
    return "assets/images/shiba2.jpg";
  }
  initSignalR(){
    this._signalRService.receiveFriendRequest.subscribe(request=>{
      this.allRequests.push(request);
      this.reDestributeRequests(this.allRequests);
    });
    this._signalRService.receiveFriendRequestResponse.subscribe(response=>{
      this.allRequests = this.allRequests.filter(r=>r.id != response.requestId);
      this.reDestributeRequests(this.allRequests);
      
    })
  }

  private loadRequests(){
    this.isLoaddingRequests =true;
    this._friendRequestService.getAllRequests().pipe(
      switchMap(requests=>{
        return this._chatAppManager.CurrentUserLoder.pipe(
          map(u=>{
            this._currentUser = u;
            return requests;
          })
        )
      })
    )
    .subscribe(
      requests=>{
        this.isLoaddingRequests = false;
        this.reDestributeRequests(requests);
      },
      err=>{
        this.isLoaddingRequests = false;
      }
    )
  }
  private enableSearch(){
    let searchControl=this.fGroup.get('searchControl');
    this._searchSubscription = searchControl.statusChanges.pipe(
      map(v=>{
        let value:string =searchControl.value;
        value = value.split(' ').reduce((prev,curr)=> prev + curr.trim());
        if(value != searchControl.value){
          searchControl.setValue(value);
        }
        this.isLoaddingContacts=true;
        return value;
      }),
      debounceTime(750),
      filter(value=> value ? this.isLoaddingContacts = true : this.isLoaddingContacts = false),
      distinctUntilChanged(),
      switchMap(inputVal=>{
        return this.search(inputVal);
      }))
      .subscribe(
        value=>{
          this.isLoaddingContacts = false;
          this.users = value;
        }
      )
  }
  private search(inputVal:string ,page=1,take=5,buffer?:Contact[]):Observable<Contact[]>{
      let tmp = buffer ? buffer : new Array<Contact>();
      return this._accountService.getUsers(page,take,inputVal).pipe(
      switchMap(users=>{
        return this._chatAppManager.LoadContacts().pipe(

        map(contacts=>{
            users.forEach(user => {
              if(!contacts.find(c=>c.id==user.id) && !this.requestsTo.find(r=>r.fromUser.id==user.id))
                tmp.push(user);
            });
            if(users.length == take * page && users.length - tmp.length < 3)
              throw("");          
            return tmp;
          })
        )
      }),catchError(error => {
        return  this.search(inputVal,++page,take,buffer);
      })
      )
  }
  private reDestributeRequests(requests:FriendRequest[]){
    this.allRequests = requests;
    this.requestsFrom = requests.filter(f=>f.fromUser.id ==this._currentUser.id);
    this.requestsTo = requests.filter(f=>f.toUser.id ==this._currentUser.id);
  }

}
