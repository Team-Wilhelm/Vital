import {firstValueFrom, Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {HttpClient, HttpResponse} from "@angular/common/http";
import {ToastService} from "./toast.service";
import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export default class HttpService{
  constructor(private httpClient: HttpClient, private toastService: ToastService) {}

  private async handleRequest<T = any>(request: Observable<HttpResponse<T>>, message?: string): Promise<T | null | undefined> {
    try {
      const response = await firstValueFrom(request);
      if (response.status === 200 && message) {
        this.toastService.show(message, 'Success', 'success', 5000);
      }
      return response.body;
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
      return;
    }
  }

  public get<T = any>(url:string, message?:string): Promise<T | null | undefined> {
    const request = this.httpClient.get<T>(environment.baseUrl + url, {observe: 'response'});
    return this.handleRequest(request, message);
  }

  public post<T = any>(url:string, dto:any, message?:string): Promise<T | null | undefined> {
    const request = this.httpClient.post<T>(environment.baseUrl + url, dto, {observe: 'response'});
    return this.handleRequest(request, message);
  }

  public put<T = any>(url:string, dto:any, message?:string): Promise<T | null | undefined> {
    const request = this.httpClient.put<T>(environment.baseUrl + url, dto, {observe: 'response'});
    return this.handleRequest(request, message);
  }

  public delete<T = any>(url:string, message?:string): Promise<T | null | undefined> {
    const request = this.httpClient.delete<T>(environment.baseUrl + url, {observe: 'response'});
    return this.handleRequest(request, message);
  }
}
