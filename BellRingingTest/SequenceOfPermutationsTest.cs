using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BellRinging;

namespace BellRingingTest
{
  [TestFixture]
  public class SequenceOfPermutationsTest
  {
    [Test]
    public void InitFromString()
    {
      {
        SequenceOfPermutations seq = new SequenceOfPermutations("x 14 x 14 x 14 x ", 4);
        Row r = new Row(4);
        Assert.AreEqual("1234", r.ToString());
        List<Row> rows  = seq.Apply(r);
        Row endRow = rows[rows.Count - 1];
        Assert.AreEqual("1324", endRow.ToString());
      }

      //SequenceOfPermutations glasgow = new SequenceOfPermutations("36X56.14.58X58.36X14X38.16X16.38-18", 8);

      {
        SequenceOfPermutations glasgowHalflead = new SequenceOfPermutations("36X56.14.58X58.36X14X38.16X16.38", 8);
        Row r = new Row(8);
        Assert.AreEqual("12345678", r.ToString());
        List<Row> rows = glasgowHalflead.Apply(r);
        Row endRow = rows[rows.Count - 1];
        Assert.AreEqual("25374681", endRow.ToString());
      }

    }
  }
}
