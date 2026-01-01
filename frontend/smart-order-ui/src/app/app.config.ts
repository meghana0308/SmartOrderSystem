import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core'
import { provideRouter } from '@angular/router'
import { provideClientHydration, withEventReplay } from '@angular/platform-browser'
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http'
import { jwtInterceptor } from './core/interceptors/jwt-interceptor'
import { appRoutes } from './app.routes'

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(appRoutes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withFetch(), withInterceptors([jwtInterceptor]))
  ]
}
