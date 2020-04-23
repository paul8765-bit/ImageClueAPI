using Microsoft.VisualStudio.TestTools.UnitTesting;
using PairAndImagesLibrary;
using System;
using System.Collections.Generic;
using Twilio.Exceptions;

namespace PairAndImagesGameTest
{
    [TestClass]
    public class PairAndImagesMainTest
    {
        [TestMethod]
        public void TestGetAdjectives()
        {
            string adjectivesFilePath = "Adjectives.txt";
            List<string> adjectives = PairAndImagesLibraryMain.ReadAllLinesFromFile(adjectivesFilePath);
            Assert.IsTrue(adjectives.Count > 10);
            Assert.AreEqual("itchy", adjectives[0]);
            Assert.AreEqual("horny", adjectives[1]);
        }

        [TestMethod]
        public void TestGetNouns()
        {
            string nounsFilePath = "Nouns.txt";
            List<string> nouns = PairAndImagesLibraryMain.ReadAllLinesFromFile(nounsFilePath);
            Assert.IsTrue(nouns.Count > 10);
            Assert.AreEqual("cat", nouns[0]);
            Assert.AreEqual("lemon", nouns[1]);
            Assert.AreEqual("gourd", nouns[2]);
        }

        [TestMethod]
        public void TestCheckTeamsAndCluesLengthSuccess()
        {
            PairAndImagesLibraryMain.CheckTeamsAndCluesLength(3, 3);
            // Not expecting an exception here so this is success
        }

        [TestMethod]
        public void TestCheckTeamsAndCluesLengthFailure()
        {
            try
            {
                PairAndImagesLibraryMain.CheckTeamsAndCluesLength(3, 2);
            }
            catch (Exception e)
            {
                // Are expecting an exception here though!
                Assert.IsTrue(e.Message.Equals(
                    "Expected teamsCount and cluesCount to be equal. Instead teamsCount=3 and cluesCount=2"));
                return;
            }
            // If no exception was thrown this test is failed
            Assert.Fail("Expected exception to be thrown");
        }

        [TestMethod]
        public void FormatPhoneNumberTestAddPlus()
        {
            string input = "447986869466";
            string output = PairAndImagesLibraryMain.FormatPhoneNumber(input);
            Assert.AreEqual("+447986869466", output);
        }

        [TestMethod]
        public void FormatPhoneNumberTestExistingPlus()
        {
            string input = "+447986869466";
            string output = PairAndImagesLibraryMain.FormatPhoneNumber(input);
            Assert.AreEqual("+447986869466", output);
        }

        [TestMethod]
        public void SendTwilioSMSTestSuccess()
        {
            try
            {
                bool result = PairAndImagesLibraryMain.SendTwilioSMS(
                new Tuple<string, string>("Paul", "+447986869466"), new Clue("randy", "nun"));
            }
            catch (ApiException e)
            {
                // Expect to receive an auth exception, as we have no way to set the environment
                // variable privately in github
                Assert.IsTrue(e.Message.Contains("TWILIO_AUTH"));
                return;
            }
            // Expect to get an exception, so fail if not
            Assert.Fail("Expected exception");
        }

        [TestMethod]
        public void SendTwilioSMSTestFailure()
        {
            try
            {
                bool result = PairAndImagesLibraryMain.SendTwilioSMS(
                    new Tuple<string, string>("Paul", "+4472618"), new Clue("randy", "nun"));
            }
            catch (ApiException e)
            {
                // We expected this exception
                Assert.IsTrue(e.Message.Contains("TWILIO_AUTH"));
                //Assert.IsTrue(e.Message.Contains("not a valid phone number"));
                return;
            }

            // If we reach this stage, fail the test
            Assert.Fail("Expected an exception as the phone number was invalid");
        }
    }
}
