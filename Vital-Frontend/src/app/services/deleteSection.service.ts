import {Injectable} from "@angular/core";
import {Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class DeleteSectionService {
  private deleteSectionOpenedSource = new Subject<number>();
  deleteSectionOpened$ = this.deleteSectionOpenedSource.asObservable();

  openDeleteSection(id: number) {
    this.deleteSectionOpenedSource.next(id);
  }
}
