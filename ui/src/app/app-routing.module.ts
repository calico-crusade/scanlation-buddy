import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './routes/dashboard/dashboard.component';
import { ErrorComponent } from './routes/error/error.component';

const routes: Routes = [
    {
       path: 'dashboard',
       component: DashboardComponent 
    }, {
        path: 'error',
        component: ErrorComponent
    }, {
        path: '',
        pathMatch: 'full',
        redirectTo: '/dashboard'
    }, { 
        path: 'projects', 
        loadChildren: () => import('./routes/projects/projects.module').then(m => m.ProjectsModule) 
    }, { 
        path: 'account', 
        loadChildren: () => import('./routes/account/account.module').then(m => m.AccountModule) 
    }, { 
        path: 'settings', 
        loadChildren: () => import('./routes/settings/settings.module').then(m => m.SettingsModule) 
    }, {
        path: '**',
        redirectTo: '/error?error=Page not found'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
