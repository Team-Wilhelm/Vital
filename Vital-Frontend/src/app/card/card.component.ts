import {Component, Input, OnInit} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-card',
  template: `
    <div [ngClass]="getClassList()" (click)="onClick()">
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
  @Input() redirectLink: string = '';

  classList: string[] = [];

  constructor(private router: Router) {

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

    if (this.redirectLink) {
      this.classList.push('cursor-pointer');
    }
  }

  getClassList() {
    return this.classList.join(' ');
  }

  onClick(){
    this.router.navigate([this.redirectLink]);
  }
}
