import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-card',
  template: `
    <div [ngClass]="getClassList()" (click)="onClickCard()">
      <div class="card-body">
        <div class="flex justify-between w-full">
          <h2 class="card-title text-2xl lg:text-3xl">{{cardTitle}}</h2>
          <svg class="cursor-pointer" [ngClass]="getPlusVisibility()" (click)="onClickPlusButton()"
               height="32px" id="Layer_1"
               viewBox="0 0 32 32" width="32px" xmlns="http://www.w3.org/2000/svg">
            <path
              d="M28,14H18V4c0-1.104-0.896-2-2-2s-2,0.896-2,2v10H4c-1.104,0-2,0.896-2,2s0.896,2,2,2h10v10c0,1.104,0.896,2,2,2  s2-0.896,2-2V18h10c1.104,0,2-0.896,2-2S29.104,14,28,14z"/>
          </svg>
        </div>
        <ng-content></ng-content>
      </div>
    </div>
  `
})

export class CardComponent implements OnInit {
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
    this.classList.push('card', 'card-compact', 'xl:card-normal', 'h-full', 'w-full', 'shadow-md');

    if (this.isTextContent) {
      this.classList.push('text-primary-content');
    }

    if (this.compact) {
      this.classList.push('card-compact');
    }

    if (this.cardColor) {
      this.classList.push(this.cardColor);
    } else {
      this.classList.push('bg-card');
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
