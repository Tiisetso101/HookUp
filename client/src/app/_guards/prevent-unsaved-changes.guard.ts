import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { Injectable, inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn:'root'
})
export class unsavedChanges {

  canDeactivate(component: MemberEditComponent) : boolean{
    const confirmservice =  inject(ConfirmService);
    
    if(component.editForm?.dirty){
       confirmservice.confirm();
    }
    return true;
  }
}

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component: MemberEditComponent): boolean => {  
  
  
  return inject(unsavedChanges).canDeactivate(component);
};


