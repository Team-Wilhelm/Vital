import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'medication-list-item',
  template: `
      <div [ngClass]="classString">
        <div class="flex-col">
          <p class="font-bold">{{medicationName}}</p>
          <p>{{medicationDosage}}</p>
        </div>

        <div class="flex-col">
          <p>{{medicationTime}}</p>
        </div>
      </div>
  `
})

//TODO
export class MedicationListItemComponent implements OnInit {
  title = 'medication-list-item';
  @Input() medicationName: string = '';
  @Input() medicationTime: string = '';
  @Input() medicationDosage: string = '';
  @Input() listIndex: number = 1;

  classString: string = "flex justify-between items-center rounded-xl p-5 my-2";

  constructor() {

  }

  ngOnInit() {
    if (this.listIndex % 2 == 0) {
      this.classString+= " bg-green-accent"
    } else {
      this.classString+= " bg-green-light-accent"
    }
  }
}
