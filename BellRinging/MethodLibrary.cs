using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BellRinging
{
    /// <summary>
    /// Provides place notation for methods
    /// </summary>
    /// <remarks>
    /// Current version relies on finding method library on disk in hard-coded locationS</remarks>
    public class MethodLibrary
    {
        XmlDocument _doc;

        /// <summary>
        /// Constructor
        /// </summary>
        public MethodLibrary()
        {
            _doc = new XmlDocument();
            _doc.Load(@"C:\Users\rober\Downloads\surprise-xml\surprise.xml");
    }

    public IEnumerable<string> MethodNames
        {
            get
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(_doc.NameTable);
                nsmgr.AddNamespace("x", "http://www.cccbr.org.uk/methods/schemas/2007/05/methods");
                string path =
                  string.Format(@"*//x:method/x:title");
                //while (true)
                //{
                //  XmlNodeList list = _doc.SelectNodes(path,nsmgr);
                //  Console.WriteLine(list.Count);
                //}
                var nodes = _doc.SelectNodes(path, nsmgr);
                foreach (XmlNode node in nodes)
                {
                    var name = node.InnerText;
                    var suffix = " Surprise Major";
                    if (name.EndsWith(suffix))
                    {
                        yield return name.Replace(suffix, "");
                    }
                }
            }
        }
        /// <summary>
        /// Return the place notation for a specified methid
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public string GetNotation(string methodName)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(_doc.NameTable);
            nsmgr.AddNamespace("x", "http://www.cccbr.org.uk/methods/schemas/2007/05/methods");
            string path =
              string.Format(@"*//x:method[x:title=""{0} Surprise Major""]/x:notation", methodName);
            //while (true)
            //{
            //  XmlNodeList list = _doc.SelectNodes(path,nsmgr);
            //  Console.WriteLine(list.Count);
            //}
            XmlNode node = _doc.SelectSingleNode(path, nsmgr);
            string baseNotation = node.InnerText;
            string notation = baseNotation.Replace('-', 'X').Replace(',', '-');
            return notation;
        }
    }
}
