import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProjectsRoutingModule } from './projects-routing.module';
import { ProjectsListComponent } from './projects-list/projects-list.component';
import { ProjectsNewComponent } from './projects-new/projects-new.component';
import { ProjectsImportComponent } from './projects-import/projects-import.component';
import { ProjectsScheduleComponent } from './projects-schedule/projects-schedule.component';
import { ProjectsEditComponent } from './projects-edit/projects-edit.component';


@NgModule({
    declarations: [
        ProjectsListComponent,
        ProjectsNewComponent,
        ProjectsImportComponent,
        ProjectsScheduleComponent,
        ProjectsEditComponent
    ],
    imports: [
        CommonModule,
        ProjectsRoutingModule
    ]
})
export class ProjectsModule { }
