import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UsersListComponent } from './users-list/users-list.component';
import { UsersRolesComponent } from './users-roles/users-roles.component';

const routes: Routes = [
    {
        path: 'list',
        component: UsersListComponent
    }, {
        path: 'roles',
        component: UsersRolesComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UsersRoutingModule { }
