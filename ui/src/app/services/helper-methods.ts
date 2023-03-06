import { ActivatedRoute } from "@angular/router";
import { BehaviorSubject, map, mergeMap, Observable, shareReplay } from "rxjs";

export function queryParam(route: ActivatedRoute, key: string) {
    return route.queryParams.pipe(map(t => <string | undefined>t[key]));
}

export function routeParams(route: ActivatedRoute, key: string) {
    return route.params.pipe(map(t => <string | undefined>t[key]));
}

export class CachedObservable<T> {
    private _subject = new BehaviorSubject<void>(undefined);
    private _request = this._fn();

    data = this._subject.pipe(
        mergeMap(() => this._request),
        shareReplay(1)
    );

    constructor(
        private _fn: () => Observable<T>
    ) { }

    invalidate() {
        this._subject.next();
    }
}