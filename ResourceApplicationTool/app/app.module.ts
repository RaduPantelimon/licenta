import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { DragAndDropModule } from 'angular-draggable-droppable';
import { ResizableModule } from 'angular2-resizable';

//import { ShContextMenuModule } from 'ng2-right-click-menu';
//import { ShContextMenuModule } from '/node_modules/ng2-right-click-menu/src/sh-context-menu.module.js';

import { Ng2SliderComponent } from "ng2-slider-component/ng2-slider.component";
import { SlideAbleDirective } from 'ng2-slideable-directive/slideable.directive';
import { Ng2StyledDirective } from 'ng2-styled-directive/ng2-styled.directive';

import { AppComponent } from './app.component';
import { TasksListComponent } from './projects/tasks-list.component';
import { SprintAddComponent } from './projects/sprint-add.component';

import { SprintsService } from './projects/sprints.service';
import { EmployeesService } from './projects/employees.service';
import { TasksService } from './projects/tasks.service';

import { SprintFilterPipe } from './projects/sprint-filter.pipe';

import { NouisliderModule } from 'ng2-nouislider';

@NgModule({
    imports: [
        BrowserModule,
        //ShContextMenuModule,
        HttpModule,
        FormsModule,
        CommonModule,
        RouterModule.forRoot([
            { path: 'tasks', component: TasksListComponent },
            { path: 'tasks/:id', component: TasksListComponent },
            { path: 'addsprint', component: SprintAddComponent },
            { path: '', component: TasksListComponent },
            { path: '**', redirectTo: 'tasks', pathMatch: 'full' }]),
        DragAndDropModule.forRoot(),
        ResizableModule,
        NouisliderModule,
        
    ],
    declarations: [
        AppComponent,
        SlideAbleDirective,
        Ng2StyledDirective,
        Ng2SliderComponent,
        TasksListComponent,
        SprintAddComponent,
        SprintFilterPipe
    ],

    bootstrap: [AppComponent],
    providers: [
        EmployeesService,
        SprintsService,
        TasksService
    ]
})
export class AppModule { }