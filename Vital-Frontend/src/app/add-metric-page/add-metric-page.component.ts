import {Component, OnInit} from '@angular/core';
import {DataService} from '../services/data.service';
import {MetricService} from '../services/metric.service';

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent  {

  constructor(public metricService: MetricService) {
  }
}
