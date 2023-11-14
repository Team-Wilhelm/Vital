import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-card',
  template: `
    <div [ngClass]="getClassList()">
      <div class="card-body">
        <h2 class="card-title">{{cardTitle}}</h2>
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

  classList: string[] = [];

  constructor() {

  }

  ngOnInit() {
    this.classList.push('card', 'h-full', 'w-full');

    if (this.isTextContent) {
      this.classList.push('text-primary-content');
    }

    if (this.cardColor) {
      this.classList.push(this.cardColor);
    } else {
      this.classList.push('bg-card');
    }
  }

  getClassList() {
    return this.classList.join(' ');
  }
}
