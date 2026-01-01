import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { jwtInterceptor } from './interceptors/jwt-interceptor';

export const CORE_PROVIDERS = [
  {
    provide: HTTP_INTERCEPTORS,
    useValue: jwtInterceptor,
    multi: true
  }
];
