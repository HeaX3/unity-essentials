using System.Globalization;
using UnityEngine;

namespace Essentials
{
    public static class NumberExtensions
    {
        private static NumberFormatInfo _numberFormat;


        private static NumberFormatInfo numberFormat
        {
            get
            {
                if (_numberFormat == null)
                {
                    var numberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                    numberFormat.NumberGroupSeparator = "'";
                    numberFormat.NumberDecimalSeparator = ".";
                    _numberFormat = numberFormat;
                }

                return _numberFormat;
            }
        }

        public static string FormatNumber(this int number)
        {
            return number.ToString("N0", numberFormat);
        }

        public static string FormatNumber(this uint number)
        {
            return number.ToString("N0", numberFormat);
        }

        public static string FormatNumberCompressed(this uint number)
        {
            if (number >= 1000000000)
            {
                var value = (number / 1000000000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "B";
            }

            if (number >= 1000000)
            {
                var value = (number / 1000000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "M";
            }

            if (number >= 1000)
            {
                var value = (number / 1000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "K";
            }

            return FormatNumber(number);
        }

        public static string FormatNumberCompressed(this int number)
        {
            if (number >= 1000000000)
            {
                var value = (number / 1000000000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "B";
            }

            if (number >= 1000000)
            {
                var value = (number / 1000000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "M";
            }

            if (number >= 1000)
            {
                var value = (number / 1000f);
                return (Mathf.Abs(value - Mathf.RoundToInt(value)) > 0
                    ? value.ToString("N", numberFormat)
                    : value.ToString("N0", numberFormat)) + "K";
            }

            return FormatNumber(number);
        }
    }
}