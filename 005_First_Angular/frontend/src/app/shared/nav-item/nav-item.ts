import {Component, input, InputSignal} from '@angular/core';
import {MatListItem, MatListItemIcon} from '@angular/material/list';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-nav-item',
  imports: [
    MatListItem,
    RouterLink,
    RouterLinkActive,
    MatIcon,
    MatListItemIcon
  ],
  templateUrl: './nav-item.html',
  styleUrl: './nav-item.scss'
})
export class NavItem {
  public readonly iconName: InputSignal<string> = input.required<string>();
  public  readonly route: InputSignal<string> = input.required<string>();


}
