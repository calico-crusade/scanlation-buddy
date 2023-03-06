import { Injectable } from "@angular/core";
import { HttpService } from "@cardboard-box/ngx-box";
import { CachedObservable } from "./helper-methods";
import { BuddyPermission, BuddyRole } from "./models";

@Injectable({ providedIn: 'root' })
export class RolesService {

    private _roles$ = new CachedObservable<BuddyRole[]>(() => this.http.get<BuddyRole[]>(`roles`).observable);
    private _perms$ = new CachedObservable<BuddyPermission[]>(() => this.http.get<BuddyPermission[]>(`roles/permissions`).observable);

    roles$ = this._roles$.data;
    perms$ = this._perms$.data;

    constructor(
        private http: HttpService
    ) { }

    post(role: BuddyRole) {
        return this.http.post(`roles`, role)
            .tap(() => this._roles$.invalidate());
    }

    put(role: BuddyRole) {
        return this.http.put(`roles`, role)
            .tap(() => this._roles$.invalidate());
    }
}