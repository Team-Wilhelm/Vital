import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'stat-card',
  template: `
    <div [ngClass]="getClassList()" (click)="onClickCard()">
      <div class="card-body">
        <ng-content></ng-content>
      </div>
    </div>
  `
})

export class StatCardComponent implements OnInit {
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
      this.classList.push('bg-base-100');
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
