import {Component, ElementRef, Input, ViewChild} from "@angular/core";
import {MetricViewDto} from "../../interfaces/dtos/metric.dto.interface";
import {MetricService} from "../../services/metric.service";

@Component({
  selector: 'metric-selection-item',
  template: `
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

          <input type="checkbox" id="had-flow" class="checkbox checkbox-accent"  #checkbox
                 [defaultChecked]="metric && metricService.isMetricSelected(metric.id)"
                 (click)="metric && metricService.addOrRemoveMetric(metric)"
          />
        </div>
      </label>
  `
})

export class MetricSelectionItemComponent {
  @Input() metric: MetricViewDto | undefined;
  @ViewChild('checkbox') checkbox: ElementRef<HTMLInputElement> | undefined;

  constructor(public metricService: MetricService) {

  }
}
