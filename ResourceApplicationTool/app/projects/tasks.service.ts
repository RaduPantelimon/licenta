import { Injectable } from '@angular/core';

import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class TasksService {
    private _tempaltetasksUrl = '/api/restapi/GetTemplateTasks';
    private _sprinttasksUrl = '/api/restapi/GetSprintTasks/';
    private _edittaskUrl = '/api/restapi/EditTask';
    //private _createtaskUrl = '/api/restapi/SaveTask';
    private _createtaskUrl = '/Projects/SaveTask';
    private _deleteUrl = '/api/restapi/DeleteTask';
    constructor(private _http: Http) {

    }
    getTemplateTasks(): Observable<any[]> {

        return this._http.get(this._tempaltetasksUrl).map(
            (response: Response) => <any[]>response.json()
        );
    }
    getSprintTasks(sprintID: number): Observable<any[]> {

        return this._http.get(this._sprinttasksUrl + sprintID.toString())
            .map(
            (response: Response) => <any[]>response.json()
        );
    }
    //delete existing task
    deleteTask(taskID: number): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.delete(this._deleteUrl + "/" + taskID, options).map(
            (response: Response) => <any>this.extractData(response));
    }

    //add new task
    addTask(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.post(this._createtaskUrl, data, options)
            .map(
            (response: Response) => <any>this.extractData(response));
    }

    //update a task
    updateTask(task: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http.post(this._edittaskUrl, task, options)
            .map(
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