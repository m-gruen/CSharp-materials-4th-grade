import { Component, inject, input, numberAttribute, OnInit, signal, WritableSignal } from '@angular/core';
import {
  MatCard,
  MatCardActions, MatCardContent,
  MatCardHeader,
  MatCardImage,
  MatCardSubtitle,
  MatCardTitle
} from '@angular/material/card';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatButton } from '@angular/material/button';
import { MatList, MatListItem } from '@angular/material/list';
import { ToStringOrDefaultPipe } from '../../core/util/to-string-or-default-pipe';
import { DatePipe } from '@angular/common';
import { ExhibitDetails, ExhibitsService } from '../../core/services/exhibit-service';
import { Router } from '@angular/router';

@Component({
  selector: "app-exhibit-detail",
  standalone: true,
  imports: [
    MatProgressBar,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardSubtitle,
    MatCardImage,
    MatCardContent,
    MatCardActions,
    MatButton,
    MatList,
    MatListItem,
    ToStringOrDefaultPipe,
    DatePipe
  ],
  templateUrl: "./exhibit-details.html",
  styleUrl: "./exhibit-details.scss"
})
export class ExhibitDetail implements OnInit {
  public readonly id = input.required<number, unknown>({ transform: numberAttribute });
  protected readonly exhibit: WritableSignal<ExhibitDetails | undefined> = signal(undefined);
  private readonly service: ExhibitsService = inject(ExhibitsService);
  private readonly router: Router = inject(Router);

  public async ngOnInit(): Promise<void> {
    const id: number = this.id();
    if (isNaN(id)) {
      return;
    }

    const data: ExhibitDetails | undefined = await this.service.getExhibit(id);
    if (data) {
      this.exhibit.set(data);
    } else {
      console.log(`Error getting exhibit with id ${id}`);
    }
  }

  public async handleBackBtnClick(): Promise<void> {
    await this.router.navigate(["/exhibits"]);
  }

  protected readonly isNaN = isNaN;
}
