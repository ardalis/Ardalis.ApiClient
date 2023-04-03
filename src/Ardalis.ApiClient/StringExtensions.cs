using System;

namespace Ardalis.ApiClient
{
  internal static class StringExtensions
  {
    internal static T ConvertTo<T>(this string text)
    {
      if (typeof(T) == typeof(int) && int.TryParse(text, out var intResult))
      {
        return (T)Convert.ChangeType(intResult, typeof(T));
      }
      else if (typeof(T) == typeof(double) && double.TryParse(text, out var doubleResult))
      {
        return (T)Convert.ChangeType(doubleResult, typeof(T));
      }
      else if (typeof(T) == typeof(decimal) && decimal.TryParse(text, out var decimalResult))
      {
        return (T)Convert.ChangeType(decimalResult, typeof(T));
      }
      else if (typeof(T) == typeof(float) && float.TryParse(text, out var floatResult))
      {
        return (T)Convert.ChangeType(floatResult, typeof(T));
      }
      else if (typeof(T) == typeof(long) && long.TryParse(text, out var longResult))
      {
        return (T)Convert.ChangeType(longResult, typeof(T));
      }
      else if (typeof(T) == typeof(bool) && bool.TryParse(text, out var boolResult))
      {
        return (T)Convert.ChangeType(boolResult, typeof(T));
      }
      else if (typeof(T) == typeof(byte) && byte.TryParse(text, out var byteResult))
      {
        return (T)Convert.ChangeType(byteResult, typeof(T));
      }
      else if (typeof(T) == typeof(char) && char.TryParse(text, out var charResult))
      {
        return (T)Convert.ChangeType(charResult, typeof(T));
      }
      else if (typeof(T) == typeof(sbyte) && sbyte.TryParse(text, out var sbyteResult))
      {
        return (T)Convert.ChangeType(sbyteResult, typeof(T));
      }
      else if (typeof(T) == typeof(short) && short.TryParse(text, out var shortResult))
      {
        return (T)Convert.ChangeType(shortResult, typeof(T));
      }
      else if (typeof(T) == typeof(uint) && uint.TryParse(text, out var uintResult))
      {
        return (T)Convert.ChangeType(uintResult, typeof(T));
      }
      else if (typeof(T) == typeof(ulong) && ulong.TryParse(text, out var ulongResult))
      {
        return (T)Convert.ChangeType(ulongResult, typeof(T));
      }
      else if (typeof(T) == typeof(ushort) && ushort.TryParse(text, out var ushortResult))
      {
        return (T)Convert.ChangeType(ushortResult, typeof(T));
      }

      return (T)Convert.ChangeType(text, typeof(T));
    }
  }
}
