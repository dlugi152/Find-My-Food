import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HomeComponent } from './components/home/home.component';
import { PromotionDelete } from './components/promotion/delete/promDelete.component';
import { PromotionAdd } from './components/promotion/add/promAdd.component';
import { SidebarModule } from './components/sidebar/sidebar.module';
import { FooterModule } from './components/shared/footer/footer.module';
import { NavbarModule } from './components/shared/navbar/navbar.module';

@NgModule({
    declarations: [
        AppComponent,
        PromotionAdd,
        DashboardComponent,
        PromotionDelete,
        HomeComponent
    ],
    imports: [
        CommonModule,
        SidebarModule,
        NavbarModule,
        FooterModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'promotion-add', component: PromotionAdd },
            { path: 'promotion-delete', component: PromotionDelete },
            { path: 'dashboard', component: DashboardComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}