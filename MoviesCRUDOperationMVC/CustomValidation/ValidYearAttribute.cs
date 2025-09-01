using System.ComponentModel.DataAnnotations;

namespace MoviesCRUDOperationMVC.CustomValidation
{
    public class ValidYearAttribute : ValidationAttribute
    {
        private readonly int _minYear;

        public ValidYearAttribute(int minYear)
        {
            _minYear = minYear;
        }

        public override bool IsValid(object? value)
        {
            if (value is int year)
            {
                int maxYear = DateTime.Now.Year;
                if (year >= _minYear && year <= maxYear)
                {
                    return true;
                }
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Year must be between {_minYear} and {DateTime.UtcNow.Year}";
        }
    }
}
