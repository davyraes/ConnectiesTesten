using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;

namespace ClassLibrary1
{
    public class KleurValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || value.ToString() == string.Empty)
                return new ValidationResult(false, "Veld moet ingevuld zijn");
            return ValidationResult.ValidResult;
        }
    }
}
