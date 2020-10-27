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
      const RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
      foreach (var replace in replaces)
      {
        var replaceWith = replace.Replacement;
        if (Regex.Match(replace.Replacement, randomRegex, regexOptions).Success)
        {
          replaceWith = new Random().Next().ToString();
        }
        responseBodyString = Regex.Replace(responseBodyString, replace.Pattern, replaceWith);
      }
      return responseBodyString;
    }
  }
}