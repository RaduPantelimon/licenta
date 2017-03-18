import { Injectable } from '@angular/core';

import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class EmployeesService {
    private _employeesUrl = '/api/restapi/GetEmployees/';
    constructor(private _http: Http) {

    }
    getEmployees(projectID: number): Observable<any[]> {

        return this._http.get(this._employeesUrl + projectID.toString()).map(
            (response: Response) => <any[]>response.json()
        );
    }
    private handleError(error: Response) {
        console.log(error);
        return Observable.throw(error.json().error || 'Server error');
    }
}