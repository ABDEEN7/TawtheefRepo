import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignFiles } from './assign-files';

describe('AssignFiles', () => {
  let component: AssignFiles;
  let fixture: ComponentFixture<AssignFiles>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignFiles]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignFiles);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
