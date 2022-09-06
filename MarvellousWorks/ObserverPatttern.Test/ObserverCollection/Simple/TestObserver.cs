using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObserverPattern.ObserverCollection.Simple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPatttern.Test.ObserverCollection.Simple
{
    [TestClass]
    public class TestObserver
    {
        string key = "hellow";
        string value = "world";
        [TestMethod]
        public void Test()
        {
            IObserverableDictionary<string, string> dictionary = new ObserverableDictionary<String, string>();
            dictionary.NewItemAdded += this.Validate;
            dictionary.Add(key, value);
        }

        public void Validate(object sender,DictionaryEventArgs<string,string> args)
        {
            Assert.IsNotNull(sender);
            Type expectedType = typeof(ObserverableDictionary<string, string>);
            Assert.AreEqual<Type>(expectedType, sender.GetType());
            Assert.IsNotNull(args);
            expectedType = typeof(DictionaryEventArgs<string, string>);
            Assert.AreEqual<Type>(expectedType, args.GetType());
            Assert.AreEqual<string>(key, args.Key);
            Assert.AreEqual<String>(value, args.Value);
            Trace.WriteLine(args.Key + " " + args.Value);
        }
    }
}
