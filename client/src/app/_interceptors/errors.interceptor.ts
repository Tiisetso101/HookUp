import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorsInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if(error) {
          switch(error.status){
            case 400: 
            if(error.error.errors){
              const modelStateErrors = [];
              for(const key in error.error.errors){
                if(error.error.errors[key]){
                  modelStateErrors.push(key)
                }
              }
              throw modelStateErrors.flat;
            }
            else {
              this.toastr.error(error.error, error.status.toString());
            }
            break;
            case 401: 
            this.toastr.error('Unauthorized', error.status.toString());
            break;
            case 404:
              this.router.navigateByUrl('./error-page');
              break;
            case 500:
              const navigateExtras: NavigationExtras = {state:{error: error.error}}
              this.router.navigateByUrl('./server-error', navigateExtras);
              break;
              default:
                this.toastr.error('unexpected error occured');
                console.log(error.message);
                break;
          }         

        }
        throw error;
        
  }))
  }
}
