using korjornalen.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace korjornalen.Utils
{
    public class OdometerStart : ValidationAttribute, IClientValidatable
    {
        private int _minValue;
        private readonly string _minProperty;

        public OdometerStart(string propertyName ) {
            _minProperty = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minProperty = validationContext.ObjectType.GetProperty(_minProperty);

            this._minValue = (int)minProperty.GetValue(validationContext.ObjectInstance, null);
            int inputValue = (int)value;

            //if (inputValue <= this._minValue)
                //return new ValidationResult(String.Format(ErrorMessage, this._minValue));

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationParameters.Add("min", _minValue);
            rule.ValidationType = "range";
            yield return rule;
        }
    }
}