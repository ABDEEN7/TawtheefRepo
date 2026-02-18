import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';
import {LanguageService} from '../../../core/services/language.service';
import {TranslateService} from '@ngx-translate/core';
import {ExternalLoginService} from '../../../core/auth/external-login';
import {HttpService} from '../../../core/http/http.service';
import {RESIDENCY_CHOSEN_MANUALLY_KEY, RESIDENCY_MODE_KEY} from '../../../core/constants/website-storage.const';
import { DialogService } from 'primeng/dynamicdialog';
import { QatarResidentOtpDialogComponent } from './components/qatar-resident-otp-dialog/qatar-resident-otp-dialog.component';
import {GeoIpService} from '../../../core/services/geo-ip.service';


type ResidencyMode = 'resident' | 'nonresident';
@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
  standalone: false
})
export class Login implements OnInit, OnDestroy{

  private lang = inject(LanguageService);
  readonly auth = inject(ExternalLoginService);
  private http = inject(HttpService);
  private dialog = inject(DialogService);
  private translate = inject(TranslateService);
  private geoIpService = inject(GeoIpService);

  currentLang: 'ar' | 'en' = 'ar';
  residencyMode: ResidencyMode = (localStorage.getItem(RESIDENCY_MODE_KEY) as ResidencyMode) || 'resident';

  private subs: Subscription[] = [];


  ngOnInit(): void {
    const s = this.lang.current$.subscribe(code => {
      this.currentLang = (code as 'ar' | 'en') || 'ar';
    });
    this.subs.push(s);

    // best-effort GeoIP (short timeouts) if user didn’t choose manually this session
    if (sessionStorage.getItem(RESIDENCY_CHOSEN_MANUALLY_KEY) !== '1') {
      this.bestEffortGeoip();
    }
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  toggleLang(): void {
    const next = this.currentLang === 'ar' ? 'en' : 'ar';
    this.lang.set(next);
  }

  setMode(mode: ResidencyMode): void {
    this.residencyMode = mode;
    localStorage.setItem(RESIDENCY_MODE_KEY, mode);
    sessionStorage.setItem(RESIDENCY_CHOSEN_MANUALLY_KEY, '1');
  }

  onSegmentKey(e: KeyboardEvent): void {
    if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
      const next: ResidencyMode = this.residencyMode === 'resident' ? 'nonresident' : 'resident';
      this.setMode(next);
    }
  }

  startLogin(kind: 'qatar_pass' | 'qatar_resident' | 'google'): void {
    switch (kind) {
      case 'google':
        this.auth.loginUsingGoogle();
        break;
      case 'qatar_pass':
        this.auth.loginUsingQatarPass();
        break;
      case 'qatar_resident':
        this.dialog.open(QatarResidentOtpDialogComponent, {
          header: this.translate.instant('auth.login.qatarResidentDialog.title'),
          contentStyle: { 'border-radius': '12px' },
          dismissableMask: false,
          draggable: false,
          closable: false
        });
        break;
    }
  }

  private bestEffortGeoip(): void {
    const sub = this.geoIpService.getCountryIso2().subscribe(code => {
      const mode: ResidencyMode = code === 'QA' ? 'resident' : 'nonresident';
      this.setMode(mode);
    });

    this.subs.push(sub);
  }
}
