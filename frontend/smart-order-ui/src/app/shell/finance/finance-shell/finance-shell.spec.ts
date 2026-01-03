import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinanceShellComponent } from './finance-shell';

describe('FinanceShell', () => {
  let component: FinanceShellComponent;
  let fixture: ComponentFixture<FinanceShellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinanceShellComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinanceShellComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
