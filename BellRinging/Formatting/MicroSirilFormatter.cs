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
            List<Course> courses = MakeCourseList(composition);

           
            sb.AppendLine("touch = " + string.Join(",", courses.Select(c => c.Name)));

            // write out each distinct course
            foreach (var course in courses.GroupBy(c => c.Name).Select(g => g.First()))
            {
                sb.Append(course.Name + " = ");
                sb.Append(string.Join(",", course.Leads.Select(c => c.LeadDefinition)));
                sb.AppendLine(",\"@\"");
            }

            // write out definitions of all the methods
            foreach (var method in composition.Problem.Methods)
            {
                method.WriteMicroSiril(sb);
            }

            return sb.ToString();
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
            { 2, "I/2" },
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
