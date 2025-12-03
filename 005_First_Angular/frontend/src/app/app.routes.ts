import {Routes} from '@angular/router';
import {Contact} from './contact/contact';
import {News} from './news/news';
import {ExhibitList} from './exhibit-list/exhibit-list';
import {ExhibitDetail} from './exhibit-details/exhibit-details';
import {ExhibitEdit} from './exhibit-edit/exhibit-edit';
import {authGuard} from '../core/util/auth-guard';

const exhibitEditImport =
  () => import('./exhibit-edit/exhibit-edit')
    .then(m => m.ExhibitEdit);

export const routes: Routes = [
  {path: 'news', component: News},
  {path: 'contact', component: Contact},
  {path: 'exhibits', component: ExhibitList},
  {path: 'exhibits/:id', component: ExhibitDetail},
  {path: 'exhibit-edit', loadComponent: exhibitEditImport, canActivate: [authGuard]},
  {path: 'exhibit-edit/:id', loadComponent: exhibitEditImport, canActivate: [authGuard]},
  {path: '', redirectTo: 'news', pathMatch: 'full'}
];
