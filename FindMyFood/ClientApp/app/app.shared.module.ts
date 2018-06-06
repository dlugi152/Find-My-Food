import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { RouterModule } from "@angular/router";

import { AppComponent } from "./components/app/app.component";
import { ProfileComponent } from "./components/profile/profile.component";
import { PromotionDelete } from "./components/promotion/delete/promDelete.component";
import { PromotionAdd } from "./components/promotion/add/promAdd.component";
import { SidebarModule } from "./components/sidebar/sidebar.module";
import { AngularMultiSelectModule } from "angular2-multiselect-dropdown/angular2-multiselect-dropdown";
import { FooterModule } from "./components/shared/footer/footer.module";
import { NavbarModule } from "./components/shared/navbar/navbar.module";

@NgModule({
    declarations: [
        AppComponent,
        PromotionAdd,
        PromotionDelete,
        ProfileComponent
    ],
    imports: [
        CommonModule,
        SidebarModule,
        NavbarModule,
        ReactiveFormsModule,
        AngularMultiSelectModule,
        FooterModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: "", redirectTo: "profile", pathMatch: "full" },
            { path: "profile", component: ProfileComponent },
            { path: "promotion-add", component: PromotionAdd },
            { path: "promotion-delete", component: PromotionDelete },
            { path: "**", redirectTo: "profile" }
        ])
    ]
})
export class AppModuleShared {
}