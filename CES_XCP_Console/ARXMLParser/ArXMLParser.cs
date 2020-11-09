using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibArxml;

namespace CES_XCP_Console.ARXMLParser
{
    class ArXMLParser
    {

        public ArXMLParser(string xml)
        {
            LibArxml.ArxmlDocument arxmlDoc = new ArxmlDocument();
            arxmlDoc.LoadXml(xml);
            //LibArxml.Frame frame = new Frame();
            
        }
    }
}
