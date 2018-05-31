import { Component, OnInit } from "@angular/core";

declare var $: any;

export interface IRouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
}

export const ROUTES: IRouteInfo[] = [
    { path: "home", title: "Przegląd", icon: "ti-panel", class: "" },
    { path: "promotion-add", title: "Dodaj promocje", icon: "ti-gift", class: "" },
    { path: "promotion-delete", title: "Usuń promocje", icon: "ti-trash", class: "" },
    { path: "dashboard", title: "Informacje", icon: "ti-home", class: "" },
    /*{ path: 'typography', title: 'Typography',  icon:'ti-text', class: '' },
    { path: 'icons', title: 'Icons',  icon:'ti-pencil-alt2', class: '' },
    { path: 'maps', title: 'Maps',  icon:'ti-map', class: '' },
    { path: 'notifications', title: 'Notifications',  icon:'ti-bell', class: '' },
    { path: 'upgrade', title: 'Upgrade to PRO',  icon:'ti-export', class: 'active-pro' },*/
];

@Component({
    moduleId: module.id + "",
    selector: "sidebar-cmp",
    templateUrl: "sidebar.component.html",
})
export class SidebarComponent implements OnInit {
    menuItems: any[];

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