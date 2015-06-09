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
    public class TokenOracle
    {
        public string Id { get; set; }
        public string Form { get; set; }
        public string Lemma { get; set; }
        public string Cpostag { get; set; }
        [Feature]
        public string Postag { get; set; }
        public string Feats { get; set; }
        public string Head { get; set; }

        [Feature]
        public Deprel PredictedDeprel { get; set; }
        
        [Label]
        public Deprel CorrectDeprel { get; set; }

        public Deprel ParentCorrectDeprel { get; set; }
        public Deprel GParentCorrectDeprel { get; set; }

        [Feature]
        public string ParentPos { get; set; }
        [Feature]
        public string GParentPos { get; set; }

        public bool IsReparsed { get; set; }

        [Feature]
        public string ChildrenPos { get; set; }

        //feats
        [Feature]
        public F_pos f_pos { get; set; }
        //[Feature]
        public F_negativeness f_negativeness { get; set; }
        //[Feature]
        public F_number f_number { get; set; }
        //[Feature]
        public int f_person { get; set; }
        //[Feature]
        public F_verbform f_verbform { get; set; }
        //[Feature]
        public F_mood f_mood { get; set; }
        //[Feature]
        public F_tense f_tense { get; set; }
        //[Feature]
        public F_aspect f_aspect { get; set; }
        //[Feature]
        public F_gender f_gender { get; set; }
        //[Feature]
        public F_case f_case { get; set; }
        //[Feature]
        public bool f_prontype { get; set; }
        //[Feature]
        public bool f_reflex { get; set; }
        //[Feature]
        public F_animateness f_animateness { get; set; }
        //[Feature]
        public F_degree f_degree { get; set; }
        //[Feature]
        public F_subpos f_subpos { get; set; }
        //[Feature]
        public bool f_numform { get; set; }
        //[Feature]
        public bool f_abbr { get; set; }
        //[Feature]
        public bool f_hyph { get; set; }
        //[Feature]
        public bool f_foreign { get; set; }
        //[Feature]
        public F_voice f_voice { get; set; }
        //[Feature]
        public bool f_typo { get; set; }

        public TokenOracle(string token, bool hasResult, bool hasPredictedDeprel)
        {
            string[] splitted = token.Split(new string[] { "\t" }, StringSplitOptions.None);
            Id = splitted[0];
            Form = splitted[1];
            Lemma = splitted[2];
            Cpostag = splitted[3];
            Postag = splitted[4];
            Feats = splitted[5];
            Head = splitted[6];

            if(hasPredictedDeprel)
                PredictedDeprel = (Deprel)Enum.Parse(typeof(Deprel), splitted[7]);

            if (hasResult)
            {
                //    IsCorrectHead = Convert.ToBoolean(splitted[8]);
                //    IsCorrectDeprel = Convert.ToBoolean(splitted[9]);
                //    CorrectHead = splitted[10];
                CorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), splitted[11]);

                //odpoznamkovat
                ParentPos = splitted[12];
                GParentPos = splitted[13];
                ChildrenPos = splitted[14];
                

            }
            else
            {
                //CorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), splitted[7]);
                CorrectDeprel = Deprel.NULL;
                ParentCorrectDeprel = Deprel.NULL;
                GParentCorrectDeprel = Deprel.NULL;
            }

            IsReparsed = false;

            FillFeats();
        }

        
        public void FillFeats()
        {
            //Console.Out.WriteLine("FillFeats start");

            List<string> fList = new List<string>();
            string[] parsed = this.Feats.Split('|');
            if (parsed.Length > 0)
                fList = parsed.ToList();

            string strPom = string.Empty;

            strPom = Token.getStrFromStringList(fList, "pos");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom.Trim()))
                f_pos = F_pos.NULL;
            else
                f_pos = (F_pos)Enum.Parse(typeof(F_pos), strPom.ToUpper());

            /*
            strPom = Token.getStrFromStringList(fList, "negativeness");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_negativeness = F_negativeness.NULL;
            else
                f_negativeness = (F_negativeness)Enum.Parse(typeof(F_negativeness), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "number");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_number = F_number.NULL;
            else
                f_number = (F_number)Enum.Parse(typeof(F_number), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "person");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_person = 0;
            else
                f_person = int.Parse(strPom);

            strPom = Token.getStrFromStringList(fList, "verbform");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_verbform = F_verbform.NULL;
            else
                f_verbform = (F_verbform)Enum.Parse(typeof(F_verbform), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "mood");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_mood = F_mood.NULL;
            else
                f_mood = (F_mood)Enum.Parse(typeof(F_mood), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "tense");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_tense = F_tense.NULL;
            else
                f_tense = (F_tense)Enum.Parse(typeof(F_tense), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "aspect");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_aspect = F_aspect.NULL;
            else
                f_aspect = (F_aspect)Enum.Parse(typeof(F_aspect), strPom.ToUpper().Replace(";", "_"));

            strPom = Token.getStrFromStringList(fList, "gender");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_gender = F_gender.NULL;
            else
                f_gender = (F_gender)Enum.Parse(typeof(F_gender), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "case");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_case = F_case.NULL;
            else
                f_case = (F_case)Enum.Parse(typeof(F_case), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "prontype");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_prontype = false;
            else
                f_prontype = true;

            strPom = Token.getStrFromStringList(fList, "reflex");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_reflex = false;
            else
                f_reflex = true;

            strPom = Token.getStrFromStringList(fList, "animateness");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_animateness = F_animateness.NULL;
            else
                f_animateness = (F_animateness)Enum.Parse(typeof(F_animateness), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "degree");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_degree = F_degree.NULL;
            else
                f_degree = (F_degree)Enum.Parse(typeof(F_degree), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "subpos");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_subpos = F_subpos.NULL;
            else
                f_subpos = (F_subpos)Enum.Parse(typeof(F_subpos), strPom.ToUpper());


            strPom = Token.getStrFromStringList(fList, "numform");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_numform = false;
            else
                f_numform = true;

            strPom = Token.getStrFromStringList(fList, "abbr");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_abbr = false;
            else
                f_abbr = true;

            strPom = Token.getStrFromStringList(fList, "hyph");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_hyph = false;
            else
                f_hyph = true;

            strPom = Token.getStrFromStringList(fList, "foreign");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_foreign = false;
            else
                f_foreign = true;

            strPom = Token.getStrFromStringList(fList, "voice");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_voice = F_voice.NULL;
            else
                f_voice = (F_voice)Enum.Parse(typeof(F_voice), strPom.ToUpper());

            strPom = Token.getStrFromStringList(fList, "typo");
            strPom = strPom.Contains('=') ? strPom.Split('=')[1] : string.Empty;
            if (string.IsNullOrEmpty(strPom))
                f_typo = false;
            else
                f_typo = true;

            */
            //Console.Out.WriteLine("FillFeats end");
        }

        public static List<TokenOracle> GetTokensFromCoNLLFile(string filepath, bool hasResult)
        {
            List<TokenOracle> resultList = new List<TokenOracle>();

            try
            {
                string[] lines = File.ReadAllLines(filepath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (!string.IsNullOrEmpty(lines[j].Trim()))
                        resultList.Add(new TokenOracle(lines[j], hasResult, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return resultList;
        }

    }
}
