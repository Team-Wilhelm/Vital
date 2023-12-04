import {Component} from "@angular/core";
import {animate, state, style, transition, trigger} from '@angular/animations';

@Component({
  selector: 'app-register',
  templateUrl: './auth.component.html',
  animations: [
    trigger('flipState', [
      state('front', style({
        transform: 'rotateY(0)'
      })),
      state('back', style({
        transform: 'rotateY(180deg)'
      })),
      transition('front => back', animate('400ms ease-out')),
      transition('back => front', animate('400ms ease-in'))
    ])
  ]

})
export class AuthComponent {

  flip: string = 'front';

  toggleFlip() {
    this.flip = (this.flip == 'front') ? 'back' : 'front';
  }
}
