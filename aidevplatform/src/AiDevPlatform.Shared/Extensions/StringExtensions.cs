using System.Text.RegularExpressions;

namespace AiDevPlatform.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Regex.Replace(value, "(?<!^)([A-Z])", "_$1").ToLowerInvariant();
    }

    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Regex.Replace(value, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }

    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }

    public static string RemoveSpecialCharacters(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return Regex.Replace(value, @"[^a-zA-Z0-9\u4e00-\u9fa5]", "");
    }

    public static string ToSlug(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Convert to lowercase
        value = value.ToLowerInvariant();

        // Remove special characters and replace spaces with hyphens
        value = Regex.Replace(value, @"[^a-z0-9\u4e00-\u9fa5\s-]", "");
        value = Regex.Replace(value, @"\s+", "-");
        value = Regex.Replace(value, @"-+", "-");

        // Remove leading and trailing hyphens
        return value.Trim('-');
    }

    public static bool IsValidEmail(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(value);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhone(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            var phoneRegex = new Regex(@"^1[3-9]\d{9}$");
            return phoneRegex.IsMatch(value);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidUrl(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            var urlRegex = new Regex(@"^https?://[^\s/$.?#].[^\s]*$");
            return urlRegex.IsMatch(value);
        }
        catch
        {
            return false;
        }
    }

    public static string MaskEmail(this string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        if (parts[0].Length <= 2)
            return email;

        var masked = parts[0][0] + new string('*', parts[0].Length - 2) + parts[0][^1];
        return $"{masked}@{parts[1]}";
    }

    public static string MaskPhone(this string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 7)
            return phone;

        return phone[..3] + new string('*', phone.Length - 6) + phone[^3..];
    }
}
