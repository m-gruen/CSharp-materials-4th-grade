import {Component, model, ModelSignal} from '@angular/core';
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-news-filter',
  imports: [
    MatFormField,
    MatLabel,
    FormsModule,
    MatInput,
    MatSlideToggle
  ],
  templateUrl: './news-filter.html',
  styleUrl: './news-filter.scss'
})
export class NewsFilter {
  public readonly textFilter: ModelSignal<string> = model("");
  public readonly sortAscending: ModelSignal<boolean> = model(true);
}
