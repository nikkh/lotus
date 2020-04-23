import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TerrorvisionComponent } from './terrorvision.component';

describe('TerrorvisionComponent', () => {
  let component: TerrorvisionComponent;
  let fixture: ComponentFixture<TerrorvisionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TerrorvisionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TerrorvisionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
