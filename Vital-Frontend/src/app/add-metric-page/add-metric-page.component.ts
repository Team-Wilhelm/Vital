import {Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {DataService} from "../services/data.service";
import {MetricService} from "../services/metric.service";

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent implements OnInit{
  public allMetrics: Metric[] = [];
  public selectedMetrics: Metric[] = [];
  public clickedDate: string | undefined;

  constructor(private dataService: DataService, private metricService: MetricService) {
    this.getMetrics().then();
  }

  ngOnInit(): void {
    this.dataService.clickedDate$.subscribe(clickedDate => {
      this.clickedDate = clickedDate?.toISOString();
    });
  }

  onCheckboxChange(metric: Metric) {
    // Toggle the metric's selected state
    metric.isSelected = !metric.isSelected;

    // Reset the selected value if the metric is being deselected
    if (!metric.isSelected) {
      metric.selectedValue = null;
    }

    // Update the selectedMetrics array
    this.updateSelectedMetrics();
  }

  onRadioChange(metric: Metric, selectedValue: string) {
    // Update the selected value directly in the metric
    metric.selectedValue = selectedValue;

    // Update the selectedMetrics array
    this.updateSelectedMetrics();
  }

  public updateSelectedMetrics() {
    // Clear the selectedMetrics array
    this.selectedMetrics = [];

    // Add selected metrics to the selectedMetrics array
    this.allMetrics.forEach((metric) => {
      if (metric.isSelected) {
        this.selectedMetrics.push({ ...metric }); // Create a copy to avoid reference issues
      }
    });
  }

  public async saveMetrics() {
    const metricDtos: MetricDto[] = [];
    this.selectedMetrics.forEach((metric) => {
      metricDtos.push({
        id: metric.name,
        value: metric.selectedValue!,
      });
    });
    await this.metricService.addMetricsForDay(this.clickedDate!, metricDtos);
  }

  public async getMetrics() {
    this.allMetrics = await this.metricService.getAllMetricsWithValues();
  }

}

