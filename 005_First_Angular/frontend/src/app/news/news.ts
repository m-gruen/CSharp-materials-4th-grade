import {
  Component,
  computed, inject, Injectable,
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
import {FavoriteExhibitService} from '../../core/services/favorite-exhibit-service';
import {MatIcon} from '@angular/material/icon';
import {NewsFilter} from './news-filter/news-filter';
import {MatButton} from '@angular/material/button';

// we have to define the service before using it
@Injectable()
class FilterService {
  public filterSortAndMap(newsItems: NewsItem[], favoriteIds: Set<number>,
                          textFilter: string, sortAscending: boolean):
    NewsItemDisplay[] {
    const filter = textFilter.toLowerCase().trim();
    let source = newsItems;
    if (filter !== "") {
      source = newsItems
        .filter((newsItem: NewsItem) => newsItem.title.toLowerCase().includes(filter)
          || newsItem.content.toLowerCase().includes(filter));
    }
    return source
      .map((newsItem: NewsItem) => ({
        ...newsItem,
        referencesFavorite: newsItem.relatedExhibitIds.some(id => favoriteIds.has(id))
      }))
      .sort((a: NewsItemDisplay, b: NewsItemDisplay) => {
        const dateComparison: number = a.publishedAt.compareTo(b.publishedAt);
        return sortAscending ? dateComparison : -dateComparison;
      });
  }
}

@Component({
  selector: 'app-news',
  imports: [
    DatePipe,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatIcon,
    NewsFilter,
    MatButton
  ],
  providers: [FilterService],
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
  private readonly favoriteExhibitService: FavoriteExhibitService =
    inject(FavoriteExhibitService);
  private readonly filterService: FilterService = inject(FilterService);
  private readonly newsItems: WritableSignal<NewsItem[]> = signal([]);
  protected readonly displayItems: Signal<NewsItemDisplay[]> = computed(() => {
    return this.filterService.filterSortAndMap(this.newsItems(),
      this.favoriteExhibitService.favoriteIds(),
      this.textFilter(), this.sortAscending());
  });

  protected readonly textFilter: WritableSignal<string> = signal("");
  protected readonly sortAscending: WritableSignal<boolean> = signal(true);

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

type NewsItemDisplay = NewsItem & {
  referencesFavorite: boolean;
}
