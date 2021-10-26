using System;
using System.Collections.Generic;
using Xunit;

namespace Ardalis.ApiClient.UnitTests
{
  public class DictionaryAddIfNotNull
  {
    private Dictionary<string, object> _dictionary = new();

    [Fact]
    public void DoesNothingGivenNullValue()
    {
      _dictionary.AddIfNotNull("key", null);

      Assert.Empty(_dictionary.Values);
    }

    [Fact]
    public void AddsNonNullValue()
    {
      _dictionary.AddIfNotNull("key", "val");

      var result = _dictionary["key"];

      Assert.Equal("val", result);
    }
  }
}
