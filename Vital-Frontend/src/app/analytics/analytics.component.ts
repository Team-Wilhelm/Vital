import { Component, ViewEncapsulation, OnInit } from '@angular/core';
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
              'rgb(249, 168, 212, 0.2)'
            ],
            borderColor: [
              'rgb(249, 168, 212, 1)'
            ],
            borderWidth: 2
          },
          {
            label: "Cycle",
            data: [20, 29, 19, 10, 18, 17, 21, 30],
            backgroundColor: [
              'rgb(238, 175, 58, 0.2)'
            ],
            borderColor: [
              'rgb(238, 175, 58, 1)'
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

    var myChart2 = new Chart('myChart2', {
      type: 'bar',
      data: {
        labels: ['January', 'February', 'March', 'April', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datasets: [{
          label: 'Length of Periods',
          data: [6, 5, 4, 8, 1, 9, 1, 2, 8, 7, 3, 0],
          backgroundColor: [
            'rgb(238, 175, 58, 0.2)'
          ],
          borderColor: [
            'rgb(238, 175, 58, 1)'
          ],
          borderWidth: 1
        }]
      }
    });
  }

  ngOnInit(): void {
    this.createChart();
  }

}
