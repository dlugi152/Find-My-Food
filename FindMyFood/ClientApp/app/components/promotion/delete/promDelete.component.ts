import { Component, Inject } from "@angular/core";
import { Http } from "@angular/http";
import { HttpClient } from "@angular/common/http";

@Component({
    moduleId: module.id + "",
    selector: "promotion-delete",
    templateUrl: "./promDelete.component.html"
})
export class PromotionDelete {
    promotions: IPromotion[];

    constructor(private http: Http, @Inject("BASE_URL") baseUrl: string) {
        http.get(baseUrl + "api/MyPromotions").subscribe(result => {
            this.promotions = result.json() as IPromotion[];
                console.log(this.promotions);
            },
            error => console.error(error));
    }

    deleteById(id: number) {
        this.http.get(`/api/DeletePromotion/${id}`).subscribe((val: any): void => {
                let response = val.json() as IStandardResponse;
                if (response.response) {
                    this.http.get("api/MyPromotions").subscribe(result => {
                            this.promotions = result.json() as IPromotion[];
                            console.log(this.promotions);
                        },
                        error => console.error(error));
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
}

interface IPromotion {
    id: number;
    description: string;
    tags: string;
    dateStart: string;
    dateEnd: string;
}

interface IStandardResponse {
    response: boolean,
    message: string,
}