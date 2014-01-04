using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using BellRinging;

namespace BellRingingTest
{
  [TestFixture] 
  public class FalseLeadHeads
  {
    [Test]
    public void Bastow()
    {
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Bastow","X12-18");
      c.Compose();
    }

    [Test]
    public void Cambridge()
    {
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Cambridge","X38X14X1258X36X14X58X16X78-12");
      c.Compose();
    }
    [Test]
    public void Glasgow()
    {
      Method m = new Method("Glasgow","g","36X56.14.58X58.36X14X38.16X16.38-18", 8);
      foreach (Lead l in m.GeneratePlainCourse(new Row(8)))
      {
        foreach ( Row r in m.Rows((short)l.LeadHead().ToNumber() ) )
        {
          Console.WriteLine(r);
        }
      }
      //SimpleComposer c = new SimpleComposer();
      //c.Initialise("36X56.14.58X58.36X14X38.16X16.38-18");
      //c.Compose();
    }
    [Test]
    public void CambridgeR()
    {
      RotationalBlockComposer c = new RotationalBlockComposer();
      c.Initialise("Cambridge","X38X14X1258X36X14X58X16X78-12");
      c.Compose();
    }

    [Test]
    public void Yorkshire()
    {
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Yorkshire","X38X14X58X16X12X38X14X78-12");
      c.Compose();
    }

    [Test]
    public void Bristol()
    {
      RotationalBlockComposer c = new RotationalBlockComposer();
      c.Initialise("Bristol","X58X14.58X58.36.14X14.58X14X18-18");
      c.Compose();
    }

    [Test]
    public void BristolSimpleComposer()
    {
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Bristol","X58X14.58X58.36.14X14.58X14X18-18");
      c.Compose();
    }

    [Test]
    public void BristolCambridgeSimpleComposer()
    {
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Bristol","B","X58X14.58X58.36.14X14.58X14X18-18", "Cambridge", "C", "X38X14X1258X36X14X58X16X78-12");
      c.Compose();
    }

    //[Test]
    //public void Misc()
    //{
    //  // i) print out the lead heads that are false against the plain course of Cambridge
    //  // ii) reduce/display by the course heads
    //  // iii) tabulate by coursing order
    //  // iv) what happens

    //  // generate generate changes in a plain course of Cambridge

    //  Method cambridge =    new Method("Cambridge","C","X38X14X1258X36X14X58X16X78-12", 8);
    //  Method newCambridge = new Method("X38X14X58X1236X14X58X16X78-12", 8);
    //  Method bristol = new Method("X58X14.58X58.36.14X14.58X14X18-18", 8);
    //  Method rutland = new Method("X38X14X58X16X14X38X34X18-12", 8);
    //  Method yorkshire = new Method("X38X14X58X16X12X38X14X78-12", 8);
    //  Row r = new Row(8);
    //  List<Lead> leads = cambridge.GeneratePlainCourse(r);
    //  List<Lead> allLeads = new List<Lead>(bristol.AllLeads);
    //  Assert.AreEqual(5040, allLeads.Count);

    //  Lead firstLead = allLeads[0];
    //  foreach (Lead l in allLeads)
    //  {
    //    if (l.IsFalseAgainst(firstLead))
    //    {
    //      Console.WriteLine(l.LeadHead() + " " + l.LeadHead().CoursingOrder() + " " + l.LeadHead().InCourse());
    //    }
    //  }
    //}
  }
}
