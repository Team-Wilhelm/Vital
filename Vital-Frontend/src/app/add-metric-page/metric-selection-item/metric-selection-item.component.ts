import {Component, ElementRef, Input, OnInit, ViewChild} from "@angular/core";
import {MetricViewDto} from "../../interfaces/dtos/metric.dto.interface";
import {MetricService} from "../../services/metric.service";
import {DataService} from "../../services/data.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'metric-selection-item',
  template: `
    <div class="flex pt-3 min-h-[150px]">
      <div class="flex flex-grow items-center">
        <label class="label cursor-pointer" for="had-flow">
          <span class="label-text text-xl mr-10"> {{metric?.name}}</span>
        </label>
        <input type="checkbox" id="had-flow" class="checkbox checkbox-accent" #checkbox
               (click)="metric && metricService.addOrRemoveMetric(metric)"/>
      </div>

      <div class="flex items-center">
        <!-- Optional values -->
        <div class="dropdown dropdown-hover dropdown-left mr-5">
          <label tabindex="0" class="btn m-1">{{metric && metricService.getSelectedOptionalValue(metric.id)}}</label>
          <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
            <li *ngFor="let value of metric?.values"
                (click)="metric && metricService.selectOptionalValue(metric.id, value.id)">
              <a>
                {{value.name}}
              </a>
            </li>
          </ul>
        </div>

        <input type="number" class="input max-w-[6rem] me-1" [formControl]="timeFormGroup.controls.hour"
               (input)="updateMetricTime()" inputmode="numeric" pattern="[0-9]*">
        <span>:</span>
        <input type="number" class="input max-w-[6rem] ms-1" [formControl]="timeFormGroup.controls.minute"
               (input)="updateMetricTime()" inputmode="numeric" pattern="[0-9]*">
      </div>
    </div>
  `
})

//TODO: Add time dropdown menus
export class MetricSelectionItemComponent implements OnInit {
  @Input() metric: MetricViewDto | undefined;
  @ViewChild('checkbox') checkbox: ElementRef<HTMLInputElement> | undefined;

  timeFormGroup = new FormGroup({
    hour: new FormControl(0, [Validators.min(0), Validators.max(23), Validators.required]),
    minute: new FormControl(0, [Validators.min(0), Validators.max(59), Validators.required])
  });

  constructor(public metricService: MetricService, private dataService: DataService) {

  }

  async ngOnInit() {
    await this.metricService.getUsersMetric(this.dataService.clickedDate!);
    const currentTime = this.dataService.getCurrentLocalTime();

    this.timeFormGroup.setValue({
      hour: currentTime.getHours(),
      minute: currentTime.getMinutes()
    });

    this.timeFormGroup.valueChanges.subscribe(() => {
      this.updateMetricTime();
    });
  }

  updateMetricTime() {
    if (this.metric) {
      const metricTime = new Date(this.dataService.clickedDate!);
      metricTime.setHours(this.timeFormGroup.value.hour || 0, this.timeFormGroup.value.minute || 0, 0, 0);
      this.metricService.updateMetricTime(this.metric.id, metricTime);
    }
  }
}
