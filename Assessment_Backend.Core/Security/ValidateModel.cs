namespace Assessment_Backend.Core.Security
{
    /// <summary>
    /// اعتبارسنجی متمرکز با DataAnnotations.
    /// در حالت old-style (غیر strict) همان رفتار قبلی را دارد، اما به جای لاگ و ساختن
    /// OutPutModel خطا، یک ValidationAppException با جزئیات هر فیلد پرتاب می‌کند.
    /// </summary>
    public static class ValidateModel
    {
        /// <summary>رفتار قدیمی - برای سازگاری حفظ شد.</summary>
        public static bool Validate<T>(T model, out string validationResult)
        {
            validationResult = string.Join(" ", Validate(model, strict: false)
                .SelectMany(e => e.Value));

            return validationResult.Length == 0;
        }

        /// <summary>
        /// اعتبارسنجی استاندارد جدید: در صورت خطا، ValidationAppException پرتاب می‌کند.
        /// </summary>
        public static void ValidateOrThrow<T>(T model)
        {
            var errors = Validate(model, strict: true);
            if (errors.Count > 0)
                throw new ValidationAppException(errors);
        }

        public static Dictionary<string, string[]> Validate<T>(T model, bool strict)
        {
            var errors = new Dictionary<string, string[]>();
            if (model is null)
            {
                errors["model"] = new[] { "اطلاعات ارسال شده خالی است." };
                return errors;
            }

            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: strict))
            {
                foreach (var result in validationResults)
                {
                    var key = result.MemberNames.FirstOrDefault() ?? "model";
                    if (errors.TryGetValue(key, out var existing))
                        errors[key] = existing.Concat(result.ErrorMessage != null
                            ? new[] { result.ErrorMessage }
                            : Array.Empty<string>()).ToArray();
                    else
                        errors[key] = new[] { result.ErrorMessage ?? "مقدار نامعتبر." };
                }
            }

            return errors;
        }
    }
}
