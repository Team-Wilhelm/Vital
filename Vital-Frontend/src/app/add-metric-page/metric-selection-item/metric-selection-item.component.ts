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
              <div *ngIf="!isFlowMetric()" class="flex items-center mr-3">
                  <input type="number" class="input input-bordered max-w-[5rem] me-1"
                         [formControl]="timeFormGroup.controls.hour"
                         (input)="updateMetricTime()"
                         (keydown)="numericInput($event)"
                         [class.input-error]="timeFormGroup.controls.hour.invalid">
                  <span>:</span>
                  <input type="number" class="input input-bordered max-w-[5rem] ms-1"
                         [formControl]="timeFormGroup.controls.minute"
                         (input)="updateMetricTime()" [class.input-error]="timeFormGroup.controls.minute.invalid">
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
              <input type="checkbox" id="had-flow" class="checkbox checkbox-primary border-2 order-1 md:order-2 "
                     #checkbox
                     (click)="metric && addOrRemoveMetric(metric)"/>
          </div>
      </div>
  `
})

// TODO: Increment minutes in 5 minute blocks
export class MetricSelectionItemComponent implements OnInit {
  @Input() metric: MetricViewDto | undefined;
  @ViewChild('checkbox') checkbox: ElementRef<HTMLInputElement> | undefined;

  timeFormGroup = new FormGroup({
    hour: new FormControl(0, [Validators.min(0), Validators.max(23), Validators.required, Validators.pattern('^([0-1]?[0-9]|2[0-3])$')]),
    minute: new FormControl(0, [Validators.min(0), Validators.max(59), Validators.required, Validators.pattern('^([0-5]?[0-9])$')])
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

  // This is called every time the input changes
  updateMetricTime() {
    this.checkIfTimeIsOutOfBounds();
    if (!this.timeFormGroup.valid) {
      return;
    }

    if (this.metric) {
      const metricTime = new Date(this.dataService.clickedDate!);
      metricTime.setHours(this.timeFormGroup.value.hour || 0, this.timeFormGroup.value.minute || 0, 0, 0);
      this.metricService.updateMetricTime(this.metric.id, metricTime);
    }
  }

  // This is called when the user clicks the up or down arrow, so they don't go outside 0-23 and 0-59
  checkIfTimeIsOutOfBounds() {
    let hour = this.timeFormGroup.value.hour;
    let minute = this.timeFormGroup.value.minute;

    // Check the hour boundary and adjust if it's out of bounds
    if (hour) {
      if (hour < 0) {
        hour = 23; // If hour becomes less than 0, set to 23
      } else if (hour > 23) {
        hour = 0; // If hour is more than 23, set to 0
      }
      this.timeFormGroup.controls.hour.setValue(hour, {emitEvent: false}); // This {emitEvent: false} option to not emit more events and prevent looping
    }

    if (minute) {
      // Check the minute boundary and adjust if it's out of bounds
      if (minute < 0) {
        minute = 59 // If minute becomes less than 0, set to 59
      } else if (minute > 59) {
        minute = 0; // If minute is more than 59, set to 0
      }
      this.timeFormGroup.controls.minute.setValue(minute, {emitEvent: false}); // This {emitEvent: false} option to not emit more events and prevent looping
    }
  }

  // This is called when the user presses a key
  numericInput(event: KeyboardEvent) {
    if (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'ArrowLeft' || event.key === 'ArrowRight' ||
      event.key === 'Backspace' || event.key === 'Delete' || event.key === 'Tab' || event.key === 'Enter') {
      this.updateMetricTime();
      return;
    }

    if (event.key === '0' || event.key === '1' || event.key === '2' || event.key === '3' || event.key === '4' ||
      event.key === '5' || event.key === '6' || event.key === '7' || event.key === '8' || event.key === '9') {
      this.updateMetricTime();
      return;
    }
    event.preventDefault();
  }

  addOrRemoveMetric(metric: MetricViewDto) {
    this.metricService.addOrRemoveMetric(metric);
    this.updateMetricTime();
  }

  isFlowMetric() {
    return this.metric?.name === 'Flow';
  }
}
