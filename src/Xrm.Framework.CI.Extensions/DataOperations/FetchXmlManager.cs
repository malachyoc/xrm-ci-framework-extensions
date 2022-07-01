using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    public class FetchXmlManager
    {
        public const string CONST_PAGINGCOOKIE = "paging-cookie";

        public static string LineriseFetchXml(string xml, string pagingToken, int page, int count)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);


                XmlAttributeCollection attrs = document.DocumentElement.Attributes;
                if (pagingToken != null)
                {
                    XmlAttribute pagingAttr = document.CreateAttribute("paging-cookie");
                    pagingAttr.Value = pagingToken;
                    attrs.Append(pagingAttr);
                }

                XmlAttribute pageAttr = document.CreateAttribute("page");
                pageAttr.Value = Convert.ToString(page);
                attrs.Append(pageAttr);

                XmlAttribute countAttr = document.CreateAttribute("count");
                countAttr.Value = Convert.ToString(count);
                attrs.Append(countAttr);

                writer.Formatting = Formatting.None;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
                throw;
            }
            finally
            {

                mStream.Close();
                writer.Close();
            }

            return result;
        }
    }
}
