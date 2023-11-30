import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class AnalyticsComponent implements OnInit { //corrected typo here, replaced imlements with implements
  public chart: any;
  public chart2: any;

  createChart(){
    var myChart = new Chart('myChart', {
      type: 'bar',
      data: {
        labels: ['26 days', '33 days', '26 days', '15 days', '26 days', '21 days', '27 days', '35 days'],
        datasets: [
          {
            label: "Period",
            data: [6, 4, 7, 5, 8, 4, 6, 5],
            backgroundColor: [
              'rgb(246, 203, 209, 0.2)'
            ],
            borderColor: [
              'rgb(246, 203, 209, 1)'
            ],
            borderWidth: 2
          },
          {
            label: "Cycle",
            data: [20, 29, 19, 10, 18, 17, 21, 30],
            backgroundColor: [
              'rgb(112, 172, 199, 0.2)'
            ],
            borderColor: [
              'rgb(112, 172, 199, 1)'
            ],
            borderWidth: 2
          }
        ]
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        scales: {
          x: {
            stacked: true,
          },
          y: {
            stacked: true
          }
        }
      }
    });
  }

  ngOnInit(): void {
    this.createChart();
  }

}
