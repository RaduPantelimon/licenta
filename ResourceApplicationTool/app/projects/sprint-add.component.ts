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

    //page initialization
    errorMessage: string;

    constructor(private _sprintService: SprintsService,
        private _route: ActivatedRoute,
        private _router: Router) {

    }
    ngOnInit(): void {
        try {
          
        }
        catch (ex) {
            console.log('Error while retrieving data on Init: ' + ex);
            this.errorMessage = ex.toString();
        }
    }
}