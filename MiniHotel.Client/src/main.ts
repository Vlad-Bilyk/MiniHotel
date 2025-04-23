import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app.module';

// import and register Ukrainian locale data
import { registerLocaleData } from '@angular/common';
import localeUk from '@angular/common/locales/uk';

registerLocaleData(localeUk, 'uk-UA');

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));
