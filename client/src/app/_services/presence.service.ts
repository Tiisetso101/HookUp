import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/Environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';
import { group } from '@angular/animations';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;

  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable(); 

  constructor(private tostr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token

      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next([...usernames, username])        
      })
    });

    

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next(usernames.filter(x => x !== username))
      })
    });

    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlineUserSource.next(username);
    });

    this.hubConnection.on('NewMessageRecieved', ({username, knownaAs}) => {
      this.tostr.info(knownaAs + ' has sent you a message')
      .onTap
      .pipe(take(1))
      .subscribe({
        next: () => this.router.navigateByUrl('/members/' + username +'?tab=Messages')
      })
    });
  }
  stopConnection() {
    this.hubConnection?.stop().catch(error => console.log(error))
  }



}
