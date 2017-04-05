import { Component, OnInit, ViewEncapsulation, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
/*import { ContextMenuComponent } from 'angular2-contextmenu/src/contextMenu.component';
import { ContextMenuService } from 'angular2-contextmenu/src/contextMenu.service';*/

import { ResizeEvent } from 'angular2-resizable';


import * as moment from 'moment/moment';

import { EmployeesService } from './employees.service';
import { SprintsService } from './sprints.service';
import { TasksService } from './tasks.service';


@Component({
    selector: 'pm-app',
    moduleId: module.id,
    templateUrl: 'tasks-list.component.html',
    styleUrls: ['tasks-list.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class TasksListComponent implements OnInit {

    //data
    employees: any[] = [];
    templateTasks: any[] = [];
    sprintTasks: any[] = [];
    sprints: any[] = [];
    days: any[] = [];
    dayDescriptions: any[] = [];
    _sub: any;




    //page state Info
    showTaskDetails: boolean;
    sprintsLoaded: boolean;
    employeesLoaded: boolean;
    currentSprint: any;
    currentTaskID: number;
    currentDay: any;
    errorMessage: string;
    taskDescriptionValidationText: string;
    taskDescriptionValidation: boolean;

    //drag drop resize functionality
    droppedData: string;
    draggingCorner: boolean = false;
    draggedTaskID: string;
    startRectangle: any;

    constructor(private _employeesService: EmployeesService,
        private _sprintService: SprintsService,
        private _tasksService: TasksService,
        private _route: ActivatedRoute,
        private _router: Router,
        //private contextMenuService: ContextMenuService
    )
    {

    }
    ngOnInit(): void {
        try
        {
            

            //we subscribe to the changes in order to update the input when the parameters change
            let id = window["projectID"];
            this._sub = this._route.params.subscribe((params:any) => {

                //the edit task is disabled each time we change the view
                this.showTaskDetails = false;

                this.days.length = 0;
                if (id && !isNaN(parseInt(id)))
                {
                    this.getData(parseInt(id), params);
                }
            });

            this._tasksService.getTemplateTasks().subscribe(templateTasks => this.templateTasks = templateTasks,
                error => this.errorMessage = <any>error);
        }
        catch(ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    }
    //Drag Drop Events
    dragEnd(event: DragEvent, taskID: string) {
        this.draggedTaskID = taskID;
        console.log('Element was dragged', event);
    }
    onDrop(event: DragEvent, day: any, employee: any): void {
        try {
            console.log("Using the following template: " + this.draggedTaskID);
            for (let i = 0; i < employee.Days.length; i++)
            {
                if (employee.Days[i] && employee.Days[i].date == day.date)
                {
                    //employee.Days[i].task = this.templateTasks[0];
                    let data: any = {};
                    data.templateTaskID = this.draggedTaskID;
                    data.startDate = day.date;
                    data.employeeID = employee.EmployeeID;
                    data.sprintID = this.currentSprint.SprintID;
                    this._tasksService.addTask(data).subscribe(response => {

                        console.log(response);
                        if (!response.Estimation) {
                            response.Estimation = 0;
                        }
                        employee.Days[i].task = response;
                        this.sprintTasks.push(response);
                    },
                        error => this.errorMessage = <any>error
                    );
                }
            }

        }
        catch (ex)
        {
            console.log("Could not save new task:" + ex);
        }
        

    }


    //Resizable Events
    onResizeEnd(event: ResizeEvent, resizedTask: any, empIndex: number, dayIndex: number): void {
        console.log('Element was resized', event);

        if (!this.startRectangle) return;

        let currentRectangle = event.rectangle;
        try {
            let topDiff = Math.floor((Math.abs(this.startRectangle.top - currentRectangle.top) + 3) / 45);
            let bottomDiff = Math.floor((Math.abs(this.startRectangle.bottom - currentRectangle.bottom) + 3) / 45);
            let leftDiff = Math.floor((Math.abs(this.startRectangle.left - currentRectangle.left)+3)/130);
            let rightDiff = Math.floor((Math.abs(this.startRectangle.right - currentRectangle.right) +3)/130);


            let daysStartIndex = Math.max(0, dayIndex - leftDiff);
            let daysEndIndex = Math.min(this.days.length - 1, dayIndex + rightDiff);
            let employeesStartIndex = Math.max(0, empIndex - topDiff);
            let employeesEndIndex = Math.min(this.employees.length - 1, empIndex + bottomDiff);

            for (let i = employeesStartIndex; i <= employeesEndIndex; i++)
            {
                let employee = this.employees[i];
                for (let j = daysStartIndex; j <= daysEndIndex; j++)
                {
                    let day = this.employees[i].Days[j];
                    if (!this.employees[i].Days[j].task)
                    {
                        //no task added to this element. deleteing the current one
                        console.log("Following Field is empty: (" + i + " , " + j +")");
                        //this.employees[i].FirstName + " , " + this.employees[i].Days[j].date);
                        let data: any = {};
                        data.templateTaskID = resizedTask.TaskID;
                        data.startDate = day.date;
                        data.employeeID = employee.EmployeeID;
                        data.sprintID = this.currentSprint.SprintID;

                        //adding the new task
                        this._tasksService.addTask(data).subscribe(response => {

                            console.log(response);
                            if (!response.Estimation) {
                                response.Estimation = 0;
                            }
                            day.task = response;
                            this.sprintTasks.push(response);
                        },
                            error => this.errorMessage = <any>error
                        );
                    }
                }
            }

            this.LoseFocus();

        }
        catch (ex)
        {
            console.log("Could not set new items");
        }
    }

    onResizeStart(event: ResizeEvent): void {
        this.startRectangle = event.rectangle;
        console.log('Element is getting resized', event);
    }
    onFocus(event: MouseEvent, day: any)
    {
        this.LoseFocus();
        console.log("Focusing element:" + day);
        day.focused = true;
    }
    onLoseFocus(event: MouseEvent, day: any)
    {
        console.log("Removing Focus");
        this.LoseFocus();
    }

    //will delete a task
    onDelete(event: MouseEvent, day: any, taskID:number)
    {
        this._tasksService.deleteTask(taskID).subscribe(response => {
            if (response.status)
            {
                //operation succeded
                day.task = undefined;
                console.log("Task " + taskID + " successfully removed.");
            }
            else {
                //operation failed
                console.log("Task " + taskID + " could not be removed.Error:" + response.message);
            }
        },
            error => this.errorMessage = <any>error
        );
    }
    // currentTaskID: number;
    // currentDay: any;
    // taskDescriptionValidationText: string;
    // taskDescriptionValidation: boolean;
    //will open the edit task feature
    onEdit(event: MouseEvent, day: any, taskID: number)
    {
        //initializing variables
        this.currentTaskID = taskID;
        this.currentDay = day;
        this.showTaskDetails = true;
        this.taskDescriptionValidation = false;
        this.taskDescriptionValidationText = "";
    }
   
    //used to save the taskID
    saveValues(event: MouseEvent, day: any, taskID: number)
    {
        if (!this.currentDay.task.TaskDescription || this.currentDay.task.TaskDescription.trim() == "")
        {
            this.taskDescriptionValidation = true;
            this.taskDescriptionValidationText = "The Title cannot be empty";
            return;
        }
        this.taskDescriptionValidation = false;

        this._tasksService.updateTask(this.currentDay.task).subscribe(response => {
            if (response.status) {
                //operation succeded
                console.log("Task " + taskID + " successfully updated.");
            }
            else {
                //operation failed
                console.log("Task " + taskID + " could not be upodated.Error:" + response.message);
            }
            this.cancelEdit();
        },
            error => this.errorMessage = <any>error
        );
        console.log(day);
    }
    cancelEdit()
    {
        //deinitializing variables
        this.currentTaskID = null;
        this.currentDay = null;
        this.showTaskDetails = false;
        this.taskDescriptionValidation = false;
        this.taskDescriptionValidationText = "";
    }
    validate(event: ResizeEvent): boolean {
        const MIN_DIMENSIONS_PX: number = 50;
        

        return true;
    }
    private LoseFocus(): any
    {
        for (let employee of this.employees) {
            for (let day of employee.Days) {
                day.focused = false;
            }
        }
    }
    //Table initialization
    private getData(id: number, params:any): void {
        try
        {
            if (id)
            {
                console.log('Test tasks');
                let sprintID = params['id'];
                this.employeesLoaded = false;
                this.sprintsLoaded = false;

                this._employeesService.getEmployees(id).subscribe(employees => this.employees = employees,
                    error => this.errorMessage = <any>error, () => {

                        this.employeesLoaded = true;
                        //we will retrieve the tasks once we processed the sprints and the employees
                        if (this.employeesLoaded && this.sprintsLoaded && this.currentSprint)
                        {
                            //after we obtained the employees we can get the tasks
                            this._tasksService.getSprintTasks(this.currentSprint.SprintID).subscribe(tasks => this.setTasks(tasks),
                                error => this.errorMessage = <any>error
                            );
                        }
                    });
                this._sprintService.getSprints(id).subscribe(sprints => this.sprints = sprints,
                    error => this.errorMessage = <any>error, () => {
                        this.sprintsLoaded = true;
                        this.setDays(sprintID)
                        //we will retrieve the tasks once we processed the sprints and the employees
                        if (this.employeesLoaded && this.sprintsLoaded && this.currentSprint) {
                            //after we obtained the employees we can get the tasks
                            this._tasksService.getSprintTasks(this.currentSprint.SprintID).subscribe(tasks => this.setTasks(tasks),
                                error => this.errorMessage = <any>error
                              
                            );
                        }
                        
                    });
            }
        }
        catch (ex) {
            console.log('Error while retrieving data: ' + ex);
            this.errorMessage = ex.toString();
        }
    }
    private setDays(sprintID: string)
    {
        let currentSprintArray = this.sprints.filter(x => x.SprintID == sprintID);
        
        if (currentSprintArray.length > 0) {
            this.currentSprint = currentSprintArray[0];
        }
        else if (!sprintID && this.sprints.length > 0) {
            this.currentSprint = this.sprints[0];
        }
        else return;

        //once we found our sprint/ we start initializing the table
        let startDate = moment(this.currentSprint.StartDate);
        let endDate = moment(this.currentSprint.EndDate);

        while (startDate <= endDate)
        {
            this.days.push(moment(startDate));
            startDate = startDate.add(1, 'days');
        }
       


        console.log(this.currentSprint.SprintID);
    }
    private setTasks(tasks: any[])
    {
        this.sprintTasks = tasks;


        for (let i= 0; i < this.sprintTasks.length;i++)
        {
            if (!this.sprintTasks[i].Estimation)
            {
                this.sprintTasks[i].Estimation = 0;
            }
        }

        //finding the tasks assigned to each employee
        for (let employee of this.employees)
        {
            let employeeDays: any[] = [];
            for (let day of this.days)
            {
                let pjDay: any = {};
                pjDay.date = day.format("YYYY-MM-DDTHH:mm:ss");
                pjDay.focused = false;
                let currentTasksArray = tasks.filter(x =>
                        (x.EmployeeID == employee.EmployeeID &&
                        x.StartDate == pjDay.date));
                if (currentTasksArray.length > 0)
                {
                    pjDay.task = currentTasksArray[0];
                    console.log("here");
                }

                employeeDays.push(pjDay);
            }
            employee.Days = employeeDays;
        }

    }
}

