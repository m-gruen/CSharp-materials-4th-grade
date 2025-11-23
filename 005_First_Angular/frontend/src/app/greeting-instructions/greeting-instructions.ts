import { Component, InputSignal, WritableSignal, input, signal, } from '@angular/core';

@Component({
  selector: 'app-greeting-instructions',
  imports: [],
  templateUrl: './greeting-instructions.html',
  styleUrl: './greeting-instructions.scss'
})
export class GreetingInstructions {
  public readonly notStarted: InputSignal<boolean> = input.required<boolean>();
  protected readonly currentColor: WritableSignal<Color> = signal(Color.Transparent);

  public changeColor(): void {
    this.currentColor.update(oldColor => {
      switch (oldColor) {
        case Color.Transparent: {
          return Color.Red;
        }
        case Color.Red: {
          return Color.Green;
        }
        case Color.Green: {
          return Color.Blue;
        }
        case Color.Blue: {
          return Color.Transparent;
        }
        default: {
          throw new Error('Invalid color');
        }
      }
    });
  }

  protected readonly Color = Color;
}

enum Color {
  Transparent = 0,
  Red = 1,
  Green = 2,
  Blue = 3
}
