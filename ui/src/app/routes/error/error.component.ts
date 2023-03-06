import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { combineLatest, forkJoin, map } from 'rxjs';
import { queryParam } from 'src/app/services';

@Component({
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss']
})
export class ErrorComponent {

    error$ = queryParam(this.route, 'error');

    constructor(
        private route: ActivatedRoute,
        private router: Router
    ) { }
}
