import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { DragAndDropModule } from 'angular-draggable-droppable';
import { ResizableModule } from 'angular2-resizable';

import { AppComponent }  from './app.component';
import { TasksListComponent } from './projects/tasks-list.component';
import { SprintAddComponent } from './projects/sprint-add.component';

import { SprintsService } from './projects/sprints.service';
import { EmployeesService } from './projects/employees.service';
import { TasksService } from './projects/tasks.service';

@NgModule({
    imports: [
        BrowserModule,
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
        ResizableModule
    ],
    declarations: [
        AppComponent,
        TasksListComponent,
        SprintAddComponent
    ],

  bootstrap: [AppComponent],
  providers: [
      EmployeesService,
      SprintsService,
      TasksService
  ]
})
export class AppModule { }
