import { TestBed } from '@angular/core/testing';

import { SignalRConnectionService } from './signal-r-connection.service';

describe('SignalRConnectionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SignalRConnectionService = TestBed.get(SignalRConnectionService);
    expect(service).toBeTruthy();
  });
});
