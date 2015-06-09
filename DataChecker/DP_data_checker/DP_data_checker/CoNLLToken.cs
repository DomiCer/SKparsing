using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_data_checker
{
    public class CoNLLToken
    {
        public bool IsInFilter { get; set; }

        public string Id { get; set; }
        public string Form { get; set; }
        public string Lemma { get; set; }
        public string Cpostag { get; set; }
        public string Postag { get; set; }
        public string Feats { get; set; }
        public string Head { get; set; }
        public string Deprel { get; set; }

        public bool IsCorrectHead { get; set; }
        public bool IsCorrectDeprel { get; set; }

        public string CorrectHead { get; set; }
        public string CorrectDeprel { get; set; }
        public string ParentCorrectDeprel { get; set; }
        public string GrandParentCorrectDeprel { get; set; }

        public string ParentCpos { get; set; }
        public string GrandParentCpos { get; set; }

        public string ParentPos { get; set; }
        public string GrandParentPos { get; set; }

        public List<string> ChildrenPos { get; set; }

        public string f_pos{ get; set; }
        public string f_negativeness{ get; set; }
        public string f_number{ get; set; }
        public string f_person{ get; set; }
        public string f_verbform{ get; set; }
        public string f_mood{ get; set; }
        public string f_tense{ get; set; }
        public string f_aspect{ get; set; }
        public string f_gender{ get; set; }
        public string f_case{ get; set; }
        public string f_prontype{ get; set; }
        public string f_reflex{ get; set; }
        public string f_animateness{ get; set; }
        public string f_degree{ get; set; }
        public string f_subpos{ get; set; }
        public string f_numform{ get; set; }
        public string f_abbr{ get; set; }
        public string f_hyph{ get; set; }
        public string f_foreign{ get; set; }
        public string f_voice{ get; set; }
        public string f_typo { get; set; }

        public CoNLLToken()
        {
            Id = string.Empty;
            Form = string.Empty;
            Lemma = string.Empty;
            Cpostag = string.Empty;
            Postag = string.Empty;
            Feats = string.Empty;
            Head = string.Empty;
            Deprel = string.Empty;
            ParentCorrectDeprel = string.Empty;

            IsCorrectHead = false;
            IsCorrectDeprel = false;
            ChildrenPos=new List<string>();
        }

        public CoNLLToken(string token)
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
            ParentCorrectDeprel = string.Empty;

            IsCorrectHead = false;
            IsCorrectDeprel = false;
            ChildrenPos = new List<string>();
        }

        public CoNLLToken(string token, bool hasResult, bool hasParentDeprel)
        {
            try
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

                    if (hasParentDeprel)
                    {
                        ParentCorrectDeprel = splitted[12];
                    }
                }
                else
                {
                    IsCorrectHead = true;
                    IsCorrectDeprel = true;
                    ParentCorrectDeprel = string.Empty;
                }
                ChildrenPos = new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public CoNLLToken(string token, bool hasResult, bool hasGrandParentDeprel, bool hasGPPosInfo)
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

                if (hasGrandParentDeprel)
                {
                    ParentCorrectDeprel = splitted[12];
                    GrandParentCorrectDeprel = splitted[13];
                }
                if (hasGPPosInfo)
                {
                    ParentPos = splitted[14];
                    GrandParentPos = splitted[15];
                }
            }
            else
            {
                IsCorrectHead = true;
                IsCorrectDeprel = true;
                ParentCorrectDeprel = string.Empty;
                GrandParentCorrectDeprel = string.Empty;
            }
            ChildrenPos = new List<string>();
        }

        public CoNLLToken(string token, bool hasResult, bool hasGrandParentDeprel, bool hasGPPosInfo, bool hasChildPosInfo)
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

            ChildrenPos = new List<string>();

            if (hasResult)
            {
                IsCorrectHead = Convert.ToBoolean(splitted[8]);
                IsCorrectDeprel = Convert.ToBoolean(splitted[9]);
                CorrectHead = splitted[10];
                CorrectDeprel = splitted[11];

                if (hasGrandParentDeprel)
                {
                    ParentCorrectDeprel = splitted[12];
                    GrandParentCorrectDeprel = splitted[13];
                }
                if (hasGPPosInfo)
                {
                    ParentPos = splitted[13];
                    GrandParentPos = splitted[14];
                }
                if (hasChildPosInfo)
                {
                    ChildrenPos = splitted[15].Split('|').ToList();
                }
            }
            else
            {
                IsCorrectHead = true;
                IsCorrectDeprel = true;
                ParentCorrectDeprel = string.Empty;
                GrandParentCorrectDeprel = string.Empty;
            }
        }

        public CoNLLToken(string token, bool hasResult, bool hasGrandParentDeprel, bool hasGPCPosInfo, bool hasGPPosInfo, bool hasChildPosInfo)
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

            ChildrenPos = new List<string>();

            if (hasResult)
            {
                IsCorrectHead = Convert.ToBoolean(splitted[8]);
                IsCorrectDeprel = Convert.ToBoolean(splitted[9]);
                CorrectHead = splitted[10];
                CorrectDeprel = splitted[11];

                if (hasGrandParentDeprel)
                {
                    ParentCorrectDeprel = splitted[12];
                    GrandParentCorrectDeprel = splitted[13];
                }
                if (hasGPCPosInfo)
                {
                    if (hasGrandParentDeprel)
                    {
                        ParentCpos = splitted[14];
                        GrandParentCpos = splitted[15];
                    }
                    else
                    {
                        ParentCpos = splitted[12];
                        GrandParentCpos = splitted[13];
                    }
                }
                if (hasGPPosInfo)
                {
                    if (hasGPCPosInfo)
                    {
                        if (hasGrandParentDeprel)
                        {
                            ParentPos = splitted[16];
                            GrandParentPos = splitted[17];
                        }
                        else
                        {
                            ParentPos = splitted[14];
                            GrandParentPos = splitted[15];
                        }
                    }
                    else
                    {
                        if (hasGrandParentDeprel)
                        {
                            ParentPos = splitted[14];
                            GrandParentPos = splitted[15];
                        }
                        else
                        {
                            ParentPos = splitted[12];
                            GrandParentPos = splitted[13];
                        }
                    }
                }
                if (hasChildPosInfo)
                {
                    if (hasGPCPosInfo)
                    {
                        if (hasGrandParentDeprel)
                            ChildrenPos = splitted[18].Split('|').ToList();
                        else
                            ChildrenPos = splitted[16].Split('|').ToList();
                    }
                    else
                    {
                        if (hasGrandParentDeprel)
                            ChildrenPos = splitted[16].Split('|').ToList();
                        else
                            ChildrenPos = splitted[14].Split('|').ToList();
                    }
                }

            }
            else
            {
                IsCorrectHead = true;
                IsCorrectDeprel = true;
                ParentCorrectDeprel = string.Empty;
                GrandParentCorrectDeprel = string.Empty;
            }
        }

        public void SetFeats()
        {
            switch (this.Cpostag)
            {
                case "Z":
                    this.Feats = "pos=punc";
                    break;
                case "S":
                    this.Feats = "pos=noun";
                    break;
                case "P":
                    this.Feats = "pos=noun";
                    break;
                case "O":
                    this.Feats = "pos=conj";
                    break;
                case "V":
                    this.Feats = "pos=verb";
                    break;
                case "A":
                    this.Feats = "pos=adj";
                    break;
                case "E":
                    this.Feats = "pos=prep";
                    break;
                case "D":
                    this.Feats = "pos=adv";
                    break;
                case "T":
                    this.Feats = "pos=part";
                    break;
                case "R":
                    this.Feats = "pos=noun|prontype=prs|reflex=reflex";
                    break;
                case "0":
                    this.Feats = "pos=num|numform=digit";
                    break;
                case "N":
                    this.Feats = "pos=num";
                    break;
                case "W":
                    this.Feats = "abbr=abbr";
                    break;
                case "G":
                    this.Feats = "gender=masc|animateness=inan";
                    break;
                case "Q":
                    this.Feats = "hyph=hyph";
                    break;
                case "%":
                    this.Feats = "foreign=foreign";
                    break;
                case "Y":
                    this.Feats = "pos=verb|mood=cnd";
                    break;
                case "#":
                    this.Feats = "pos=punc";
                    break;
                case "J":
                    this.Feats = "pos=int";
                    break;



            }
        }

        public bool IsEqual(CoNLLToken t)
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

        public bool IsEqualInHead(CoNLLToken t)
        {
            bool bResult = false;
            if (this.Id.Equals(t.Id) && this.Head.Equals(t.Head))
                bResult = true;
            return bResult;
        }

        public bool IsEqualInDeprel(CoNLLToken t)
        {
            bool bResult = false;
            if (this.Id.Equals(t.Id) && this.Deprel.Equals(t.Deprel))
                bResult = true;
            return bResult;
        }
        public bool IsEqualInHeadAndDeprel(CoNLLToken t)
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
            Console.Out.WriteLine("FillFeats start");

            List<string> fList = ParseFeats();

            f_pos = getStrFromStringList(fList, "pos");
            f_negativeness = getStrFromStringList(fList, "negativeness");
            f_number = getStrFromStringList(fList, "number");
            f_person = getStrFromStringList(fList, "person");
            f_verbform = getStrFromStringList(fList, "verbform");
            f_mood = getStrFromStringList(fList, "mood");
            f_tense = getStrFromStringList(fList, "tense");
            f_aspect = getStrFromStringList(fList, "aspect");
            f_gender = getStrFromStringList(fList, "gender");
            f_case = getStrFromStringList(fList, "case");
            f_prontype = getStrFromStringList(fList, "prontype");
            f_reflex = getStrFromStringList(fList, "reflex");
            f_animateness = getStrFromStringList(fList, "animateness");
            f_degree = getStrFromStringList(fList, "degree");
            f_subpos = getStrFromStringList(fList, "subpos");
            f_numform = getStrFromStringList(fList, "numform");
            f_abbr = getStrFromStringList(fList, "abbr");
            f_hyph = getStrFromStringList(fList, "hyph");
            f_foreign = getStrFromStringList(fList, "foreign");
            f_voice = getStrFromStringList(fList, "voice");
            f_typo = getStrFromStringList(fList, "typo");


            if (!string.IsNullOrEmpty(f_pos))
                f_pos = f_pos.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_negativeness))
                f_negativeness = f_negativeness.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_number))
                f_number = f_number.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_person))
                f_person = f_person.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_verbform))
                f_verbform = f_verbform.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_mood))
                f_mood = f_mood.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_tense))
                f_tense = f_tense.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_aspect))
                f_aspect = f_aspect.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_gender))
                f_gender = f_gender.Split('=')[1];

            if (!string.IsNullOrEmpty(f_case))
                f_case = f_case.Split('=')[1];

            if (!string.IsNullOrEmpty(f_prontype))
                f_prontype = f_prontype.Split('=')[1];

            if (!string.IsNullOrEmpty(f_reflex))
                f_reflex = f_reflex.Split('=')[1];
            
            if (!string.IsNullOrEmpty(f_animateness))
                f_animateness = f_animateness.Split('=')[1];

            if (!string.IsNullOrEmpty(f_degree))
                f_degree = f_degree.Split('=')[1];

            if (!string.IsNullOrEmpty(f_subpos))
                f_subpos = f_subpos.Split('=')[1];

            if (!string.IsNullOrEmpty(f_numform))
                f_numform = f_numform.Split('=')[1];

            if (!string.IsNullOrEmpty(f_abbr))
                f_abbr = f_abbr.Split('=')[1];

            if (!string.IsNullOrEmpty(f_hyph))
                f_hyph = f_hyph.Split('=')[1];

            if (!string.IsNullOrEmpty(f_foreign))
                f_foreign = f_foreign.Split('=')[1];

            if (!string.IsNullOrEmpty(f_voice))
                f_voice = f_voice.Split('=')[1];

            if (!string.IsNullOrEmpty(f_typo))
                f_typo = f_typo.Split('=')[1];

            Console.Out.WriteLine("FillFeats end");
        }

        public string featsToString()
        {
            string strResult = string.Empty;
            /*
                strResult += f_pos + ",";
            
                strResult += f_negativeness + ",";
            
                strResult += f_number + ",";
            
                strResult += f_person + ",";
            
                strResult += f_verbform + ",";
            
                strResult += f_mood + ",";
           
                strResult += f_tense + ",";
            
                strResult += f_aspect + ",";
            
                strResult += f_gender + ",";
            
                strResult += f_case + ",";
            
                strResult += f_prontype + ",";
            
                strResult += f_reflex + ",";
            
                strResult += f_animateness + ",";
            
                strResult += f_degree + ",";
            
                strResult += f_subpos + ",";
            
                strResult += f_numform + ",";
            
                strResult += f_abbr + ",";
            
                strResult += f_hyph + ",";
            
                strResult += f_foreign + ",";
            
                strResult += f_voice + ",";
            
                strResult += f_typo + ",";
            */
            
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

        private string getStrFromStringList(List<string> list, string str)
        {
            foreach (string s in list)
            {
                if (s.Contains(str))
                    return s;
            }
            return string.Empty;
        }

        public static bool AreEqual(CoNLLToken t1, CoNLLToken t2)
        {
            bool result = false;

            if (t1.Id.Equals(t2.Id) && t1.Form.Trim().Equals(t2.Form.Trim()) && t1.Lemma.Trim().Equals(t2.Lemma.Trim()) && t1.Cpostag.Trim().Equals(t2.Cpostag.Trim()) && t1.Postag.Trim().Equals(t2.Postag.Trim()) && t1.Feats.Trim().Equals(t2.Feats.Trim()) && t1.Head.Trim().Equals(t2.Head.Trim()) && t1.Deprel.Trim().Equals(t2.Deprel.Trim()))
            //if (t1.Id.Equals(t2.Id) && t1.Form.Trim().Equals(t2.Form.Trim()) && t1.Lemma.Trim().Equals(t2.Lemma.Trim()))
            {
                result = true;
            }
            else
                result = false;
            return result;
        }
    }
}
