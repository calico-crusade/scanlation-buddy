import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

const TOKEN_KEY = 'sb-auth-token';

@Injectable({ providedIn: 'root' })
export class ConfigService {
    get apiUrl() { return environment.apiUrl; }
    get isProd() { return environment.production; }

    get token() { 
        const data = localStorage.getItem(TOKEN_KEY); 
        if (!data) return undefined;
        return data;
    }
    set token(value: string | undefined) { 
        if (!value) localStorage.removeItem(TOKEN_KEY);
        else localStorage.setItem(TOKEN_KEY, value);
    }

    get SkipAuthUrls(): string[] { return []; }

    get corsFallback(): string { return 'https://cba-proxy.index-0.com'; }
}