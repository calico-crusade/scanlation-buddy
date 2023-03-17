import { Component, OnInit } from '@angular/core';
import { PermCheck } from '../components';
import { AuthService } from '../services';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent extends PermCheck implements OnInit {

    state$ = this._auth.state$;
    user$ = this._auth.user$;

    drawerOpen: boolean = true;

    constructor(
        private _auth: AuthService
    ) { super(_auth); }

    ngOnInit(): void {
        this._auth.bump();
    }

    login() {
        this._auth.login();
    }

    logout() {
        this._auth.logout();
    }
}
