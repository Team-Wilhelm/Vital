import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';

export interface Toast {
  title?: string;
  message: string;
  type: 'info' | 'success' | 'error';
  closeAfter?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  toastsSubject = new BehaviorSubject<Toast[]>([]);

  show(message: string, title?: string, type: Toast['type'] = 'info', closeAfter: number = 3000) {
    const toast: Toast = {title, message, type, closeAfter};

    // Add the toast to the end of the current array of toasts
    const newToastList = this.toastsSubject.value.concat([toast]);
    this.toastsSubject.next(newToastList);

    // If the new toast is visible (one of the first three toasts in an array), set a timeout to remove it
    if (newToastList.indexOf(toast) < 3) {
      this.setToastTimeout(toast, closeAfter);
    }
  }

  remove(toast: Toast) {
    const newList = this.toastsSubject.value.filter(t => t !== toast);
    this.toastsSubject.next(newList);

    // If there are more toasts in the array, set a timeout to remove the next toast
    const moreToasts = newList.slice(0, 3);
    if (moreToasts.length) {
      this.setToastTimeout(moreToasts[0], moreToasts[0].closeAfter || 3000);
    }
  }

  private setToastTimeout(toast: Toast, timeout: number) {
    setTimeout(() => {
      this.remove(toast);
      // If there are more toasts in the array, set a timeout to remove the next toast
      const moreToasts = this.toastsSubject.value.slice(3);
      if (moreToasts.length) {
        this.setToastTimeout(moreToasts[0], moreToasts[0].closeAfter || 3000);
      }
    }, timeout);
  }
}
