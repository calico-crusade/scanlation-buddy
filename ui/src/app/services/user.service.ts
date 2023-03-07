import { Injectable } from "@angular/core";
import { HttpService } from "@cardboard-box/ngx-box";
import { BehaviorSubject, Observable } from "rxjs";
import { CachedObservable } from "./helper-methods";
import { BuddyRole, BuddyUserRoles, Paginated } from "./models";

export interface UserFilters {
    page: number;
    size: number;
    username?: string;
    provider?: string;
    roleId?: number;
}

@Injectable({ providedIn: 'root' })
export class UserService {

    private _page: number = 1;
    private _size: number = 100;
    private _users = new BehaviorSubject<Paginated<BuddyUserRoles> | undefined>(undefined);
    private _providers = new CachedObservable<string[]>(() => this.http.get<string[]>('users/providers').observable);

    users$ = this._users.asObservable();
    providers$ = this._providers.data;

    get page() { return this._page; }
    set page(value: number) {
        this._page = value;
        this.search().subscribe(() => {});
    }

    constructor(
        private http: HttpService
    ) { this.search().subscribe(() => {}); }
    
    search(filters?: UserFilters) {
        const params: { [key: string]: string | number } = {};

        params['page'] = filters?.page || this._page;
        params['size'] = filters?.size || this._size;

        if (filters?.username) params['username'] = filters.username;
        if (filters?.provider) params['provider'] = filters.provider;
        if (filters?.roleId && filters.roleId != -1) params['roleId'] = filters.roleId;

        return this.http.get<Paginated<BuddyUserRoles>>(`users`, { params }).tap(t => this._users.next(t)).observable;
    }

    setRoles(id: number, roles: number[]): Observable<unknown>;
    setRoles(id: number, roles: BuddyRole[]): Observable<unknown>;
    setRoles(id: number, roles: number[] | BuddyRole[]) {
        const ids = roles.map(t => typeof t === 'number' ? t : t.id);
        return this.http.put(`users/${id}/roles`, ids).observable;
    }
}