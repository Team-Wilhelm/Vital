import {Component, OnInit} from '@angular/core';
import {DataService} from '../services/data.service';
import {MetricService} from '../services/metric.service';
import {
  MetricRegisterMetricDto,
  MetricValueViewDto,
  MetricViewDto,
  CalendarDayMetricDto
} from '../interfaces/dtos/metric.dto.interface';

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent implements OnInit {
  public allMetrics: MetricViewDto[] = [];
  public selectedMetrics: MetricRegisterMetricDto[] = [];
  public clickedDate: Date = new Date();
  public metricSelectionMap: Map<string, string> = new Map(); // <MetricId, MetricValueId>
  periodMetric: MetricViewDto | undefined;

  constructor(private dataService: DataService, private metricService: MetricService) {
    this.getMetrics().then();
  }

  ngOnInit(): void {
    this.dataService.clickedDate$.subscribe((clickedDate) => {
      // When the date changes, update the selected metrics for the new date
      this.getSelectedMetricsForDay().then(() => {
        this.updateSelectedMetrics();
      });
    });
  }

  // TODO: Add option to deselect optional values
  addOrRemoveMetric(metric: MetricViewDto, adding: boolean, event: MouseEvent) {
    if (event.target !== event.currentTarget) event.preventDefault(); // Prevents the click event from propagating to the parent element, otherwise deselecting optional values would deselect the metric as well

    if (adding) {
      // Add the metric to the selected metrics
      this.metricSelectionMap.set(metric.id, '');
    } else {
      // Remove the metric from the selected metrics
      this.metricSelectionMap.delete(metric.id);
    }
    console.log(this.metricSelectionMap);
  }

  selectOptionalValue(metricId: string, optionalValueId: string) {
    // Check if the metric is already selected, if not, exit
    if (!this.metricSelectionMap.has(metricId)) {
      return;
    }

    if (this.metricSelectionMap.get(metricId) === optionalValueId) {
      // If the optional value is already selected, deselect it
      this.metricSelectionMap.set(metricId, '');
      return;
    }

    this.metricSelectionMap.set(metricId, optionalValueId);
    console.log("Value selected: " + optionalValueId);
  }

  isMetricSelected(metricId: string) {
    return this.metricSelectionMap.has(metricId);
  }

  getSelectedOptionalValue(metricId: string) {
    const valueId = this.metricSelectionMap.get(metricId);
    if (!valueId) {
      return "Optional";
    }
    const value = this.allMetrics.filter((metric) =>
      metric.id === metricId)[0].values.filter((value) => value.id === valueId)[0];
    return value.name;
  }

  public updateSelectedMetrics() {
    // Clear the selectedMetrics array
    this.selectedMetrics = [];

    // Add selected metrics to the selectedMetrics array
    this.allMetrics.forEach((metric) => {
      const selection = this.metricSelectionMap.get(metric.id);
      if (selection) {
        this.selectedMetrics.push({
          metricsId: metric.id,
          metricValueId: selection,
        });
      }
    });
  }

  public async saveMetrics() {
    await this.metricService.addMetricsForDay(this.clickedDate.toISOString()!, this.selectedMetrics);
  }

  public async getMetrics() {
    // Fetch metrics from your service and populate allMetrics
    // For example:
    this.allMetrics = await this.metricService.getAllMetricsWithValues();

    this.periodMetric = this.allMetrics.filter((metric) => metric.name === 'Flow')
      ? this.allMetrics.filter((metric) => metric.name === 'Flow')[0]
      : undefined;
  }

  public async getSelectedMetricsForDay() {
    // Fetch selected metrics for the day from your service and update metricSelectionMap
    // For example:
    const metrics = await this.metricService.getMetricsForDay(this.clickedDate!);
    metrics.forEach((metric) => {
      this.metricSelectionMap.set(metric.metricsId, metric.metricValueId || '');
    });
  }
}
