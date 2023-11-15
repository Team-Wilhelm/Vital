import {Component, Input, OnInit} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-card',
  template: `
    <div [ngClass]="getClassList()" (click)="onClick()">
      <div class="card-body">
        <h2 class="card-title text-2xl lg:text-3xl">{{cardTitle}}</h2>
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

    this.classList.push('hover:bg-card-hover');
  }

  getClassList() {
    return this.classList.join(' ');
  }

  onClick(){
    this.router.navigate([this.redirectLink]);
  }
}
