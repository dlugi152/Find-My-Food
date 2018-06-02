import { Component, Inject } from "@angular/core";
import { Http } from "@angular/http";
import {
    ReactiveFormsModule,
    FormsModule,
    FormGroup,
    FormControl,
    Validators,
    FormBuilder
} from "@angular/forms";

@Component({
    moduleId: module.id + "",
    selector: "profile",
    templateUrl: "./profile.component.html"
})
export class ProfileComponent {
    restaurant: IRestaurantFull;
    ports: string[];
    myform: FormGroup;

    updateProfileInfo() {
        this.http.get("api/GetExtendedRestaurant").subscribe(result => {
                this.restaurant = result.json() as IRestaurantFull;
            },
            error => console.error(error));
    }

    constructor(private http: Http, @Inject("BASE_URL") baseUrl: string) {
        http.get(baseUrl + "api/GetExtendedRestaurant").subscribe(result => {
                this.restaurant = result.json() as IRestaurantFull;
                this.ports = ["https", "http"];
                var port = this.ports[0];
                if (this.restaurant.website == null)
                    this.restaurant.website = "";
                for (let port1 of this.ports)
                    if (this.restaurant.website.search(port1 + "://") != -1) {
                        port = port1;
                        break;
                    }
                this.myform = new FormGroup({
                    name: new FormControl(this.restaurant.name,
                        [
                            Validators.required
                        ]),
                    ceofirstname: new FormControl(this.restaurant.ceofirstname),
                    ceolastname: new FormControl(this.restaurant.ceolastname),
                    address: new FormControl(this.restaurant.address),
                    city: new FormControl(this.restaurant.city),
                    country: new FormControl(this.restaurant.country),
                    postalCode: new FormControl(this.restaurant.postalCode),
                    //longitude: number;
                    //latitude: number;
                    longDescription: new FormControl(this.restaurant.longDescription),
                    motto: new FormControl(this.restaurant.motto),
                    website: new FormControl(this.restaurant.website.replace("https://", "").replace("http://", "")),
                    county: new FormControl(this.restaurant.county),
                    street: new FormControl(this.restaurant.street),
                    streetNumber: new FormControl(this.restaurant.streetNumber),
                    province: new FormControl(this.restaurant.province),
                    port: new FormControl(port, Validators.required)
                });
            },
            error => console.error(error));
    }


    onSubmit() {
        console.log("sumbit");
        this.myform.value.website = this.myform.value.port + "://" + this.myform.value.website;
        this.myform.value.address = this.getFullAddress(this.myform.value);
        this.http.post("/api/UpdateProfile", this.myform.value).subscribe((val: any): void => {
                let response = val.json() as IStandardResponse;
                if (response.response) {
                    alert("zaktualizowano");
                    this.updateProfileInfo();
                } else
                    alert(`Niepowodzenie z powodu: ${response.message}`);
            },
            response => {
                console.log("POST call in error", response);
            },
            () => {
                console.log("The POST observable is now completed.");
            });
    }

    getFullAddress(value: any): string {
        return value.street +
            " " +
            value.streetNumber +
            ", " +
            value.postalCode +
            " " +
            value.city +
            ", " +
            value.country;
    }
}

interface ISingleRate {
    login: string;
    rate: number;
}

interface IRestaurantFull extends IRestaurantInfo  {
    email: string;
    longitude: number;
    latitude: number;
    nopromotions: number;
    rating: number;
    norates: number;
    lastRates: ISingleRate[];
}

interface IRestaurantInfo {
    name: string;
    ceofirstname: string;
    ceolastname: string;
    address: string;
    city: string;
    country: string;
    postalCode: string;
    //longitude: number;
    //latitude: number;
    longDescription: string;
    motto: string;
    website: string;
    county: string;
    street: string;
    streetNumber: string;
    province: string;
}

interface IStandardResponse {
    response: boolean,
    message: string,
}