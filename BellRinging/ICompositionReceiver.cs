using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  public interface ICompositionReceiver
  {
    /// <summary>
    /// Add a composition to the receiver
    /// </summary>
    /// <param name="c"></param>
    void AddComposition(Composition c);
  }
}
