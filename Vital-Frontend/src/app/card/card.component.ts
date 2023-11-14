import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-card',
  template: `
    <div class="card bg-card text-primary-content h-full w-full">
      <div class="card-body">
        <h2 class="card-title">{{cardTitle}}</h2>
        <p>{{cardBody}}</p>
      </div>
    </div>
  `
})

export class CardComponent {
  title = 'card';
  @Input() cardTitle: string = 'card title';
  @Input() cardBody: string = 'card body';
}
