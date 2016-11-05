using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Common.Models.Messages;

namespace Common.Abstractions
{
    [Serializable]
    public abstract class Message
    {
        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(SerializeToXml());
        }

        public virtual string SerializeToXml()
        {

            var xmlSerializer = new XmlSerializer(GetType());
            var xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, "http://www.mini.pw.edu.pl/ucc/");
            
            //TextWriter writer = new StringWriter();
            //xmlSerializer.Serialize(writer, this,xmlNamespaces);

            //UTF8 Change
            MemoryStream memStrm = new MemoryStream();
            memStrm.Flush();
            XmlTextWriter writer = new XmlTextWriter(memStrm, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;

            xmlSerializer.Serialize(writer, this, xmlNamespaces);
            memStrm = (MemoryStream)writer.BaseStream;
            string resultString = Encoding.UTF8.GetString(memStrm.ToArray());
            
            //removing BOM symbol
            int i =0;
            while (resultString[i] != '<' && ++i >= resultString.Length) ;
            if (i <= resultString.Length) resultString = resultString.Substring(i);
          
            return resultString;
        }

        public static Type GetMessageType(string message)
        {
            XmlDocument xmlMessage = new XmlDocument();
            string messageName = string.Empty;
            try
            {
                xmlMessage.LoadXml(message);
                messageName = xmlMessage.ChildNodes[1].Name;
            }
            catch (Exception)
            {
                throw;
            }

            switch (messageName)
            {
                case "Register":
                    return typeof(RegisterMessage);
                case "RegisterResponse":
                    return typeof(RegisterResponseMessage);
                case "DivideProblem":
                    return typeof(DivideProblemMessage);
                case "SolutionRequest":
                    return typeof(SolutionRequestMessage);
                case "SolvePartialProblems":
                    return typeof(SolvePartialProblemsMessage);
                case "SolveRequest":
                    return typeof(SolveRequestMessage);
                case "Solutions":
                    return typeof(SolutionsMessage);
                case "SolveRequestResponse":
                    return typeof(SolveRequestResponseMessage);
                case "Status":
                    return typeof(StatusMessage);
            }

            throw new Exception("Nieznany typ wiadomości");
        }

        public static T Deserialize<T>(string message)
        {
            Type messageType = Message.GetMessageType(message);
            XmlSerializer serializer = new XmlSerializer(messageType);

            using (TextReader reader = new StringReader(message))
            {
                //Ku pamięci xD
                try
                {
                    return (T)serializer.Deserialize(reader);
                }
                catch(Exception e)
                {
                    throw;
                }
            }
        }
    }
}
