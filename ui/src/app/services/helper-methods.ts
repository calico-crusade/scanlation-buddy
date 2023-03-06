import { ActivatedRoute } from "@angular/router";
import { map } from "rxjs";

export function queryParam(route: ActivatedRoute, key: string) {
    return route.queryParams.pipe(map(t => <string | undefined>t[key]));
}

export function params(route: ActivatedRoute, key: string) {
    return route.params.pipe(map(t => <string | undefined>t[key]));
}