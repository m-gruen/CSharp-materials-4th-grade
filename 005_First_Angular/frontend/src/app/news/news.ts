import {
  Component,
  computed, inject,
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
import {NewsItem, NewsService} from '../../core/services/news-service';
import {MatCard, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';

@Component({
  selector: 'app-news',
  imports: [
    DatePipe,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent
  ],
  templateUrl: './news.html',
  styleUrl: './news.scss',
})
export class News implements OnChanges {
  public readonly from: InputSignal<string | undefined> = input();
  public readonly to: InputSignal<string | undefined> = input();
  protected readonly fromDate: WritableSignal<LocalDate | undefined> = signal(undefined);
  protected readonly toDate: WritableSignal<LocalDate | undefined> = signal(undefined);
  protected readonly showAllNames: Signal<boolean> = computed(() => {
    return !this.fromDate() && !this.toDate();
  });
  private readonly service: NewsService = inject(NewsService);
  protected readonly newsItems: WritableSignal<NewsItem[]> = signal([]);

  private async loadData(): Promise<void> {
    let news
      = await this.service.getNewsItems(this.fromDate(), this.toDate());

    this.newsItems.set(news ?? []);
  }

  public async ngOnChanges(changes: SimpleChanges): Promise<void> {
    const today: LocalDate = LocalDate.now();

    let newFrom: LocalDate | undefined = News
      .parseDateParam(changes['from']?.currentValue);
    let newTo: LocalDate | undefined = News
      .parseDateParam(changes['to']?.currentValue);

    if (newFrom || newTo) {
      if (newFrom && newTo && (newTo.isBefore(newFrom))) {
        [newFrom, newTo] = [newTo, newFrom];
      }
      if (newFrom && newFrom.isAfter(today)) {
        newFrom = today;
      }
      if (newTo && newTo.isAfter(today)) {
        newTo = today;
      }

      this.fromDate.set(newFrom);
      this.toDate.set(newTo);
    }

    await this.loadData();
  }

  private static parseDateParam(paramValue: string | undefined): LocalDate | undefined {
    return paramValue ? LocalDate.parse(paramValue) : undefined;
  }
}
