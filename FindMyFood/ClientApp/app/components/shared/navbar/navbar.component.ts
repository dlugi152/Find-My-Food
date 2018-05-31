import { Component, OnInit, Renderer, ViewChild, ElementRef } from "@angular/core";
import { ROUTES } from "../../sidebar/sidebar.component";
import { Location } from "@angular/common";

@Component({
    moduleId: module.id + "",
    selector: "navbar-cmp",
    templateUrl: "navbar.component.html"
})
export class NavbarComponent implements OnInit {
    private listTitles: any[];
    location: Location;
    private nativeElement: Node;
    private toggleButton: any;
    private sidebarVisible: boolean;

    @ViewChild("navbar-cmp")
    button: any;

    constructor(location: Location, private renderer: Renderer, private element: ElementRef) {
        this.location = location;
        this.nativeElement = element.nativeElement;
        this.sidebarVisible = false;
    }

    ngOnInit() {
        this.listTitles = ROUTES.filter(listTitle => listTitle);
        const navbar: HTMLElement = this.element.nativeElement;
        this.toggleButton = navbar.getElementsByClassName("navbar-toggle")[0];
    }

    getTitle() {
        let titlee = "titleeee"; //window.location.pathname;
        titlee = titlee.substring(1);
        for (let item = 0; item < this.listTitles.length; item++) {
            if (this.listTitles[item].path === titlee) {
                return this.listTitles[item].title;
            }
        }
        return "Nazar";
    }

    sidebarToggle() {
        var toggleButton = this.toggleButton;
        const body = document.getElementsByTagName("body")[0];

        if (this.sidebarVisible === false) {
            setTimeout(() => {
                    toggleButton.classList.add("toggled");
                },
                500);
            body.classList.add("nav-open");
            this.sidebarVisible = true;
        } else {
            this.toggleButton.classList.remove("toggled");
            this.sidebarVisible = false;
            body.classList.remove("nav-open");
        }
    }
}