using System.Text.RegularExpressions;


namespace FloristAI.Application.Validation
{
    public class Validator
    {
        public static bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+373\d{8}$");
        }
    }
}
