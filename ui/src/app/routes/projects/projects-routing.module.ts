import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProjectsEditComponent } from './projects-edit/projects-edit.component';
import { ProjectsImportComponent } from './projects-import/projects-import.component';
import { ProjectsListComponent } from './projects-list/projects-list.component';
import { ProjectsNewComponent } from './projects-new/projects-new.component';
import { ProjectsScheduleComponent } from './projects-schedule/projects-schedule.component';

const routes: Routes = [
    {
        path: 'list',
        component: ProjectsListComponent
    }, {
        path: 'new',
        component: ProjectsNewComponent
    }, {
        path: 'import',
        component: ProjectsImportComponent
    }, {
        path: 'schedule',
        component: ProjectsScheduleComponent
    }, {
        path: 'edit/:id',
        component: ProjectsEditComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProjectsRoutingModule { }
