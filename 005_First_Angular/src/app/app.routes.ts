import {Routes} from '@angular/router';
import {Contact} from './contact/contact';
import {News} from './news/news';

export const routes: Routes = [
  {path: 'news', component: News},
  {path: 'contact', component: Contact},
  {path: '', redirectTo: 'news', pathMatch: 'full'}
];
