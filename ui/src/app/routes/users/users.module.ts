import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UsersListComponent } from './users-list/users-list.component';
import { UsersRolesComponent } from './users-roles/users-roles.component';
import { ComponentsModule } from 'src/app/components';
import { COMMON_IMPORTS } from 'src/app/common-imports';


@NgModule({
    declarations: [
        UsersListComponent,
        UsersRolesComponent
    ],
    imports: [
        CommonModule,
        UsersRoutingModule,
        ComponentsModule,
        ...COMMON_IMPORTS
    ]
})
export class UsersModule { }
