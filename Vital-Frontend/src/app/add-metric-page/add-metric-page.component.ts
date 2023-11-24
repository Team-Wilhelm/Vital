import {Component, OnInit} from '@angular/core';
import {DataService} from '../services/data.service';
import {MetricService} from '../services/metric.service';

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent  {
  public clickedDate: Date = new Date();

  constructor(private dataService: DataService, public metricService: MetricService) {

  }
}
