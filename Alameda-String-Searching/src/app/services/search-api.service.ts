import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { QueryInput } from '../models/query-input';

@Injectable({
  providedIn: 'root'
})
export class SearchApiService {
  private apiURL = "https://localhost:7026";

  constructor(private http: HttpClient) {}

  searchQuery(input: QueryInput): Observable<string[]> {
    return this.http.post<string[]>(this.apiURL + '/api/Search', input)
  }

}