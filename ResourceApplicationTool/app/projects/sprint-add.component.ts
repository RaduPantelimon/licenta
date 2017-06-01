import { Component, OnInit, ViewEncapsulation, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import * as moment from 'moment/moment';


import { SprintsService } from './sprints.service';



@Component({
    selector: 'pm-app',
    moduleId: module.id,
    templateUrl: 'sprint-add.component.html',
    styleUrls: ['sprint-add.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class SprintAddComponent implements OnInit {

    months: any[] = [];


    //page initialization
    errorMessage: string;
    sprintAlreadyExists: boolean;
    existingSprint : any;
    momentWrapper: any;

    //data
    sprints: any[] = [];
    
    weeks: any[] = [];
    selectedMonth: any;
    sprintsLoaded: boolean;
    selectedYear: number;

    constructor(private _sprintService: SprintsService,
        private _route: ActivatedRoute,
        private _router: Router) {

    }
    ngOnInit(): void {
        try {

            //initializing variables
            this.momentWrapper = moment;
            this.selectedYear = moment().year();
            this.selectedMonth = moment().format("MMMM");
            this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
            this.months = moment.months();

            let id = window["projectID"];
            if (id && !isNaN(parseInt(id)))
            {
                //getting existing sprints
                this._sprintService.getSprints(id).subscribe(sprints => this.sprints = sprints,
                    error => this.errorMessage = <any>error, () => {
                        this.sprintsLoaded = true;
                    });
            }

        }
        catch (ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    }
    saveSprint(event: MouseEvent) {
        let selectedWeek = this.weeks.filter(s => s.selected)[0];
        let pjID = window["projectID"];
        if (pjID && selectedWeek) {
            let sprint: any = {};
            sprint.StartDate = selectedWeek.days[0].date.format("YYYY-MM-DD");
            sprint.EndDate = selectedWeek.days[6].date.format("YYYY-MM-DD");
            sprint.ProjectID = pjID;
            this._sprintService.addSprint(sprint).subscribe(responseSprint => {

                this._sprintService.registerNewlyCreateSprint(responseSprint);
                console.log(responseSprint);
            },
                error => this.errorMessage = <any>error
            );
        }
    }
    addMonth(event: MouseEvent) {
        //going to the next month
        let monthIndex = this.getMonthIndex(this.selectedMonth);
        if (monthIndex == 11) this.selectedYear++;
        monthIndex++;
        monthIndex = monthIndex % 12;

        this.selectedMonth = this.months[monthIndex];
        console.log("Selected values:" + this.selectedYear + " " + this.selectedMonth);
        this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
    }
    substractMonth(event: MouseEvent) {
        //going to the previous month
        let monthIndex = this.getMonthIndex(this.selectedMonth);
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
    }
    changeMonth(event: MouseEvent): void{
        console.log("Selected values:" + this.selectedYear + " " + this.selectedMonth);
        this.weeks = this.getWeeks(this.selectedYear + "-" + this.selectedMonth + "-" + "01");
    }
    onChangeWeek(week: any, event: MouseEvent):void {
        
        let oldWeek = this.weeks.filter(s => s.selected)[0];
        if (oldWeek) oldWeek.selected = false;
        week.selected = true;
        this.checkExistingStandard();
    }
    private getWeeks(startDateString: string): any{

        this.existingSprint = undefined;
        this.sprintAlreadyExists = false;

        let weeks: any[] = [];
        let startDate = moment(startDateString, 'YYYY-MMMM-DD');
        let currentMonth = startDate.month();
        let currentYear = startDate.year();
        //getting to the closest monday
        while (startDate.isoWeekday() != 1)
        {
            startDate.subtract(1, 'd');
        }
        //adding each week to the array
        while (startDate.year() < currentYear || (startDate.month() <= currentMonth && startDate.year() == currentYear ))
        {
            let week: any = {};
            week.days = [];
            week.selected = false;
            //adding each day to the days array
            let firstDay: any = {};
            firstDay.date = startDate.clone();
            firstDay.month = startDate.format("MMMM");
            week.days.push(firstDay);

            for (let i = 0; i < 6; i++)
            {
                startDate.add(1, 'd');
                let day : any = {};
                day.date = startDate.clone();
                day.month = startDate.format("MMMM");
                
                week.days.push(day);
            }
            weeks.push(week);
            startDate.add(1, 'd');
        }
        console.log("Found: " + weeks);
        return weeks;
    }

    private getMonthIndex(month: string): number
    {
        return this.months.indexOf(month);
    }
    private checkExistingStandard(): void {
        try {
            this.existingSprint = undefined;
            this.sprintAlreadyExists = false;
            let selectedWeek = this.weeks.filter(s => s.selected)[0];
            if (selectedWeek) {

                //(StartA <= EndB) and (EndA >= StartB)

                let weekStart = selectedWeek.days[0].date.format("YYYY-MM-DD");
                let weekEnd = selectedWeek.days[6].date.format("YYYY-MM-DD");

                let existingSprint = this.sprints.filter(x => (moment(x.StartDate).format("YYYY-MM-DD") <= weekEnd) &&
                    (moment(x.EndDate).format("YYYY-MM-DD") >= weekStart));

                if (existingSprint.length > 0) {
                    //we just found an existing sprint on the same interval
                    this.existingSprint = existingSprint[0];
                    this.sprintAlreadyExists = true;
                }
            }
        }
        catch (ex)
        {
            console.log("Could not determine if there are any previous sprints with the same date:" + ex);
        }
       
    }
}