import {Component, ElementRef, Input, OnInit, ViewChild} from "@angular/core";
import {MetricViewDto} from "../../interfaces/dtos/metric.dto.interface";
import {MetricService} from "../../services/metric.service";
import {DataService} from "../../services/data.service";
import {co} from "@fullcalendar/core/internal-common";

@Component({
  selector: 'metric-selection-item',
  template: `
    <div class="flex justify-between pt-3">
      <label class="label cursor-pointer">
        <span class="label-text text-xl mr-10"> {{metric?.name}}</span>
        <div class="flex items-center">

          <!-- Optional values
          <div class="dropdown dropdown-hover dropdown-left mr-5">
            <label tabindex="0" class="btn m-1">{{metric&& metricService.getSelectedOptionalValue(metric.id)}}</label>
            <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
              <li *ngFor="let value of metric?.values"
                  (click)="metric && metricService.selectOptionalValue(metric.id, value.id)">
                <a>
                  {{value.name}}
                </a>
              </li>
            </ul>
          </div>
          -->

          <input type="checkbox" id="had-flow" class="checkbox checkbox-accent" #checkbox
                 [defaultChecked]="metric && metricService.isMetricSelected(metric.id)"
                 (click)="metric && metricService.addOrRemoveMetric(metric)"
          />
        </div>
      </label>

      <div class="flex items-center">
        <input type="number" class="input max-w-[6rem] me-1" [(ngModel)]="hourValue" (ngModelChange)="updateMetricTime()" [value]="hourValue | number:'2.0-0'">
        <span>:</span>
        <input type="number" class="input max-w-[6rem] ms-1" [(ngModel)]="minuteValue" (ngModelChange)="updateMetricTime()" [value]="minuteValue | number:'2.0-0'">
      </div>
    </div>
  `
})

//TODO: Add time dropdown menus
export class MetricSelectionItemComponent implements OnInit {
  @Input() metric: MetricViewDto | undefined;
  @ViewChild('checkbox') checkbox: ElementRef<HTMLInputElement> | undefined;

  hourValue: number = 0;
  minuteValue: number = 0;

  constructor(public metricService: MetricService, private dataService: DataService) {
    const currentTime = this.dataService.getCurrentLocalTime();
    this.hourValue = currentTime.getHours();
    this.minuteValue = currentTime.getMinutes();
  }

  ngOnInit() {
    this.hourValue = this.formatTime(this.hourValue);
    this.minuteValue = this.formatTime(this.minuteValue);
  }

  updateMetricTime() {
    if (this.metric) {
      const metricTime = new Date(this.dataService.clickedDate!);
      metricTime.setHours(this.hourValue, this.minuteValue, 0, 0);
      console.log(this.hourValue + ":" + this.minuteValue);
      this.metricService.updateMetricTime(this.metric.id, metricTime);
    }
  }

  formatTime(time: number) {
    return Number(time.toString().padStart(2, '0'));
  }
}
