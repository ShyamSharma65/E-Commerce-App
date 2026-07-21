import { TestBed } from '@angular/core/testing';

import { Cutomerservice } from './cutomerservice';

describe('Cutomerservice', () => {
  let service: Cutomerservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Cutomerservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
