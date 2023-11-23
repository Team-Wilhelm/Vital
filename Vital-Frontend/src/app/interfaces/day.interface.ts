import {Metrics, MetricValue} from "./metric.interface";
import {Cycle} from "./cycle.interface";
import {ApplicationUser} from "./user.interface";

export interface CalendarDay {
  id: string; //guid
  date: string; //Date
  userId: string; //guid
  user: ApplicationUser;
  state: string;
  selectedMetrics: CalendarDayMetric[];
}

export interface CycleDay extends CalendarDay {
  cycleId: string; //guid
  cycle: Cycle;
  isPeriodDay: boolean;
}

export interface CalendarDayMetric {
  id: string; //guid
  calendarDayId: string; //guid
  calendarDay: CalendarDay;
  metricsId: string; //guid
  metrics: Metrics;
  metricValueId: string; //guid
  metricValue: MetricValue;
}

export interface CycleDay extends CalendarDay {
  cycleId: string; //guid
  cycle: Cycle;
  isPeriod: boolean;
}
