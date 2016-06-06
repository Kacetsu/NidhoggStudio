﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class PropertyTest {

        [TestMethod]
        public void Property_InToleranceIntegerProperty() {
            IntegerProperty property = new IntegerProperty("IntegerProperty", 20, 0, 30);
            Assert.IsTrue(property.InTolerance);
        }

        [TestMethod]
        public void Property_OutToleranceIntegerProperty() {
            IntegerProperty property = new IntegerProperty("IntegerProperty", 40, 0, 30);
            Assert.IsFalse(property.InTolerance);

            property = new IntegerProperty("IntegerProperty", -40, 0, 30);
            Assert.IsFalse(property.InTolerance);
        }

        /// <summary>
        /// Saves the load integer property.
        /// </summary>
        [TestMethod]
        public void Property_SaveLoadIntegerProperty() {
            MemoryStream stream = new MemoryStream();
            IntegerProperty property = new IntegerProperty("IntegerProperty", 20);

            XmlSerializer serializer = new XmlSerializer(property.GetType());
            stream = new MemoryStream();
            serializer.Serialize(stream, property);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            Trace.WriteLine(str);

            property = null;
            stream.Position = 0;
            serializer = new XmlSerializer(typeof(IntegerProperty));
            property = serializer.Deserialize(stream) as IntegerProperty;
            Assert.AreEqual(property.Value, 20);
        }

        /// <summary>
        /// Saves the load double property.
        /// </summary>
        [TestMethod]
        public void Property_SaveLoadDoubleProperty() {
            MemoryStream stream = new MemoryStream();
            DoubleProperty property = new DoubleProperty("DoubleProperty", 543.21);

            XmlSerializer serializer = new XmlSerializer(property.GetType());
            stream = new MemoryStream();
            serializer.Serialize(stream, property);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            Trace.WriteLine(str);

            property = null;
            stream.Position = 0;
            serializer = new XmlSerializer(typeof(DoubleProperty));
            property = serializer.Deserialize(stream) as DoubleProperty;
            Assert.AreEqual(property.Value, 543.21);
        }

        /// <summary>
        /// Saves the load string property.
        /// </summary>
        [TestMethod]
        public void Property_SaveLoadStringProperty() {
            MemoryStream stream = new MemoryStream();
            StringProperty property = new StringProperty("StringProperty", "Test string");

            XmlSerializer serializer = new XmlSerializer(property.GetType());
            stream = new MemoryStream();
            serializer.Serialize(stream, property);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            Trace.WriteLine(str);

            property = null;
            stream.Position = 0;
            serializer = new XmlSerializer(typeof(StringProperty));
            property = serializer.Deserialize(stream) as StringProperty;
            Assert.AreEqual(property.Value, "Test string");
        }

        /// <summary>
        /// Saves the load rectangle property.
        /// </summary>
        [TestMethod]
        public void Property_SaveLoadRectangleProperty() {
            MemoryStream stream = new MemoryStream();
            RectangleProperty property = new RectangleProperty("RectangleProperty", 8.7, 6.5, 4.3, 2.1);

            XmlSerializer serializer = new XmlSerializer(property.GetType());
            stream = new MemoryStream();
            serializer.Serialize(stream, property);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            Trace.WriteLine(str);

            property = null;
            stream.Position = 0;
            serializer = new XmlSerializer(typeof(RectangleProperty));
            property = serializer.Deserialize(stream) as RectangleProperty;
            Assert.AreEqual(property.Value.X, 8.7);
            Assert.AreEqual(property.Value.Y, 6.5);
            Assert.AreEqual(property.Value.Width, 4.3);
            Assert.AreEqual(property.Value.Height, 2.1);
        }

        [TestMethod]
        public void Property_SaveLoadListOfProperties() {
            MemoryStream stream = new MemoryStream();
            List<Property> properties = new List<Property>();
            properties.Add(new IntegerProperty("IntegerProperty", 20));
            properties.Add(new DoubleProperty("DoubleProperty", 543.21));
            properties.Add(new StringProperty("StringProperty", "Test string"));
            properties.Add(new RectangleProperty("RectangleProperty", 8.7, 6.5, 4.3, 2.1));

            XmlSerializer serializer = new XmlSerializer(properties.GetType());
            stream = new MemoryStream();
            serializer.Serialize(stream, properties);

            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            Trace.WriteLine(str);

            properties = null;
            stream.Position = 0;
            serializer = new XmlSerializer(typeof(List<Property>));
            properties = serializer.Deserialize(stream) as List<Property>;
            Assert.AreEqual(properties.Count, 4);
        }
    }
}