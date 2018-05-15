import { Component, OnInit } from "@angular/core";

declare var $:any;

export interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}

export const ROUTES: RouteInfo[] = [
    { path: "home", title: "Przegl¹d",  icon: "ti-panel", class: "" },
    { path: "counter", title: "Dodaj promocje",  icon:"ti-gift", class: "" },
    { path: "fetch-data", title: "Usuñ promocje", icon: "ti-trash", class: "" },
    { path: 'dashboard', title: 'Informacje', icon: 'ti-home', class: '' },
    /*{ path: 'typography', title: 'Typography',  icon:'ti-text', class: '' },
    { path: 'icons', title: 'Icons',  icon:'ti-pencil-alt2', class: '' },
    { path: 'maps', title: 'Maps',  icon:'ti-map', class: '' },
    { path: 'notifications', title: 'Notifications',  icon:'ti-bell', class: '' },
    { path: 'upgrade', title: 'Upgrade to PRO',  icon:'ti-export', class: 'active-pro' },*/
];

@Component({
    moduleId: module.id + '',
    selector: "sidebar-cmp",
    templateUrl: "sidebar.component.html",
})

export class SidebarComponent implements OnInit {
    public menuItems: any[];
    ngOnInit() {
        this.menuItems = ROUTES.filter(menuItem => menuItem);
    }
    isNotMobileMenu() {
        return false;
        /*if($(window).width() > 991){
            return false;
        }
        return true;*/
    }

}
