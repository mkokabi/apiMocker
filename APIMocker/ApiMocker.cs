using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace APIMocker
{
  public static class ApiMocker
  {
    public static string Replace(string responseBodyString, List<Replace> replaces)
    {
      const string randomRegex = @"\{\s*Random\s*\(\s*(?:[0-9]*|[0-9]\s*\,\s*[0-9]*)\s*\)\s*}";
      const string randomHeadRegex = @"\{\s*Random\s*\(";
      char[] randomTailChars = {' ', ')', '}'};
      const RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
      var random = new Random();
      foreach (var replace in replaces)
      {
        var replaceWith = replace.Replacement;
        if (Regex.Match(replace.Replacement, randomRegex, regexOptions).Success)
        {
          var numbersPart = Regex.Replace(replace.Replacement, randomHeadRegex, "").TrimEnd(randomTailChars);
          var numbers = numbersPart.Split(',', StringSplitOptions.RemoveEmptyEntries);
          
          replaceWith = (numbers.Length == 2 && int.TryParse(numbers[0], out int minValue)
          && int.TryParse(numbers[1], out int maxValue) && (minValue < maxValue)) ? 
            random.Next(minValue, maxValue).ToString() :
            (
              (numbers.Length == 1 && int.TryParse(numbers[0], out maxValue)) ? 
              random.Next(maxValue).ToString() : random.Next().ToString()
            );
        }
        responseBodyString = Regex.Replace(responseBodyString, replace.Pattern, replaceWith);
      }
      return responseBodyString;
    }
  }
}