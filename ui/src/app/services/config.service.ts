import { Injectable } from "@angular/core";
import { HttpService } from "@cardboard-box/ngx-box";
import { map } from "rxjs";
import { environment } from "src/environments/environment";
import { CachedObservable } from "./helper-methods";
import { BuddyConfig } from "./models";

const TOKEN_KEY = 'sb-auth-token';

export type GroupedSettings = {
    name?: string;
    settings: BuddyConfig[];
};

@Injectable({ providedIn: 'root' })
export class StaticConfigService {
    get apiUrl() { return environment.apiUrl; }
    get isProd() { return environment.production; }
    get token() { return localStorage.getItem(TOKEN_KEY) || undefined; }
    set token(value: string | undefined) { value ? localStorage.setItem(TOKEN_KEY, value) : localStorage.removeItem(TOKEN_KEY); }
    get SkipAuthUrls(): string[] { return []; }
    get corsFallback(): string { return 'https://cba-proxy.index-0.com'; }

    
}

@Injectable({ providedIn: 'root' })
export class ConfigService {
    private _config$ = new CachedObservable<BuddyConfig[]>(() => this.http.get<BuddyConfig[]>(`config`)
        .error(t => {}, [])
        .observable);

    config$ = this._config$.data;
    groupedConfig$ = this.config$.pipe(map(t => this.groupSettings(t)));

    constructor(
        private http: HttpService
    ) { }

    put(config: BuddyConfig) {
        return this.http.put('config', config)
            .tap(() => this._config$.invalidate());
    }

    private groupSettings(settings: BuddyConfig[]) {
        const keys: { [key: string]: BuddyConfig[] } = {};

        for(let setting of settings) {
            if (!keys[setting.groupName]) keys[setting.groupName] = [];
            keys[setting.groupName].push(setting);
        }

        return Object.keys(keys)
            .map(t => <GroupedSettings>{
                name: t,
                settings: keys[t]
            });
    }
}