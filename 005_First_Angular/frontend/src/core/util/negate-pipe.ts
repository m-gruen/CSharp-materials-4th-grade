import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'negate',
})
export class NegatePipe implements PipeTransform {

  transform(value: boolean, ...args: unknown[]): boolean {
    return !value;
  }

}
