"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var http_1 = require('@angular/http');
var Observable_1 = require('rxjs/Observable');
require('rxjs/add/operator/map');
var TasksService = (function () {
    function TasksService(_http) {
        this._http = _http;
        this._tempaltetasksUrl = '/api/restapi/GetTemplateTasks';
        this._sprinttasksUrl = '/api/restapi/GetSprintTasks/';
        this._edittaskUrl = '/api/restapi/EditTask';
        this._createtaskUrl = '/api/restapi/SaveTask';
        this._deleteUrl = '/api/restapi/DeleteTask';
    }
    TasksService.prototype.getTemplateTasks = function () {
        return this._http.get(this._tempaltetasksUrl).map(function (response) { return response.json(); });
    };
    TasksService.prototype.getSprintTasks = function (sprintID) {
        return this._http.get(this._sprinttasksUrl + sprintID.toString())
            .map(function (response) { return response.json(); });
    };
    //delete existing task
    TasksService.prototype.deleteTask = function (taskID) {
        var _this = this;
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.delete(this._deleteUrl + "/" + taskID, options).map(function (response) { return _this.extractData(response); });
    };
    //add new task
    TasksService.prototype.addTask = function (data) {
        var _this = this;
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(this._createtaskUrl, data, options)
            .map(function (response) { return _this.extractData(response); });
    };
    //update a task
    TasksService.prototype.updateTask = function (task) {
        var _this = this;
        var headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        var options = new http_1.RequestOptions({ headers: headers });
        return this._http.post(this._edittaskUrl, task, options)
            .map(function (response) { return _this.extractData(response); });
    };
    TasksService.prototype.handleError = function (error) {
        console.log(error);
        return Observable_1.Observable.throw(error.json().error || 'Server error');
    };
    TasksService.prototype.extractData = function (res) {
        var body = res.json();
        return body || {};
    };
    TasksService = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [http_1.Http])
    ], TasksService);
    return TasksService;
}());
exports.TasksService = TasksService;
//# sourceMappingURL=tasks.service.js.map