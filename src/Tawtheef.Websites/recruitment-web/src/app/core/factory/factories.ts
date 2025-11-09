import {MultiTranslateHttpLoader} from 'ngx-translate-multi-http-loader';
import {HttpBackend, HttpClient} from '@angular/common/http';

export function HomeLoaderFactory(http: HttpBackend) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: '/i18n/pages/home/', suffix: '.json' }
  ]);
}
