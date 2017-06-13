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
var employees_service_1 = require("./employees.service");
var sprints_service_1 = require("./sprints.service");
var tasks_service_1 = require("./tasks.service");
var TasksListComponent = (function () {
    function TasksListComponent(_employeesService, _sprintService, _tasksService, _route, _router) {
        this._employeesService = _employeesService;
        this._sprintService = _sprintService;
        this._tasksService = _tasksService;
        this._route = _route;
        this._router = _router;
        //data
        this.employees = [];
        this.templateTasks = [];
        this.sprintTasks = [];
        this.sprints = [];
        this.days = [];
        this.dayDescriptions = [];
        this.draggingCorner = false;
    }
    TasksListComponent.prototype.ngOnInit = function () {
        var _this = this;
        try {
            //we subscribe to the changes in order to update the input when the parameters change
            var id_1 = window["projectID"];
            this.permissions = window["accessLevel"];
            this._sub = this._route.params.subscribe(function (params) {
                //the edit task is disabled each time we change the view
                _this.showTaskDetails = false;
                _this.days.length = 0;
                if (id_1 && !isNaN(parseInt(id_1))) {
                    _this.getData(parseInt(id_1), params);
                }
            });
            this._tasksService.getTemplateTasks().subscribe(function (templateTasks) { return _this.templateTasks = templateTasks; }, function (error) { return _this.errorMessage = error; });
        }
        catch (ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    };
    //Drag Drop Events
    TasksListComponent.prototype.dragEnd = function (event, taskID) {
        this.draggedTaskID = taskID;
        console.log('Element was dragged', event);
    };
    TasksListComponent.prototype.onDrop = function (event, day, employee) {
        var _this = this;
        try {
            console.log("Using the following template: " + this.draggedTaskID);
            var _loop_1 = function (i) {
                if (employee.Days[i] && employee.Days[i].date == day.date) {
                    //initialize the data sent to the server
                    var data = {};
                    data.templateTaskID = this_1.draggedTaskID;
                    data.startDate = day.date;
                    data.employeeID = employee.EmployeeID;
                    data.sprintID = this_1.currentSprint.SprintID;
                    this_1._tasksService.addTask(data).subscribe(function (response) {
                        if (!response.Estimation) {
                            response.Estimation = 0;
                        }
                        //we add the new task to the day object
                        employee.Days[i].task = response;
                        _this.sprintTasks.push(response);
                    }, function (error) { return _this.errorMessage = error; });
                }
            };
            var this_1 = this;
            for (var i = 0; i < employee.Days.length; i++) {
                _loop_1(i);
            }
        }
        catch (ex) {
            console.log("Could not save new task:" + ex);
        }
    };
    //Resizable Events
    TasksListComponent.prototype.onResizeEnd = function (event, resizedTask, empIndex, dayIndex) {
        var _this = this;
        console.log('Element was resized', event);
        if (!this.startRectangle)
            return;
        var currentRectangle = event.rectangle;
        try {
            var topDiff = Math.floor((Math.abs(this.startRectangle.top - currentRectangle.top) + 3) / 45);
            var bottomDiff = Math.floor((Math.abs(this.startRectangle.bottom - currentRectangle.bottom) + 3) / 45);
            var leftDiff = Math.floor((Math.abs(this.startRectangle.left - currentRectangle.left) + 3) / 130);
            var rightDiff = Math.floor((Math.abs(this.startRectangle.right - currentRectangle.right) + 3) / 130);
            var daysStartIndex = Math.max(0, dayIndex - leftDiff);
            var daysEndIndex = Math.min(this.days.length - 1, dayIndex + rightDiff);
            var employeesStartIndex = Math.max(0, empIndex - topDiff);
            var employeesEndIndex = Math.min(this.employees.length - 1, empIndex + bottomDiff);
            for (var i = employeesStartIndex; i <= employeesEndIndex; i++) {
                var employee = this.employees[i];
                var _loop_2 = function (j) {
                    var day = this_2.employees[i].Days[j];
                    if (!this_2.employees[i].Days[j].task) {
                        //no task added to this element. deleteing the current one
                        console.log("Following Field is empty: (" + i + " , " + j + ")");
                        var data = {};
                        if (resizedTask.TemplateID) {
                            data.templateTaskID = resizedTask.TemplateID;
                        }
                        else {
                            data.templateTaskID = resizedTask.TaskID;
                        }
                        data.startDate = day.date;
                        data.employeeID = employee.EmployeeID;
                        data.sprintID = this_2.currentSprint.SprintID;
                        data.duration = resizedTask.Estimation;
                        data.directDescendant = resizedTask.TaskID;
                        //adding the new task
                        this_2._tasksService.addTask(data).subscribe(function (response) {
                            console.log(response);
                            if (!response.Estimation) {
                                response.Estimation = 0;
                            }
                            day.task = response;
                            _this.sprintTasks.push(response);
                        }, function (error) { return _this.errorMessage = error; });
                    }
                };
                var this_2 = this;
                for (var j = daysStartIndex; j <= daysEndIndex; j++) {
                    _loop_2(j);
                }
            }
            this.LoseFocus();
        }
        catch (ex) {
            console.log("Could not set new items");
        }
    };
    TasksListComponent.prototype.onResizeStart = function (event) {
        this.startRectangle = event.rectangle;
        console.log('Element is getting resized', event);
    };
    TasksListComponent.prototype.onFocus = function (event, day) {
        this.LoseFocus();
        console.log("Focusing element:" + day);
        day.focused = true;
    };
    TasksListComponent.prototype.onLoseFocus = function (event, day) {
        console.log("Removing Focus");
        this.LoseFocus();
    };
    //will delete a task
    TasksListComponent.prototype.onDelete = function (event, day, taskID) {
        var _this = this;
        this._tasksService.deleteTask(taskID).subscribe(function (response) {
            if (response.status) {
                //operation succeded
                day.task = undefined;
                console.log("Task " + taskID + " successfully removed.");
            }
            else {
                //operation failed
                console.log("Task " + taskID + " could not be removed.Error:" + response.message);
            }
        }, function (error) { return _this.errorMessage = error; });
    };
    // currentTaskID: number;
    // currentDay: any;
    // taskDescriptionValidationText: string;
    // taskDescriptionValidation: boolean;
    //will open the edit task feature
    TasksListComponent.prototype.onEdit = function (event, day, taskID) {
        //initializing variables
        this.currentTaskID = taskID;
        this.currentDay = day;
        this.showTaskDetails = true;
        this.taskDescriptionValidation = false;
        this.taskDescriptionValidationText = "";
    };
    //used to save the taskID
    TasksListComponent.prototype.saveValues = function (event, day, taskID) {
        var _this = this;
        if (!this.currentDay.task.TaskDescription || this.currentDay.task.TaskDescription.trim() == "") {
            this.taskDescriptionValidation = true;
            this.taskDescriptionValidationText = "The Title cannot be empty";
            return;
        }
        this.taskDescriptionValidation = false;
        this._tasksService.updateTask(this.currentDay.task).subscribe(function (response) {
            if (response.status) {
                //operation succeded
                console.log("Task " + taskID + " successfully updated.");
            }
            else {
                //operation failed
                console.log("Task " + taskID + " could not be upodated.Error:" + response.message);
            }
            _this.cancelEdit();
        }, function (error) { return _this.errorMessage = error; });
        console.log(day);
    };
    TasksListComponent.prototype.cancelEdit = function () {
        //deinitializing variables
        this.currentTaskID = null;
        this.currentDay = null;
        this.showTaskDetails = false;
        this.taskDescriptionValidation = false;
        this.taskDescriptionValidationText = "";
    };
    TasksListComponent.prototype.validate = function (event) {
        var MIN_DIMENSIONS_PX = 50;
        return true;
    };
    TasksListComponent.prototype.LoseFocus = function () {
        for (var _i = 0, _a = this.employees; _i < _a.length; _i++) {
            var employee = _a[_i];
            for (var _b = 0, _c = employee.Days; _b < _c.length; _b++) {
                var day = _c[_b];
                day.focused = false;
            }
        }
    };
    //Table initialization
    TasksListComponent.prototype.getData = function (id, params) {
        var _this = this;
        try {
            if (id) {
                console.log('Test tasks');
                var sprintID_1 = params['id'];
                this.employeesLoaded = false;
                this.sprintsLoaded = false;
                this._employeesService.getEmployees(id).subscribe(function (employees) { return _this.employees = employees; }, function (error) { return _this.errorMessage = error; }, function () {
                    _this.employeesLoaded = true;
                    //we will retrieve the tasks once we processed the sprints and the employees
                    if (_this.employeesLoaded && _this.sprintsLoaded && _this.currentSprint) {
                        //after we obtained the employees we can get the tasks
                        _this._tasksService.getSprintTasks(_this.currentSprint.SprintID).subscribe(function (tasks) { return _this.setTasks(tasks); }, function (error) { return _this.errorMessage = error; });
                    }
                });
                this._sprintService.getSprints(id).subscribe(function (sprints) { return _this.sprints = sprints; }, function (error) { return _this.errorMessage = error; }, function () {
                    _this.sprintsLoaded = true;
                    _this.setDays(sprintID_1);
                    //we will retrieve the tasks once we processed the sprints and the employees
                    if (_this.employeesLoaded && _this.sprintsLoaded && _this.currentSprint) {
                        //after we obtained the employees we can get the tasks
                        _this._tasksService.getSprintTasks(_this.currentSprint.SprintID).subscribe(function (tasks) { return _this.setTasks(tasks); }, function (error) { return _this.errorMessage = error; });
                    }
                });
            }
        }
        catch (ex) {
            console.log('Error while retrieving data: ' + ex);
            this.errorMessage = ex.toString();
        }
    };
    TasksListComponent.prototype.setDays = function (sprintID) {
        var currentSprintArray = this.sprints.filter(function (x) { return x.SprintID == sprintID; });
        if (currentSprintArray.length > 0) {
            this.currentSprint = currentSprintArray[0];
        }
        else if (!sprintID && this.sprints.length > 0) {
            this.currentSprint = this.sprints[0];
        }
        else
            return;
        //once we found our sprint/ we start initializing the table
        var startDate = moment(this.currentSprint.StartDate);
        var endDate = moment(this.currentSprint.EndDate);
        while (startDate <= endDate) {
            this.days.push(moment(startDate));
            startDate = startDate.add(1, 'days');
        }
        console.log(this.currentSprint.SprintID);
    };
    TasksListComponent.prototype.setTasks = function (tasks) {
        this.sprintTasks = tasks;
        for (var i = 0; i < this.sprintTasks.length; i++) {
            if (!this.sprintTasks[i].Estimation) {
                this.sprintTasks[i].Estimation = 0;
            }
        }
        var _loop_3 = function (employee) {
            var employeeDays = [];
            var _loop_4 = function (day) {
                var pjDay = {};
                pjDay.date = day.format("YYYY-MM-DDTHH:mm:ss");
                pjDay.focused = false;
                var currentTasksArray = tasks.filter(function (x) {
                    return (x.EmployeeID == employee.EmployeeID &&
                        x.StartDate == pjDay.date);
                });
                if (currentTasksArray.length > 0) {
                    pjDay.task = currentTasksArray[0];
                    console.log("here");
                }
                employeeDays.push(pjDay);
            };
            for (var _i = 0, _a = this_3.days; _i < _a.length; _i++) {
                var day = _a[_i];
                _loop_4(day);
            }
            employee.Days = employeeDays;
        };
        var this_3 = this;
        //finding the tasks assigned to each employee
        for (var _i = 0, _a = this.employees; _i < _a.length; _i++) {
            var employee = _a[_i];
            _loop_3(employee);
        }
    };
    return TasksListComponent;
}());
TasksListComponent = __decorate([
    core_1.Component({
        selector: 'pm-app',
        moduleId: module.id,
        templateUrl: 'tasks-list.component.html',
        styleUrls: ['tasks-list.component.css'],
        encapsulation: core_1.ViewEncapsulation.None
    }),
    __metadata("design:paramtypes", [employees_service_1.EmployeesService,
        sprints_service_1.SprintsService,
        tasks_service_1.TasksService,
        router_1.ActivatedRoute,
        router_1.Router])
], TasksListComponent);
exports.TasksListComponent = TasksListComponent;
//# sourceMappingURL=tasks-list.component.js.map