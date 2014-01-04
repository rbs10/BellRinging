using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using BellRinging;

namespace BellRingingTest
{
  
  [TestFixture]
  public class PermutationTest
  {
    [Test]
    public void SimplePermuations()
    {
      Permutation x = Permutation.FromPlaceNotation("x", 8);
      Assert.AreEqual(x.Apply("12345678"),"21436587");

      Permutation P34 = Permutation.FromPlaceNotation("34", 8);
      Assert.AreEqual(P34.Apply("12345678"), "21346587");

      Permutation P18 = Permutation.FromPlaceNotation("18", 8);
      Assert.AreEqual(P18.Apply("12345678"), "13254768");

      Permutation P36 = Permutation.FromPlaceNotation("36", 8);
      Assert.AreEqual(P36.Apply("12345678"), "21354687");

      Permutation r3 = Permutation.FromRotation(1, 3, 8);
      Assert.AreEqual(r3.Apply("12345678"), "13425678");


      Permutation r8 = Permutation.FromRotation(0, 8, 8);
      Assert.AreEqual(r8.Apply("12345678"), "23456781");
    }
  }


}
