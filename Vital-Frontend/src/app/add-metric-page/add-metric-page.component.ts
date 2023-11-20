import {Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {DataService} from "../services/data.service";

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent implements OnInit{
  public allMetrics: Metric[] = [];
  public selectedMetrics: Metric[] = [];
  public dateParameter: string | null = null;

  constructor(private route: ActivatedRoute, private dataService: DataService) {
    this.allMetrics.push(
      {
        name: 'Period',
        isSelected: false,
        selectedValue: null,
        values: ['None', 'Light', 'Moderate', 'Heavy'],
      },
      {
        name: 'Cramps',
        isSelected: false,
        selectedValue: null,
        values: ['None', 'Mild', 'Moderate', 'Severe'],
      },
      {
        name: 'Headache',
        isSelected: false,
        selectedValue: null,
        values: ['None', 'Mild', 'Moderate', 'Severe'],
      }
    );
  }

  //TODO check routing
  ngOnInit(): void {
      //this.route.paramMap.subscribe(params => {
      //const clickedDate = params.get('clickedDate');
    this.dataService.clickedDate$.subscribe(clickedDate => {
      console.log('Clicked Date Add-Metrics:', clickedDate);
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

  //TODO send Date as ISOString: const isoString = currentDate.toISOString();
  private updateSelectedMetrics() {
    // Clear the selectedMetrics array
    this.selectedMetrics = [];

    // Add selected metrics to the selectedMetrics array
    this.allMetrics.forEach((metric) => {
      if (metric.isSelected) {
        this.selectedMetrics.push({ ...metric }); // Create a copy to avoid reference issues
      }
    });
  }
}

interface Metric {
  name: string;
  isSelected: boolean;
  selectedValue: string | null;
  values: string[];
}
