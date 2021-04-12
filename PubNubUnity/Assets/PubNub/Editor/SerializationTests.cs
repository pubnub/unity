using NUnit.Framework;
using UnityEngine;
using System.Linq;
using System;
using System.Text;

namespace PubNubAPI.Tests
{
    public class SerializationTests
    {
        [Test]
        public void TestEmojiSerialization (){
            string message = "Text with emoji 🙀 👸 🐥 😜 🎉";
            SerializationCommon(message);
        }

        public void SerializationCommon(object message){
            string serialized = EditorCommon.Serialize(message);
            string expected = string.Format("\"{0}\"", message);
            Assert.True (expected.Equals (serialized));
        } 

        [Test]
        public void TestEmojiDeserialization (){
            string message = "\"Text with emoji 🙀 👸 🐥 😜 🎉\"";
            DeserializationCommon(message, message);
        }

        public void DeserializationCommon(string message, string expected){
            string deserialized = EditorCommon.Deserialize<string>(message);
            
            string deserializedWithQuotes = string.Format("\"{0}\"", deserialized);
            Assert.True (message.Equals (deserializedWithQuotes));
        } 

    }
}