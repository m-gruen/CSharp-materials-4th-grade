import { CanActivateFn } from '@angular/router';
import {AuthService} from '../services/auth-service';
import {inject} from '@angular/core';
import {SnackbarService} from '../services/snackbar-service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService: AuthService = inject(AuthService);
  const snackbar:SnackbarService = inject(SnackbarService);

  if (authService.isLoggedIn()) {
    return true;
  }

  snackbar.show("You need to be logged in to access this component.");
  return authService.isLoggedIn();
};
