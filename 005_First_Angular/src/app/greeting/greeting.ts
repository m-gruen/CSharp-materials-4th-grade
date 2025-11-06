import { Component, computed, Signal, signal, WritableSignal } from '@angular/core';

@Component({
  selector: 'app-greeting',
  imports: [],
  templateUrl: './greeting.html',
  styleUrl: './greeting.scss'
})
export class Greeting {
  private readonly names: string[] = ['Alice', 'Bob Brown Bob Brown', 'Charlie', 'Diana'];
  private currentNameIdx: WritableSignal<number> = signal(0);
  protected readonly currentName: Signal<string> = computed(() => {
    return this.names[this.currentNameIdx()];
  });
  protected readonly greetedPeople: WritableSignal<string[]> = signal([]);

  protected greet(): void {
    this.currentNameIdx.update(n => n + 1);

    if (this.currentNameIdx() >= this.names.length) {
      this.currentNameIdx.set(0);
    }
    this.greetedPeople.update(old => [...old, this.currentName()]);
  }
}
