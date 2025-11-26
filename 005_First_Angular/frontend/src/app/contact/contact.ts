import {Component, signal, WritableSignal} from '@angular/core';

@Component({
  selector: 'app-contact',
  imports: [],
  templateUrl: './contact.html',
  styleUrl: './contact.scss'
})
export class Contact {
  protected readonly MESSAGE_MIN_LENGTH : number = 10;
  private  readonly  contactFormModel: WritableSignal<ContactFormModel> = signal({
    name: '',
    email: '',
    message: ''
  });

}

type ContactFormModel = {
  name: string;
  email: string;
  message: string;
};
