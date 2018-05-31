import { Component } from "@angular/core";

@Component({
    moduleId: module.id + "",
    selector: "promotion-add",
    templateUrl: "./promAdd.component.html"
})
export class PromotionAdd {
    //public forecasts: IWeatherForecast[];
    currentCount = 0;

    incrementCounter() {
        this.currentCount++;
    }
}