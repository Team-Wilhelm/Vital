import {Component, OnInit} from '@angular/core';
import {Calendar, CalendarOptions} from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, {DateClickArg} from '@fullcalendar/interaction';
import timeGridPlugin from '@fullcalendar/timegrid';
import {FullCalendarComponent} from "@fullcalendar/angular";
import {Router} from "@angular/router";
import {cw} from "@fullcalendar/core/internal-common";
import {DataService} from "../services/data.service";
@Component({
  selector: 'calendar',
  templateUrl: './calendar.component.html',
})
export class CalendarComponent implements OnInit{

  constructor(private dataService: DataService, private router: Router) {}

  private newEvent: any;
  private eventList: any[] = [];

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
    const clickedDate = arg.date;
    if(clickedDate > new Date()) return;
    try {
      console.log('Clicked Date:', clickedDate);
      this.dataService.setClickedDate(clickedDate);
      await this.router.navigate(['/add-metric']);
    } catch (error) {
      console.error('Navigation error:', error);
    }
  }

  createEvent() {
    this.newEvent = {
      title: 'Period',
      start: '2023-11-01',
      id: '123',
      allDay: true,
      editable: true,
      color: 'red',
      textColor: 'white',
      extendedProps: {
        value: 'Heavy Flow',
      }
    };
    this.eventList.push(this.newEvent);
    this.calendarOptions.events = this.eventList;
  }

  ngOnInit() {
    this.createEvent()
  }
}



