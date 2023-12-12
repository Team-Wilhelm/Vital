import {Component, ViewEncapsulation} from "@angular/core";
import {animate, state, style, transition, trigger} from '@angular/animations';

@Component({
  selector: 'app-register',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css'],
  animations: [
    trigger('flipState', [
      state('front', style({
        transform: 'rotateY(0deg)'
      })),
      state('back', style({
        transform: 'rotateY(-180deg)'
      })),
      transition('front => back', animate('500ms ease')),
      transition('back => front', animate('500ms ease')),
    ])
  ],
  encapsulation: ViewEncapsulation.None

})
export class AuthComponent {

  flip: string = 'front';

  toggleFlip() {
    this.flip = (this.flip == 'front') ? 'back' : 'front';
  }
}
