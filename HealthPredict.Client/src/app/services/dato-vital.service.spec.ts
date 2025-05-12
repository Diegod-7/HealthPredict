import { TestBed } from '@angular/core/testing';

import { DatoVitalService } from './dato-vital.service';

describe('DatoVitalService', () => {
  let service: DatoVitalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DatoVitalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
