import {Component, Input, OnInit} from "@angular/core";

@Component({
  selector: 'app-stat-card',
  template: `
      <div [ngClass]="getClassList()">
          <div class="card-body items-center justify-center">
              <div class="flex flex-col items-center justify-center">
                  <div class="text-sm text-slate-500 text-center">{{ title }}</div>
                  <div class="text-3xl sm:text-4xl font-extrabold whitespace-nowrap"
                       [class.text-accent]="accentFont">{{ value }} {{ unit }}
                  </div>
              </div>
          </div>
      </div>
  `,
})
export class ApplicationStatCardComponent implements OnInit {
  @Input() title: string = '';
  @Input() value: number = 0;
  @Input() unit: string = '';
  @Input() cardBackground: string = '';
  @Input() accentFont: boolean = false;

  classList: string[] = [];

  constructor() {
  }

  ngOnInit() {
    this.classList.push('card', 'card-compact', 'lg:card-normal', 'h-full', 'w-full', 'shadow-lg');

    if (this.cardBackground) {
      this.classList.push(this.cardBackground);
    } else {
      this.classList.push('bg-base-100');
    }
  }

  getClassList() {
    return this.classList.join(' ');
  }
}
