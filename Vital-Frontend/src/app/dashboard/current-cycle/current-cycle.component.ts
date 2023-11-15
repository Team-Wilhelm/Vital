import {Component} from '@angular/core';

@Component({
  selector: 'app-current-cycle',
  template: `
    <div class="flex justify-center items-center text-center rounded-full bg-pink-300 aspect-square p-2
    w-10
    sm:w-16
    2xl:w-24">
      <p class="text-sm sm:text-base md:text-lg lg:text-lg 2xl:text-2xl">13.11</p>
    </div>
  `
})

export class CurrentCycleComponent {
  title = 'current-cycle';

  constructor() {

  }
}
