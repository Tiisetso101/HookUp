import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/Environments/environment';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl = environment.apiUrl;
  private currentuserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentuserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any) {
    return this.http.post<User>(this.baseurl + 'Accounts/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentuser(user);
        }
        return user; // Add this line to return the user object to the consumer
      })
    );
  }
  register(model: any) {
    return this.http.post<User>(this.baseurl + 'Accounts/register', model).pipe(
      map((user => {
        if (user) {
          this.setCurrentuser(user);
        }

      }))
    )
  }
  setCurrentuser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentuserSource.next(user);
    this.presenceService.createHubConnection(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentuserSource.next(null);
    this.presenceService.stopConnection();
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split(',')[1]));
  }

}
