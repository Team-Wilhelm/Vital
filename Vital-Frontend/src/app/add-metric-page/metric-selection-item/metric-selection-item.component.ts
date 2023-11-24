import {Component, Input} from "@angular/core";
import {MetricViewDto} from "../../interfaces/dtos/metric.dto.interface";
import {MetricService} from "../../services/metric.service";

@Component({
  selector: 'metric-selection-item',
  template: `
      <label for="had-flow" class="label cursor-pointer"
             (click)="metric && metricService.addOrRemoveMetric(metric, !selected, $event)">
        <span class="label-text text-xl mr-10">Had {{metric?.name}}</span>
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

          <!-- I am using mousedown, because the (click) of the list item in optional values interferes with the (click) of this one, and  (mousedown) is handled before click -->
          <input type="checkbox" id="had-flow" class="checkbox checkbox-accent"
                 [defaultChecked]="metric && metricService.isMetricSelected(metric.id)"
          />
        </div>
      </label>
  `
})

export class MetricSelectionItemComponent {
  @Input() metric: MetricViewDto | undefined;
  @Input() selected: boolean = false;

  constructor(public metricService: MetricService) {
  }

}
