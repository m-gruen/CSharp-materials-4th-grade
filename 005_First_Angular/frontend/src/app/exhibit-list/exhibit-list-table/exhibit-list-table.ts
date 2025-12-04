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
import {MatPaginator} from '@angular/material/paginator';
import {MatButton} from '@angular/material/button';

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
    MatInput,
    MatPaginator,
    MatButton
  ],
  templateUrl: './exhibit-list-table.html',
  styleUrl: './exhibit-list-table.scss',
})
export class ExhibitListTable {
  public exhibits: InputSignal<ExhibitInfo[]> = input.required();
  public onExhibitSelected: OutputEmitterRef<ExhibitInfo> = output();
  private readonly sort: Signal<MatSort> = viewChild.required(MatSort);
  private readonly paginator: Signal<MatPaginator> = viewChild.required(MatPaginator);
  protected readonly displayedColumns: string[] = ["name", "serviceStartYear", "serviceEndYear"];
  protected readonly expandedElements: WritableSignal<Set<number>> = signal(new Set<number>());
  protected readonly exhibitData: Signal<MatTableDataSource<ExhibitInfo>> = computed(() => {
    const src = new MatTableDataSource(this.exhibits());
    src.sort = this.sort();
    src.filterPredicate = ((data: ExhibitInfo, filter: string) => {
      if (filter === '') {
        return true;
      }
      return data.name.toLowerCase().includes(filter);
    });
    src.paginator = this.paginator();
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
    this.expandedElements.update(set => {
      if (set.has(exhibit.id)) {
        set.delete(exhibit.id);
      } else {
        set.add(exhibit.id);
      }
      return new Set<number>(set);
    });
  }

  public handleDetailsRequested(exhibit: ExhibitInfo): void {
    this.onExhibitSelected.emit(exhibit);
  }
}
