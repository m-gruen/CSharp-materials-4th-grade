import {Component, inject, signal, WritableSignal} from '@angular/core';
import {customError, Field, form, minLength, required, submit, validate} from '@angular/forms/signals';
import {AuthService} from '../../core/services/auth-service';
import {SnackbarService} from '../../core/services/snackbar-service';
import {MatDialogRef} from '@angular/material/dialog';
import {MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';

@Component({
  selector: 'app-login-dialog',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatFormField,
    MatLabel,
    Field,
    MatInput,
    MatError,
    MatCardActions,
    MatButton
  ],
  templateUrl: './login-dialog.html',
  styleUrl: './login-dialog.scss',
})
export class LoginDialog {
  private readonly USERNAME_MIN_LENGTH: number = 3;
  private readonly authService: AuthService = inject(AuthService);
  private readonly snackbar = inject(SnackbarService);
  private readonly loginFormModel: WritableSignal<LoginFormModel> = signal({
    username: "",
    password: ""
  });
  protected loginForm = form(this.loginFormModel, path => {
    required(path.username, { message: "Username is required" });
    minLength(path.username, this.USERNAME_MIN_LENGTH, { message: `At least ${this.USERNAME_MIN_LENGTH} characters required` });
    required(path.password, { message: "Password is required" });
    validate(path.password, ctx => {
      const value = ctx.valueOf(path.password);

      const hasNumber = /\d/.test(value);
      const hasLetter = /[a-zA-Z]/.test(value);
      const hasExclamationMark = /!/.test(value);

      if (hasNumber && hasLetter && hasExclamationMark) {
        // requirements met
        return undefined;
      }

      return customError({
        kind: "passwordRequirements",
        message: "Must contain at least one number, one letter, and the \"!\" symbol"
      });
    });
  });
  private readonly dialogRef = inject(MatDialogRef<LoginDialog>);

  public async handleLogin(): Promise<void> {
    await submit(this.loginForm, async form => {
      const data: LoginFormModel = form().value();
      const success = this.authService.login(data.username, data.password);
      if (success) {
        this.close({
          username: data.username,
          success: true,
          cancelled: false
        });
        return;
      }

      this.snackbar.show("Login failed!");
      form().reset();
    });
  }

  public cancel(): void {
    this.close({
      cancelled: true,
      username: null,
      success: false
    });
  }

  private close(result: LoginDialogResult): void {
    this.dialogRef.close(result);
  }
}

export type LoginDialogResult = {
  cancelled: boolean;
  username: string | null;
  success: boolean;
};

type LoginFormModel = {
  username: string;
  password: string;
}
