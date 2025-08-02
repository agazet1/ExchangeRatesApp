import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { API_BASE_URL } from './services/api.services';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { firstValueFrom } from 'rxjs';

let appConfigData: AppConfig;

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    {
      provide: APP_INITIALIZER,
      useFactory: loadConfig,
      deps: [HttpClient],
      multi: true
    },
    {
      provide: 'APP_CONFIG',
      useFactory: () => appConfigData,
    },
    { 
      provide: API_BASE_URL, 
      useFactory: () => appConfigData.apiBaseUrl 
    },
    provideHttpClient(), 
    importProvidersFrom(BrowserAnimationsModule) 
  ]
};



export interface AppConfig {
  apiBaseUrl: string;
}

export function loadConfig(http: HttpClient): () => Promise<void> {
  return () => firstValueFrom(http.get<AppConfig>('/assets/config/config.json'))
              .then(config => {
                appConfigData = config;
              });
}

