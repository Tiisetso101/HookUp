import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalref?: BsModalRef<ConfirmDialogComponent>;

  constructor(private modalService: BsModalService) { }

  confirm( title = 'Confirmation',
    message = 'Are you sure about this?',
    btnOkText = 'Ok',
    btnCancelText= 'Cancel'): Observable<boolean>{

      const config = {
        initialState :{
          title,
          message,
          btnOkText,
          btnCancelText
        }
      }
   this.bsModalref = this.modalService.show(ConfirmDialogComponent, config);
   return this.bsModalref.onHidden!.pipe(
    map(() => {
      return this.bsModalref!.content!.result;
    })
   )
  }
}
