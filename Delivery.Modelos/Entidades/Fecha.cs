using System;

namespace Delivery.Modelos.Entidades
{
    public class Fecha
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Fecha() 
        {
            var today = DateTime.Today;
            Day = today.Day;
            Month = today.Month;
            Year = today.Year;
        }

        public Fecha(int day, int month, int year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        // Copy constructor
        public Fecha(Fecha oldFecha)
        {
            if (oldFecha != null)
            {
                this.Day = oldFecha.Day;
                this.Month = oldFecha.Month;
                this.Year = oldFecha.Year;
            }
        }

        public int getDay() { return Day; }
        public void setDay(int day) { Day = day; }
        
        public int getMonth() { return Month; }
        public void setMonth(int month) { Month = month; }
        
        public int getYear() { return Year; }
        public void setYear(int year) { Year = year; }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }

        public static Fecha FromDateTime(DateTime dt)
        {
            return new Fecha(dt.Day, dt.Month, dt.Year);
        }
    }
}
