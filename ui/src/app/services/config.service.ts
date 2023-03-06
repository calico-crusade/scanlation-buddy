import { Injectable } from "@angular/core";
import { LocalStorageVar } from "@cardboard-box/ngx-box";
import { environment } from "src/environments/environment";

@Injectable({ providedIn: 'root' })
export class ConfigService {
    private _token = new LocalStorageVar<string | undefined | null>('', 'sb-auth-token');
    
    get apiUrl() { return environment.apiUrl; }
    get isProd() { return environment.production; }

    get token() { return this._token.value; }
    set token(value: string | undefined | null) { this._token.value = value; }

    get SkipAuthUrls(): string[] { return []; }

    get corsFallback(): string { return 'https://cba-proxy.index-0.com'; }
}