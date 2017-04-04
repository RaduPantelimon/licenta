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
var router_1 = require('@angular/router');
var sprints_service_1 = require('./projects/sprints.service');
var employees_service_1 = require('./projects/employees.service');
var moment = require('moment/moment');
var AppComponent = (function () {
    function AppComponent(_employeesService, _sprintService, _route, _router) {
        this._employeesService = _employeesService;
        this._sprintService = _sprintService;
        this._route = _route;
        this._router = _router;
        this.sprintMonths = [];
        //employees: any[] = [];
        this.sprints = [];
    }
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        try {
            var id_1 = window["projectID"];
            if (id_1 && !isNaN(parseInt(id_1))) {
                var currentComponent_1 = this;
                //getting the sprint id from the url
                var urlComponents = location.href.split("/");
                var spid_1 = parseInt(urlComponents[urlComponents.length - 1]);
                //initializing the Sprints Page
                this._sprintService.getSprints(id_1).subscribe(function (sprints) {
                    if (location.href.indexOf("addsprint") != -1) {
                        //redirected to the addsprints page
                        currentComponent_1.createSprintEnabled = true;
                    }
                    currentComponent_1.sprints = sprints.map(function (val, index) {
                        if ((spid_1 && spid_1 == val.SprintID) || ((!spid_1 || isNaN(spid_1) || location.href.indexOf("task") == -1) && index == 0)) {
                            val.selected = true;
                            currentComponent_1.selectedMonth = moment(val.StartDate).format("YYYY MMMM");
                        }
                        else {
                            val.selected = false;
                        }
                        return val;
                    });
                    currentComponent_1.setMonths();
                }, function (error) { return _this.errorMessage = error; });
                //waiting for newly added sprints
                this._sprintService.newSprints.subscribe(function (newSprint) {
                    //reObtaining the Sprints from the DataBase
                    var indices = 0;
                    _this._sprintService.getSprints(id_1).subscribe(function (sprints) {
                        _this.sprints = sprints.map(function (val, index) {
                            if ((newSprint.SprintID == val.SprintID)) {
                                val.selected = true;
                                currentComponent_1.selectedMonth = moment(val.StartDate).format("YYYY MMMM");
                            }
                            else {
                                val.selected = false;
                            }
                            return val;
                        });
                        currentComponent_1.setMonths();
                    }, function (error) { return _this.errorMessage = error; });
                    if (newSprint.SprintID) {
                        //navigating to our sprint
                        _this._router.navigate(['/tasks', newSprint.SprintID]);
                        _this.createSprintEnabled = false;
                    }
                }, function (error) { return _this.errorMessage = error; });
            }
        }
        catch (ex) {
            console.log('Error while retrieving data ' + ex);
            this.errorMessage = ex.toString();
        }
    };
    AppComponent.prototype.onTest = function () {
        //console.log(this.employees);
        console.log(this.sprints);
        this.sprints[1].selected = true;
    };
    AppComponent.prototype.onChangeSprint = function (originalSprint, event) {
        this.createSprintEnabled = false;
        var sprint = this.sprints.filter(function (s) { return s.selected; })[0];
        if (sprint)
            sprint.selected = false;
        originalSprint.selected = true;
        //navigating away
        //this._router.navigate(['/products']);
    };
    AppComponent.prototype.onChangeToSprintCreation = function (event) {
        this.createSprintEnabled = true;
    };
    AppComponent.prototype.changeMonth = function (event) {
        //auto navigating to the first sprint of the week
        var foundElement = false;
        var newSprint;
        if (this.sprints && this.sprints.length > 0) {
            for (var i = 0; i < this.sprints.length; i++) {
                var currentSprint = this.sprints[i];
                currentSprint.selected = false;
                if ((this.selectedMonth == moment(currentSprint.StartDate).format("YYYY MMMM") ||
                    this.selectedMonth == moment(currentSprint.EndDate).format("YYYY MMMM")) && !foundElement) {
                    foundElement = true;
                    currentSprint.selected = true;
                    newSprint = currentSprint;
                }
            }
            this._router.navigate(['/tasks', newSprint.SprintID]);
        }
    };
    AppComponent.prototype.setMonths = function () {
        if (this.sprints && this.sprints.length > 0) {
            //reinitializing the array
            this.sprintMonths.length = 0;
            var _loop_1 = function(i) {
                var sprintStartDate = moment(this_1.sprints[i].StartDate);
                var sprintEndDate = moment(this_1.sprints[i].EndDate);
                var startSprint = this_1.sprintMonths.filter(function (s) { return s.displayDate == sprintStartDate.format("YYYY MMMM"); })[0];
                var endSprint = this_1.sprintMonths.filter(function (s) { return s.displayDate == sprintEndDate.format("YYYY MMMM"); })[0];
                //adding these values to the array
                if (!startSprint) {
                    var month = {};
                    month.date = moment(sprintStartDate.format("YYYY-MM") + "-01", "YYYY-MM-DD");
                    month.displayDate = sprintStartDate.format("YYYY MMMM");
                    this_1.sprintMonths.push(month);
                }
                if ((sprintStartDate.format("YYYY MM") != sprintEndDate.format("YYYY MM")) && !endSprint) {
                    var month = {};
                    month.date = moment(sprintEndDate.format("YYYY-MM") + "-01", "YYYY-MM-DD");
                    month.displayDate = sprintEndDate.format("YYYY MMMM");
                    this_1.sprintMonths.push(month);
                }
            };
            var this_1 = this;
            for (var i = 0; i < this.sprints.length; i++) {
                _loop_1(i);
            }
        }
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'pm-app',
            moduleId: module.id,
            templateUrl: 'app.component.html',
            styleUrls: ['app.component.css'],
            encapsulation: core_1.ViewEncapsulation.None
        }), 
        __metadata('design:paramtypes', [employees_service_1.EmployeesService, sprints_service_1.SprintsService, (typeof (_a = typeof router_1.ActivatedRoute !== 'undefined' && router_1.ActivatedRoute) === 'function' && _a) || Object, (typeof (_b = typeof router_1.Router !== 'undefined' && router_1.Router) === 'function' && _b) || Object])
    ], AppComponent);
    return AppComponent;
    var _a, _b;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map