import { Injectable } from '@angular/core';
import { ServiceBase } from './service-base';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from "@angular/common/http";
import { z } from 'zod';
import { LocalDateSchema } from '../util/zod-schemas';

@Injectable({
  providedIn: "root"
})
export class ExhibitsService extends ServiceBase {
  protected override get controller(): string {
    return "exhibits";
  }

  public async getExhibits(): Promise<ExhibitInfo[] | undefined> {
    const url = this.buildUrl(null);
    try {
      const response = await firstValueFrom(this.http.get<ExhibitListResponse>(url));
      const data = exhibitListResponseZod.parse(response) as ExhibitListResponse;
      return data.items;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error getting exhibits: ${JSON.stringify(error)}`);
        return undefined;
      }
      throw error;
    }
  }

  public async getExhibit(id: number): Promise<ExhibitDetails | undefined> {
    const url = this.buildUrl(`${id}`);
    try {
      const response = await firstValueFrom(this.http.get<ExhibitDetails>(url));
      return exhibitDetailsZod.parse(response);
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error getting exhibit: ${JSON.stringify(error)}`);
        return undefined;
      }
      throw error;
    }
  }

  public async addExhibit(newExhibit: NewExhibitData): Promise<ExhibitDetails | undefined> {
    const url = this.buildUrl(null);
    try {
      const response = await firstValueFrom(this.http.post<ExhibitDetails>(url, newExhibit, {observe: "response"}));
      console.log(`New exhibit address: ${response.headers.get("Location")}`);
      return exhibitDetailsZod.parse(response.body);
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error adding exhibit: ${JSON.stringify(error)}`);
        return undefined;
      }
      throw error;
    }
  }

  public async updateExhibit(newDetails: ExhibitDetails): Promise<boolean> {
    const url = this.buildUrl(`${newDetails.id}`);
    try {
      await firstValueFrom(this.http.patch(url, newDetails));
      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error updating exhibit: ${JSON.stringify(error)}`);
        return false;
      }
      throw error;
    }
  }

  public async deleteExhibit(id: number): Promise<boolean> {
    const url = this.buildUrl(`${id}`);
    try {
      await firstValueFrom(this.http.delete(url));
      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        console.log(`Error deleting exhibit: ${JSON.stringify(error)}`);
        return false;
      }
      throw error;
    }
  }
}

const exhibitInfoZod = z.object({
  id: z.number().int().positive(),
  name: z.string().min(1),
  serviceStartYear: z.number().int().positive(),
  serviceEndYear: z.number().int().positive().nullable()
});
export type ExhibitInfo = z.infer<typeof exhibitInfoZod>;

const exhibitListResponseZod = z.object({
  items: exhibitInfoZod.array()
});
type ExhibitListResponse = z.infer<typeof exhibitListResponseZod>;

const exhibitDetailsZod = exhibitInfoZod.extend({
  description: z.string().min(1),
  unitsProduced: z.number().int().nonnegative(),
  addedToMuseumCollectionAt: LocalDateSchema,
  country: z.string().min(1),
  armor: z.string().min(1),
  armament: z.string().min(1),
  crew: z.number().int().positive(),
  imageUrl: z.string().url().nullable()
});
export type ExhibitDetails = z.infer<typeof exhibitDetailsZod>;

export class NewExhibitData {
  constructor(
    public name: string,
    public description: string,
    public serviceStartYear: number,
    public serviceEndYear: number | null,
    public unitsProduced: number,
    public country: string,
    public armor: string,
    public armament: string,
    public crew: number,
    public imageUrl: string | null
  ) {
  }
}
