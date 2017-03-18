import { Component, OnInit, ViewEncapsulation, HostListener} from '@angular/core';

import { ActivatedRoute, Router, Params } from '@angular/router';

import { SprintsService } from './projects/sprints.service';
import { EmployeesService } from './projects/employees.service';

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

    //employees: any[] = [];
    sprints: any[] = [];
    errorMessage: string;

    constructor(private _employeesService: EmployeesService,
        private _sprintService: SprintsService,
        private _route: ActivatedRoute,
        private _router: Router)
    {

    }
    ngOnInit(): void {
        try {

            let id = window["projectID"];

            if (id && !isNaN(parseInt(id)))
            {
                console.log('Test');
                let currentComponent = this;

                //getting the sprint id from the url
                let urlComponents: string[] = location.href.split("/");
                let spid: number = parseInt(urlComponents[urlComponents.length - 1]);

                //console.log(this._route.snapshot.params['id']);

                this._route.params.subscribe((params: Params) => {
                    let userId = params['id'];
                    console.log(userId);
                });

                this._sprintService.getSprints(id).subscribe(function (sprints: any[]) {

                    if (location.href.indexOf("addsprint") != -1)
                    {
                        //redirected to the addsprints page
                        currentComponent.createSprintEnabled = true;
                    }
                    currentComponent.sprints = sprints.map(function (val, index)
                    {

                        if ((spid && spid == val.SprintID) || ((!spid || isNaN(spid) || location.href.indexOf("task") == -1) && index == 0))
                        {
                            val.selected = true;
                        }
                        else {
                            val.selected = false;
                        }
                        return val;

                    });
                },
                    error => this.errorMessage = <any>error);
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

}
