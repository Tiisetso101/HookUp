import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/Environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseurl = environment.apiUrl;
  private currentuserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentuserSource.asObservable();

  constructor(private http: HttpClient) { }

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
    localStorage.setItem('user', JSON.stringify(user));
    this.currentuserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentuserSource.next(null);
  }

}
