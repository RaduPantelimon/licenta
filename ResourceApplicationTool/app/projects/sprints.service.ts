import { Injectable } from '@angular/core';

import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class SprintsService {
    private _sprintsUrl = '/api/restapi/GetSprints/';
    private _sprintsObservable: Observable<any[]>;
    public _obsvInitialized: boolean = false;
    constructor(private _http: Http) {

    }
    getSprints(projectID: number): Observable<any[]> {

        return this._http.get(this._sprintsUrl + projectID.toString()).map(
            (response: Response) => <any[]>response.json());
    }
    
    private handleError(error: Response) {
        console.log(error);
        return Observable.throw(error.json().error || 'Server error');
    }
}