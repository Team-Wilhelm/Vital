import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {Calendar, CalendarOptions} from '@fullcalendar/core';
import {FullCalendarComponent} from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, {DateClickArg} from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import {DataService} from "../services/data.service";
import {MetricService} from "../services/metric.service";

@Component({
  selector: 'calendar',
  templateUrl: './calendar.component.html',
})
export class CalendarComponent implements OnInit, AfterViewInit {

  @ViewChild('calendar') calendarComponent!: FullCalendarComponent;

  private eventList: any[] = [];
  private periodDays: Date[] = [];
  clickedDate = new Date();
  selectedDateElement: HTMLElement | null = null;
  private calendarApi!: Calendar;

  constructor(private dataService: DataService, private metricService: MetricService) {
  }

  async ngOnInit() {
    await this.getPeriodDays();
  }

  async ngAfterViewInit() {
    this.calendarApi = this.calendarComponent.getApi();
  }

  calendarOptions: CalendarOptions = {
    initialView: `dayGridMonth`,
    plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin],
    dateClick: this.handleDateClick.bind(this),
    handleWindowResize: true,
    weekNumberCalculation: 'ISO',
    height: 'auto',
    events: this.eventList,
  };

  async handleDateClick(arg: DateClickArg) {
    this.clickedDate = arg.date;
    const today = new Date();
    today.setHours(0,0,0,0);

    if (this.clickedDate > today) return;

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
    const thisMonthLastDay = new Date(2023, 11, 27);
    this.periodDays = await this.metricService.getPeriodDays(previousMonthFirstDay, thisMonthLastDay);

    for (let date of this.periodDays) {
      date = new Date(date);
      this.createEvent(date);
    }
  }

  createEvent(date: Date) {
    const newEvent = {
      start: date,
      allDay: true,
      editable: true,
      color: 'red',
      textColor: 'white',
      extendedProps: {
        value: 'Heavy Flow',
      }
    };
    // Use the addEvent method to dynamically add the new event
    this.calendarApi.addEvent(newEvent);

    // Alternatively, you can update the eventList array if needed
    this.eventList.push(newEvent);
  }
}



