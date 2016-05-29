using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BellRinging
{
    /// <summary>
    /// Description of a course to help in writing out compositions
    /// </summary>
    class Course
    {
        public Course()
        {
            Leads = new List<LeadDescription>();
        }

        /// <summary>
        /// The leads making up the course
        /// </summary>
        public List<LeadDescription> Leads { get; private set; }

        public string Name
        {
            get { return string.Join("", Leads.Where(c => !string.IsNullOrWhiteSpace(c.Call)).Select(c => c.CourseNameBit)); }
        }
    }
}
