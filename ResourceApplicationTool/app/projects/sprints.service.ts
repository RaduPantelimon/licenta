import { Injectable } from '@angular/core';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { Subject } from 'rxjs/Subject';


@Injectable()
export class SprintsService {
    private _sprintsUrl = '/api/restapi/GetSprints/';
    private _createSprintUrl = '/api/restapi/SaveSprint';
    private _deleteUrl = '/api/restapi/DeleteSprint';

    private _sprintsObservable: Observable<any[]>;
    public _obsvInitialized: boolean = false;
    private addedSprints = new Subject<any>();

    constructor(private _http: Http) {

    }

    // Service message commands
    registerNewlyCreateSprint(sprint: any) {
        this.addedSprints.next(sprint);
    }

    newSprints = this.addedSprints.asObservable();

    getSprints(projectID: number): Observable<any[]> {

        return this._http.get(this._sprintsUrl + projectID.toString()).map(
            (response: Response) => <any[]>response.json());
    }
    
    //add new sprint
    addSprint(sprint: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.post(this._createSprintUrl, sprint, options)
            .map(
            (response: Response) => <any>this.extractData(response));
    }

    //delete existing task
    deleteSprint(sprintID: number): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.delete(this._deleteUrl + "/" + sprintID, options).map(
            (response: Response) => <any>this.extractData(response));
    }



    private handleError(error: Response) {
        console.log(error);
        return Observable.throw(error.json().error || 'Server error');
    }

    private extractData(res: Response) {
        let body = res.json();
        return body || {};
    }
}