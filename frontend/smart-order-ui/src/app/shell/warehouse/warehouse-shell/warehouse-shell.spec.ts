import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseShell } from './warehouse-shell';

describe('WarehouseShell', () => {
  let component: WarehouseShell;
  let fixture: ComponentFixture<WarehouseShell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WarehouseShell]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WarehouseShell);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
