import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesShell } from './sales-shell';

describe('SalesShell', () => {
  let component: SalesShell;
  let fixture: ComponentFixture<SalesShell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalesShell]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SalesShell);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
