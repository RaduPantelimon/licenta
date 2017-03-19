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
var platform_browser_1 = require('@angular/platform-browser');
var forms_1 = require('@angular/forms');
var common_1 = require('@angular/common');
var http_1 = require('@angular/http');
var router_1 = require('@angular/router');
var angular_draggable_droppable_1 = require('angular-draggable-droppable');
var angular2_resizable_1 = require('angular2-resizable');
var app_component_1 = require('./app.component');
var tasks_list_component_1 = require('./projects/tasks-list.component');
var sprint_add_component_1 = require('./projects/sprint-add.component');
var sprints_service_1 = require('./projects/sprints.service');
var employees_service_1 = require('./projects/employees.service');
var tasks_service_1 = require('./projects/tasks.service');
var sprint_filter_pipe_1 = require('./projects/sprint-filter.pipe');
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                http_1.HttpModule,
                forms_1.FormsModule,
                common_1.CommonModule,
                router_1.RouterModule.forRoot([
                    { path: 'tasks', component: tasks_list_component_1.TasksListComponent },
                    { path: 'tasks/:id', component: tasks_list_component_1.TasksListComponent },
                    { path: 'addsprint', component: sprint_add_component_1.SprintAddComponent },
                    { path: '', component: tasks_list_component_1.TasksListComponent },
                    { path: '**', redirectTo: 'tasks', pathMatch: 'full' }]),
                angular_draggable_droppable_1.DragAndDropModule.forRoot(),
                angular2_resizable_1.ResizableModule
            ],
            declarations: [
                app_component_1.AppComponent,
                tasks_list_component_1.TasksListComponent,
                sprint_add_component_1.SprintAddComponent,
                sprint_filter_pipe_1.SprintFilterPipe
            ],
            bootstrap: [app_component_1.AppComponent],
            providers: [
                employees_service_1.EmployeesService,
                sprints_service_1.SprintsService,
                tasks_service_1.TasksService
            ]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map