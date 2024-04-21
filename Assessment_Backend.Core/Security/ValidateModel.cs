namespace Assessment_Backend.Core.Security
{
    public class ValidateModel
    {
        public static bool Validate<T>(T model, out string validationResult)
        {
            validationResult = "";

            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                foreach (var validationResultItem in validationResults)
                {
                    validationResult += validationResultItem.ErrorMessage + " ";
                }

                return false;
            }

            return true;
        }
    }
}
