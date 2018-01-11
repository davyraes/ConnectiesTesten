using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClassLibrary1
{
    public class PrijsValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal bedrag;
            NumberStyles style = NumberStyles.Currency;
            if (value == null||value.ToString()==string.Empty)
                return new ValidationResult(false, "Getal Moet ingevuld zijn");            
            if (!decimal.TryParse(value.ToString(),style,cultureInfo, out bedrag))
                return new ValidationResult(false, "Moet een getal zijn");
            if (bedrag <= 0m)
                return new ValidationResult(false, "Getal Moet groter dan 0 zijn");
            return ValidationResult.ValidResult;
        }
    }
}
