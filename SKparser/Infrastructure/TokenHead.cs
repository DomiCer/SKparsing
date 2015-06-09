using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numl;
using numl.Model;
using System.IO;

namespace Infrastructure
{
    [Serializable]
    public class TokenHead
    {
        public string Id { get; set; }
        //[Feature]
        public string Form { get; set; }
        //[Feature]
        public string Lemma { get; set; }
        [Feature]
        public string Cpostag { get; set; }
        [Feature]
        public string Postag { get; set; }
        //[Feature]
        public string Feats { get; set; }
        //[Feature]
        public string Head { get; set; }
        //[Feature]
        public string Deprel { get; set; }

        [Label]
        public bool IsCorrectHead { get; set; }
        
        public bool IsCorrectDeprel { get; set; }

        public string CorrectHead { get; set; }
        public string CorrectDeprel { get; set; }

        //[Feature]
        public string ParentCpos { get; set; }
        //[Feature]
        public string GParentCpos { get; set; }
        //[Feature]
        public string ParentPos { get; set; }
        //[Feature]
        public string GParentPos { get; set; }
        //[Feature]
        public string ChildrenPos { get; set; }


        public bool IsCritical { get; set; }

        #region FEATS

        [Feature]
        public F_pos f_pos { get; set; }
        [Feature]
        public F_negativeness f_negativeness { get; set; }
        [Feature]
        public F_number f_number { get; set; }
        [Feature]
        public int f_person { get; set; }
        [Feature]
        public F_verbform f_verbform { get; set; }
        [Feature]
        public F_mood f_mood { get; set; }
        [Feature]
        public F_tense f_tense { get; set; }
        [Feature]
        public F_aspect f_aspect { get; set; }
        [Feature]
        public F_gender f_gender { get; set; }
        [Feature]
        public F_case f_case { get; set; }
        [Feature]
        public bool f_prontype { get; set; }
        [Feature]
        public bool f_reflex { get; set; }
        [Feature]
        public F_animateness f_animateness { get; set; }
        [Feature]
        public F_degree f_degree { get; set; }
        [Feature]
        public F_subpos f_subpos { get; set; }
        [Feature]
        public bool f_numform { get; set; }
        [Feature]
        public bool f_abbr { get; set; }
        [Feature]
        public bool f_hyph { get; set; }
        [Feature]
        public bool f_foreign { get; set; }
        [Feature]
        public F_voice f_voice { get; set; }
        [Feature]
        public bool f_typo { get; set; }

        #endregion

        public TokenHead()
        {
            Id = string.Empty;
            Form = string.Empty;
            Lemma = string.Empty;
            Cpostag = string.Empty;
            Postag = string.Empty;
            Feats = string.Empty;
            Head = string.Empty;
            Deprel = string.Empty;

            IsCorrectHead = false;
            IsCorrectDeprel = false;
        }

        public TokenHead(Token t)
        {
            Id = t.Id;
            Form = t.Form;
            Lemma = t.Lemma;
            Cpostag = t.Cpostag;
            Postag = t.Postag;
            Feats = t.Feats;
            Head = t.Head;
            Deprel = t.Deprel;
        }

        public TokenHead(string token)
        {
            string[] splitted = token.Split(new string[] { "\t" }, StringSplitOptions.None);
            Id = splitted[0];
            Form = splitted[1];
            Lemma = splitted[2];
            Cpostag = splitted[3];
            Postag = splitted[4];
            Feats = splitted[5];
            Head = splitted[6];
            Deprel = splitted[7];

            IsCorrectHead = false;
            IsCorrectDeprel = false;
        }

        public TokenHead(string token, bool hasResult, bool separateFeats)
        {
            string[] splitted = token.Split(new string[] { "\t" }, StringSplitOptions.None);
            Id = splitted[0];
            Form = splitted[1];
            Lemma = splitted[2];
            Cpostag = splitted[3];
            Postag = splitted[4];
            Feats = splitted[5];
            Head = splitted[6];
            Deprel = splitted[7];

            if (hasResult)
            {
                IsCorrectHead = Convert.ToBoolean(splitted[8]);
                IsCorrectDeprel = Convert.ToBoolean(splitted[9]);
                CorrectHead = splitted[10];
                CorrectDeprel = splitted[11];
            }
            else
            {
                IsCorrectHead = false;
                IsCorrectDeprel = false;
                CorrectHead = string.Empty;
                CorrectDeprel = string.Empty;
            }

            if (separateFeats)
                FillFeats();

        }

        public TokenHead(string token, bool hasResult, bool separateFeats, bool hasParentCpos, bool hasParentPos, bool hasChildrenPos)
        {
            string[] splitted = token.Split(new string[] { "\t" }, StringSplitOptions.None);
            Id = splitted[0];
            Form = splitted[1];
            Lemma = splitted[2];
            Cpostag = splitted[3];
            Postag = splitted[4];
            Feats = splitted[5];
            Head = splitted[6];
            Deprel = splitted[7];

            if (hasResult)
            {
                IsCorrectHead = Convert.ToBoolean(splitted[8]);
                IsCorrectDeprel = Convert.ToBoolean(splitted[9]);
                CorrectHead = splitted[10];
                CorrectDeprel = splitted[11];
            }
            else
            {
                IsCorrectHead = false;
                IsCorrectDeprel = false;
                CorrectHead = string.Empty;
                CorrectDeprel = string.Empty;
            }

            if (separateFeats)
                FillFeats();

            if (hasParentCpos)
            {
                ParentCpos = splitted[12];
                GParentCpos = splitted[13];
            }
            if (hasParentPos)
            {
                ParentPos = splitted[14];
                GParentPos = splitted[15];
            }
            if (hasChildrenPos)
                ChildrenPos = splitted[16];
        }
        public bool IsEqual(Token t)
        {
            bool bResult = false;

            if (this.Id.Equals(t.Id)
                && this.Form.Equals(t.Form)
                && this.Lemma.Equals(t.Lemma)
                && this.Cpostag.Equals(t.Cpostag)
                && this.Postag.Equals(t.Postag))
            {
                bResult = true;
            }

            return bResult;
        }

        public bool IsEqualInHead(Token t)
        {
            bool bResult = false;
            if (this.Id.Equals(t.Id) && this.Head.Equals(t.Head))
                bResult = true;
            return bResult;
        }

        public bool IsEqualInDeprel(Token t)
        {
            bool bResult = false;
            if (this.Id.Equals(t.Id) && this.Deprel.Equals(t.Deprel))
                bResult = true;
            return bResult;
        }
        public bool IsEqualInHeadAndDeprel(Token t)
        {
            bool bResult = false;
            if (this.Id.Equals(t.Id) && this.Head.Equals(t.Head) && this.Deprel.Equals(t.Deprel))
                bResult = true;
            return bResult;
        }

        public List<string> ParseFeats()
        {
            List<string> resultList = new List<string>();
            string[] parsed = this.Feats.Split('|');
            if (parsed.Length > 0)
                resultList = parsed.ToList();
            return resultList;
        }

        public void FillFeats()
        {
            //Console.Out.WriteLine("FillFeats start");

            List<string> fList = ParseFeats();

            string strPom = string.Empty;

            strPom = getStrFromStringList(fList, "pos");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom.Trim()))
                f_pos = F_pos.NULL;
            else
                f_pos = (F_pos)Enum.Parse(typeof(F_pos), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "negativeness");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_negativeness = F_negativeness.NULL;
            else
                f_negativeness = (F_negativeness)Enum.Parse(typeof(F_negativeness), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "number");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_number = F_number.NULL;
            else
                f_number = (F_number)Enum.Parse(typeof(F_number), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "person");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_person = 0;
            else
                f_person = int.Parse(strPom);

            strPom = getStrFromStringList(fList, "verbform");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_verbform = F_verbform.NULL;
            else
                f_verbform = (F_verbform)Enum.Parse(typeof(F_verbform), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "mood");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_mood = F_mood.NULL;
            else
                f_mood = (F_mood)Enum.Parse(typeof(F_mood), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "tense");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_tense = F_tense.NULL;
            else
                f_tense = (F_tense)Enum.Parse(typeof(F_tense), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "aspect");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_aspect = F_aspect.NULL;
            else
                f_aspect = (F_aspect)Enum.Parse(typeof(F_aspect), strPom.ToUpper().Replace(";", "_"));

            strPom = getStrFromStringList(fList, "gender");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_gender = F_gender.NULL;
            else
                f_gender = (F_gender)Enum.Parse(typeof(F_gender), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "case");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_case = F_case.NULL;
            else
                f_case = (F_case)Enum.Parse(typeof(F_case), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "prontype");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_prontype = false;
            else
                f_prontype = true;

            strPom = getStrFromStringList(fList, "reflex");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_reflex = false;
            else
                f_reflex = true;

            strPom = getStrFromStringList(fList, "animateness");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_animateness = F_animateness.NULL;
            else
                f_animateness = (F_animateness)Enum.Parse(typeof(F_animateness), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "degree");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_degree = F_degree.NULL;
            else
                f_degree = (F_degree)Enum.Parse(typeof(F_degree), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "subpos");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_subpos = F_subpos.NULL;
            else
                f_subpos = (F_subpos)Enum.Parse(typeof(F_subpos), strPom.ToUpper());


            strPom = getStrFromStringList(fList, "numform");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_numform = false;
            else
                f_numform = true;

            strPom = getStrFromStringList(fList, "abbr");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_abbr = false;
            else
                f_abbr = true;

            strPom = getStrFromStringList(fList, "hyph");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_hyph = false;
            else
                f_hyph = true;

            strPom = getStrFromStringList(fList, "foreign");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_foreign = false;
            else
                f_foreign = true;

            strPom = getStrFromStringList(fList, "voice");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_voice = F_voice.NULL;
            else
                f_voice = (F_voice)Enum.Parse(typeof(F_voice), strPom.ToUpper());

            strPom = getStrFromStringList(fList, "typo");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_typo = false;
            else
                f_typo = true;


            //Console.Out.WriteLine("FillFeats end");
        }

        public static List<TokenHead> GetTokensFromCoNLLFile(string filepath, bool hasResult, bool separateFeats, bool hasParentCpos, bool hasParentPos, bool hasChildPos)
        {
            List<TokenHead> resultList = new List<TokenHead>();

            try
            {
                string[] lines = File.ReadAllLines(filepath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (!string.IsNullOrEmpty(lines[j].Trim()))
                        resultList.Add(new TokenHead(lines[j], hasResult, separateFeats, hasParentCpos, hasParentPos, hasChildPos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return resultList;
        }

        /*
        public string featsToString()
        {
            string strResult = string.Empty;
            
            if (!string.IsNullOrEmpty(f_pos))
                strResult += f_pos + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_negativeness))
                strResult += f_negativeness + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_number))
                strResult += f_number + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_person))
                strResult += f_person + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_verbform))
                strResult += f_verbform + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_mood))
                strResult += f_mood + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_tense))
                strResult += f_tense + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_aspect))
                strResult += f_aspect + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_gender))
                strResult += f_gender + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_case))
                strResult += f_case + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_prontype))
                strResult += f_prontype + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_reflex))
                strResult += f_reflex + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_animateness))
                strResult += f_animateness + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_degree))
                strResult += f_degree + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_subpos))
                strResult += f_subpos + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_numform))
                strResult += f_numform + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_abbr))
                strResult += f_abbr + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_hyph))
                strResult += f_hyph + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_foreign))
                strResult += f_foreign + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_voice))
                strResult += f_voice + ",";
            else
                strResult += "-,";

            if (!string.IsNullOrEmpty(f_typo))
                strResult += f_typo + ",";
            else
                strResult += "-,";

            return strResult;
        }
        */

        public static string getStrFromStringList(List<string> list, string str)
        {
            foreach (string s in list)
            {
                if (s.Contains(str))
                    return s;
            }
            return string.Empty;
        }
    }
}
