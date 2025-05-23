﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class PreconditionListTests
    {
        [TestMethod()]
        public void CloneTest()
        {
            PreconditionList o = new PreconditionList();
            PreconditionList clone = o.Clone() as PreconditionList;

            Assert.IsNotNull(clone);
            Assert.AreEqual(o.Count, clone.Count);
            Assert.AreEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreEqual(o.FalseCaseValue, clone.FalseCaseValue);

            o.Add(new Precondition());
            o.ExecuteOnFalse = true;
            o.FalseCaseValue = "1";

            Assert.AreNotEqual(o.Count, clone.Count);
            Assert.AreNotEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreNotEqual(o.FalseCaseValue, clone.FalseCaseValue);

            clone = o.Clone() as PreconditionList;
            Assert.AreEqual(o.Count, clone.Count);
            Assert.AreEqual(o.ExecuteOnFalse, clone.ExecuteOnFalse);
            Assert.AreEqual(o.FalseCaseValue, clone.FalseCaseValue);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsNotNull(o.GetEnumerator());
            Assert.IsTrue(o.GetEnumerator() is List<Precondition>.Enumerator);
        }

        [TestMethod()]
        public void AddTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsTrue(o.Count == 0);
            o.Add(new Precondition());
            Assert.IsTrue(o.Count == 1);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            PreconditionList o = new PreconditionList();
            Precondition p = new Precondition();
            o.Add(p);
            Assert.IsTrue(o.Count == 1);
            o.Remove(p);
            Assert.IsTrue(o.Count == 0);
        }

        [TestMethod()]
        public void GetSchemaTest()
        {
            PreconditionList o = new PreconditionList();
            Assert.IsNull(o.GetSchema());
        }

        [TestMethod()]
        public void ReadXmlTest()
        {
            PreconditionList o = new PreconditionList();
            String s = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\ReadXmlTest.1.xml");
            StringReader sr = new StringReader(s);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("preconditions");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Count, 0);

            o = new PreconditionList();
            s = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\ReadXmlTest.2.xml");
            sr = new StringReader(s);
            settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            xmlReader = System.Xml.XmlReader.Create(sr, settings);
            xmlReader.ReadToDescendant("preconditions");
            o.ReadXml(xmlReader);

            Assert.AreEqual(o.Count, 2);
            Assert.AreEqual(o.ExecuteOnFalse, false);
            Assert.AreEqual(o.FalseCaseValue, "");
        }

        [TestMethod()]
        public void WriteXmlTest()
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(sw, settings);

            PreconditionList o = new PreconditionList();
            o.Add(new Precondition() { 
                PreconditionType = "config",
                PreconditionLabel = "TestPreCon",
                PreconditionActive = true,
                PreconditionRef = "TestRef",
                PreconditionOperand = "<",
                PreconditionValue = "TestValue",
                PreconditionLogic = "or"
            });
            o.ExecuteOnFalse = true;
            o.FalseCaseValue = "1";
            o.WriteXml(xmlWriter);

            xmlWriter.Flush();
            string s = sw.ToString();

            String result = System.IO.File.ReadAllText(@"assets\Base\PreconditionList\WriteXmlTest.1.xml");

            Assert.AreEqual(s, result, "The both strings are not equal");
        }

        [TestMethod()]
        public void EqualsTest()
        {
            PreconditionList o1 = new PreconditionList();
            PreconditionList o2 = new PreconditionList();

            Assert.IsTrue(o1.Equals(o2));
            o1.Add(new Precondition());
            o1.ExecuteOnFalse = true;
            o1.FalseCaseValue = "1";

            Assert.IsFalse(o1.Equals(o2));

            o2.Add(new Precondition());
            o2.ExecuteOnFalse = true;
            o2.FalseCaseValue = "1";

            Assert.IsTrue(o1.Equals(o2));
        }

        [TestMethod()]
        public void JsonSerializationDeserializationTest()
        {
            var originalList = new PreconditionList
            {
                new Precondition
                {
                    PreconditionType = "config",
                    PreconditionLabel = "TestPreCon1",
                    PreconditionActive = true,
                    PreconditionRef = "TestRef1",
                    PreconditionOperand = "<",
                    PreconditionValue = "TestValue1",
                    PreconditionLogic = "or"
                },
                new Precondition
                {
                    PreconditionType = "config",
                    PreconditionLabel = "TestPreCon2",
                    PreconditionActive = false,
                    PreconditionRef = "TestRef2",
                    PreconditionOperand = ">",
                    PreconditionValue = "TestValue2",
                    PreconditionLogic = "and"
                }
            };

            string json = JsonConvert.SerializeObject(originalList);
            PreconditionList deserializedList = JsonConvert.DeserializeObject<PreconditionList>(json);

            Assert.IsNotNull(deserializedList);
            Assert.AreEqual(originalList.Count, deserializedList.Count);
            
            var originalArray = originalList.ToArray();
            var deserializedArray = deserializedList.ToArray();

            for (int i = 0; i < originalList.Count; i++)
            {
                Assert.AreEqual(originalArray[i].PreconditionType, deserializedArray[i].PreconditionType);
                // labels are not serialized
                Assert.AreNotEqual(originalArray[i].PreconditionLabel, deserializedArray[i].PreconditionLabel);
                // labels are using specifc format
                var PreconditionRef = originalArray[i].PreconditionRef;
                var PreconditionOperand = originalArray[i].PreconditionOperand;
                var PreconditionValue = originalArray[i].PreconditionValue;
                var PreconditionLogic = originalArray[i].PreconditionLogic;                
                var exptectedLabel = $"Config: <Ref:{PreconditionRef}> {PreconditionOperand} {PreconditionValue} <Logic:{PreconditionLogic}>";
                Assert.AreEqual(deserializedArray[i].PreconditionLabel, exptectedLabel);
                Assert.AreEqual(originalArray[i].PreconditionActive, deserializedArray[i].PreconditionActive);
                Assert.AreEqual(originalArray[i].PreconditionRef, deserializedArray[i].PreconditionRef);
                Assert.AreEqual(originalArray[i].PreconditionOperand, deserializedArray[i].PreconditionOperand);
                Assert.AreEqual(originalArray[i].PreconditionValue, deserializedArray[i].PreconditionValue);
                Assert.AreEqual(originalArray[i].PreconditionLogic, deserializedArray[i].PreconditionLogic);
            }
        }
    }
}