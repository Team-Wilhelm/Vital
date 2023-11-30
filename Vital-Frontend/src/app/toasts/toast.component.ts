import {Component} from '@angular/core';
import {Toast, ToastService} from "../services/toast.service";
import {animate, style, transition, trigger} from "@angular/animations";


@Component({
  selector: 'app-toast',
  template: `
      <div *ngFor="let toast of toasts$ | async | slice:0:3; let i = index" [@appear]=""
           class="fixed right-0 m-4 p-4 rounded-xl shadow flex flex-col items-start
           max-w-[300px] w-[300px] z-50 min-h-[100px] max-h-[150px]"
           [class.bg-accent]="toast.type === 'success'" [class.bg-red-500]="toast.type === 'error'"
           [class.bg-blue-500]="toast.type === 'info'"
           [class.top-0]="i === 0" [class.top-[125px]]="i === 1" [class.top-[250px]]="i === 2"
      >
          <p class="text-2xl">{{ toast.title }}</p>
          <p class="text-xl">{{ toast.message }}</p>
          <button (click)="removeToast(toast)" class="self-end text-xl">Close</button>
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
