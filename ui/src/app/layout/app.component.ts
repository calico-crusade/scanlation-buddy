import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

    state$ = this.auth.onState;
    user$ = this.auth.onUser;

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
