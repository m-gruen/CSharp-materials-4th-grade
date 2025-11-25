import { Injectable } from '@angular/core';
import { LocalDate } from "@js-joda/core";
import { ServiceBase } from './service-base';
import { firstValueFrom } from 'rxjs';
import { z } from 'zod';
import { InstantSchema } from '../util/zod-schemas';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NewsService extends ServiceBase {
  protected override get controller(): string {
    return "news";
  }

  public async getNewsItems(from: LocalDate | undefined, to: LocalDate | undefined): Promise<NewsItem[] | undefined> {
    const url = this.buildUrl(null);
    const params = this.createHttpParams(from === undefined ? null : ["from", from], to === undefined ? null : ["to", to]);
    try {
      const response = await firstValueFrom(this.http.get<NewsItemListResponse>(url, { params }));

      const data = newsItemListResponseZod.parse(response) as NewsItemListResponse;
      return data.items;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error getting news items: ${JSON.stringify(error)}`);
        return undefined;
      }
      throw error;
    }
  }
}

const newsItemZod = z.object({
  id: z.number().int().positive(),
  publishedAt: InstantSchema,
  title: z.string().min(1),
  content: z.string().min(1),
  relatedExhibitIds: z.number().int().positive().array()
});
export type NewsItem = z.infer<typeof newsItemZod>

const newsItemListResponseZod = z.object({
  items: newsItemZod.array()
});
type NewsItemListResponse = z.infer<typeof newsItemListResponseZod>
