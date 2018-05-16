import { Component } from '@angular/core';

@Component({
    moduleId: module.id + '',
    selector: 'promotion-add',
    templateUrl: './promAdd.component.html'
})
export class PromotionAdd {
    //public forecasts: IWeatherForecast[];
    public currentCount = 0;

    public incrementCounter() {
        this.currentCount++;
    }
}