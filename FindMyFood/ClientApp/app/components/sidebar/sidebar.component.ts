import { Component, OnInit } from "@angular/core";

declare var $: any;

export interface IRouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}

export const routes: IRouteInfo[] = [
    { path: "dashboard", title: "Informacje", icon: "ti-home", class: "" },
    { path: "promotion-add", title: "Dodaj promocje", icon: "ti-gift", class: "" },
    { path: "promotion-delete", title: "Usuń promocje", icon: "ti-trash", class: "" },
    { path: "profile", title: "Profil", icon: "ti-panel", class: "" }
];

@Component({
    moduleId: module.id + "",
    selector: "sidebar-cmp",
    templateUrl: "sidebar.component.html",
})
export class SidebarComponent implements OnInit {
    menuItems: any[];

    ngOnInit() {
        this.menuItems = routes.filter(menuItem => menuItem);
    }

    isNotMobileMenu() {
        return false;
        /*if($(window).width() > 991){
            return false;
        }
        return true;*/
    }

}