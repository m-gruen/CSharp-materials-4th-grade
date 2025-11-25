import {computed, Injectable, signal, Signal, WritableSignal} from '@angular/core';

@Injectable({
  providedIn: "root"
})
export class FavoriteExhibitService {
  private static readonly LOCAL_STORAGE_KEY: string = "favoriteExhibitIds";
  private readonly favoriteExhibitIds: WritableSignal<Set<number>> = signal(new
  Set<number>());
  public readonly favoriteIds: Signal<Set<number>> = computed(() => new
  Set(this.favoriteExhibitIds()));

  constructor() {
    const persistedIds: string | null =
      localStorage.getItem(FavoriteExhibitService.LOCAL_STORAGE_KEY);
    if (persistedIds) {
      const ids: number[] = JSON.parse(persistedIds);
      this.favoriteExhibitIds.set(new Set(ids));
    }
  }

  public toggleFavorite(id: number): void {
    this.favoriteExhibitIds.update((ids: Set<number>) => {
      if (ids.has(id)) {
        ids.delete(id);
      } else {
        ids.add(id);
      }
      const newSet = new Set(ids);
      localStorage.setItem(FavoriteExhibitService.LOCAL_STORAGE_KEY,
        JSON.stringify(Array.from(newSet)));
      return newSet;
    });
  }
}
