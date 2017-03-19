import { PipeTransform, Pipe } from '@angular/core';

import * as moment from 'moment/moment';

@Pipe({
    name: 'sprintFilter'
})
export class SprintFilterPipe implements PipeTransform {

    transform(value: any[], filterByMonth: any): any[] {

        console.log(filterByMonth);
        return filterByMonth ? value.filter(sprint =>
            (moment(sprint.StartDate).format("YYYY MMMM") == filterByMonth) ||
            (moment(sprint.EndDate).format("YYYY MMMM") == filterByMonth)) : value;
    }
}
