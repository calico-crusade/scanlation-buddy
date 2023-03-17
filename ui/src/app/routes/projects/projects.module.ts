import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProjectsRoutingModule } from './projects-routing.module';
import { ProjectsListComponent } from './projects-list/projects-list.component';
import { ProjectsNewComponent } from './projects-new/projects-new.component';
import { ProjectsImportComponent } from './projects-import/projects-import.component';
import { ProjectsScheduleComponent } from './projects-schedule/projects-schedule.component';
import { ProjectsEditComponent } from './projects-edit/projects-edit.component';
import { ComponentsModule } from 'src/app/components';
import { COMMON_IMPORTS } from 'src/app/common-imports';


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
        ProjectsRoutingModule,
        ComponentsModule,
        ...COMMON_IMPORTS
    ]
})
export class ProjectsModule { }
