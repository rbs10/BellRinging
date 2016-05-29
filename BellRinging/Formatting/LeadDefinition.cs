using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    /// <summary>
    /// Representation of a call in a composition
    /// </summary>
    public class LeadDescription
    {
        /// <summary>
        /// The name of the position at the end of the lead
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// The call made
        /// </summary>
        public string Call { get; set; }

        /// <summary>
        /// The definition of this lead to put into MicroSiril
        /// </summary>
        public string LeadDefinition { get; set; }

        public string CourseNameBit
        {
            get
            {
                string ret = string.Empty;
                if ( !string.IsNullOrWhiteSpace(Call) )
                {
                    var callBit = Call;
                    var posBit = Position;
                    if ( callBit == "b" )
                    {
                        callBit = string.Empty;
                    }
                    if (Position == "I/2")
                    {
                        if (Call == "s")
                        {
                            posBit = "U"; // "unaffected" - 2 gets used as a repeater in MicroSiril I thinkMicro
                        }
                        else
                        {
                            posBit = "I";
                        }
                    }
                    if ( callBit == "@")
                    {
                        callBit = "";
                    }
                    else if ( Call == "@" )
                    {
                        callBit = "handstrokeStart";
                    }
                    ret = callBit + posBit ;
                }
                return ret;           
            }
        }
    }
}
