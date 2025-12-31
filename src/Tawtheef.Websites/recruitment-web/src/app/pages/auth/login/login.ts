import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {race, Subscription, timer} from 'rxjs';
import {LanguageService} from '../../../core/services/language.service';
import {HttpClient} from '@angular/common/http';
import {catchError, map} from 'rxjs/operators';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {NgIf} from '@angular/common';
import {ExternalLoginService} from '../../../core/auth/external-login';
import {HttpService} from '../../../core/http/http.service';
import {RESIDENCY_CHOSEN_MANUALLY_KEY, RESIDENCY_MODE_KEY} from '../../../core/constants/website-storage.const';


type ResidencyMode = 'resident' | 'nonresident';
@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
  standalone: false
})
export class Login implements OnInit, OnDestroy{

  private lang = inject(LanguageService);
  private auth = inject(ExternalLoginService);
  private http = inject(HttpService);

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

  startLogin(kind: 'tawtheeq' | 'qatar_resident' | 'google'): void {
    switch (kind) {
      case 'tawtheeq':
        this.auth.loginUsingQatarPass();
        break;
      case 'google':
        this.auth.loginUsingGoogle();
        break;
      case 'qatar_resident':
        this.auth.loginAsQatarResident();
        break;
    }
  }

  private bestEffortGeoip(): void {
    const timeout$ = timer(1800).pipe(map(() => ({ country: '' })));

    const ipapi$ = this.http.get<any>('https://ipapi.co/json/').pipe(
      catchError(() => this.http.get<any>('https://ipwhois.app/json/')),
      catchError(() => this.http.get<any>('https://www.geoplugin.net/json.gp')),
      catchError(() => [ { country: '' } ] as any)
    );

    const sub = race(ipapi$, timeout$).subscribe(data => {
      const code = (data?.country || data?.country_code || data?.geoplugin_countryCode || '').toString().toUpperCase();
      if (!code) return;

      const mode: ResidencyMode = code === 'QA' ? 'resident' : 'nonresident';
      this.setMode(mode);
    });

    this.subs.push(sub);
  }
}
