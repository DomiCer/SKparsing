using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_data_checker
{
    public class CoNLLSentence
    {
        public List<CoNLLToken> Tokens { get; set; }

        public CoNLLSentence()
        {
            Tokens = new List<CoNLLToken>();
        }

        public static bool AreEqual(CoNLLSentence s1, CoNLLSentence s2)
        {
            bool result = false;

            try
            {
                bool tokensEqual = false;

                if (s1.Tokens.Count == s2.Tokens.Count)
                {
                    for (int i = 0; i < s1.Tokens.Count; i++)
                    {
                        if (CoNLLToken.AreEqual(s1.Tokens[i], s2.Tokens[i]))
                            tokensEqual = true;
                        else
                        {
                            tokensEqual = false;
                            //if (i > 0 && s1.Tokens[i].Form.Equals(s2.Tokens[i].Form))
                            //{
                            //    Console.WriteLine(i.ToString());
                            //    foreach (CoNLLToken t in s1.Tokens)
                            //        Console.Write(t.Form + " ");
                            //    Console.WriteLine(" ");
                            //    foreach (CoNLLToken t in s2.Tokens)
                            //        Console.Write(t.Form + " ");
                            //    Console.WriteLine(" ");
                            //}
                            break;
                        }
                    }

                    result = tokensEqual;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public void SetParentCorrectDeprelForEachToken()
        {
            foreach (CoNLLToken token in this.Tokens)
            {
                int intHead = int.Parse(token.Head);
                if (intHead == 0)
                    token.ParentCorrectDeprel = "root";
                else
                {
                    token.ParentCorrectDeprel = this.Tokens[intHead - 1].CorrectDeprel;
                }
            }
        }

        public void SetParentAndGrandparentPosForEachToken(bool postagInfo, bool cpostagInfo, bool hasResult)
        {
            foreach (CoNLLToken token in this.Tokens)
            {
                int intHead = -1;
                if(hasResult && !token.IsCorrectHead)
                    intHead = int.Parse(token.CorrectHead);
                else
                    intHead = int.Parse(token.Head);

                if (intHead == 0)
                {
                    if (cpostagInfo)
                    {
                        token.ParentCpos = "root";
                        token.GrandParentCpos = "root";
                    }
                    if (postagInfo)
                    {
                        token.ParentPos = "root";
                        token.GrandParentPos = "root";
                    }

                }
                else
                {
                    if (postagInfo)
                    {
                        CoNLLToken parent = this.Tokens[intHead - 1];
                        token.ParentPos = parent.Postag;

                        int intParentHead = int.Parse(parent.Head);

                        if (intParentHead == 0)
                            token.GrandParentPos = "root";
                        else
                        {
                            CoNLLToken grandparent = this.Tokens[intParentHead - 1];
                            token.GrandParentPos = grandparent.Postag;
                        }
                    }

                    if (cpostagInfo)
                    {
                        CoNLLToken parent = this.Tokens[intHead - 1];
                        token.ParentCpos = parent.Cpostag;

                        int intParentHead = int.Parse(parent.Head);

                        if (intParentHead == 0)
                            token.GrandParentCpos = "root";
                        else
                        {
                            CoNLLToken grandparent = this.Tokens[intParentHead - 1];
                            token.GrandParentCpos = grandparent.Cpostag;
                        }
                    }
                }
            }        
        }

        public void SetChildrenPosForEachToken(bool postagInfo, bool hasResult)
        {
            foreach (CoNLLToken token in this.Tokens)
            {
                int intHead = -1;
                if (hasResult && !token.IsCorrectHead)
                    intHead = int.Parse(token.CorrectHead);
                else
                    intHead = int.Parse(token.Head);
                
                if (intHead != 0)
                {
                    if (postagInfo)
                    {
                        CoNLLToken parent = this.Tokens[intHead - 1];

                        string pom = token.Postag;

                        parent.ChildrenPos.Add(pom);
                    }

                    
                }
            }
        }

        public void SetParentAndGrandparentDeprelForEachToken()
        {
            foreach (CoNLLToken token in this.Tokens)
            {
                int intHead = int.Parse(token.Head);
                if (intHead == 0)
                {
                    token.ParentCorrectDeprel = "root";
                    token.GrandParentCorrectDeprel = "root";
                }
                else
                {
                    CoNLLToken parent = this.Tokens[intHead - 1];
                    token.ParentCorrectDeprel = parent.CorrectDeprel;

                    int intParentHead = int.Parse(parent.Head);

                    if (intParentHead == 0)
                        token.GrandParentCorrectDeprel = "root";
                    else
                    {
                        CoNLLToken grandparent = this.Tokens[intParentHead - 1];
                        token.GrandParentCorrectDeprel = grandparent.CorrectDeprel;
                    }
                }
            }
        }
    }
}
