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
            List<Course> courses = MakeCourseList(composition);


            sb.AppendLine("touch = callingPositions, separator, " + string.Join(",", courses.Select(c => c.Name)) + ",suffix");

            // work out positions where calls happen
            var positionsWithCalls = courses.SelectMany(c => c.Leads).Where(l => l.HasCall).Select(l => l.Position).Distinct().ToList();
            // work out what order we want them in - want them in the order that the leads come up in the plain course
            var keyMethod = composition.Problem.Methods.First();
            var plainCourse = keyMethod.GeneratePlainCourse(Row.FromNumber(0));
            var positionsInOrder = plainCourse.Select(lead => CallingPosition(lead.NextLeadHead(keyMethod.PlainLeadEndPermutation))).ToList();
            // Home always wants to be 
            var orderedPositions = positionsInOrder.Where(p => positionsWithCalls.Contains(p)).ToList();

            // write out line with ordered positions
            var positionsString = string.Join("", orderedPositions.Select(p => p.PadRight(ColumnWidth(p), ' ')));
            sb.AppendLine(string.Format("callingPositions = \"12345678  {0}\"", positionsString));
            sb.AppendLine(string.Format("separator = \"========  {0}\"", 
                string.Empty.PadRight(positionsString.TrimEnd().Length,'=')));

            // write out line with separator

            // write out each distinct course
            foreach (var course in courses.GroupBy(c => c.Name).Select(g => g.First()))
            {
                sb.Append(course.Name + " = ");
                sb.Append(string.Join(",", course.Leads.Select(c => c.LeadDefinition)));
                sb.Append(",\"@  ");
                var lineBuilder = new StringBuilder();
                foreach ( var callingPosition in orderedPositions )
                {
                    int columnWidth = ColumnWidth(callingPosition);
                    var leadsEndingInPosition = course.Leads.Where(c => c.Position == callingPosition);
                    var callsAtPosition = leadsEndingInPosition.Select(l => l.Call);
                    var callsString = string.Join("", callsAtPosition).Replace("b","-");
                    callsString = callsString.PadRight(columnWidth, ' ');
                    lineBuilder.Append(callsString.Replace("@", "\\@"));
                }
                sb.Append(lineBuilder.ToString().TrimEnd());
                sb.AppendLine("\"");
            }

            // write out definitions of all the methods
            foreach (var method in composition.Problem.Methods)
            {
                method.WriteMicroSiril(sb);
            }

            var suffix = string.Empty;
            if ( courses.First().Leads.First().Call == "@") 
            {
                suffix = "\\@ = Start at handstroke bringing up the wrong after the first change.";
            }
            sb.AppendLine(string.Format("suffix = \"{0}\"",suffix));
            return sb.ToString();
        }

        private int ColumnWidth(string positionName)
        {
            //return positionName.Length + 2;
            return 6;
        }

        private List<Course> MakeCourseList(Composition composition)
        {
            var tables = composition.Tables;
            var callCount = tables.NO_CHOICES / tables.problem.Methods.Count();


            List<Course> courses = new List<Course>();
            Course course = null;
            var leadHeads = composition.LeadHeadsAndChoices.Select(x => x.Key).Concat(Enumerable.Range(0, 1)).ToList();
            for (int i = 0; i <= composition.maxLeadIndex; ++i)
            {
                var leadChoice = composition.choices[i];
                var method = tables._methodsByChoice[leadChoice];
                var call = leadChoice % callCount;

                if (course == null)
                {
                    course = new Course();
                    courses.Add(course);
                }
                var leadEnd = leadHeads[i + 1];
                var leadEndString = Row.FromNumber(leadEnd);
                course.Leads.Add(new LeadDescription()
                {
                    Position = CallingPosition(leadEndString),
                    Call = method.CallName(call),
                    LeadDefinition = method.WriteChoiceMicroSiril(call)
                });

                if (leadEndString.EndsWithTenor())
                {
                    course = null;
                }
            }

            return courses;
        }

        Dictionary<int, string> positions = new Dictionary<int, string>
        {
            { 1, "?! "},
            { 2, "2nds" },
            { 3 ,"B"},
            { 4, "4"},
            {5, "V"},
            {6, "M"},
            {7,"W"},
            {8,"H"}
        };

        /// <summary>
        /// Provide a name for a calling position
        /// </summary>
        /// <param name="leadEndString"></param>
        /// <returns></returns>
        private string CallingPosition(Row leadEndString)
        {
            return positions[leadEndString.ToString().IndexOf(Row.Tenor) + 1];
        }
    }
}
