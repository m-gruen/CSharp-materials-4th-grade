import {Component, input, InputSignal, output, OutputEmitterRef} from '@angular/core';
import {LocalDateTime} from '@js-joda/core';
import {MatCard, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';

@Component({
  selector: 'app-greeting-display',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent
  ],
  templateUrl: './greeting-display.html',
  styleUrl: './greeting-display.scss'
})
export class GreetingDisplay {
  public readonly name: InputSignal<string> = input.required<string>();
  public readonly onClicked: OutputEmitterRef<ClickEvent> = output()
  protected readonly timestamp: LocalDateTime = LocalDateTime.now();

  protected handleClick(type: TitleOrContent): void {
    const clickEvent: ClickEvent = [type, this.name()];
    this.onClicked.emit(clickEvent);
  }
}

export type ClickEvent = [TitleOrContent, string];
export type TitleOrContent = "title" | "content";
