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
        Assert.Matches(@"\[\{\""orderNumber\""\:[0-9]*\}\]", result);
      }

      [Fact]
      public void TestReplaceWithDateTimeFunction()
      {
        var input = "[{\"orderNumber\":\"23\"}]";
        var result = ApiMocker.Replace(input, new List<Replace> {
          new Replace {
            Pattern = "\"[0-9]*\"", 
            Replacement="{DateTime(yyyy-MM-dd hh:mm:ss)}"
          }
        });
        Assert.Matches(@"\[\{\""orderNumber\""\:[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}\}\]", result);
      }

      [Fact]
      public void TestReplaceWithSeqenct()
      {
        var input = "[{\"orderNumber\":\"23\"}]";
        for (int i = 0; i < 5; i++)
        {
          var result = ApiMocker.Replace(input, new List<Replace> {
            new Replace {
              Pattern = "\"[0-9]*\"", 
              Replacement="\"{Sequence()}\""
            }
          });
          Assert.Equal("[{\"orderNumber\":" + (i+1) +"}]", result);
        }
      }

      [Fact]
      public void TestReplaceWithNamedSeqenct()
      {
        var input = "[{\"orderNumber\":\"23\"}]";
        var a = 0;
        var b = 0;
        for (int i = 0; i < 10; i++)
        {
          var key = (i % 2 == 0) ? "a" : "b";
          var result = ApiMocker.Replace(input, new List<Replace> {
            new Replace {
              Pattern = "\"[0-9]*\"", 
              Replacement="\"{Sequence(" + key + ")}\""
            }
          });
          Assert.Equal("[{\"orderNumber\":" + (key == "a" ? ++a : ++b) +"}]", result);
        }
      }
    }
}