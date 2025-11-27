import {Component, signal, WritableSignal} from '@angular/core';
import {debounce, email, form, minLength, required} from '@angular/forms/signals';

@Component({
  selector: 'app-contact',
  imports: [],
  templateUrl: './contact.html',
  styleUrl: './contact.scss'
})
export class Contact {
  protected readonly MESSAGE_MIN_LENGTH: number = 10;
  private readonly contactFormModel: WritableSignal<ContactFormModel> = signal({
    name: '',
    email: '',
    message: ''
  });

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
  })
}

type ContactFormModel = {
  name: string;
  email: string;
  message: string;
};
