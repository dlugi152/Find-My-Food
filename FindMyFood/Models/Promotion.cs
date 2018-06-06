using System;
using System.Collections.Generic;

namespace FindMyFood.Models
{
    public class Promotion
    {
        private readonly List<DayOfWeek> _daysOfWeek = new List<DayOfWeek>();
        public Promotion() { }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public Enums.PeriodEnum RepetitionMode { get; set; }

        private void SetWeek(bool value, DayOfWeek day) {
            if (value) {
                if (!_daysOfWeek.Contains(day))
                    _daysOfWeek.Add(day);
            }
            else if (_daysOfWeek.Contains(day))
                _daysOfWeek.Remove(day);
        }

        public bool Monday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Monday);
            set => SetWeek(value, DayOfWeek.Monday);
        }

        public bool Tuesday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Tuesday);
            set => SetWeek(value, DayOfWeek.Tuesday);
        }

        public bool Wednesday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Wednesday);
            set => SetWeek(value, DayOfWeek.Wednesday);
        }

        public bool Thursday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Thursday);
            set => SetWeek(value, DayOfWeek.Thursday);
        }

        public bool Friday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Friday);
            set => SetWeek(value, DayOfWeek.Friday);
        }

        public bool Saturday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Saturday);
            set => SetWeek(value, DayOfWeek.Saturday);
        }

        public bool Sunday
        {
            get => _daysOfWeek.Contains(DayOfWeek.Sunday);
            set => SetWeek(value, DayOfWeek.Sunday);
        }

        public void AddDaysOfWeek(IEnumerable<DayOfWeek> daysInWeek) {
            foreach (var dayOfWeek in daysInWeek)
                switch (dayOfWeek) {
                    case DayOfWeek.Friday:
                        Friday = true;
                        break;
                    case DayOfWeek.Monday:
                        Monday = true;
                        break;
                    case DayOfWeek.Saturday:
                        Saturday = true;
                        break;
                    case DayOfWeek.Sunday:
                        Sunday = true;
                        break;
                    case DayOfWeek.Thursday:
                        Thursday = true;
                        break;
                    case DayOfWeek.Tuesday:
                        Tuesday = true;
                        break;
                    case DayOfWeek.Wednesday:
                        Wednesday = true;
                        break;
                }
        }

        public bool IsInDate(DateTime now) {
            try {
                switch (RepetitionMode) {
                    case Enums.PeriodEnum.NoLimit:
                        return true;
                    case Enums.PeriodEnum.Once:
                        if (now.CompareTo(DateStart) > 0 && now.CompareTo(DateEnd) < 0)
                            return true;
                        return false;
                    case Enums.PeriodEnum.Daily:
                        if (now.TimeOfDay.CompareTo(DateStart.Value.TimeOfDay) > 0 &&
                            now.TimeOfDay.CompareTo(DateEnd.Value.TimeOfDay) < 0)
                            return true;
                        return false;
                    case Enums.PeriodEnum.Weekly:
                        if (_daysOfWeek.Contains(now.DayOfWeek) &&
                            now.TimeOfDay.CompareTo(DateStart.Value.TimeOfDay) > 0 &&
                            now.TimeOfDay.CompareTo(DateEnd.Value.TimeOfDay) < 0)
                            return true;
                        return false;
                    case Enums.PeriodEnum.SingleDays:
                        if (_daysOfWeek.Contains(now.DayOfWeek))
                            return true;
                        return false;
                    default:
                        return false;
                }
            }
            catch (Exception) {
                return false;
            }
        }
    }
}