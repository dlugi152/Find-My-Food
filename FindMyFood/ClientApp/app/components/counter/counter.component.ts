import { Component } from '@angular/core';

@Component({
    moduleId: module.id + '',
    selector: 'counter',
    templateUrl: './counter.component.html'
})
export class CounterComponent {
    public currentCount = 0;

    public incrementCounter() {
        this.currentCount++;
    }
}
