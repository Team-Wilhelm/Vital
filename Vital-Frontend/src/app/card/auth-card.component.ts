import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'auth-card',
  template: `
      <div [ngClass]="getClassList()" (click)="onClickCard()">
          <div class="card-body">
              <div class="flex">
                  <div *ngIf="cardTitle !== '' || !getPlusVisibility()" class="flex justify-between w-full ">
                      <h2 class="card-title text-2xl lg:text-3xl">{{ cardTitle }}</h2>
                  </div>
                  <div *ngIf="!getPlusVisibility()" class="card-actions justify-end" (click)="onClickPlusButton()">
                      <button class="btn btn-square btn-sm">
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2"
                               stroke="currentColor" class="w-6 h-6">
                              <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15"/>
                          </svg>
                      </button>
                  </div>
              </div>
              <ng-content></ng-content>
          </div>
      </div>
  `
})

export class AuthCardComponent implements OnInit {
  title = 'card';
  @Input() cardTitle: string = '';
  @Input() isTextContent: boolean = true;
  @Input() cardColor: string = '';
  @Input() redirectLink: string = '';
  @Input() compact: boolean = false;
  @Input() hoverable: boolean = true;
  @Input() hasPlusButton: boolean = false;
  @Input() onClickPlusButton: () => void = () => {this.plusButtonClicked.emit()};
  @Output() plusButtonClicked = new EventEmitter<void>();
  classList: string[] = [];

  constructor(private router: Router) {

  }

  ngOnInit() {
    this.classList.push('xl:card', 'card-compact', 'xl:card-normal', 'h-full', 'w-full', 'shadow-lg', 'rounded');

    if (this.isTextContent) {
      this.classList.push('text-primary-content');
    }

    if (this.compact) {
      this.classList.push('card-compact');
    }

    if (this.cardColor) {
      this.classList.push(this.cardColor);
    } else {
      this.classList.push('bg-base-200');
    }

    if (this.redirectLink) {
      this.classList.push('cursor-pointer');
    }

    if (this.hoverable) {
      this.classList.push('hover:bg-card-hover');
    }
  }

  getClassList() {
    return this.classList.join(' ');
  }

  onClickCard() :void {
    if (this.redirectLink === '') return;
    this.router.navigate([this.redirectLink]);
  }

  getPlusVisibility() : string {
    if (!this.hasPlusButton) {
      return 'hidden';
    }
    return '';
  }
}
