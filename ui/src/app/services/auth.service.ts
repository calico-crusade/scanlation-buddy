import { PlatformLocation } from "@angular/common";
import { Injectable } from "@angular/core";
import { HttpService } from "@cardboard-box/ngx-box";
import { BehaviorSubject } from "rxjs";
import { environment } from "src/environments/environment";
import { ConfigService } from "./config.service";

export enum LoginState {
    NotLoggedIn = 99,
    LoginInProgress = 1,
    LoginSuccess = 2,
    LoginFailure = 3,
    NotVerified = 4,
    FirstTimeLogin = 5
}

export interface AuthCodeResponse {
    error?: string;
    user?: AuthUser;
    token?: string;
}

export interface AuthUser {
    nickname: string;
    avatar: string;
    id: string;
    email: string;
    roles: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {

    private _stateSub = new BehaviorSubject<LoginState>(LoginState.NotLoggedIn);
    private _userSub = new BehaviorSubject<AuthUser | undefined>(undefined);

    get onState() { return this._stateSub.asObservable(); }
    get onUser() { return this._userSub.asObservable(); }

    get isLoggingIn() { return this._stateSub.value === LoginState.LoginInProgress; }

    get rootUrl() { return this.pathCombine(window.location.origin, this.loc.getBaseHrefFromDOM()); }

    get authUrl() { return environment.authUrl; }
    get appId() { return environment.appId; }

    constructor(
        private http: HttpService,
        private loc: PlatformLocation,
        private config: ConfigService
    ) { }


    async bump() {
        this._stateSub.next(LoginState.LoginInProgress);
        if (!this.config.token || this.config.token == null) {
            this._userSub.next(undefined);
            this._stateSub.next(LoginState.NotLoggedIn);
            return false;
        }

        try {
            let me = await this.me().promise;
            this._userSub.next(me);
            if (me.roles.length === 0) {
                this._stateSub.next(LoginState.NotVerified);
                return false;
            }

            this._stateSub.next(LoginState.LoginSuccess);
            return true;
        } catch (e) {
            console.warn('Error occurred while fetching profile', { e });
            this._userSub.next(undefined);
            this._stateSub.next(LoginState.LoginFailure);
            return false;
        }
    }

    async login(): Promise<AuthCodeResponse> {
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

    async handleCode(code: string): Promise<AuthCodeResponse> {
        let auth = await this.resolve(code).promise;
        if (!auth || auth.error || !auth.token) {
            this.config.token = null;
            return auth || { error: 'Error occurred while logging in' };
        }
        
        let state = auth.user?.roles.length === 0 ? LoginState.NotVerified : LoginState.LoginSuccess;

        if (state === LoginState.NotVerified) {
            try {
                const { worked } = await this.promptFirstTime().promise;
                if (worked) {
                    alert('First time setup detected. Please login again.');
                    return await this.login();
                }
            } catch {}
        }

        this.config.token = auth.token;
        this._userSub.next(auth.user);
        this._stateSub.next(state);
        return auth;
    }

    logout() {
        this.config.token = null;
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

    private me() { return this.http.get<AuthUser>(`/auth`); }

    private promptFirstTime() { return this.http.get<{ worked: boolean }>(`auth/is-first-time`); }

    pathCombine(...parts: string[]) {
        return parts.map(t => {
            if (t.startsWith('/')) t = t.substring(1);
            if (t.endsWith('/')) t = t.substring(0, t.length - 2);

            return t;
        }).filter(t => t).join('/');
    }
}