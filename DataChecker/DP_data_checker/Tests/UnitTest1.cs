using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using DP_data_checker;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
               
        [TestMethod]
        public void TestMethod1()
        {
            List<CoNLLSentence> result = new List<CoNLLSentence>();
            XmlDocument mDoc = getSampleMFile();
            XmlDocument aDoc = getSampleAFile();
            XmlDocument wDoc = getSampleWFile();

            XmlNodeList sentenceNodeList = mDoc.SelectNodes("//s");

            foreach (XmlNode sentenceNode in sentenceNodeList)
            {
                CoNLLSentence s = new CoNLLSentence();
                s.Tokens = GetTokensFromXml(sentenceNode, aDoc);
                result.Add(s);
            }

            bool b = false;
            if(result.Count==7)
                b = true;

            string strResult = sentListToString(result);
            string sample = File.ReadAllText(@"..\..\..\DP_data_checker\data\sample.conll");

            Assert.IsTrue(sample.Trim().Contains(strResult.Trim()));
        }

        private string sentListToString(List<CoNLLSentence> sentences)
        {
            string str = string.Empty;
            foreach (CoNLLSentence s in sentences)
            {
                for (int i = 0; i < s.Tokens.Count;i++ )
                {
                    str += (i + 1).ToString() + tokenToString(s.Tokens[i]);
                    str += "\r\n";
                }
                str += "\r\n";
            }
            return str;
        }

        public List<CoNLLToken> GetTokensFromXml(XmlNode xml, XmlDocument aFile)
        {
            List<CoNLLToken> tokens = new List<CoNLLToken>();
            int id = 1;
            foreach (XmlNode n in xml.ChildNodes)
            {
                CoNLLToken t = new CoNLLToken();
                //string strId = tokenNode.Attributes["id"].Value;

                //XmlNode n = mFile.SelectSingleNode("//m[@id=\"" + strId.Replace("w-", "m-") + "\"]");

                //t.Id = id.ToString();
                t.Form = n.SelectSingleNode("form").InnerText;
                t.Lemma = n.SelectSingleNode("lemma").InnerText;
                t.Cpostag = string.IsNullOrEmpty(n.SelectSingleNode("tag").InnerText) ? "_" : n.SelectSingleNode("tag").InnerText[0].ToString();
                t.Postag = string.IsNullOrEmpty(n.SelectSingleNode("tag").InnerText) ? "_" : n.SelectSingleNode("tag").InnerText;

                t.Feats = "_";

                string strTokenId = n.SelectSingleNode("w.rf").InnerText;

                //token v .a subore
                XmlNode aNode = aFile.SelectSingleNode("//*[@id=\"" + strTokenId.Replace("w#w", "a") + "\"]");
                
                if (aNode.SelectSingleNode("ord") == null)
                {
                    Console.Write("null");
                }
                t.Id = aNode.SelectSingleNode("ord").InnerText;
                

                XmlNode parentNode = null;
                if (aNode != null)
                {
                    if (aNode.Name.Equals("LM"))
                        parentNode = aNode.ParentNode.ParentNode;
                    else
                        parentNode = aNode.ParentNode;

                    t.Head = parentNode.SelectSingleNode("ord").InnerText;
                    t.Deprel = aNode.SelectSingleNode("afun").InnerText;
                }
                else
                {
                    t.Head = "_";
                    t.Deprel = "_";
                }
                tokens.Add(t);
                
            }

            return tokens;
        }
        
        private XmlDocument getSampleAFile()
        {
            //string path = @"..\..\..\DP_data_checker\data\durovic10_kandracova_dok.a";
            string path = @"..\..\..\DP_data_checker\data\000-snehulienka_olejkarova_dok.a";
            
            string content = File.ReadAllText(path).Split(new[] { "</head>", "</adata>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<a>" + content + "</a>");
            return doc;
        }

        private XmlDocument getSampleWFile()
        {
            //string path = @"..\..\..\DP_data_checker\data\durovic10_kandracova_dok.w";
            string path = @"..\..\..\DP_data_checker\data\000-snehulienka_olejkarova_dok.w";

            string content = File.ReadAllText(path).Split(new[] { "<docmeta/>", "</doc>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ww>" + content + "</ww>");
            return doc;
        }

        private XmlDocument getSampleMFile()
        {
            string path = @"..\..\..\DP_data_checker\data\000-snehulienka_olejkarova_dok.m";
            //string path = @"..\..\..\DP_data_checker\data\durovic10_kandracova_dok.m";
            
            string content = File.ReadAllText(path).Split(new[] { "</meta>", "</mdata>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<m>" + content + "</m>");
            return doc;
        }

        

        private string tokenToString(CoNLLToken t)
        {
            return "\t" + t.Form + "\t" + t.Lemma + "\t" + t.Cpostag + "\t" + t.Postag + "\t" + t.Head + "\t" + t.Deprel;
        }
    }
}
