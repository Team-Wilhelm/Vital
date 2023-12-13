import {firstValueFrom} from "rxjs";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {ToastService} from "./toast.service";
import {Injectable} from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export default class HttpService{
  constructor(private httpClient: HttpClient, private toastService: ToastService) {}

  public async get<T = any>(url:string, message?:string|undefined){
    try {
      const response = await firstValueFrom(this.httpClient.get<T>(environment.baseUrl + url, {observe: 'response'}));
      if (response.status === 200 && message) {
        this.toastService.show(message, 'Success', 'success', 5000);
      }
      return response.body;
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    return;
  }
  public async post<T = any>(url:string, dto:any, message?:string|undefined){
    try {
      const response = await firstValueFrom(this.httpClient.post<T>(environment.baseUrl + url, dto, {observe: 'response'}));
      if (response.status === 200 && message) {
        this.toastService.show(message, 'Success', 'success', 5000);
        return response.body;
      }
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    return;
  }
  public async put<T = any>(url:string, dto:any, message?:string|undefined){
    try{
      const response = await firstValueFrom(this.httpClient.put<T>(environment.baseUrl + url, dto, {observe: 'response'}));
      if (response.status === 200 && message) {
        this.toastService.show(message, 'Success', 'success', 5000);
        return response.body;
      }
    }catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    return;
  }

  public async delete<T = any>(url:string, message?:string|undefined){
    try {
      const response = await firstValueFrom(this.httpClient.delete<T>(environment.baseUrl + url, {observe: 'response'}));
      if (response.status === 200 && message) {
        this.toastService.show(message, 'Success', 'success', 5000);
        return response.body;
      }
    } catch (error:any) {
      this.toastService.show(error.error.detail, 'Error', 'error', 5000);
    }
    return;
  }

}
