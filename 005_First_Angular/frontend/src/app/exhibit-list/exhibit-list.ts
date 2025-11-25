import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatTooltip } from '@angular/material/tooltip';
import { ExhibitInfo, ExhibitsService } from '../../core/services/exhibit-service';

@Component({
  selector: "app-exhibit-list",
  imports: [
    RouterLink,
    MatTable,
    MatHeaderCell,
    MatColumnDef,
    MatCell,
    MatCellDef,
    MatHeaderCellDef,
    MatHeaderRow,
    MatRow,
    MatRowDef,
    MatHeaderRowDef,
    MatProgressBar,
    MatTooltip
  ],
  templateUrl: "./exhibit-list.html",
  styleUrl: "./exhibit-list.scss"
})
export class ExhibitList implements OnInit {
  protected readonly displayedColumns: string[] = ["name", "serviceStartYear", "serviceEndYear"];
  protected exhibits: WritableSignal<ExhibitInfo[]> = signal([]);
  protected loading: Signal<boolean> = computed(() => this.exhibits().length === 0);
  private readonly service: ExhibitsService = inject(ExhibitsService);
  private readonly router: Router = inject(Router);

  public async ngOnInit(): Promise<void> {
    const data: ExhibitInfo[] | undefined = await this.service.getExhibits();
    if (data) {
      this.exhibits.set(data);
    } else {
      console.log("Error getting exhibits");
    }
  }

  public async handleRowClicked(exhibit: ExhibitInfo): Promise<void> {
    await this.router.navigate(['/exhibits', exhibit.id]);
  }
}
