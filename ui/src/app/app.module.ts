import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor, ConfigurationService, MagicCircleModule, NgxBoxModule } from '@cardboard-box/ngx-box';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './layout/app.component';
import { DashboardComponent } from './routes/dashboard/dashboard.component';
import { ErrorComponent } from './routes/error/error.component';
import { ChatPopupComponent } from './components/chat-popup/chat-popup.component';
import { ConfigService } from './services/config.service';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { LoginPopupComponent } from './components/login-popup/login-popup.component';

@NgModule({
    declarations: [
        AppComponent,
        DashboardComponent,
        ErrorComponent,
        ChatPopupComponent,
        LoginPopupComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        NgxBoxModule,
        MagicCircleModule,
        HttpClientModule,
        FormsModule
    ],
    providers: [
        { provide: ConfigurationService, useClass: ConfigService },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
