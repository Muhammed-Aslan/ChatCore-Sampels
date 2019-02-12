import { Component, OnInit, Input } from '@angular/core';
@Component({
  selector: 'app-left-navbar',
  templateUrl: './left-navbar.component.html',
  styleUrls: ['./left-navbar.component.scss'],
  // animations:[
  //   trigger('toggleProfile',[
  //     state('closed',style({
  //       width:0        
  //     })),
  //     state('opened',style({
  //       width:"100%"
  //     })),
  //     transition('closed => opened',animate('0.5s ease-in'))
  //   ])
  // ]
})
export class LeftNavbarComponent implements OnInit {
  isProfileOpened:boolean ;
  // profileAnimationState:string = 'closed';
  constructor() { }
  ngOnInit() {
  }

toggleProfile(){
this.isProfileOpened = ! this.isProfileOpened;
// this.profileAnimationState = this.isProfileOpened?"opened":"closed";
// console.log(this.profileAnimationState)
}


}
