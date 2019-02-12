import { Component, OnInit ,EventEmitter ,Output } from '@angular/core';

@Component({
  selector: 'app-left-profile',
  templateUrl: './left-profile.component.html',
  styleUrls: ['./left-profile.component.scss']
})
export class LeftProfileComponent implements OnInit {
  @Output() profileClosed = new EventEmitter();
  selectedFilterValue:string;
  constructor() { }

  ngOnInit() {
  }
CloseProfile(){
  this.profileClosed.emit();
}

}
