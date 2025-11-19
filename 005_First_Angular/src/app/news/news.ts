import {
  Component,
  computed,
  input,
  InputSignal,
  OnChanges,
  signal,
  Signal,
  SimpleChanges,
  WritableSignal
} from '@angular/core';
import {LocalDate} from '@js-joda/core';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-news',
  imports: [
    DatePipe
  ],
  templateUrl: './news.html',
  styleUrl: './news.scss'
})
export class News implements OnChanges {
  public readonly from: InputSignal<string | undefined> = input();
  public readonly to: InputSignal<string | undefined> = input();

  protected readonly fromDate: WritableSignal<LocalDate | undefined> = signal(undefined);
  protected readonly toDate: WritableSignal<LocalDate | undefined> = signal(undefined);
  protected readonly showAllNews: Signal<boolean> = computed(() => {
    return !this.fromDate() && !this.toDate();
  });

  ngOnChanges(changes: SimpleChanges) {
    const today: LocalDate = LocalDate.now();
    let newFrom: LocalDate | undefined = News.parseDateParam(changes['from']?.currentValue);
    let newTo: LocalDate | undefined = News.parseDateParam(changes['to']?.currentValue);

    if (newFrom || newTo) {
      if (newFrom && newTo && (newTo.isBefore(newFrom))) {
        [newFrom, newTo] = [newTo, newFrom];
      }

      if (newFrom && newFrom.isAfter(today)) {
        newFrom = today;
      }

      if (newTo && newTo.isBefore(today)) {
        newTo = today;
      }

      this.fromDate.set(newFrom);
      this.toDate.set(newTo);
    }
  }

  private static parseDateParam(paramValue: string | undefined): LocalDate | undefined {
    return paramValue ? LocalDate.parse(paramValue) : undefined;
  }
}
