import { PlatformLocation } from "@angular/common";
import { Injectable } from "@angular/core";
import { HttpService } from "@cardboard-box/ngx-box";
import { BehaviorSubject, map } from "rxjs";
import { environment } from "src/environments/environment";
import { ConfigService } from "./config.service";
import { BuddyRole, BuddyUser } from "./models";

export enum LoginState {
    NotLoggedIn = 99,
    LoginInProgress = 1,
    LoginSuccess = 2,
    LoginFailure = 3,
    NotVerified = 4
}

export interface LoginResponse {
    token?: string;
    error?: string;
    profile?: BuddyUser;
    roles?: BuddyRole[];
}

export interface AuthCodeResponse {
    token?: string;
    error?: string;
}

export interface MeResponse {
    profile: BuddyUser;
    roles: BuddyRole[];
}


@Injectable({ providedIn: 'root' })
export class AuthService {

    private _stateSub = new BehaviorSubject<LoginState>(LoginState.NotLoggedIn);
    private _userSub = new BehaviorSubject<BuddyUser | undefined>(undefined);
    private _roleSub = new BehaviorSubject<BuddyRole[]>([]);

    state$ = this._stateSub.asObservable();
    user$ = this._userSub.asObservable();
    roles$ = this._roleSub.asObservable();
    perms$ = this.roles$.pipe(map(t => this.permsFromRoles(t)));

    get isLoggingIn() { return this._stateSub.value === LoginState.LoginInProgress; }
    get rootUrl() { return this.pathCombine(window.location.origin, this.loc.getBaseHrefFromDOM()); }
    get authUrl() { return environment.authUrl; }
    get appId() { return environment.appId; }
    get user() { return this._userSub.getValue(); }

    constructor(
        private http: HttpService,
        private loc: PlatformLocation,
        private config: ConfigService
    ) { }


    async bump(): Promise<LoginResponse> {
        this._stateSub.next(LoginState.LoginInProgress);
        if (!this.config.token || this.config.token == null) {
            this._userSub.next(undefined);
            this._stateSub.next(LoginState.NotLoggedIn);
            return { error: 'Token is not set.' };
        }

        try {
            let me = await this.me().promise;
            this._userSub.next(me.profile);
            this._roleSub.next(me.roles);
            if (me.roles.length === 0) {
                try {
                    const { worked } = await this.promptFirstTime().promise;
                    if (worked) {
                        alert('First time setup detected. Please login again.');
                        return await this.login();
                    }
                } catch {}

                this._stateSub.next(LoginState.NotVerified);
                return { error: 'Not verified!' };
            }

            this._stateSub.next(LoginState.LoginSuccess);
            return { profile: me.profile, roles: me.roles };
        } catch (e) {
            console.warn('Error occurred while fetching profile', { e });
            this._userSub.next(undefined);
            this._stateSub.next(LoginState.LoginFailure);
            return { error: 'Error occurred while fetching profile' };
        }
    }

    async login(): Promise<LoginResponse> {
        this._stateSub.next(LoginState.LoginInProgress);

        let code = await this.doLoginPopup();
        if (!code) {
            this._stateSub.next(LoginState.LoginFailure);
            return { error: 'Invalid Login Code' };
        } 

        return await this.handleCode(code);
    }

    loginSame(redirect: string) {
        const redirectUrl = this.pathCombine(
            window.location.origin, 
            this.loc.getBaseHrefFromDOM(), 
            '/callback?redirect=' + encodeURI(redirect))
        const url = `${this.authUrl}/Home/Auth/${this.appId}?redirect=${encodeURI(redirectUrl)}`;
        window.location.href = url;
    }

    async handleCode(code: string): Promise<LoginResponse> {
        let auth = await this.resolve(code).promise;
        if (!auth || auth.error || !auth.token) {
            this.config.token = undefined;
            return auth || { error: 'Error occurred while logging in' };
        }
        
        this.config.token = auth.token;
        return await this.bump();
    }

    logout() {
        this.config.token = undefined;
        this._userSub.next(undefined);
        this._stateSub.next(LoginState.NotLoggedIn);
    }

    private async doLoginPopup() {
        let timer: any;
        let instance: any;

        var promise = new Promise<{ code: string }>((res, rej) => {

            window.addEventListener('message', (event) => {
                if (event.origin !== this.authUrl) return;
                res(event.data);
            });

            instance = window.open(`${this.authUrl}/Home/Auth/${this.appId}`,
                "cardboard_oauth_login_window",
                `toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=750,height=500`);

            timer = setInterval(() => {
                if (!instance.closed) return;

                clearInterval(timer);
                rej('Window Closed');
            }, 1000);
        });

        try {
            var token = await promise;
            clearInterval(timer);
            instance.close();

            return token?.code;
        } catch (ex) {
            console.warn('Error occurred during oauth', { ex, instance, timer });
            return undefined;
        }
    }

    private resolve(code: string) { return this.http.get<AuthCodeResponse>(`auth/${code}`); }

    private me() { return this.http.get<MeResponse>(`/auth`); }

    private promptFirstTime() { return this.http.get<{ worked: boolean }>(`auth/is-first-time`); }

    private permsFromRoles(roles: BuddyRole[]) {
        return roles.map(t => t.permissions)
            .flat()
            .filter((value, index, array) => array.indexOf(value) === index);
    }

    pathCombine(...parts: string[]) {
        return parts.map(t => {
            if (t.startsWith('/')) t = t.substring(1);
            if (t.endsWith('/')) t = t.substring(0, t.length - 2);

            return t;
        }).filter(t => t).join('/');
    }

    hasPerm(perm: string) {
        const perms = this.permsFromRoles(this._roleSub.getValue());
        return perms.indexOf(perm) !== -1;
    }
}