import { Component, Inject } from "@angular/core";
import { Http } from "@angular/http";

@Component({
    moduleId: module.id + "",
    selector: "profile",
    templateUrl: "./profile.component.html"
})
export class ProfileComponent {
    restaurant: Restaurant;

    constructor(private http: Http, @Inject("BASE_URL") baseUrl: string) {
        http.get(baseUrl + "api/GetExtendedRestaurant").subscribe(result => {
                //this.restaurant = result.json() as IRestaurant;
                
            },
            error => console.error(error));
        this.restaurant = new Restaurant();
        this.restaurant.address = "Jakiś adres";
        this.restaurant.name = "nazwa";
        this.restaurant.email = "mejl";
        this.restaurant.ceofirstname = "imie";
        this.restaurant.ceolastname = "nazwisko";
        this.restaurant.city = "miasto";
        this.restaurant.country = "polska";
        this.restaurant.postalCode = "kodpocztowy";
        this.restaurant.longDescription = "bardzo długo opis\nz entarami i w ogóle";
        this.restaurant.motto = "kolacja bez piwa to nie kolacja";
        this.restaurant.website = "https://www.example.com";
        this.restaurant.nopromotions = 1;
        this.restaurant.rating = 4.5;
        this.restaurant.norates = 2;
        this.restaurant.lastRates = new Array(3);
        this.restaurant.lastRates[0] = new SingleRate();
        this.restaurant.lastRates[1] = new SingleRate();
        this.restaurant.lastRates[0].login = "tomasz152";
        this.restaurant.lastRates[0].rate = 5;
        this.restaurant.lastRates[1].login = "asd";
        this.restaurant.lastRates[1].rate = 4;
    }

    update() {

    }
}

class SingleRate {
    login: string;
    rate: number;
}

class Restaurant {
    name: string;
    email: string;
    ceofirstname: string;
    ceolastname: string;
    address: string;
    city: string;
    country: string;
    postalCode: string;
    longDescription: string;
    motto: string;
    website: string;
    nopromotions: number;
    rating: number;
    norates: number;
    lastRates: SingleRate[];
}