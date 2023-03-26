import { Component } from '@angular/core';
import { HttpService } from '@cardboard-box/ngx-box';
import { DateTime } from 'luxon';
import { map } from 'rxjs';
import { CachedObservable, Calendar, CalendarGenerator } from 'src/app/services';

export interface FakeManga {
    id: number;
    createdAt: Date;
    updatedAt: Date;
    deletedAt?: Date;

    hashId: string;
    title: string;
    sourceId: string;
    provider: string;
    url: string;
    cover: string;
    tags: string[];
    altTitles: string[];
    description: string;
    nsfw: boolean;
    referer?: string;

    attributes: {
        name: string;
        value: string;
    }[];
}

@Component({
  templateUrl: './projects-schedule.component.html',
  styleUrls: ['./projects-schedule.component.scss']
})
export class ProjectsScheduleComponent {

    private _manga$ = new CachedObservable<FakeManga[]>(() => this.http.get<FakeManga[]>(`https://cba-api.index-0.com/manga/random/100`).observable);
    manga$ = this._manga$.data;

    calendar$ = this.manga$.pipe(map(t => this.genCal(t)));

    constructor(
        private http: HttpService
    ) { }
    
    genCal(manga: FakeManga[]): Calendar<FakeManga> {
        const target = DateTime.fromJSDate(new Date());

        const start = target.startOf('month').startOf('week').endOf('day');
        const end = target.endOf('month').endOf('week').endOf('day');

        const rndDate = () => {
            const t = start.toMillis() + Math.random() * (end.toMillis() - start.toMillis());
            return DateTime.fromMillis(t).toJSDate();
        };

        manga.forEach(t => t.createdAt = rndDate());

        const generator = new CalendarGenerator<FakeManga>(manga, (t) => t.createdAt);
        return generator.generate(new Date());
    }

    invalidateManga(){
        this._manga$.invalidate();
    }
}
