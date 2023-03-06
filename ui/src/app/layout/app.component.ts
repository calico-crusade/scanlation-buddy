import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

    state$ = this.auth.state$;
    user$ = this.auth.user$;

    drawerOpen: boolean = true;

    constructor(
        private auth: AuthService
    ) { }

    ngOnInit(): void {
        this.auth.bump();
    }

    login() {
        this.auth.login();
    }

    logout() {
        this.auth.logout();
    }
}
