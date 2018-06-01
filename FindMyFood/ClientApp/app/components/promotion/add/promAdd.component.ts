import {
    NgModule,
    Component,
    Pipe,
    OnInit
} from "@angular/core";
import flatpickr from "flatpickr";
import { Http } from "@angular/http";
import { Polish } from "flatpickr/dist/l10n/pl.js";
import {
    ReactiveFormsModule,
    FormsModule,
    FormGroup,
    FormControl,
    Validators,
    FormBuilder
} from "@angular/forms";
import { HttpClient } from "@angular/common/http";

@Component({
    moduleId: module.id + "",
    selector: "promotion-add",
    styleUrls: ["./promAdd.component.css"],//jeszcze nie u¿ywany
    templateUrl: "./promAdd.component.html"
})
export class PromotionAdd {
    myform: FormGroup;

    constructor(private http: HttpClient) {}

    ngOnInit() {
        flatpickr("#dateRangeId",
            {
                dateFormat: "Y-m-d H:i",
                minDate: "today",
                mode: "range",
                "locale": Polish,
            });
        const timeOpts = {
            enableTime: true,
            noCalendar: true,
            minuteIncrement: 30,
            time_24hr: true,
            "locale": Polish,
        };
        flatpickr("#timeStartId", timeOpts);
        flatpickr("#timeEndId", timeOpts);
        //lewa strona formularzy
        this.myform = new FormGroup({
            Description: new FormControl("",
                [
                    Validators.minLength(5),
                    Validators.required
                ]),
            Tags: new FormControl("",
                [
                    Validators.required,
                    Validators.pattern("([^,]{3,},){2,}[^,]{3,}")
                ]),
            DateRange: new FormControl("",),
            StartTime: new FormControl("",),
            EndTime: new FormControl("",)
        });
    }

    onSubmit() {
        if (this.myform.valid) {
            console.log("Form Submitted!");
            let value = this.myform.value as JSON;
            console.log(value);
            this.http.post("/api/AddPromotion/", value).subscribe(
                (val: IStandardResponse) => {
                    if (val.response) {
                        alert("dodano");
                        this.myform.reset();
                    } else
                        alert(`Niepowodzenie z powodu: ${val.message}`);
                },
                response => {
                    console.log("POST call in error", response);
                },
                () => {
                    console.log("The POST observable is now completed.");
                });
        }
    }
}

interface IStandardResponse {
    response: boolean,
    message: string,
}