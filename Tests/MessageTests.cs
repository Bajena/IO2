using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Common.Abstractions;
using Common.Enumerations;
using Common.Helpers.XmlValidator;
using Common.Models.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Common.Models;

namespace Tests
{
    [TestClass]
    public class MessageTests
    {
        private const string _xsdSchemasPath = "../../../Common/XsdSchemas/";

        [TestMethod]
        public void RegisterMessageSerializationTest()
        {
            var problemList = new List<string>() { "DVRP" };
            var registerMessage = new RegisterMessage(EComponentType.TaskManager, 1, problemList);

            var result = registerMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "RegisterMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath,true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0,errorsCount);

            #region ExampleResult

            //<?xml version="1.0" encoding="utf-16"?>
            //<Register xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <Type>TaskManager</Type>
            //  <SolvableProblems>
            //    <ProblemName>DVRP</ProblemName>
            //  </SolvableProblems>
            //  <ParallelThreads>1</ParallelThreads>
            //</Register>

            #endregion
        }

        [TestMethod]
        public void RegisterMessageDeserializationTest()
        {
            var problemList = new List<string>() { "DVRP" };
            var registerMessage = new RegisterMessage(EComponentType.TaskManager, 1, problemList);

            var result = Message.Deserialize<RegisterMessage>(registerMessage.SerializeToXml());
            
            Assert.AreEqual("DVRP",result.SolvableProblems[0]);
        }

        [TestMethod]
        public void RegisterResponseMessageSerializationTest()
        {
            var registerResponseMessage = new RegisterResponseMessage(123123123L, DateTime.Now);

            var result = registerResponseMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "RegisterResponseMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath,true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<RegisterResponse xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <Id>123123123</Id>
            //  <Timeout>2014-03-07T23:25:05.0728496+01:00</Timeout>
            //</RegisterResponse>
            #endregion
        }

        [TestMethod]
        public void StatusMessageSerializationTest()
        {
            IOThread ioThread = new IOThread
            {
                State = EState.Busy,
                ProblemInstanceId = 123L,
                ProblemType = "ProblemType",
                TaskId = 321L
            };

            var threadList = new List<IOThread>();
            threadList.Add(ioThread);
            var registerStatusMessage = new StatusMessage(123L, threadList);

            var result = registerStatusMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "StatusMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<Status xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <Id>123</Id>
            //  <Threads>
            //    <IOThread>
            //      <State>Busy</State>
            //      <HowLong>00:00:00</HowLong>
            //      <ProblemInstanceId>123</ProblemInstanceId>
            //      <TaskId>321</TaskId>
            //      <ProblemType>ProblemType</ProblemType>
            //    </IOThread>
            //  </Threads>
            //</Status>
            #endregion
        }

        [TestMethod]
        public void DivideProblemMessageSerializationTest()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Test Byte Array");
            var divideProblemMessageMessage = new DivideProblemMessage("Problem Type", 123L, byteArray, 321L);

            var result = divideProblemMessageMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "DivideProblemMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<DivideProblem xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <ProblemType>Problem Type</ProblemType>
            //  <Id>123</Id>
            //  <Data>VGVzdCBCeXRlIEFycmF5</Data>
            //  <ComputationalNodes>321</ComputationalNodes>
            //</DivideProblem>
            #endregion
        }

        [TestMethod]
        public void SolveRequestResponseMessageSerializationTest()
        {
            var solveRequestResponseMessage = new SolveRequestResponseMessage(123L);

            var result = solveRequestResponseMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "SolveRequestResponseMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<SolveRequestResponse xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <Id>123</Id>
            //</SolveRequestResponse>
            #endregion
        }

        [TestMethod]
        public void SolutionRequestMessageSerializationTest()
        {
            var solutionRequestMessage = new SolutionRequestMessage(123L);

            var result = solutionRequestMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "SolutionRequestMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<SolutionRequest xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <Id>123</Id>
            //</SolutionRequest>
            #endregion
        }

        [TestMethod]
        public void SolveRequestMessageSerializationTest()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Test Byte Array");
            var solveRequestMessage = new SolveRequestMessage("Problem Type", 122L, byteArray);

            var result = solveRequestMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "SolveRequestMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<SolveRequest xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <ProblemType>Problem Type</ProblemType>
            //  <SolvingTimeout>122</SolvingTimeout>
            //  <Data>VGVzdCBCeXRlIEFycmF5</Data>
            //</SolveRequest>
            #endregion
        }

        [TestMethod]
        public void SolutionsMessageSerializationTest()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Test Byte Array");

            Solution solution = new Solution
            {
                ComputationsTime = 122L,
                Data = byteArray,
                SolutionType = ESolutionType.Final,
                TaskId = 123L,
                TimeoutOccured = true
            };

            List<Solution> solutions = new List<Solution>();
            solutions.Add(solution);

            var solutionsMessage = new SolutionsMessage("Problem Type", 123L, byteArray, solutions);

            var result = solutionsMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "SolutionsMessage.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<Solutions xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <ProblemType>Problem Type</ProblemType>
            //  <Id>123</Id>
            //  <CommonData>VGVzdCBCeXRlIEFycmF5</CommonData>
            //  <Solutions>
            //    <Solution>
            //      <TaskId>123</TaskId>
            //      <TimeoutOccured>true</TimeoutOccured>
            //      <Type>Final</Type>
            //      <ComputationsTime>00:00:00</ComputationsTime>
            //      <Data>VGVzdCBCeXRlIEFycmF5</Data>
            //    </Solution>
            //  </Solutions>
            //</Solutions>
            #endregion
        }

        [TestMethod]
        public void SolvePartialProblemsSerializationTest()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Test Byte Array");

            PartialProblem partialProblem = new PartialProblem
            {
                Data = byteArray,
                TaskId = 123L,
            };

            List<PartialProblem> partialProblems = new List<PartialProblem>();
            partialProblems.Add(partialProblem);

            var partialProblemsMessage = new SolvePartialProblemsMessage("Problem Type", 123L, byteArray, 122L, partialProblems);

            var result = partialProblemsMessage.SerializeToXml();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Length);

            var xmlValidator = new XmlValidator();
            var xsdSchemaFile = "SolvePartialProblems.xsd";
            var xsdSchemaPath = Path.Combine(_xsdSchemasPath, xsdSchemaFile);
            var validationResult = xmlValidator.Validate(result, xsdSchemaPath, true);
            var errorsCount = validationResult.Errors.Count + validationResult.Warnings.Count;
            Assert.AreEqual(0, errorsCount);

            #region ExampleResult
            //<?xml version="1.0" encoding="utf-16"?>
            //<PartialProblems xmlns="http://www.mini.pw.edu.pl/ucc/">
            //  <ProblemType>Problem Type</ProblemType>
            //  <Id>123</Id>
            //  <CommonData>VGVzdCBCeXRlIEFycmF5</CommonData>
            //  <SolvingTimeout>122L</SolvingTimeout>
            //  <PartialProblems>
            //    <PartialProblem>
            //      <TaskId>123</TaskId>
            //      <Data>VGVzdCBCeXRlIEFycmF5</Data>
            //    </PartialProblem>
            //  </PartialProblems>
            //</PartialProblems>
            #endregion

        }

    }
}
