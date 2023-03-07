import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsGeneralComponent } from './settings-general/settings-general.component';

const routes: Routes = [
    {
        path: 'general',
        component: SettingsGeneralComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SettingsRoutingModule { }
