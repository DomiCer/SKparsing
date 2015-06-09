using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum Deprel
    {
        NULL,
        root,
        ExD_M,
        Coord,
        Pred,
        Obj,
        AuxX,
        Sb,
        Pnom,
        AuxP,
        Atr,
        Adv,
        AuxK,
        Pred_M,
        AuxC,
        Coord_M,
        AuxZ,
        AuxT,
        ExD,
        Obj_M,
        AuxY,
        AuxV,
        AuxG,
        AuxO,
        AuxP_M,
        Apposition,
        Atv,
        AtvV,
        Atr_M,
        AuxC_M,
        Adv_M,
        Pnom_M,
        Sb_M,
        AuxR,
        Apposition_M,
        AuxY_M,
        AuxO_M,
        AuxZ_M,
        Atv_M,
        AuxG_M,
        AuxX_M,
        AtvV_M,
        AuxV_M,
        AuxR_M,
        Apos,
        AtrAdv,
        AtrAtr,
        AdvAtr,
        ObjAtr,
        AtrObj
    }

    public enum F_animateness
    {
        NULL,
        ANIM,
        INAN
    }
    public enum F_aspect
    {
        NULL,
        PERF,
        IMP,
        IMP_PERF
    }
    public enum F_case
    {
        NOM,
        NULL,
        DAT,
        LOC,
        ACC,
        INS,
        GEN,
        VOC
    }
    public enum F_degree
    {
        NULL,
        SUP,
        POS,
        COMP
    }
    
    public enum F_gender
    {
        NEUT,
        NULL,
        MASC,
        FEM
    }
    
    public enum F_mood
    {
        NULL,
        IMP,
        IND,
        CND
    }
    public enum F_negativeness
    {
        NULL,
        POS,
        NEG
    }

    public enum F_number
    {
        SING,
        NULL,
        PLU
    }


    public enum F_pos
    {
        NOUN,
        PUNC,
        VERB,
        ADJ,
        PREP,
        CONJ,
        ADV,
        PART,
        INT,
        NUM,
        NULL,
        PROP,
        POS
    }

    public enum F_subpos
    {
        NULL,
        PROP,
        VOC,
        PREPPRON
    }
    public enum F_tense
    {
        NULL,
        PRES,
        PAST,
        FUT
    }
    
    public enum F_verbform
    {
        NULL,
        FIN,
        PART,
        INF,
        TRANS
    }
    public enum F_voice
    {
        NULL,
        PASS,
        ACT
    }


    public class Types
    {
    }
}
