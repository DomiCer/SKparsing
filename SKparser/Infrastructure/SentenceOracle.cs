using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Infrastructure
{
    public class SentenceOracle
    {
        public List<TokenOracle> Tokens { get; set; }

        public SentenceOracle()
        {
            Tokens = new List<TokenOracle>();
        }
        public void SetChildrenPosForEachToken(bool postagInfo)
        {
            
            foreach (TokenOracle token in this.Tokens)
            {
                int intHead = int.Parse(token.Head);
                if (intHead != 0)
                {
                    if (postagInfo)
                    {
                        TokenOracle parent = this.Tokens[intHead - 1];
                        if (parent.ChildrenPos == null)
                            parent.ChildrenPos = "";

                        string pom = token.Postag;

                        parent.ChildrenPos += pom + "|";
                    }
                }
            }
            foreach (TokenOracle t in this.Tokens)
            {
                if (string.IsNullOrEmpty(t.ChildrenPos))
                    t.ChildrenPos = "none";
                else
                    t.ChildrenPos = t.ChildrenPos.TrimEnd('|');
            }
        }

        public static List<SentenceOracle> ReadSentencesFromCoNLLFile(string CoNLLPath, bool hasResult)
        {
            List<SentenceOracle> resultList = new List<SentenceOracle>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new SentenceOracle());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line))
                        resultList[sentenceIdx].Tokens.Add(new TokenOracle(line, hasResult, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        public void SaveToCoNLLFile(string filePath)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
