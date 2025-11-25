import { Directive, inject } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

@Directive()
export abstract class ServiceBase {
  private static readonly baseUrl = "http://localhost:5200/api";
  protected readonly http: HttpClient = inject(HttpClient);

  protected abstract get controller(): string;

  protected buildUrl(action: string | null): string {
    let url = `${ServiceBase.baseUrl}/${this.controller}`;
    if (action !== null) {
      url = `${url}/${action}`;
    }
    return url;
  }

  protected createHttpParams(...params: (QueryParam | null)[]): HttpParams {
    let httpParams = new HttpParams();
    for (const param of params.filter(p => p != null && p[1] != null)) {
      const [key, value] = param as QueryParam;
      httpParams = httpParams.append(key, value?.toString());
    }
    return httpParams;
  }
}

export type QueryParam = [string, any];
