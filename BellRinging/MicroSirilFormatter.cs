using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    class MicroSirilFormatter
    {
        Composition composition;

        public MicroSirilFormatter(Composition composition)
        {
            this.composition = composition;
        }

        public string WriteMicroSiril()
        {
            StringBuilder sb = new StringBuilder();
            var bells = composition.Rows.First().ToString().Length;
            sb.AppendFormat("{0} bells", bells);
            sb.AppendLine();
            sb.AppendLine();

            //foreach ( var method in methodsUsed )
            {

            }
            return sb.ToString();
        }
    }
}
