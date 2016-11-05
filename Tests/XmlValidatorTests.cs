using System;
using Common.Helpers.XmlValidator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class XmlValidatorTests
    {
        [TestMethod]
        public void ValidateCorrectFile()
        {
            var testXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><testElement xmlns=\"http://tempuri.org/XMLSchema.xsd\"><element1>element11</element1><element2>1</element2></testElement>";
            var testXsd = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xs:schema targetNamespace=\"http://tempuri.org/XMLSchema.xsd\" elementFormDefault=\"qualified\" xmlns=\"http://tempuri.org/XMLSchema.xsd\" xmlns:mstns=\"http://tempuri.org/XMLSchema.xsd\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><xs:element name=\"testElement\"><xs:complexType><xs:sequence><xs:element name=\"element1\" type=\"xs:string\"/><xs:element name=\"element2\" type=\"xs:integer\"/></xs:sequence></xs:complexType></xs:element></xs:schema>";
            var xmlValidator = new XmlValidator();

            var result = xmlValidator.Validate(testXml, testXsd, false);
            var errorsCount = result.Errors.Count + result.Warnings.Count;

            Assert.AreEqual(errorsCount, 0);

        }

        [TestMethod]
        public void ValidateIncorrectFile()
        {
            var testXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><testElement xmlns=\"http://tempuri.org/XMLSchema.xsd\"><element1>element11</element1><element2>aaaa</element2></testElement>";
            var testXsd = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xs:schema targetNamespace=\"http://tempuri.org/XMLSchema.xsd\" elementFormDefault=\"qualified\" xmlns=\"http://tempuri.org/XMLSchema.xsd\" xmlns:mstns=\"http://tempuri.org/XMLSchema.xsd\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><xs:element name=\"testElement\"><xs:complexType><xs:sequence><xs:element name=\"element1\" type=\"xs:string\"/><xs:element name=\"element2\" type=\"xs:integer\"/></xs:sequence></xs:complexType></xs:element></xs:schema>";
            var xmlValidator = new XmlValidator();

            var result = xmlValidator.Validate(testXml, testXsd, false);
            var errorsCount = result.Errors.Count + result.Warnings.Count;

            Assert.AreNotEqual(errorsCount, 0);
        }
    }
}
