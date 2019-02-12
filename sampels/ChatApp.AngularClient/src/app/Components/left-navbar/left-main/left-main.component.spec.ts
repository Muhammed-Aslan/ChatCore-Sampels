import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LeftMainComponent } from './left-main.component';

describe('LeftMainComponent', () => {
  let component: LeftMainComponent;
  let fixture: ComponentFixture<LeftMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LeftMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LeftMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
