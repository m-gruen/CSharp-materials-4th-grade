import {Component, inject, signal, WritableSignal} from '@angular/core';
import {MatButton, MatIconButton} from '@angular/material/button';
import {Greeting} from './greeting/greeting';
import {MatSidenav, MatSidenavContainer, MatSidenavContent} from '@angular/material/sidenav';
import {AsyncPipe} from '@angular/common';
import {MatToolbar} from '@angular/material/toolbar';
import {MatListItem, MatNavList} from '@angular/material/list';
import {MatIcon} from '@angular/material/icon';
import {RouterOutlet} from '@angular/router';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {map, Observable, refCount, shareReplay} from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [
    MatSidenavContainer,
    MatSidenav,
    AsyncPipe,
    MatToolbar,
    MatNavList,
    MatListItem,
    MatSidenavContent,
    MatIconButton,
    MatIcon,
    RouterOutlet
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = "Tank Museum";
  private readonly breakpointObserver: BreakpointObserver = inject(BreakpointObserver);
  protected readonly isHandset$: Observable<boolean> = this.breakpointObserver
    .observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay({bufferSize: 1, refCount: true})
    );

  protected readonly isNotHandset$: Observable<boolean> = this.isHandset$
    .pipe(
      map(isHandset => !isHandset),
      shareReplay({bufferSize: 1, refCount: true})
    );
}
