import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { AssignQuestionBankEmployeesDialogComponent } from './dialogs/assign-question-bank-employees/assign-question-bank-employees.dialog.component';
import { QuestionBankRequestDetails, QuestionBankRequestStatusIds } from './models/question-bank-request.models';
import { QuestionBankRequestsService } from './services/question-bank-requests.service';

@Component({ selector: 'app-question-bank-request-details', standalone: true, templateUrl: './question-bank-request-details.page.html',
  imports: [CommonModule, TranslatePipe, I18nNamespaceDirective, ButtonModule, TableModule, TagModule], providers: [DialogService] })
export class QuestionBankRequestDetailsPage implements OnInit {
  private readonly service=inject(QuestionBankRequestsService); private readonly route=inject(ActivatedRoute); private readonly dialogs=inject(DialogService);
  private readonly auth=inject(AuthService); private readonly translate=inject(TranslateService); private readonly notification=inject(NotificationService);
  private readonly language=inject(LanguageService); private readonly destroyRef=inject(DestroyRef); readonly details=signal<QuestionBankRequestDetails|null>(null);
  readonly loading=signal(false); readonly currentLang=signal<Lang>(this.language.get()); readonly id=this.route.snapshot.paramMap.get('id')!;
  ngOnInit():void { this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(x=>this.currentLang.set(x)); this.load(); }
  name(ar?:string|null,en?:string|null):string{return(this.currentLang()==='ar'?ar:en)||'-';}
  canAssign():boolean{const status=this.details()?.statusId;return this.auth.hasPermission(Permissions.QuestionBankRequests.Assign) && (status===QuestionBankRequestStatusIds.PendingAssignment||status===QuestionBankRequestStatusIds.QuestionEntryInProgress);}
  openAssign():void { this.service.eligibleEmployees(this.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({next:employees=>{const localized=employees.map(x=>({...x,name:this.name(x.nameAr,x.nameEn)}));const ref=this.dialogs.open(AssignQuestionBankEmployeesDialogComponent,{header:this.translate.instant('QUESTION_BANK_REQUESTS.ASSIGN_EMPLOYEES'),width:'900px',modal:true,breakpoints:{'992px':'95vw'},data:{requestId:this.id,employees:localized}});ref?.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(ok=>{if(ok){this.notification.success(this.translate.instant('QUESTION_BANK_REQUESTS.ASSIGN_SUCCESS'));this.load();}});},error:()=>this.notification.error(this.translate.instant('QUESTION_BANK_REQUESTS.EMPLOYEE_LOAD_ERROR'))}); }
  private load():void {this.loading.set(true);this.service.details(this.id).pipe(finalize(()=>this.loading.set(false)),takeUntilDestroyed(this.destroyRef)).subscribe({next:x=>this.details.set(x),error:()=>this.notification.error(this.translate.instant('QUESTION_BANK_REQUESTS.DETAILS_LOAD_ERROR'))});}
}
