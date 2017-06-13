"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var moment = require("moment/moment");
var SprintFilterPipe = (function () {
    function SprintFilterPipe() {
    }
    SprintFilterPipe.prototype.transform = function (value, filterByMonth) {
        console.log(filterByMonth);
        return filterByMonth ? value.filter(function (sprint) {
            return (moment(sprint.StartDate).format("YYYY MMMM") == filterByMonth) ||
                (moment(sprint.EndDate).format("YYYY MMMM") == filterByMonth);
        }) : value;
    };
    return SprintFilterPipe;
}());
SprintFilterPipe = __decorate([
    core_1.Pipe({
        name: 'sprintFilter'
    })
], SprintFilterPipe);
exports.SprintFilterPipe = SprintFilterPipe;
//# sourceMappingURL=sprint-filter.pipe.js.map