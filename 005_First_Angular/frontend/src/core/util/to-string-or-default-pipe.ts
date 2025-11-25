import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: "toStringOrDefault"
})
export class ToStringOrDefaultPipe implements PipeTransform {
  public transform(value: unknown, defaultValue: string): string {
    if (value == undefined) {
      return defaultValue;
    }

    return value.toString();
  }
}
