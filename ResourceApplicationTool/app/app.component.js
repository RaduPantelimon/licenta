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
var AppComponent = (function () {
    function AppComponent(_employeesService, _sprintService, _route, _router) {
        this._employeesService = _employeesService;
        this._sprintService = _sprintService;
        this._route = _route;
        this._router = _router;
        //employees: any[] = [];
        this.sprints = [];
    }
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        try {
            var id = window["projectID"];
            if (id && !isNaN(parseInt(id))) {
                console.log('Test');
                var currentComponent_1 = this;
                //getting the sprint id from the url
                var urlComponents = location.href.split("/");
                var spid_1 = parseInt(urlComponents[urlComponents.length - 1]);
                //console.log(this._route.snapshot.params['id']);
                this._route.params.subscribe(function (params) {
                    var userId = params['id'];
                    console.log(userId);
                });
                this._sprintService.getSprints(id).subscribe(function (sprints) {
                    if (location.href.indexOf("addsprint") != -1) {
                        //redirected to the addsprints page
                        currentComponent_1.createSprintEnabled = true;
                    }
                    currentComponent_1.sprints = sprints.map(function (val, index) {
                        if ((spid_1 && spid_1 == val.SprintID) || ((!spid_1 || isNaN(spid_1) || location.href.indexOf("task") == -1) && index == 0)) {
                            val.selected = true;
                        }
                        else {
                            val.selected = false;
                        }
                        return val;
                    });
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
    AppComponent = __decorate([
        core_1.Component({
            selector: 'pm-app',
            moduleId: module.id,
            templateUrl: 'app.component.html',
            styleUrls: ['app.component.css'],
            encapsulation: core_1.ViewEncapsulation.None
        }), 
        __metadata('design:paramtypes', [employees_service_1.EmployeesService, sprints_service_1.SprintsService, router_1.ActivatedRoute, router_1.Router])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map