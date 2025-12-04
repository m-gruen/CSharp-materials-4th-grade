import {
  Component,
  computed,
  input,
  InputSignal,
  output,
  OutputEmitterRef, signal,
  Signal,
  viewChild,
  WritableSignal
} from '@angular/core';
import {ExhibitInfo} from '../../../core/services/exhibit-service';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable, MatTableDataSource
} from '@angular/material/table';
import {MatTooltip} from '@angular/material/tooltip';
import {MatSort, MatSortHeader} from '@angular/material/sort';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';

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
    MatTooltip,
    MatSort,
    MatSortHeader,
    MatFormField,
    MatLabel,
    MatInput
  ],
  templateUrl: './exhibit-list-table.html',
  styleUrl: './exhibit-list-table.scss',
})
export class ExhibitListTable {
  public exhibits: InputSignal<ExhibitInfo[]> = input.required();
  public onExhibitSelected: OutputEmitterRef<ExhibitInfo> = output();
  private readonly sort: Signal<MatSort> = viewChild.required(MatSort);
  private readonly filter: WritableSignal<string> = signal('');
  protected readonly displayedColumns: string[] = ["name", "serviceStartYear", "serviceEndYear"];
  protected readonly exhibitData: Signal<MatTableDataSource<ExhibitInfo>> = computed(() => {
    const src = new MatTableDataSource(this.exhibits());
    src.sort = this.sort();
    src.filterPredicate = ((data: ExhibitInfo, filter: string) => {
      if (filter === '') {
        return true;
      }
      return data.name.toLowerCase().includes(filter);
    });
    return src;
  });

  public handleNameFilterChanged(event: Event): void {
    const inputElement: HTMLInputElement = event.target as HTMLInputElement;

    if (!inputElement) {
      throw new Error('Input element not available');
    }
    const rawFilterValue: string = inputElement.value;
    this.exhibitData().filter = rawFilterValue.trim().toLowerCase();
  }

  public handleRowClicked(exhibit: ExhibitInfo): void {
    this.onExhibitSelected.emit(exhibit);
  }
}
