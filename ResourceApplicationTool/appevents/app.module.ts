import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CalendarModule } from "ap-angular2-fullcalendar";


import { AppComponent }  from './app.component';

@NgModule({
    imports: [BrowserModule, CalendarModule
       ],
    declarations: [AppComponent],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
