import {Component, ElementRef, Input, OnInit, ViewChild} from "@angular/core";
import {MetricViewDto} from "../../interfaces/dtos/metric.dto.interface";
import {MetricService} from "../../services/metric.service";
import {DataService} from "../../services/data.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'metric-selection-item',
  template: `
      <div class="flex flex-col pt-3
      md:flex-row md:items-center">
          <div class="flex flex-grow justify-between items-center mb-3
          md:mb-0">
              <label class="label cursor-pointer" for="had-flow">
                  <span class="label-text text-xl"> {{ metric?.name }}</span>
              </label>


              <!-- Time -->
              <div class="flex items-center mr-3">
                  <input type="number" class="input input-bordered max-w-[5rem] me-1" [formControl]="timeFormGroup.controls.hour"
                         (input)="updateMetricTime()" inputmode="numeric" pattern="[0-9]*">
                  <span>:</span>
                  <input type="number" class="input input-bordered max-w-[5rem] ms-1" [formControl]="timeFormGroup.controls.minute"
                         (input)="updateMetricTime()" inputmode="numeric" pattern="[0-9]*">
              </div>
          </div>

          <div class="flex  items-center justify-between mr-3">
              <div class="flex-grow"></div>
              <!-- Optional values -->
              <select class="select select-bordered order-2 ml-3
               md:order-1 md:mr-3 md:ml-0">
                  <option selected>Optional</option>
                  <option *ngFor="let value of metric?.values"
                          (click)="metric && metricService.selectOptionalValue(metric.id, value.id)">{{ value.name }}
                  </option>
              </select>

              <!-- Checkbox -->
              <input type="checkbox" id="had-flow" class="checkbox checkbox-primary border-2 order-1 md:order-2 " #checkbox
                     (click)="metric && metricService.addOrRemoveMetric(metric)"/>
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
