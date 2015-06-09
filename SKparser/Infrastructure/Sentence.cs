using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using numl;
using numl.Model;

namespace Infrastructure
{
    public class Sentence
    {
        public List<Token> Tokens { get; set; }

        public Sentence()
        {
            Tokens = new List<Token>();
        }

        public static List<Sentence> ReadSentencesFromCoNLLFile(string CoNLLPath)
        {
            List<Sentence> resultList = new List<Sentence>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new Sentence());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line))
                        resultList[sentenceIdx].Tokens.Add(new Token(line, false, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        public static List<Sentence> ReadSentencesFromCoNLLFile(string CoNLLPath, bool hasRes)
        {
            List<Sentence> resultList = new List<Sentence>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new Sentence());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line))
                        resultList[sentenceIdx].Tokens.Add(new Token(line, hasRes, true));
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

        public void SetParentAndGrandparentPosForEachToken(bool postagInfo, bool cpostagInfo)
        {
            foreach (Token token in this.Tokens)
            {
                int intHead = int.Parse(token.Head);
                if (intHead == 0)
                {
                    if (cpostagInfo)
                    {
                        token.ParentCpos = "root";
                        token.GParentCpos = "root";
                    }
                    if (postagInfo)
                    {
                        token.ParentPos = "root";
                        token.GParentPos = "root";
                    }

                }
                else
                {
                    if (postagInfo)
                    {
                        Token parent = this.Tokens[intHead - 1];
                        token.ParentPos = parent.Postag;

                        int intParentHead = int.Parse(parent.Head);

                        if (intParentHead == 0)
                            token.GParentPos = "root";
                        else
                        {
                            Token grandparent = this.Tokens[intParentHead - 1];
                            token.GParentPos = grandparent.Postag;
                        }
                    }

                    if (cpostagInfo)
                    {
                        Token parent = this.Tokens[intHead - 1];
                        token.ParentCpos = parent.Cpostag;

                        int intParentHead = int.Parse(parent.Head);

                        if (intParentHead == 0)
                            token.GParentCpos = "root";
                        else
                        {
                            Token grandparent = this.Tokens[intParentHead - 1];
                            token.GParentCpos = grandparent.Cpostag;
                        }
                    }
                }
            }
        }

        public void SetChildrenPosForEachToken(bool postagInfo)
        {
            foreach (Token token in this.Tokens)
            {
                int intHead = int.Parse(token.Head);
                if (intHead != 0)
                {
                    if (postagInfo)
                    {
                        Token parent = this.Tokens[intHead - 1];

                        string pom = token.Postag;

                        parent.ChildrenPos += "|" + pom;
                    }


                }
            }
            foreach (Token token in this.Tokens)
            {
                if (string.IsNullOrEmpty(token.ChildrenPos))
                    token.ChildrenPos = "none";
            }
        }

    }


    

   
}
