import {Component} from '@angular/core';
import {Toast, ToastService} from "../services/toast.service";
import {animate, style, transition, trigger} from "@angular/animations";


@Component({
  selector: 'app-toast',
  template: `
      <div *ngFor="let toast of toasts$ | async | slice:0:3; let i = index" [@appear]=""
           class="fixed right-0 m-4 p-4 rounded-xl shadow flex flex-col items-start
           max-w-[300px] w-[200px] z-50 min-h-[100px] max-h-[150px]
           overflow-ellipsis
           lg:w-[300px]"
           [class.bg-accent]="toast.type === 'success'" [class.bg-error]="toast.type === 'error'"
           [class.bg-info]="toast.type === 'info'"
           [class.top-0]="i === 0" [class.top-[125px]]="i === 1" [class.top-[250px]]="i === 2"
      >
          <div class="flex flex-row justify-between w-full">
              <p class="text-lg lg:text-xl font-bold">{{ toast.title }}</p>
              <button (click)="removeToast(toast)">
                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 256 256">
                      <path fill="currentColor"
                            d="M208.49 191.51a12 12 0 0 1-17 17L128 145l-63.51 63.49a12 12 0 0 1-17-17L111 128L47.51 64.49a12 12 0 0 1 17-17L128 111l63.51-63.52a12 12 0 0 1 17 17L145 128Z"/>
                  </svg>
              </button>
          </div>
          <p class="text-base lg:text-lg">{{ toast.message }}</p>
      </div>
  `,
  animations: [
    // the fade-in/fade-out animation.
    // fade in when created. fade out when destroyed.
    trigger('appear', [
      transition(':enter', [
        style({opacity: 0}),
        animate('0.3s ease-out', style({opacity: 1}))
      ]),
      transition(':leave', [
        animate('0.3s ease-out', style({opacity: 0}))
      ])
    ])
  ],
})

export class ToastComponent {
  toasts$ = this.toastService.toastsSubject;

  constructor(private toastService: ToastService) {
  }

  removeToast(toast: Toast) {
    this.toastService.remove(toast);
  }
}
