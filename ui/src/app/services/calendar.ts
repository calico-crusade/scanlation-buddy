import { DateTime } from 'luxon';

export interface Range {
    start: DateTime;
    end: DateTime;
}

export interface Day<T> extends Range { 
    events: T[];
    inMonth: boolean;
    today: boolean;
}
export interface Week<T> extends Range { days: Day<T>[]; }
export interface Calendar<T> extends Range { 
    weeks: Week<T>[];
    target: DateTime;
}

type WrapEvent<T> = {
    date: DateTime;
    event: T;
};

export class CalendarGenerator<T> {
    
    private _pred = (t: T) => DateTime.fromJSDate(this._predicate(t));

    constructor(
        private _events: T[],
        private _predicate: (t: T) => Date
    ) { }


    generate(target: Date): Calendar<T>;
    generate(start: Date, end: Date): Calendar<T>;
    generate(target: Date, targetEnd?: Date) {
        //Determine some variables we'll need for generation
        const targetDt = DateTime.fromJSDate(target);
        const [ start, end ] = this.determineRange(target, targetEnd);
        const totalDays = end.diff(start, 'days').days;
        const events = this.sortEvents(start);
        let week = this.genWeek(start);
        let eventIndex = 0;

        const cal: Calendar<T> = {
            start: start,
            end: end,
            weeks: [],
            target: targetDt
        };

        //Iterate for each day within the range
        for(let d = 0; d < totalDays; d++) {
            const cur = start.plus({ days: d });
            const day = this.genDay(cur, targetDt.hasSame(cur, 'month'));

            const weekStart = cur.startOf('week');
            //Are we still within the same week?
            //If not, we need to start a new week.
            if (weekStart > week.end) {
                cal.weeks.push(week);
                week = this.genWeek(cur);
            }

            //Determine what events are available on this day
            while(eventIndex < events.length) {
                const { date, event } = events[eventIndex];

                if (date > day.end || date < day.start) break;

                day.events.push(event);
                eventIndex++;
            }

            //Add the day to the current week.
            week.days.push(day);
        }

        //Ensure the last week is add to the calendar
        cal.weeks.push(week);
        return cal;
    }

    private sortEvents(start: DateTime) {
        return this._events
            //wrap and include DateTime
            .map(t => <WrapEvent<T>>{
                date: this._pred(t),
                event: t
            })
            //Ensure we only get dates that exist within our range
            .filter(t => t.date >= start)
            //Sort them by dates
            .sort((a, b) => a.date.toMillis() - b.date.toMillis());
    }

    private genDay(target: DateTime, inMonth: boolean) {
        return <Day<T>>{
            start: target.startOf('day'),
            end: target.endOf('day'),
            events: [],
            today: target.hasSame(DateTime.now(), 'day'),
            inMonth
        };
    }

    private genWeek(target: DateTime) {
        return <Week<T>>{
            start: target.startOf('week').startOf('day'),
            end: target.endOf('week').endOf('day'),
            days: []
        };
    }

    private determineRange(start: Date, end?: Date) {
        const targetDt = DateTime.fromJSDate(start);
        if (end) {
            const targetEnd = DateTime.fromJSDate(end);
            return [ 
                targetDt.startOf('week'),
                targetEnd.endOf('week')
            ];
        }

        return [
            targetDt.startOf('month').startOf('week'),
            targetDt.endOf('month').endOf('week')
        ];
    }
}