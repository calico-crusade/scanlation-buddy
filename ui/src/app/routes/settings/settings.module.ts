import { NgModule } from '@angular/core';
import { COMMON_IMPORTS } from 'src/app/common-imports';

import { SettingsRoutingModule } from './settings-routing.module';
import { SettingsGeneralComponent } from './settings-general/settings-general.component';
import { ComponentsModule } from 'src/app/components';


@NgModule({
    declarations: [
        SettingsGeneralComponent
    ],
    imports: [
        SettingsRoutingModule,
        ComponentsModule,
        ...COMMON_IMPORTS
    ]
})
export class SettingsModule { }
