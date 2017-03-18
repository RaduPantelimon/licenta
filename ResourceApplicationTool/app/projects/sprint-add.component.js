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
var sprints_service_1 = require('./sprints.service');
var SprintAddComponent = (function () {
    function SprintAddComponent(_sprintService, _route, _router) {
        this._sprintService = _sprintService;
        this._route = _route;
        this._router = _router;
    }
    SprintAddComponent.prototype.ngOnInit = function () {
        try {
        }
        catch (ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    };
    SprintAddComponent = __decorate([
        core_1.Component({
            selector: 'pm-app',
            moduleId: module.id,
            templateUrl: 'sprint-add.component.html',
            styleUrls: ['sprint-add.component.css'],
            encapsulation: core_1.ViewEncapsulation.None
        }), 
        __metadata('design:paramtypes', [sprints_service_1.SprintsService, router_1.ActivatedRoute, router_1.Router])
    ], SprintAddComponent);
    return SprintAddComponent;
}());
exports.SprintAddComponent = SprintAddComponent;
//# sourceMappingURL=sprint-add.component.js.map