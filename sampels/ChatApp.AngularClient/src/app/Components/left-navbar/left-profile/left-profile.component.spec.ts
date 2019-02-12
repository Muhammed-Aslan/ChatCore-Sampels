import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LeftProfileComponent } from './left-profile.component';

describe('LeftProfileComponent', () => {
  let component: LeftProfileComponent;
  let fixture: ComponentFixture<LeftProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LeftProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LeftProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
