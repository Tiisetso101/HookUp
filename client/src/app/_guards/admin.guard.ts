import { CanActivateFn } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export class adminGuard {
  constructor(private accountService: AccountService, private toastrService: ToastrService) { }


  canActivate(): Observable<boolean> {

    return this.accountService.currentUser$.pipe(
      map(user => {
        if (!user) return false;
        if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
          return true;
        }
        else {
          this.toastrService.error('You are not autorized for this')
          return false;
        }
      })
    )

  }

};
