using System;
using Xunit;
using APIMocker;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace APIMocker.Tests
{
    public class ApiMockerUnitTests 
    {
      [Fact]
      public void TestReplaceWithStaticValue()
      {
        var input = "[{\"orderNumber\":\"23\"}]";
        var result = ApiMocker.Replace(input, new List<Replace> {
          new Replace {
            Pattern = "\"[0-9]*\"", 
            Replacement="\"100\""}
          });
        Assert.Equal(
          "[{\"orderNumber\":\"100\"}]",
          result);
      }

      [Fact]
      public void TestReplaceWithRandomFunction()
      {
        var input = "[{\"orderNumber\":\"23\"}]";
        var result = ApiMocker.Replace(input, new List<Replace> {
          new Replace {
            Pattern = "\"[0-9]*\"", 
            Replacement="{Random()}"
          }
        });
        Assert.True(Regex.Match(result, @"\[\{\""orderNumber\""\:[0-9]*\}\]").Success);
      }
    }
}