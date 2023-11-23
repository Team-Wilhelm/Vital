import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { MetricService } from '../services/metric.service';
import { MetricRegisterMetricDto, MetricValueViewDto, MetricViewDto, CalendarDayMetricDto } from '../interfaces/dtos/metric.dto.interface';

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent implements OnInit {
  public allMetrics: MetricViewDto[] = [];
  public selectedMetrics: MetricRegisterMetricDto[] = [];
  public clickedDate: Date = new Date();
  public metricSelectionMap: Map<string, { selectedValue: string }> = new Map();

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

  onCheckboxChange(metric: MetricViewDto) {
    // Toggle the selected state
    if (!this.metricSelectionMap.has(metric.id)) {
      // If no value is selected for this metric, select the first one
      const firstValue = metric.values && metric.values.length > 0 ? metric.values[0].name : null;
      this.metricSelectionMap.set(metric.id, { selectedValue: firstValue || '' });
    } else {
      // If a value is already selected, remove it
      this.metricSelectionMap.delete(metric.id);
    }

    this.updateSelectedMetrics();
  }

  onRadioChange(metric: MetricViewDto, selectedValue: string) {
    // Update the selected value directly in the metric
    this.metricSelectionMap.set(metric.id, { selectedValue });

    // Update the selectedMetrics array
    this.updateSelectedMetrics();
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
          metricValueId: selection.selectedValue,
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
  }

  public async getSelectedMetricsForDay() {
    // Fetch selected metrics for the day from your service and update metricSelectionMap
    // For example:
    const metrics = await this.metricService.getMetricsForDay(this.clickedDate!);
    metrics.forEach((metric) => {
      this.metricSelectionMap.set(metric.metricsId, { selectedValue: metric.metricValueId });
    });
  }
}
