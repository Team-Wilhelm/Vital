import {Component, HostListener, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CalendarDayMetric} from "../../interfaces/day.interface";
import {MetricService} from "../../services/metric.service";
import {animate, style, transition, trigger} from "@angular/animations";
import {Subscription} from "rxjs";
import {DeleteSectionService} from "../../services/deleteSection.service";

@Component({
  selector: 'metric-list-item',
  template: `
    <div [ngClass]="classString" class="relative overflow-x-hidden" (click)="closeDeleteSection($event)">
      <div class="flex lg:flex-col">
        <p class="font-bold pe-3 lg:p-0">{{calendarDayMetric?.metrics?.name}}</p>
        <p>{{calendarDayMetric?.metricValue?.name}}</p>
      </div>

      <div class="flex-col">
        <p>{{calendarDayMetric && calendarDayMetric!.createdAt.toLocaleTimeString()}}</p>
      </div>

      <div>
        <button class="btn btn-circle btn-xs btn-ghost" (click)="openDeleteSection($event)"> ...</button>
      </div>

      <div *ngIf="!deleteHidden" (click)="deleteMetric()" [@appear]="deleteHidden ? 'void' : 'visible'" #deleteSection
           class="absolute top-0 right-0 z-1 h-full">
        <button class="bg-red-400 h-full rounded-r-xl p-3" [disabled]="deleteButtonDisabled" #deleteButton>
          <i>
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
              <path fill="currentColor"
                    d="M7 21q-.825 0-1.412-.587T5 19V6H4V4h5V3h6v1h5v2h-1v13q0 .825-.587 1.413T17 21H7ZM17 6H7v13h10V6ZM9 17h2V8H9v9Zm4 0h2V8h-2v9ZM7 6v13V6Z"/>
            </svg>
          </i>
        </button>
      </div>
    </div>
  `,
  animations: [
    trigger('appear', [
      transition(':enter', [
        style({width: '0px', overflow: 'hidden'}),
        animate('250ms ease-in-out', style({width: '*', overflow: 'hidden'}))
      ]),
      transition(':leave', [
        animate('200ms ease-in-out', style({width: '0px', overflow: 'hidden'}))
      ])
    ])
  ],
})

export class MetricListItemComponent implements OnInit, OnDestroy {
  title = 'metric-list-item';
  @Input() listIndex: number = 1;
  @Input() calendarDayMetric: CalendarDayMetric | undefined;
  @ViewChild('deleteButton') deleteButton: any | undefined;
  @ViewChild('deleteSection') deleteSection: any | undefined;

  private subscription: Subscription;

  classString: string = "flex justify-between items-center rounded-xl p-5 w-full shadow";

  deleteHidden: boolean = true;
  deleteButtonDisabled: boolean = true;

  constructor(private metricService: MetricService, private deleteSectionService: DeleteSectionService) {
    this.subscription = this.deleteSectionService.deleteSectionOpened$.subscribe(id => {
      if (id !== this.listIndex) {
        this.deleteHidden = true;
        this.deleteButtonDisabled = true;
      }
    });
  }

  ngOnInit() {
    //TODO: colour based on metric value
    this.classString += " bg-secondary"
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  // Opens the delete section and disables the delete button for 200ms to prevent the user from double-clicking it by accident
  openDeleteSection(event: MouseEvent) {
    event.stopPropagation();
    this.deleteHidden = false;
    this.deleteSectionService.openDeleteSection(this.listIndex);
    setTimeout(() => {
      this.deleteButtonDisabled = false;
    }, 300);
  }

  deleteMetric() {
    this.metricService.deleteMetric(this.calendarDayMetric!.id);
  }

  @HostListener('document:click', ['$event'])
  closeDeleteSection(event: MouseEvent) {
    if (this.deleteButton && this.deleteButton.nativeElement !== event.target && this.deleteSection && this.deleteSection.nativeElement !== event.target) {
      this.deleteHidden = true;
      this.deleteButtonDisabled = true;
    }
  }
}
