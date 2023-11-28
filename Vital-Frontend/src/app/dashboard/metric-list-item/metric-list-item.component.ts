import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {CalendarDayMetric} from "../../interfaces/day.interface";
import {MetricService} from "../../services/metric.service";

@Component({
  selector: 'metric-list-item',
  template: `
    <div [ngClass]="classString" class="relative">
      <div class="flex lg:flex-col">
        <p class="font-bold pe-3 lg:p-0">{{calendarDayMetric?.metrics?.name}}</p>
        <p>{{calendarDayMetric?.metricValue?.name}}</p>
      </div>

      <div class="flex-col">
        <p>{{calendarDayMetric && calendarDayMetric!.createdAt.toLocaleTimeString()}}</p>
      </div>

      <div>
        <button class="btn btn-circle btn-xs btn-ghost" (click)="openDeleteSection()"> ...</button>
      </div>

      <div [hidden]="deleteHidden" (click)="deleteMetric()"
           class="absolute top-0 right-0 z-1 h-full">
        <button class="bg-red-400 h-full rounded-r-xl p-3" [disabled]="deleteButtonDisabled" #deleteButton>Delete</button>
      </div>
    </div>
  `
})

export class MetricListItemComponent implements OnInit {
  title = 'metric-list-item';
  @Input() listIndex: number = 1;
  @Input() calendarDayMetric: CalendarDayMetric | undefined;
  @ViewChild('deleteButton') deleteButton: any | undefined;

  classString: string = "flex justify-between items-center rounded-xl p-5 w-full";

  deleteHidden: boolean = true;
  deleteButtonDisabled: boolean = true;

  constructor(private metricService: MetricService) {

  }

  ngOnInit() {
    //TODO: colour based on metric value
    if (this.listIndex % 2 === 0) {
      this.classString += " bg-green-accent"
    } else {
      this.classString += " bg-green-light-accent"
    }
  }

  // Opens the delete section and disables the delete button for 200ms to prevent the user from double-clicking it by accident
  openDeleteSection() {
    this.deleteHidden = false;
    setTimeout(() => {
      this.deleteButtonDisabled = false;
    }, 200);
  }

  deleteMetric() {
    console.log("Deleting metric");
    this.metricService.deleteMetric(this.calendarDayMetric!.id);
  }
}
