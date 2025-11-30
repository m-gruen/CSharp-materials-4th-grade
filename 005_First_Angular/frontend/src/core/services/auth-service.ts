import {Injectable, Signal, signal, WritableSignal} from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // NEVER DO IT LIKE THAT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

  private static readonly USERNAME = 'admin';
  private static readonly PASSWORD = 'g3heim!';

  private readonly loggedIn: WritableSignal<boolean> = signal(false);
  public readonly isLoggedIn: Signal<boolean> = this.loggedIn.asReadonly();

  public login(username: string, password: string): boolean {
    if (username === AuthService.USERNAME && password === AuthService.PASSWORD) {
      this.loggedIn.set(true);
    }

    return this.loggedIn();
  }

  public logout(): void {
    this.loggedIn.set(false);
  }
}
