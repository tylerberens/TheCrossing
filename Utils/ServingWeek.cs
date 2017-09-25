using System;
using Rock;

namespace com.bricksandmortarstudio.TheCrossing.Utils
{
    public static class ServingWeek
    {
        private static int GetWeekNumber()
        {
            const DayOfWeek day = DayOfWeek.Monday;
            var start = new DateTime(2017, 08, 25);
            var end = RockDateTime.Today;
            var ts = end - start;                       // Total duration
            int count = ( int ) Math.Floor( ts.TotalDays / 7 );   // Number of whole weeks
            int remainder = ( int ) ( ts.TotalDays % 7 );         // Number of remaining days
            int sinceLastDay = end.DayOfWeek - day;   // Number of days since last [day]
            if ( sinceLastDay < 0 )
                sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if ( remainder >= sinceLastDay )
                count++;

            return count;
        }

        public static int GetTeamNumber()
        {
            return GetWeekNumber()%2 == 0 ? 1 : 2;
        }
    }
}
