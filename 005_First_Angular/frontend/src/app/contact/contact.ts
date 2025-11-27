import {Component, inject, signal, WritableSignal} from '@angular/core';
import {debounce, email, Field, form, minLength, required, submit} from '@angular/forms/signals';
import {MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {SnackbarService} from '../../core/services/snackbar-service';

@Component({
  selector: 'app-contact',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatFormField,
    MatLabel,
    MatInput,
    Field,
    MatError,
    MatCardActions,
    MatButton
  ],
  templateUrl: './contact.html',
  styleUrl: './contact.scss'
})
export class Contact {
  private static readonly defaultFormState: ContactFormModel = {
    name: '',
    email: '',
    message: ''
  };
  private readonly MESSAGE_MIN_LENGTH: number = 10;
  private readonly snackBar: SnackbarService = inject(SnackbarService);
  private readonly contactFormModel: WritableSignal<ContactFormModel> = signal(Contact.defaultFormState);

  protected readonly contactForm = form(this.contactFormModel, path => {
    required(path.name, {message: "Name is required."});
    required(path.email, {message: "Email is required."});
    required(path.message, {message: "Message is required."});
    email(path.email, {message: "Has to be a valid email address."});
    debounce(path.email, 250);
    minLength(
      path.message,
      this.MESSAGE_MIN_LENGTH,
      {message: `Message must be at least ${this.MESSAGE_MIN_LENGTH} characters long.`}
    );
  });


  protected async handleFormSubmitted(): Promise<void> {
    await submit(this.contactForm, async form => {
      const data = form().value();
      console.log(`${data.email} said: ${data.message}`);
      this.snackBar.show(`Thank you so much for your message ${data.name}!
      Due to the large amount of inquiries at the moment,
      please allow for up to 7000 business days for a response.`);
      form().reset(Contact.defaultFormState);
    });
  }
}

type ContactFormModel = {
  name: string;
  email: string;
  message: string;
};
