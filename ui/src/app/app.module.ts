import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor, ConfigurationService } from '@cardboard-box/ngx-box';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { COMMON_IMPORTS } from './common-imports';
import { StaticConfigService } from './services/config.service';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './layout/app.component';
import { DashboardComponent } from './routes/dashboard/dashboard.component';
import { ErrorComponent } from './routes/error/error.component';
import { ComponentsModule } from './components/components.module';

@NgModule({
    declarations: [
        AppComponent,
        DashboardComponent,
        ErrorComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        ComponentsModule,
        COMMON_IMPORTS
    ],
    providers: [
        { provide: ConfigurationService, useClass: StaticConfigService },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
