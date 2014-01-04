using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BellRinging;

namespace BellRingingTest
{
  
  [TestFixture]
  public class RowTest
  {
    [Test]
    public void CoursingOrders()
    {
      string rounds = new Row(8).ToString();
      for (int i = 1; i < 8; ++i)
      {
        Console.WriteLine(i.ToString() + " " + Row.CoursingAfter(i, 8) + " " + rounds[i] + rounds[Row.CoursingAfter(i, 8)]);
      }

    }
    [Test]
    public void Equality()
    {
      Assert.IsTrue(new Row(4).Equals(new Row(4)));
      Row r1 = new Row(4);
      Permutation p = Permutation.FromPlaceNotation("x", 4);
      Row r2 = r1.Apply(p);
      Assert.IsFalse(r1 .Equals( r2));
      Row r3 = r2.Apply(p);
      Assert.IsTrue(r1 .Equals( r3));
    }
  }
}
