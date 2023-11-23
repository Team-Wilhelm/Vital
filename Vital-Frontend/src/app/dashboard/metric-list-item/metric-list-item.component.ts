import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'metric-list-item',
  template: `
      <div [ngClass]="classString">
        <div class="flex lg:flex-col">
          <p class="font-bold pe-3 lg:p-0">{{metricName}}</p>
          <p>{{metricValue}}</p>
        </div>

        <div class="flex-col">
          <p>{{metricTime}}</p>
        </div>
      </div>
  `
})

//TODO
export class MetricListItemComponent implements OnInit {
  title = 'metric-list-item';
  @Input() metricName: string = '';
  @Input() metricValue: string = '';
  @Input() metricTime: string = '';
  @Input() listIndex: number = 1;

  classString: string = "flex justify-between items-center rounded-xl p-5 w-full";

  constructor() {

  }

  ngOnInit() {
    //TODO: colour based on metric value
    if (this.listIndex % 2 === 0) {
      this.classString+= " bg-green-accent"
    } else {
      this.classString+= " bg-green-light-accent"
    }
  }
}
