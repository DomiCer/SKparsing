using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure;
using numl;
using numl.Model;
using numl.Supervised;
using numl.Supervised.DecisionTree;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
//using System.Xml;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Oracle_TestModel()
        {
            List<TokenOracle> tokens = TokenOracle.GetTokensFromCoNLLFile(@"C:\Users\Domi\Documents\Visual Studio 2012\Projects\NumlSample\NumlSample\bin\Debug\parsing\criticalDeprel_test_part3.conll", true);
            int truePositivesFalseNegatives = 0;

            var d = Descriptor.Create<TokenOracle>();
            var g = new DecisionTreeGenerator(d);
            g.SetHint(false);
            LearningModel ModelOracle = Learner.Learn(tokens, 0.9, 1, g);
            
            foreach (TokenOracle t in tokens)
            {
                TokenOracle tPredicion = ModelOracle.Model.Predict(t);
                if (t.CorrectDeprel == tPredicion.CorrectDeprel)
                    truePositivesFalseNegatives++;
            }
            double result = (double)tokens.Count / (double)truePositivesFalseNegatives;
            Console.WriteLine("Precission: " + result.ToString() + " %");

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Test_IsTokenCritical_TestModel()
        {
            AppSettingsReader AppSettings = new AppSettingsReader();
            List<Token> tokens = Token.GetTokensFromCoNLLFile(@"C:\Users\Domi\Documents\Visual Studio 2012\Projects\NumlSample\NumlSample\bin\Debug\parsing\criticalToken_test_part2.conll",true,true,true,true, true);
            
            var d = Descriptor.Create<Token>();
            var g = new DecisionTreeGenerator(d);
            g.SetHint(false);
            LearningModel ModelIsTokenCritical = Learner.Learn(tokens, 0.9, 1, g);
            
            int truePositivesFalseNegatives = 0;
            foreach (Token t in tokens)
            {
                Token tPredicion = ModelIsTokenCritical.Model.Predict(t);
                if (t.IsCorrectDeprel == tPredicion.IsCorrectDeprel)
                    truePositivesFalseNegatives++;
            }
            double result = (double)tokens.Count / (double)truePositivesFalseNegatives;
            Console.WriteLine("Precission: " + result.ToString() + " %");

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Test_LoadBaseParserOutput()
        {
            string baseParserOutputFile = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\experiment_sk_korpus_polovica/TEST_parsed.conll";
            List<SentenceOracle> sentencesO = FillParentDeprel(SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false));
            //List<SentenceOracle> sentencesO = SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false);

            bool hasParentSet = false;
            foreach (SentenceOracle so in sentencesO)
            {
                foreach (TokenOracle tokOr in so.Tokens)
                {
                    if (tokOr.ParentCorrectDeprel != Deprel.NULL)
                        hasParentSet = true;
                    else
                    {
                        hasParentSet = false;
                        break;
                    }
                }
            }
            Assert.IsTrue(hasParentSet);
        }

        public static List<SentenceOracle> FillParentDeprel(List<SentenceOracle> sentences)
        {
            List<SentenceOracle> resultList = sentences;
            foreach (SentenceOracle so in resultList)
            {
                foreach (TokenOracle to in so.Tokens)
                {
                    int intHeadToken = int.Parse(to.Head);

                    if (intHeadToken == 0)
                        to.ParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                    else
                    {
                        TokenOracle parentToken = so.Tokens[intHeadToken - 1];
                        to.ParentCorrectDeprel = parentToken.CorrectDeprel;
                    }
                }
            }
            return resultList;
        }
    }
}
