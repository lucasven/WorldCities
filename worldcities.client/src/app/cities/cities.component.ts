import { Component, OnInit, ViewChild } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { City } from './city';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})
export class CitiesComponent {
  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(query?: string){
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    var url = environment.baseUrl + 'api/Cities';
    var params = new HttpParams()
      .set('pageIndex', event.pageIndex.toString())
      .set('pageSize', event.pageSize.toString())
      .set('sortColumn', this.sort ? this.sort.active : this.defaultSortColumn)
      .set('sortOrder', this.sort ? this.sort.direction : this.defaultSortOrder);
    if(this.filterQuery){
      params = params
        .set('filterColumn', this.defaultFilterColumn)
        .set('filterQuery', this.filterQuery);
    }
    this.http.get<any>(url, {params})
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageSize = result.pageSize;
          this.paginator.pageIndex = result.pageIndex;
          this.cities = new MatTableDataSource<City>(result.data);
        },
        error: (error: HttpErrorResponse) => {
          console.error(error);
        }
      });
  }
}
