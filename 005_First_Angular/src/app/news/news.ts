import {Component, computed, input, InputSignal, Signal} from '@angular/core';
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
export class News {
  public readonly from: InputSignal<string | undefined> = input();
  public readonly to: InputSignal<string | undefined> = input();

  protected readonly fromDate: Signal<LocalDate | undefined> = computed(() => {
    return News.parseDateParam(this.from());
  });
  protected readonly toDate: Signal<LocalDate | undefined> = computed(() => {
    return News.parseDateParam(this.to());
  });
  protected readonly showAllNews: Signal<boolean> = computed(() => {
    return !this.fromDate() && !this.toDate();
  });

  private static parseDateParam(paramValue: string | undefined): LocalDate | undefined {
    return paramValue ? LocalDate.parse(paramValue) : undefined;
  }
}
