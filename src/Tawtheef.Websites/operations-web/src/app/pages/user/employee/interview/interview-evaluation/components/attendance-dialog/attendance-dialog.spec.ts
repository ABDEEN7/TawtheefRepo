import { TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { AttendanceStatus } from '../../models/enums';
import { AttendanceDialogComponent, AttendanceDialogData } from './attendance-dialog';

describe('AttendanceDialogComponent', () => {
  let dialogRef: jasmine.SpyObj<DynamicDialogRef>;
  let notify: jasmine.SpyObj<NotificationService>;

  function create(currentStatus: number | null = null) {
    const data: AttendanceDialogData = { candidateName: 'Candidate', candidateQid: null, jobTitle: 'Job', currentStatus };
    TestBed.configureTestingModule({
      imports: [AttendanceDialogComponent, TranslateModule.forRoot()],
      providers: [
        { provide: DynamicDialogConfig, useValue: { data } },
        { provide: DynamicDialogRef, useValue: dialogRef },
        { provide: NotificationService, useValue: notify },
      ],
    });
    const fixture = TestBed.createComponent(AttendanceDialogComponent);
    fixture.detectChanges();
    return fixture.componentInstance;
  }

  beforeEach(() => {
    dialogRef = jasmine.createSpyObj<DynamicDialogRef>('DynamicDialogRef', ['close']);
    notify = jasmine.createSpyObj<NotificationService>('NotificationService', ['error']);
  });

  it('offers only Present and Absent - withdrawal and lateness are operational issues', () => {
    const component = create();
    expect(component.statusOptions.map((o) => o.value)).toEqual([AttendanceStatus.Present, AttendanceStatus.NoShow]);
  });

  it('closes with the chosen status and requires one', () => {
    const component = create();

    component.confirm();
    expect(notify.error).toHaveBeenCalled();
    expect(dialogRef.close).not.toHaveBeenCalled();

    component.status = AttendanceStatus.NoShow;
    component.confirm();
    expect(dialogRef.close).toHaveBeenCalledWith(AttendanceStatus.NoShow);
  });
});
