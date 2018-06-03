import {
    Component, 
    OnInit
} from "@angular/core";
import flatpickr from "flatpickr";
import { Polish } from "flatpickr/dist/l10n/pl.js";
import {
    FormGroup,
    FormControl,
    Validators
} from "@angular/forms";
import { HttpClient } from "@angular/common/http";
declare var jquery: any;
declare var $: any;

@Component({
    moduleId: module.id + "",
    selector: "promotion-add",
    styleUrls: ["./promAdd.component.css"], //jeszcze nie używany
    templateUrl: "./promAdd.component.html"
})
export class PromotionAdd implements OnInit {
    opts: any;
    myform: FormGroup;
    periods: { value: string; key: string }[];
    weekDays: { [index: number]: string; key: string;value: string }[];
    dropdownDaysSettingsMulti: any = {};
    dropdownDaysSettingsSingle: any = {};
    dropdownPeriodSettingsSingle: any = {};

    constructor(private http: HttpClient) {}

    ngOnInit() {
        this.periods = [
            { value: "Bez limitu", key: "nolimit" },
            { value: "Jednorazowo", key: "once" },
            { value: "Codziennie", key: "daily" },
            { value: "Co tydzień", key: "weekly" },
            { value: "Wybrane dni tygodnia", key: "singledays" }
        ];

        this.dropdownDaysSettingsMulti = {
            singleSelection: false,
            text: "Wybierz dni",
            selectAllText: "Zaznacz wszystkie",
            unSelectAllText: "Odznacz wszystkie",
            enableSearchFilter: false,
            maxHeight: 200,
            primaryKey: "key",
            labelKey: "value",
            classes: "myclass custom-class"
        };
        this.dropdownDaysSettingsSingle = {
            singleSelection: true,
            text: "Wybierz dni",
            maxHeight: 200,
            primaryKey: "key",
            labelKey: "value",
            enableSearchFilter: false,
            classes: "myclass custom-class"
        };
        this.dropdownPeriodSettingsSingle = {
            singleSelection: true,
            text: "Wybierz okres powtarzania",
            primaryKey: "key",
            labelKey: "value",
            enableSearchFilter: false,
            classes: "myclass custom-class"
        };

        this.weekDays = [
            { key: "monday", value: "Poniedziałek" },
            { key: "tuesday", value: "Wtorek" },
            { key: "wednesday", value: "Środa" },
            { key: "thursday", value: "Czwartek" },
            { key: "friday", value: "Piątek" },
            { key: "saturday", value: "Sobota" },
            { key: "sunday", value: "Niedziela" }
        ];

        this.opts = {
            calendar: {
                dateFormat: "Y-m-d H:i",
                minDate: "today",
                mode: "range",
                "locale": Polish,
            },
            clocks: {
                enableTime: true,
                noCalendar: true,
                minuteIncrement: 30,
                time_24hr: true,
                "locale": Polish
            }
        };
        
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
            EndTime: new FormControl("",),
            repetitionMode: new FormControl("",Validators.required),
            daysInWeek: new FormControl("",)
        });
    }

    repetitionChoosen: string;

    updateCalendar(key: any) {
        $("#timePeriodForm").hide(300,
            () => {
                this.repetitionChoosen = key.key;
                $("#timePeriodForm").show(300,
                    () => {
                        flatpickr("#dateRangeId", this.opts.calendar);
                        flatpickr("#timeStartId", this.opts.clocks);
                        flatpickr("#timeEndId", this.opts.clocks);
                    });
            });
    }

    onSubmit() {
        if (this.myform.valid) {
            console.log("Form Submitted!");
            var value = this.myform.value;
            var days: string[] = new Array();
            for (let i in value.daysInWeek)
                if (value.daysInWeek.hasOwnProperty(i))
                    days.push(value.daysInWeek[i].key);
            value.daysInWeek = days;
            value.repetitionMode = value.repetitionMode[0].key;
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