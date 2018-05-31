import { Component, Inject } from "@angular/core";
import { Http } from "@angular/http";

@Component({
    moduleId: module.id + "",
    selector: "promotion-delete",
    templateUrl: "./promDelete.component.html"
})
export class PromotionDelete {
    forecasts: IWeatherForecast[];

    constructor(http: Http, @Inject("BASE_URL") baseUrl: string) {
        http.get(baseUrl + "api/SampleData/WeatherForecasts").subscribe(result => {
                this.forecasts = result.json() as IWeatherForecast[];
            },
            error => console.error(error));
    }
}

interface IWeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}