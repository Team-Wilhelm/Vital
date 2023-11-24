import {Component, OnInit} from '@angular/core';
import {DataService} from '../services/data.service';
import {MetricService} from '../services/metric.service';
import {MetricRegisterMetricDto, MetricViewDto, CalendarDayMetricDto} from '../interfaces/dtos/metric.dto.interface';

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent  {
  public clickedDate: Date = new Date();

  constructor(private dataService: DataService, private metricService: MetricService) {
  }

  ngOnInit(): void {
    this.dataService.clickedDate$.subscribe((clickedDate) => {
      // When the date changes, update the selected metrics for the new date
      if (clickedDate) {
        this.clickedDate = clickedDate;
      }
    });
  }
}
