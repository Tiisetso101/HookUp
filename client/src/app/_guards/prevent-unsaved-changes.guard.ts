import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn:'root'
})
export class unsavedChanges {

  canDeactivate(component: MemberEditComponent) : boolean{
    
    
    if(component.editForm?.dirty){
      return confirm('Are you sure');
    }
    return true;
  }
}

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component: MemberEditComponent): boolean => {  
  
  
  return inject(unsavedChanges).canDeactivate(component);
};


