import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsGeneralComponent } from './settings-general/settings-general.component';
import { SettingsRolesComponent } from './settings-roles/settings-roles.component';

const routes: Routes = [
    {
        path: 'general',
        component: SettingsGeneralComponent
    }, {
        path: 'roles',
        component: SettingsRolesComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SettingsRoutingModule { }
