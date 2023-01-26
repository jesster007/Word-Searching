import { Component } from '@angular/core';
import { SearchApiService } from '../../services/search-api.service';
import { QueryInput } from '../../models/query-input';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-text-entry',
  templateUrl: './text-entry.component.html',
  styleUrls: ['./text-entry.component.scss']
})
export class TextEntryComponent {
  searchResults:string[] = [];
  waiting:boolean = false;
  searched:boolean = false;
  data = new QueryInput;
  constructor(private searchService:SearchApiService, private snackBar: MatSnackBar){}
  
  callQuery(){
    this.waiting = true;
    this.searchService.searchQuery(this.data).subscribe({
      next: (output) => {
        setTimeout(() => { //Added delay for the progress bar
          this.waiting = false;
          this.searched = true;
          this.searchResults = output;
        }, 200);
      },
      error: (error) => {
        this.waiting = false;
        console.log(error);
        this.snackBar.open('Searching Failed, Is the Server Online?', "Close", { 
          duration: 4000
        });
      }
    })

  }


  clear(){
    this.searched = false;
    this.searchResults = [];
    this.data.Input = "";
    this.data.Query = "";
  }
}
