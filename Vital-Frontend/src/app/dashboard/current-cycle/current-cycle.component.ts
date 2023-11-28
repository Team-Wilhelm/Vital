import {AfterViewInit, Component, Input, OnDestroy, OnInit} from '@angular/core';
import {DataService} from "../../services/data.service";
import {DatePipe} from "@angular/common";
import {Subscription} from "rxjs";
import {CycleService} from "../../services/cycle.service";
import {MetricService} from "../../services/metric.service";

@Component({
  selector: 'app-current-cycle',
  template: `
    <div class="flex h-full justify-between items-center">
      <div *ngFor="let day of dateMap.keys()" class="flex justify-center items-center text-center rounded-full bg-pink-300 aspect-square p-2
    w-10
    sm:w-16
    2xl:w-24">
        <p class="text-sm sm:text-base md:text-lg lg:text-lg 2xl:text-2xl">{{dateString(day)}}</p>
      </div>
    </div>
  `
})

export class CurrentCycleComponent implements OnInit, AfterViewInit {
  title = 'current-cycle';
  @Input() date: string = '';
  today: Date = new Date();
  dateMap: Map<Date, string> = new Map();
  dateMapKeys: Date[] = [];
  periodDays: Date[] = [];
  predictedPeriodDays: Date[] = [];

  constructor(private cycleService: CycleService, private metricService: MetricService, private datePipe: DatePipe) {

  }

  async ngOnInit() {
    const threeDaysAgo = new Date(this.today);
    threeDaysAgo.setDate(this.today.getDate() - 3);
    const threeDaysHence = new Date(this.today);
    threeDaysHence.setDate(this.today.getDate() + 3);

    this.periodDays = await this.metricService.getPeriodDays(threeDaysAgo, threeDaysHence);
    console.log('period days: ' + this.periodDays);

    this.predictedPeriodDays = this.cycleService.predictedPeriod;
    console.log('predicted days: ' + this.predictedPeriodDays);

    this.initializeDateMap();
    console.log(this.dateMap);
  }

  ngAfterViewInit() {
    this.dateMapKeys = Array.from(this.dateMap.keys());
    console.log('keys: ' + this.dateMapKeys);
  }


  //TODO make this shit work!
  initializeDateMap() {
    const interval = 3;

    for (let i = -interval; i <= interval; i++) {
      const date = new Date(this.today);
      date.setDate(this.today.getDate() + i);
      this.dateMap.set(date, 'not-period');
      for (let p of this.periodDays)
        if (this.isSameDate(p, date)) {
          this.dateMap.set(date, 'period');
        }
      for (let pp of this.predictedPeriodDays)
        if (this.isSameDate(pp, date)) {
          this.dateMap.set(date, 'predicted-period');
        }
    }
  }

  private isSameDate(date1: Date, date2: Date): boolean {
    return date1.getFullYear() === date2.getFullYear() &&
      date1.getMonth() === date2.getMonth() &&
      date1.getDate() === date2.getDate();
  }

  dateString(date: Date): string {
    const dateFormat = 'dd/MM'; // Adjust the date format based on your requirements
    return this.datePipe.transform(date, dateFormat) || '';
  }

}
