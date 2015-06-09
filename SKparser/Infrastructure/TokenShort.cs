using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numl;
using numl.Model;

namespace Infrastructure
{
    public class TokenShort
    {
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
        [Label]
        public bool IsCorrectDeprel { get; set; }

        public TokenShort(Token t)
        {
        f_pos = t.f_pos;
        f_negativeness = t.f_negativeness;
        f_number = t.f_number;
        f_person = t.f_person;
        f_verbform = t.f_verbform;
        f_mood = t.f_mood;
        f_tense = t.f_tense;
        f_aspect = t.f_aspect;
        f_gender = t.f_gender;
        f_case = t.f_case;
        f_prontype = t.f_prontype;
        f_reflex = t.f_reflex;
        f_animateness = t.f_animateness;
        f_degree = t.f_degree;
        f_subpos = t.f_subpos;
        f_numform = t.f_numform;
        f_abbr = t.f_abbr;
        f_hyph = t.f_hyph;
        f_foreign = t.f_foreign;
        f_voice = t.f_voice;
        f_typo = t.f_typo;
        IsCorrectDeprel = t.IsCorrectDeprel;
 
        }
        
    }
}
