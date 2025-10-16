import { Component, signal, WritableSignal } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { Greeting } from './greeting/greeting';

@Component({
  selector: 'app-root',
  imports: [MatButton, Greeting],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly clickCount: WritableSignal<number> = signal(0);

  protected handleClick(): void {
    this.clickCount.update((n: number) => n + 1);
  }
}
