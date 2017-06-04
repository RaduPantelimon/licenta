import { Component, OnInit, ViewEncapsulation, HostListener} from '@angular/core';

import { ActivatedRoute, Router, Params } from '@angular/router';

import { SprintsService } from './projects/sprints.service';
import { EmployeesService } from './projects/employees.service';

import * as moment from 'moment/moment';

@Component({
    selector: 'pm-app',
    moduleId: module.id,
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css'],
    encapsulation: ViewEncapsulation.None

})
export class AppComponent implements OnInit{

    //page initialization
    createSprintEnabled: boolean;
    sprintMonths: any[] = [];
    selectedMonth: any;
    //employees: any[] = [];
    sprints: any[] = [];
    errorMessage: string;
    permissions: string;

    constructor(private _employeesService: EmployeesService,
        private _sprintService: SprintsService,
        private _route: ActivatedRoute,
        private _router: Router)
    {

    }
    ngOnInit(): void {
        try {

            let id = window["projectID"];
            this.permissions = window["accessLevel"];

            if (id && !isNaN(parseInt(id)))
            {
                let currentComponent = this;

                //getting the sprint id from the url
                let urlComponents: string[] = location.href.split("/");
                let spid: number = parseInt(urlComponents[urlComponents.length - 1]);

                //initializing the Sprints Page with the results from the server
                this._sprintService.getSprints(id).subscribe(function (sprints: any[]) {

                    if (location.href.indexOf("addsprint") != -1)
                    {
                        //redirected to the addsprints page
                        currentComponent.createSprintEnabled = true;
                    }

                    currentComponent.sprints = sprints.map(function (val, index)
                    {
                        //spid represents the url of the current sprint, retrieved from the URL
                        if ((spid && spid == val.SprintID) ||
                            ((!spid || isNaN(spid) || location.href.indexOf("task") == -1) && index == 0))
                        {
                            val.selected = true;
                            currentComponent.selectedMonth = moment(val.StartDate).format("YYYY MMMM");
                        }
                        else {
                            val.selected = false;
                        }
                        return val;

                    });
                    currentComponent.setMonths();
                },
                    error => this.errorMessage = <any>error);

                //waiting for newly added sprints
                this._sprintService.newSprints.subscribe(newSprint => {
                    //reObtaining the Sprints from the DataBase
                    let indices = 0;

                    this._sprintService.getSprints(id).subscribe(sprints => {

                        this.sprints = sprints.map(function (val, index) {

                            if ((newSprint.SprintID == val.SprintID)) {
                                val.selected = true;
                                currentComponent.selectedMonth = moment(val.StartDate).format("YYYY MMMM");
                            }
                            else {
                                val.selected = false;
                            }
                            return val;

                        });
                        currentComponent.setMonths();
                    }, error => this.errorMessage = <any>error);

                    if (newSprint.SprintID)
                    {
                        //navigating to our sprint
                        this._router.navigate(['/tasks', newSprint.SprintID]);

                        this.createSprintEnabled = false;
                    }
                    
                    
                }, error => this.errorMessage = <any>error);
            }
        }
        catch (ex)
        {
            console.log('Error while retrieving data ' + ex);
            this.errorMessage = ex.toString();
        }
        
       
    }

    onTest(): void {
        //console.log(this.employees);
        console.log(this.sprints);
        this.sprints[1].selected = true;
    }
    onChangeSprint(originalSprint: any, event: MouseEvent): void{
        this.createSprintEnabled = false;
        let sprint = this.sprints.filter(s => s.selected)[0];
        if(sprint)sprint.selected = false;
        originalSprint.selected = true;

        //navigating away
        //this._router.navigate(['/products']);
    }
    onChangeToSprintCreation(event: MouseEvent): void {
        this.createSprintEnabled = true;
    }

    changeMonth(event: MouseEvent): void {

        //auto navigating to the first sprint of the week
        let foundElement: boolean = false;
        let newSprint: any;
        if (this.sprints && this.sprints.length > 0) {
            for (let i = 0; i < this.sprints.length; i++)
            {
                let currentSprint = this.sprints[i];
                currentSprint.selected = false;
                if ((this.selectedMonth == moment(currentSprint.StartDate).format("YYYY MMMM") ||
                    this.selectedMonth == moment(currentSprint.EndDate).format("YYYY MMMM")) && !foundElement ){

                    foundElement = true;
                    currentSprint.selected = true;
                    newSprint = currentSprint;
                }
            }
            this._router.navigate(['/tasks', newSprint.SprintID]);
        }
    }

    //used to change the month input when we perform a route from the code
    private changeMonthInput():void
    {
        for (let i = 0; i < this.sprints.length; i++) {
            let currentSprint = this.sprints[i];
            if (currentSprint.selected == true) {
                this.selectedMonth = moment(currentSprint.StartDate).format("YYYY MMMM");
            }
        }
    }

    private setMonths() {
        if (this.sprints && this.sprints.length > 0)
        {
            //reinitializing the array
            this.sprintMonths.length = 0;
            for (let i = 0; i < this.sprints.length; i++)
            {
                let sprintStartDate = moment(this.sprints[i].StartDate);
                let sprintEndDate = moment(this.sprints[i].EndDate)
                let startSprint = this.sprintMonths.filter(s => s.displayDate == sprintStartDate.format("YYYY MMMM"))[0];
                let endSprint = this.sprintMonths.filter(s => s.displayDate == sprintEndDate.format("YYYY MMMM"))[0];

                //adding these values to the array
                if (!startSprint)
                {
                    let month: any = {};
                    month.date = moment(sprintStartDate.format("YYYY-MM") + "-01", "YYYY-MM-DD");
                    month.displayDate = sprintStartDate.format("YYYY MMMM");
                    this.sprintMonths.push(month);
                }
                if ((sprintStartDate.format("YYYY MM") != sprintEndDate.format("YYYY MM")) && !endSprint)
                {
                    let month: any = {};
                    month.date = moment(sprintEndDate.format("YYYY-MM") + "-01", "YYYY-MM-DD");
                    month.displayDate = sprintEndDate.format("YYYY MMMM");
                    this.sprintMonths.push(month);
                }
            }
            //this.selectedMonth = this.sprintMonths[0].displayDate;
        }
    }

    public removeDate(date:string):void
    {
        let sprintMonthIndex: number = -1; 

        for (let i = 0; i < this.sprintMonths.length; i++)
        {
            if (date = this.sprintMonths[i].displayDate)
            {
                sprintMonthIndex = i;
            }
        }

        if (sprintMonthIndex >= 0)
        {
            this.sprintMonths.splice(sprintMonthIndex, 1);
        }
    }

    public deleteCurrentSprint(event: MouseEvent)
    {
        let selectedSprintIndex: number = -1;
        var selectedSprint:any;
        for (let i = 0; i < this.sprints.length; i++) {
            let currentSprint = this.sprints[i];
            if (currentSprint.selected == true)
            {
                selectedSprint = currentSprint;
                selectedSprintIndex = i;
            }
        }

        if (selectedSprint)
        {
           this.createSprintEnabled = false;
           selectedSprint.selected = false;

           this._sprintService.deleteSprint(selectedSprint.SprintID).subscribe(response => {
               if (response.status) {

                   //operation succeded
                   this.sprints.splice(selectedSprintIndex, 1);

                   //checking if we need to delete the month values from the month selector
                   let sprintStartDate = moment(selectedSprint.StartDate).format("YYYY MMMM");
                   let sprintEndDate = moment(selectedSprint.EndDate).format("YYYY MMMM");

                   let startDateExists = false;
                   let endDateExists = false;

                   //let startSprint = this.sprintMonths.filter(s => s.displayDate == sprintStartDate.format("YYYY MMMM"))[0];
                   for (let i = 0; i < this.sprints.length; i++) {
                       let currentSprint = this.sprints[i];
                       if (moment(currentSprint.StartDate).format("YYYY MMMM") == sprintStartDate) {
                           startDateExists = true;
                       }
                       if (moment(currentSprint.EndDate).format("YYYY MMMM") == sprintEndDate) {
                           endDateExists = true;
                       }
                   }
                   if (!startDateExists) this.removeDate(sprintStartDate);
                   if (!endDateExists) this.removeDate(sprintEndDate);
                  

                   if (this.sprints.length > 0)
                   {
                       let defaultSprint = this.sprints[0];
                       defaultSprint.selected = true;
                       this.changeMonthInput();
                       this._router.navigate(['/tasks']);
                   }
               }
               else {
                   //operation failed
                   console.log("Task " + selectedSprint.SprintID + " could not be removed.Error:" + response.message);
               }
           },
               error => this.errorMessage = <any>error
           );
        }
    }
}
