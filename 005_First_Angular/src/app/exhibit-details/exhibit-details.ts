import {Component, input, numberAttribute} from '@angular/core';

@Component({
  selector: 'app-exhibit-details',
  imports: [],
  templateUrl: './exhibit-details.html',
  styleUrl: './exhibit-details.scss'
})
export class ExhibitDetails {
  public readonly id = input.required<number, unknown>({transform: numberAttribute});
}
