import {Component, input, InputSignal, output, OutputEmitterRef} from '@angular/core';
import {ExhibitInfo} from '../../../core/services/exhibit-service';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatTooltip} from '@angular/material/tooltip';

@Component({
  selector: 'app-exhibit-list-table',
  imports: [
    MatTable,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatCell,
    MatCellDef,
    MatHeaderRow,
    MatRow,
    MatHeaderRowDef,
    MatRowDef,
    MatTooltip
  ],
  templateUrl: './exhibit-list-table.html',
  styleUrl: './exhibit-list-table.scss',
})
export class ExhibitListTable {
  public exhibits: InputSignal<ExhibitInfo[]> = input.required();
  public onExhibitSelected: OutputEmitterRef<ExhibitInfo> = output();
  protected readonly displayedColumns: string[] = ["name", "serviceStartYear", "serviceEndYear"];

  public handleRowClicked(exhibit: ExhibitInfo): void {
    this.onExhibitSelected.emit(exhibit);
  }
}
