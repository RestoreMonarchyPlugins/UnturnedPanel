using System;
using System.Text.RegularExpressions;

namespace RestoreMonarchy.WebUnturnedPanel.Helpers
{
    public static class FormattingHelper
    {
        public static ulong GetSteamId(this string value)
        {
            if (!ulong.TryParse(Regex.Match(value, @"\d+").Value, out ulong returnValue))
            {
                returnValue = 0;
            }

            return returnValue;
        }

        public static string ToPrettyFormat(this int? duration)
        {
            if (duration.HasValue)
                return TimeSpan.FromSeconds(duration.Value).ToPrettyFormat();
            else
                return "permanent";
        }

        public static string ToPrettyFormat(this TimeSpan span)
        {
            if (span == null || span < TimeSpan.Zero)
                return "permanent";

            string formatted = string.Format("{0}{1}{2}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "< 1 minute";

            return formatted;
        }
    }
}
