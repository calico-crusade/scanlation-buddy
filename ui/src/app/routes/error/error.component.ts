import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { queryParam } from 'src/app/services';

@Component({
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss']
})
export class ErrorComponent {

    error$ = queryParam(this.route, 'error');

    constructor(
        private route: ActivatedRoute
    ) { }
}
