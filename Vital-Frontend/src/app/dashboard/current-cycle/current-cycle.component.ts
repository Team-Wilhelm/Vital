import {Component, Input, OnInit} from '@angular/core';
import {DatePipe} from "@angular/common";
import {CycleService} from "../../services/cycle.service";
import {MetricService} from "../../services/metric.service";

@Component({
  selector: 'app-current-cycle',
  template: `
      <div class="flex h-full justify-between items-baseline">
          <div *ngFor="let day of dateKeys" class="flex flex-col items-center text-center justify-center mx-2"
               [ngStyle]="{
         'opacity': getOpacity(day),
         'font-size': 'inherit',
         'width': getSize(day) + 'px',
          'font-weight': getFontWeight(day),
       }">

              <div class="tooltip tooltip-primary" [attr.data-tip]="getTooltip(day)">
                  <div
                          class="rounded-md overflow-hidden border-[3px] {{getBackgroundColor(day).bgColour}} {{getBackgroundColor(day).borderColour}} p-2 w-12 h-20"
                          [ngStyle]="{'height': getSize(day) + 'px'}">
                  </div>
                  <p class="text-sm sm:text-base md:text-lg lg:text-lg xs:text-xs mt-2">{{ dateString(day) }}</p>
              </div>
          </div>
      </div>
  `
})

export class CurrentCycleComponent implements OnInit {
  title = 'current-cycle';
  @Input() date: string = '';
  today: Date = new Date();
  dateMap: Map<Date, {
    bgColour: string,
    borderColour: string,
    opacity: number,
    size: number,
    fontWeight: number,
    tooltip: string
  }> = new Map();
  dateKeys: Date[] = [];
  periodDays: Date[] = [];
  predictedPeriodDays: Date[] = [];

  constructor(private cycleService: CycleService, private metricService: MetricService, private datePipe: DatePipe) {

  }

  async ngOnInit() {
    const originalToday = new Date(this.today);
    const threeDaysAgo = new Date(originalToday.setDate(originalToday.getDate() - 3));
    const threeDaysHence = new Date(originalToday.setDate(originalToday.getDate() + 6));

    this.periodDays = await this.metricService.getPeriodDays(threeDaysAgo, threeDaysHence);
    this.predictedPeriodDays = this.cycleService.predictedPeriod;

    this.initializeDateMap();
  }

  initializeDateMap() {
    const interval = 3;

    for (let i = -interval; i <= interval; i++) {
      const date = new Date(this.today);
      date.setDate(this.today.getDate() + i);

      const conditionMapping = {
        isToday: this.isSameDate(date, this.today),
        isFutureDate: date > this.today,
        isPredictedPeriod: this.predictedPeriodDays.some(pp => this.isSameDate(pp, date)),
        isActualPeriod: this.periodDays.some(p => this.isSameDate(p, date)),
      };

      let bgColour = 'bg-non-period-day';
      let borderColour = 'border-non-period-day-border';
      let opacity = 1;
      let size = 75;
      let fontWeight = 400;
      let tooltip = '';

      if (conditionMapping.isToday) {
        size = 90;
        fontWeight = 600;
      }

      if (conditionMapping.isFutureDate) {
        opacity = 0.4;
      }

      if (conditionMapping.isPredictedPeriod) {
        bgColour = 'bg-predicted-period-day';
        tooltip = 'Predicted period';
        borderColour = 'border-predicted-period-day-border';
      } else if (conditionMapping.isActualPeriod) {
        bgColour = 'bg-period-day';
        borderColour = 'border-period-day-border';
        tooltip = 'Period';
      } else {
        tooltip = 'Non-period day';
      }

      this.dateMap.set(date, {bgColour, borderColour, opacity, size, fontWeight, tooltip});
    }
    this.dateKeys = Array.from(this.dateMap.keys());
  }

  private isSameDate(date1: Date, date2: Date): boolean {
    return date1.getFullYear() === date2.getFullYear() &&
      date1.getMonth() === date2.getMonth() &&
      date1.getDate() === date2.getDate();
  }

  getBackgroundColor(date: Date): { bgColour: string, borderColour: string } {
    const colors = this.dateMap.get(date);

    if (colors) {
      return colors;
    } else {
      return {bgColour: '', borderColour: ''};
    }
  }

  getOpacity(date: Date): number {
    return this.dateMap.get(date)?.opacity || 100;
  }

  getSize(date: Date): number {
    return this.dateMap.get(date)?.size || 100;
  }

  getFontWeight(date: Date): number {
    return this.dateMap.get(date)?.fontWeight || 400;
  }

  getTooltip(day: Date) {
    return this.dateMap.get(day)?.tooltip || '';
  }

  dateString(date: Date): string {
    const dateFormat = 'dd/MM';
    return this.datePipe.transform(date, dateFormat) || '';
  }
}
