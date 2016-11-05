using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace DllProject.Helpers.XmlValidator
{
    public enum XmlValidationType
    {
        Strict,
        PassWarnings
    }

    public class XmlValidator
    {

        private List<string> _lastValidationErrors = new List<string>();
        private List<string> _lastValidationWarnings = new List<string>();
        private XmlDocument _xmlDocument;
        private XmlSchema _xmlSchema;

        public XmlValidationType XmlValidationType { get; set; }

        public XmlValidator()
        {
            XmlValidationType = XmlValidationType.Strict;
        }

        public bool IsValid()
        {
            _xmlDocument.Schemas.Add(_xmlSchema);
            _xmlDocument.Validate(ValidationCallback);

            if (XmlValidationType == XmlValidationType.Strict && (_lastValidationWarnings.Count > 0 || _lastValidationErrors.Count > 0) ||
            (XmlValidationType == XmlValidationType.PassWarnings && _lastValidationErrors.Count > 0))
                return false;

            return true;
        }

        public XmlValidationResult Validate(string xmlContent, string xsdString, bool loadSchemaFromFile)
        {
            _lastValidationErrors = new List<string>();
            _lastValidationWarnings = new List<string>();

            try
            {
                _xmlDocument = ReadXmlFromString(xmlContent);
                _xmlSchema = loadSchemaFromFile ? ReadXsdFromFile(xsdString) : ReadXsdFromString(xsdString);
            }
            catch (Exception xcp)
            {
                _lastValidationErrors.Add(xcp.Message);
            }
            bool isValid = IsValid();
            return new XmlValidationResult(isValid, _lastValidationErrors, _lastValidationWarnings);
        }

        public XmlValidationResult Validate(string xmlContent, XmlSchema xmlSchema)
        {
            _lastValidationErrors = new List<string>();
            _lastValidationWarnings = new List<string>();

            try
            {
                _xmlDocument = ReadXmlFromString(xmlContent);
                _xmlSchema = xmlSchema;
            }
            catch (Exception xcp)
            {
                _lastValidationErrors.Add(xcp.Message);
            }
            bool isValid = IsValid();
            return new XmlValidationResult(isValid, _lastValidationErrors, _lastValidationWarnings);
        }

        public bool IsValidXml(string text)
        {
            try
            {
                ReadXmlFromString(text);
            }
            catch (XmlException)
            {
                return false;
            }
            return true;
        }

        private XmlDocument ReadXmlFromString(string xml)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                return xmlDocument;
            }
            catch (Exception)
            {
                throw new XmlException("Podany dokument nie jest poprawnym plikiem .xml");
            }
        }


        private XmlSchema ReadXsdFromString(string xsd)
        {
            try
            {
                XmlSchema xmlSchema = XmlSchema.Read(new StringReader(xsd), ValidationCallback);

                return xmlSchema;
            }
            catch (Exception)
            {
                throw new XmlSchemaException("Podany dokument nie jest poprawnym plikiem .xsd");
            }
        }

        private XmlDocument ReadXmlFromFile(string xsdFile)
        {
            try
            {
                var xmlDocument = new XmlDocument();

                xmlDocument.Load(xsdFile);
                return xmlDocument;
            }
            catch (Exception)
            {
                throw new XmlException("Plik: " + xsdFile + " nie jest poprawnym plikiem .xml");
            }
        }

        private XmlSchema ReadXsdFromFile(string xsdFile)
        {
            try
            {
                var xmlReader = new XmlTextReader(xsdFile);
                XmlSchema xmlSchema = XmlSchema.Read(xmlReader, ValidationCallback);

                return xmlSchema;
            }
            catch (Exception)
            {
                throw new XmlSchemaException("Plik: " + xsdFile + " nie jest poprawnym plikiem .xsd");
            }
        }
        private void ValidationCallback(object sender, ValidationEventArgs args)
        {

            if (args.Severity == XmlSeverityType.Warning)
                _lastValidationWarnings.Add(args.Message);
            else if (args.Severity == XmlSeverityType.Error)
                _lastValidationErrors.Add(args.Message);
        }
    }
}
