import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

//https://stackoverflow.com/questions/75816948/canactivate-is-deprecated-how-to-refactor

@Injectable({
  providedIn : 'root'
})

export class AuthGuard {
  constructor(private service: AccountService, private toastr: ToastrService)
   {}

   canActivate() : Observable<boolean>{

    return this.service.currentUser$.pipe(
      map(user =>{
        if(user)
          return true;
          else{
            this.toastr.error('You are not autorized for this')
            return false;
          }
      })
    )

   }
}

export const authGuard: CanActivateFn = (
): Observable<boolean >  => {

  return inject(AuthGuard).canActivate();
};
