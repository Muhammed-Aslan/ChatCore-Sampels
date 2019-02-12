import { Component, OnDestroy } from '@angular/core';
import { AuthService } from './Shared/Services/auth.service';
import { Subscription } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnDestroy {

  title = 'ChatApp';
  rightNavOpend:boolean;
  leftNavOpend:boolean =true;
  isLoggedIn:boolean;
  profileEnable=false;
  loginStatusSubscription:Subscription;

  constructor(private _auth:AuthService) {
    this.isLoggedIn = this._auth.isLoggedIn;
    
    this.loginStatusSubscription = this._auth.getLoginStatusEvent().subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
    });
  }



  toggleRight(){
    this.rightNavOpend=!this.rightNavOpend;
    console.log(this.rightNavOpend );
  }  
  toggleLeft(){
    this.leftNavOpend=!this.leftNavOpend;
    console.log(this.leftNavOpend );
  }
  enableProfile(isEnable:boolean)
  {
    this.profileEnable =isEnable;
  }
  ngOnDestroy(){
    this.loginStatusSubscription.unsubscribe();
  }
}
