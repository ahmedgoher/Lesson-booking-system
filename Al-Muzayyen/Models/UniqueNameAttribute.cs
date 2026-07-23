using System.ComponentModel.DataAnnotations;

namespace Al_Muzayyen.Models
{
    public class UniqueNameAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null) return null;
            string newName = value.ToString().Trim();
            // 1. Get AppDbContext directly from Dependency Injection Service Provider
            var context = validationContext.GetService<AppDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("AppDbContext is not registered in Dependency Injection.");
            }
            Student? std = context.Students.FirstOrDefault(s => s.Name == newName);
            if (std != null)
            {
                return new ValidationResult($"{newName} مسجل قبل كده في المنصة بنفس الاسم");
            }
            return ValidationResult.Success;
        }
    }
}
