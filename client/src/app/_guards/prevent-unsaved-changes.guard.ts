import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';


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
  //return true;
};


