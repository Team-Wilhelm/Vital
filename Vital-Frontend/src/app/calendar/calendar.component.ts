import {AfterViewInit, Component, ViewChild} from '@angular/core';
import {Calendar, CalendarOptions} from '@fullcalendar/core';
import {FullCalendarComponent} from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, {DateClickArg} from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import {DataService} from "../services/data.service";
import {MetricService} from "../services/metric.service";
import {CycleService} from "../services/cycle.service";

@Component({
  selector: 'calendar',
  templateUrl: './calendar.component.html'
})
export class CalendarComponent implements AfterViewInit {

  @ViewChild('calendar') calendarComponent!: FullCalendarComponent;

  private eventList: any[] = [];
  private periodDays: Date[] = [];
  clickedDate = new Date();
  selectedDateElement: HTMLElement | null = null;
  private calendarApi?: Calendar;

  constructor(private dataService: DataService, private metricService: MetricService, private cycleService: CycleService) {
  }

  async ngAfterViewInit() {
    this.calendarApi = this.calendarComponent.getApi();
    await this.getPeriodDays();
    await this.getPredictedPeriodDays();

    this.calendarApi?.setOption('dateClick', this.handleDateClick.bind(this));
    this.calendarApi?.setOption('datesSet', this.handleDatesSet.bind(this));
  }

  calendarOptions: CalendarOptions = {
    initialView: `dayGridMonth`,
    plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin],
    dateClick: this.handleDateClick.bind(this),
    handleWindowResize: true,
    weekNumberCalculation: 'ISO',
    height: 'auto',
    events: this.eventList,
    eventMouseEnter: function (info) {
      info.el.style.cursor = 'pointer';
      const tooltip = document.createElement('div');
      tooltip.classList.add('fc-tooltip');
      tooltip.style.position = 'absolute';
      tooltip.style.zIndex = '10000';
      tooltip.style.backgroundColor = 'rgba(0, 0, 0, 0.85)';
      tooltip.style.color = 'white';
      tooltip.style.padding = '5px 10px';
      tooltip.style.borderRadius = '3px';
      tooltip.style.fontSize = '14px';
      tooltip.innerText = info.event.extendedProps['description'];
      document.body.appendChild(tooltip);
      info.el.onmousemove = (e) => {
        tooltip.style.left = e.pageX + 10 + 'px';
        tooltip.style.top = e.pageY + 10 + 'px';
      };
    },
    eventMouseLeave: function () {
      const tooltip = document.querySelector('.fc-tooltip');
      if (tooltip) {
        tooltip.remove();
      }
    }
  };

  async handleDateClick(arg: DateClickArg) {
    this.clickedDate = arg.date;
    const today = new Date();
    today.setHours(0,0,0,0);

    if (this.clickedDate > today) return;

    this.dataService.setClickedDate(this.clickedDate);
    if (this.selectedDateElement) {
      this.selectedDateElement.classList.remove('bg-non-period-day-border');
    }
    // Add the custom class to the clicked date cell
    this.selectedDateElement = arg.dayEl;
    this.selectedDateElement.classList.add('bg-non-period-day-border');
  }

  async handleDatesSet(arg: any) {
    await this.getPeriodDays();
    await this.getPredictedPeriodDays();
  }

  async getPeriodDays() {
    if (!this.calendarApi) {
      return;
    }
    this.eventList = [];
    this.calendarApi?.removeAllEvents();

    const currentDate = this.calendarApi.getDate(); // Get the currently displayed month
    const firstDay = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
    const lastDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 2, 0);
    const today = new Date();

    this.periodDays = await this.metricService.getPeriodDays(firstDay, lastDay > today ? today : lastDay);

    for (let date of this.periodDays) {
      date = new Date(date);
      this.createEvent(date);
    }
  }

  async getPredictedPeriodDays() {
    for (let date of this.cycleService.predictedPeriod) {
      date = new Date(date);
      this.createPredictedEvent(date);
    }
  }

  createEvent(date: Date) {
    const newEvent = {
      start: date,
      allDay: true,
      backgroundColor: '#CB9292',
      borderColor:'#BA6E6E',
      display: 'block',
      description: 'Period'
      //url: maybe route to add metric page for that day?
    };
    this.calendarApi?.addEvent(newEvent);
    this.eventList.push(newEvent);
  }

  createPredictedEvent(date: Date) {
    const newEvent = {
      start: date,
      allDay: true,
      backgroundColor: '#DBC2C6',
      borderColor: '#BA6E6E',
      description: 'Predicted period',
      display: 'block'
      //url: maybe route to add metric page for that day?
    };
    this.calendarApi?.addEvent(newEvent);
    this.eventList.push(newEvent);
  }
}



