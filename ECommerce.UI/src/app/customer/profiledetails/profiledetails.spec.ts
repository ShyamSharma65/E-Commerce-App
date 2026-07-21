import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Profiledetails } from './profiledetails';

describe('Profiledetails', () => {
  let component: Profiledetails;
  let fixture: ComponentFixture<Profiledetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Profiledetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Profiledetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
