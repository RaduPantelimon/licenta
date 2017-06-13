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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var moment = require("moment/moment");
var sprints_service_1 = require("./sprints.service");
var SprintAddComponent = (function () {
    function SprintAddComponent(_sprintService, _route, _router) {
        this._sprintService = _sprintService;
        this._route = _route;
        this._router = _router;
        this.months = [];
        //data
        this.sprints = [];
        this.weeks = [];
    }
    SprintAddComponent.prototype.ngOnInit = function () {
        var _this = this;
        try {
            //initializing variables
            this.momentWrapper = moment;
            this.selectedYear = moment().year();
            this.selectedMonth = moment().format("MMMM");
            this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
            this.months = moment.months();
            var id = window["projectID"];
            if (id && !isNaN(parseInt(id))) {
                //getting existing sprints
                this._sprintService.getSprints(id).subscribe(function (sprints) { return _this.sprints = sprints; }, function (error) { return _this.errorMessage = error; }, function () {
                    _this.sprintsLoaded = true;
                });
            }
        }
        catch (ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    };
    SprintAddComponent.prototype.saveSprint = function (event) {
        var _this = this;
        var selectedWeek = this.weeks.filter(function (s) { return s.selected; })[0];
        var pjID = window["projectID"];
        if (pjID && selectedWeek) {
            var sprint = {};
            sprint.StartDate = selectedWeek.days[0].date.format("YYYY-MM-DD");
            sprint.EndDate = selectedWeek.days[6].date.format("YYYY-MM-DD");
            sprint.ProjectID = pjID;
            this._sprintService.addSprint(sprint).subscribe(function (responseSprint) {
                _this._sprintService.registerNewlyCreateSprint(responseSprint);
                console.log(responseSprint);
            }, function (error) { return _this.errorMessage = error; });
        }
    };
    SprintAddComponent.prototype.addMonth = function (event) {
        //going to the next month
        var monthIndex = this.getMonthIndex(this.selectedMonth);
        if (monthIndex == 11)
            this.selectedYear++;
        monthIndex++;
        monthIndex = monthIndex % 12;
        this.selectedMonth = this.months[monthIndex];
        console.log("Selected values:" + this.selectedYear + " " + this.selectedMonth);
        this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
    };
    SprintAddComponent.prototype.substractMonth = function (event) {
        //going to the previous month
        var monthIndex = this.getMonthIndex(this.selectedMonth);
        if (monthIndex == 0) {
            this.selectedYear--;
            monthIndex = 11;
        }
        else {
            monthIndex--;
        }
        this.selectedMonth = this.months[monthIndex];
        console.log("Selected values:" + this.selectedYear + " " + this.selectedMonth);
        this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
    };
    SprintAddComponent.prototype.changeMonth = function (event) {
        console.log("Selected values:" + this.selectedYear + " " + this.selectedMonth);
        this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
    };
    SprintAddComponent.prototype.onChangeWeek = function (week, event) {
        var oldWeek = this.weeks.filter(function (s) { return s.selected; })[0];
        if (oldWeek)
            oldWeek.selected = false;
        week.selected = true;
        this.checkExistingStandard();
    };
    SprintAddComponent.prototype.getWeeks = function (startDateString) {
        this.existingSprint = undefined;
        this.sprintAlreadyExists = false;
        var weeks = [];
        var startDate = moment(startDateString, 'YYYY-MMMM-DD');
        var currentMonth = startDate.month();
        var currentYear = startDate.year();
        //getting to the closest monday
        while (startDate.isoWeekday() != 1) {
            startDate.subtract(1, 'd');
        }
        //adding each week to the array
        while (startDate.year() < currentYear || (startDate.month() <= currentMonth && startDate.year() == currentYear)) {
            var week = {};
            week.days = [];
            week.selected = false;
            //adding each day to the days array
            var firstDay = {};
            firstDay.date = startDate.clone();
            firstDay.month = startDate.format("MMMM");
            week.days.push(firstDay);
            for (var i = 0; i < 6; i++) {
                startDate.add(1, 'd');
                var day = {};
                day.date = startDate.clone();
                day.month = startDate.format("MMMM");
                week.days.push(day);
            }
            weeks.push(week);
            startDate.add(1, 'd');
        }
        console.log("Found: " + weeks);
        return weeks;
    };
    SprintAddComponent.prototype.getMonthIndex = function (month) {
        return this.months.indexOf(month);
    };
    SprintAddComponent.prototype.checkExistingStandard = function () {
        try {
            this.existingSprint = undefined;
            this.sprintAlreadyExists = false;
            var selectedWeek = this.weeks.filter(function (s) { return s.selected; })[0];
            if (selectedWeek) {
                //(StartA <= EndB) and (EndA >= StartB)
                var weekStart_1 = selectedWeek.days[0].date.format("YYYY-MM-DD");
                var weekEnd_1 = selectedWeek.days[6].date.format("YYYY-MM-DD");
                var existingSprint = this.sprints.filter(function (x) { return (moment(x.StartDate).format("YYYY-MM-DD") <= weekEnd_1) &&
                    (moment(x.EndDate).format("YYYY-MM-DD") >= weekStart_1); });
                if (existingSprint.length > 0) {
                    //we just found an existing sprint on the same interval
                    this.existingSprint = existingSprint[0];
                    this.sprintAlreadyExists = true;
                }
            }
        }
        catch (ex) {
            console.log("Could not determine if there are any previous sprints with the same date:" + ex);
        }
    };
    return SprintAddComponent;
}());
SprintAddComponent = __decorate([
    core_1.Component({
        selector: 'pm-app',
        moduleId: module.id,
        templateUrl: 'sprint-add.component.html',
        styleUrls: ['sprint-add.component.css'],
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [sprints_service_1.SprintsService,
        router_1.ActivatedRoute,
        router_1.Router])
], SprintAddComponent);
exports.SprintAddComponent = SprintAddComponent;
//# sourceMappingURL=sprint-add.component.js.map