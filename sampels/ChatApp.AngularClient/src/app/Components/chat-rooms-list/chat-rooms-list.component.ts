import { Component, OnDestroy } from '@angular/core';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { fromEvent, Observable, Subscription } from 'rxjs';
import { FromEventTarget } from 'rxjs/internal/observable/fromEvent';

@Component({
  selector: 'app-chat-rooms-list',
  templateUrl: './chat-rooms-list.component.html',
  styleUrls: ['./chat-rooms-list.component.scss']
})
export class ChatRoomsListComponent implements OnDestroy {

  private _searchObs:Observable<string>;
  private _searchSubscription:Subscription;
  constructor() { }

  ngOnDestroy(){
    if(this._searchSubscription)
    {
      this._searchSubscription.unsubscribe();
    }
  }
  search(event:Event){
    if(!this._searchObs)
    {
      this._searchObs = fromEvent((<FromEventTarget<Event>>event.target),"input")
            .pipe(
              map(e=>
                  (<HTMLInputElement>e.target).value
                ),
              debounceTime(800),
              distinctUntilChanged()
              );
      this._searchSubscription = this._searchObs.subscribe(v=>{
        if(v.trim()==="") return;
        //search logic
        console.log(v)
      });
    }
  }
  joinChatRoom(id:string){
    //
    console.log(id);
  }
}
