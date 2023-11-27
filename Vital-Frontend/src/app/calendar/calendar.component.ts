import {AfterViewInit, Component, OnInit} from '@angular/core';
import {Calendar, CalendarApi, CalendarOptions, EventSourceInput} from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, {DateClickArg} from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import {Router} from "@angular/router";
import {DataService} from "../services/data.service";
import {MetricService} from "../services/metric.service";
import {EventContainer} from "@fullcalendar/core/internal";

@Component({
  selector: 'calendar',
  templateUrl: './calendar.component.html',
})
export class CalendarComponent implements OnInit, AfterViewInit{

  calendarApi!: CalendarApi;
  private newEvent: any;
  private eventList: any[] = [];
  private periodDays: Date[] = [];
  clickedDate = new Date();
  selectedDateElement: HTMLElement | null = null;
  constructor(private dataService: DataService, private metricService: MetricService, private router: Router) {}

  async ngOnInit() {
    await this.getPeriodDays();
  }

  async ngAfterViewInit() {
    for (const date of this.periodDays) {
      const formattedDate = date.toISOString().split('T')[0];
      this.createEvent(formattedDate);
    }
    this.calendarApi.refetchEvents();
  }

  calendarOptions: CalendarOptions = {
    initialView: `dayGridMonth`,
    plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin],
    dateClick: this.handleDateClick.bind(this),
    handleWindowResize: true,
    weekNumberCalculation: 'ISO',
    height: 'auto',
    events: this.eventList,
    datesSet: (info) => {
      this.calendarApi = info.view.calendar;
    }
  };

  async handleDateClick(arg: DateClickArg) {
    this.clickedDate = arg.date;
    const today = new Date();
    today.setHours(0,0,0,0);
    //console.log(this.clickedDate);
    //console.log(today);
    //console.log(this.clickedDate > today);
    //TODO

    if(this.clickedDate > today) return;

    this.dataService.setClickedDate(this.clickedDate);
    if (this.selectedDateElement) {
      this.selectedDateElement.classList.remove('bg-green-accent');
    }
    // Add the custom class to the clicked date cell
    this.selectedDateElement = arg.dayEl;
    this.selectedDateElement.classList.add('bg-green-accent');
  }

  async getPeriodDays() {
    const previousMonthFirstDay = new Date(2023, 10, 1);
    const thisMonthLastDay = new Date(2023, 11, 23);
    this.periodDays = await this.metricService.getPeriodDays(previousMonthFirstDay, thisMonthLastDay);
  }

  createEvent(date: string){
    this.newEvent = {
      title: 'Period',
      start: date,
      allDay: true,
      editable: true,
      color: 'red',
      textColor: 'white',
      duration: {days: 1},
    };
    this.eventList.push(this.newEvent);
  }
}



