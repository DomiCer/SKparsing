using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Data;
using numl;
using numl.Model;
using numl.Supervised;
using numl.Supervised.DecisionTree;
using Infrastructure;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace NumlSample
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startAll = DateTime.Now;
            if (args.Length == 0 || (args.Length==1 && args[0].Equals("-h")))
            {
                NoParam();
            }
            //malt parser create parsing batch
            else if (args[0].Equals("-bp") && args.Length == 3)
            {
                BuildMaltBatchParse(args[1], args[2]);
            }
            //malt parser create training batch
            else if (args[0].Equals("-bt") && args.Length == 4)
            {
                BuildMaltBatchTrain(args[1], args[2], args[3]);
            }
            //parse oracle - R_dt
            else if (args[0].Equals("-p") && args.Length == 4)
            {
                ParseMode(args[1], args[2], float.Parse(args[3]));
            }
            //train oracle
            else if (args[0].Equals("-o") && args.Length == 3)
            {
                TrainMode(args[1], args[2]);
            }
            //train rozhodovac
            else if (args[0].Equals("-d") && args.Length == 3)
            {
                TrainModelRozhodovac(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("Chyba: neznama kombinacia parametrov!");
            }
            
            //Oracle oracleModels = Train();
            
            DateTime endAll = DateTime.Now;
            TimeSpan ts = endAll - startAll;
            Console.WriteLine("Global duration: " + ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString() + "." + ts.Milliseconds.ToString());
            //Console.ReadKey();
        }

        

        public static void BuildMaltBatchParse(string maltParserLocation, string sourceFile)
        {
            try
            {
                string content = string.Empty;
                content += "cd " + maltParserLocation + "\r\n";
                string baseParserOut = new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString();
                string modelName = new AppSettingsReader().GetValue("BaseParserModelName", typeof(string)).ToString();
                content += @"java -Xmx4g -jar maltparser-1.8.jar -c " + modelName + @" -i " + sourceFile + " -o " + AppDomain.CurrentDomain.BaseDirectory+ baseParserOut + " -a stacklazy -m parse";
                string batchFile = new AppSettingsReader().GetValue("BaseParserTrainBatch",typeof(string)).ToString();

                content += "\r\n";
                content += "cd " + AppDomain.CurrentDomain.BaseDirectory;
                File.WriteAllText(batchFile, content);
                Console.WriteLine("Batch file created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void BuildMaltBatchTrain(string maltParserLocation, string sourceFile, string modelName)
        {
            try
            {
                string content = string.Empty;
                content += "cd " + maltParserLocation + "\r\n";
                content += @"java -Xmx7g -jar maltparser-1.8.jar -c " + modelName + " -i " + sourceFile + " -a stacklazy -m learn";
                string batchFile = new AppSettingsReader().GetValue("BaseParserTrainBatch", typeof(string)).ToString();

                content += "\r\n";
                content += "cd " + AppDomain.CurrentDomain.BaseDirectory;
                File.WriteAllText(batchFile, content);
                Console.WriteLine("Batch file created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void TrainMode(string trainingFile, string modelName)
        {
            Oracle oracleModels = new Oracle();
            oracleModels.Oracle_TrainModel(trainingFile, modelName);
            Console.WriteLine("New oracle trained.");
        }

        public static void TrainModelRozhodovac(string trainingFile, string modelName)
        {
            Oracle oracleModels = new Oracle();
            oracleModels.IsTokenCritical_TrainModel(trainingFile, true, modelName);
            Console.WriteLine("New decision maker trained.");
        }

        public static void ParseMode(string input, string output, float param)
        {
            try
            {
                Oracle oracleModels = LoadModels();
                //oracleModels.ParseBase();
                Parse(oracleModels, param, output);

                Console.WriteLine("Evaluating...");
                string result = output.Replace(".conll", "_result.txt");
                File.WriteAllText(result, string.Empty);

                //string baseFile = new AppSettingsReader().GetValue("TestFile", typeof(string)).ToString();
                string baseFile = input;
                string baseParserOut = new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString();

                string resultFile = output;

                Console.WriteLine("Malt parser results:");
                File.AppendAllText(result, "Malt parser results:\r\n");

                ResultMetrics rm1 = CompareResult(baseFile, baseParserOut);
                File.AppendAllText(result, rm1.ToStringData());

                Console.WriteLine("***");
                File.AppendAllText(result, "\r\n *** \r\n\r\n");

                Console.WriteLine("New parser results:");
                File.AppendAllText(result, "New parser results:\r\n");
                ResultMetrics rm2 = CompareResult(baseFile, resultFile);
                File.AppendAllText(result, rm2.ToStringData());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void NoParam()
        {
            Console.WriteLine("SK parser - Help");
            Console.WriteLine("--------------");
            //Console.WriteLine("Vitaje!");
            Console.WriteLine("Pre parsovanie slovenskych viet prosim zadajte:");
            Console.WriteLine("-m parse -sf vstup.conll -tf vystup.conll");
        }

        
        public static void Parse(Oracle oracleModels, double critValue, string outputFile)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Parsing...");

            try
            {
                List<long> indexesToReParse = new List<long>();
                Dictionary<long, List<int>> indexesToReParseTokens = new Dictionary<long, List<int>>();

                List<Sentence> sentences = Sentence.ReadSentencesFromCoNLLFile(new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString());
                for (int i = 0; i < sentences.Count; i++)
                {
                    Sentence sent = sentences[i];
                    sent.SetParentAndGrandparentPosForEachToken(true, true);
                    sent.SetChildrenPosForEachToken(true);

                    double k = CountProbabilityCritical(sent, oracleModels);

                    if (k > critValue)
                    {
                        indexesToReParse.Add(i);
                        indexesToReParseTokens.Add(i, new List<int>());
                        for (int j = 0; j < sent.Tokens.Count; j++)
                        {
                            Token t = sent.Tokens[j];
                            if (t.IsCritical)
                                indexesToReParseTokens[i].Add(j);
                        }
                    }
                    else
                    {
                        //oracleModels.ParseBase(sent);
                    }
                    //}
                }

                string baseParserOutputFile = new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString();
                List<SentenceOracle> sentencesO = SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false);

                int allTokensToReparseCount = 0;

                if (sentences.Count == sentencesO.Count)
                {
                    Console.WriteLine("sentences to reparse: " + indexesToReParse.Count);

                    foreach (int idx in indexesToReParse)
                    {
                        SentenceOracle currentSent = sentencesO[idx];

                        List<int> tokensToReparse = indexesToReParseTokens[idx];
                        allTokensToReparseCount += tokensToReparse.Count;

                        currentSent = ReparseSentence(currentSent, tokensToReparse, oracleModels);
                    }
                    Console.WriteLine("tokens to reparse: " + allTokensToReparseCount);

                }

                if (string.IsNullOrEmpty(outputFile))
                    SaveSentencesToFile(sentencesO, new AppSettingsReader().GetValue("OracleOutput", typeof(string)).ToString());
                else
                    SaveSentencesToFile(sentencesO, outputFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
        
        }

        public static void Parse_RS_rand(Oracle oracleModels, double critValue)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Parsing using RS_random...");

            List<long> indexesToReParse = new List<long>();
            Dictionary<long, List<int>> indexesToReParseTokens = new Dictionary<long, List<int>>();

            //List<Sentence> sentencesToCompare = Sentence.ReadSentencesFromCoNLLFile(new AppSettingsReader().GetValue("TestFile", typeof(string)).ToString());

            List<Sentence> sentences = Sentence.ReadSentencesFromCoNLLFile(new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString());
            for (int i = 0; i < sentences.Count; i++)
            {
                Sentence sent = sentences[i];
                //if (IsStructureOKInSentence(sent, oracleModels))
                //{
                sent.SetParentAndGrandparentPosForEachToken(true, true);
                sent.SetChildrenPosForEachToken(true);

                double k = CountProbabilityCritical_random(sent, oracleModels);
                //double value = Convert.ToDouble(new AppSettingsReader().GetValue("CriticalValue", typeof(string)).ToString(), CultureInfo.InvariantCulture);

                if (k > critValue)
                {
                    //oracleModels.ParseBase(sent);
                    //oracleModels.ParseOracle(sent);
                    indexesToReParse.Add(i);
                    indexesToReParseTokens.Add(i, new List<int>());
                    for (int j = 0; j < sent.Tokens.Count; j++)
                    {
                        Token t = sent.Tokens[j];
                        if (t.IsCritical)
                            indexesToReParseTokens[i].Add(j);
                    }
                }
                else
                {
                    //oracleModels.ParseBase(sent);
                }
                //}
            }
            oracleModels.ParseBase();

            string baseParserOutputFile = new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString();
            List<SentenceOracle> sentencesO = SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false);
            //List<SentenceOracle> sentencesO = FillParentDeprel(SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false));

            if (sentences.Count == sentencesO.Count)
            {
                Console.WriteLine("indexes to reparse: " + indexesToReParse.Count);

                foreach (int idx in indexesToReParse)
                {
                    SentenceOracle currentSent = sentencesO[idx];

                    List<int> tokensToReparse = indexesToReParseTokens[idx];

                    currentSent = ReparseSentence(currentSent, tokensToReparse, oracleModels);
                }
            }
            SaveSentencesToFile(sentencesO, new AppSettingsReader().GetValue("OracleOutput", typeof(string)).ToString());
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");

        }

        public static void Parse_RS_perf(Oracle oracleModels, double critValue, string output)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Parsing using RS_perfect...");

            List<long> indexesToReParse = new List<long>();
            Dictionary<long, List<int>> indexesToReParseTokens = new Dictionary<long, List<int>>();

            //List<Sentence> sentencesToCompare = Sentence.ReadSentencesFromCoNLLFile(new AppSettingsReader().GetValue("TestFile", typeof(string)).ToString());

            List<Sentence> sentences = Sentence.ReadSentencesFromCoNLLFile(new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString());
            List<Sentence> sentencesRes = Sentence.ReadSentencesFromCoNLLFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\experimenty\2015_05_25_CZ_korpus\division\validate_part5_half2\result_parsed_validate.conll", true);
            for (int i = 0; i < sentences.Count; i++)
            {
                Sentence sent = sentences[i];
                //if (IsStructureOKInSentence(sent, oracleModels))
                //{
                sent.SetParentAndGrandparentPosForEachToken(true, true);
                sent.SetChildrenPosForEachToken(true);

                double k = CountProbabilityCritical_perfect(sent, sentencesRes[i], oracleModels);
                //double value = Convert.ToDouble(new AppSettingsReader().GetValue("CriticalValue", typeof(string)).ToString(), CultureInfo.InvariantCulture);

                if (k > critValue)
                {
                    //oracleModels.ParseBase(sent);
                    //oracleModels.ParseOracle(sent);
                    indexesToReParse.Add(i);
                    indexesToReParseTokens.Add(i, new List<int>());
                    for (int j = 0; j < sent.Tokens.Count; j++)
                    {
                        Token t = sent.Tokens[j];
                        if (t.IsCritical)
                            indexesToReParseTokens[i].Add(j);
                    }
                }
                else
                {
                    //oracleModels.ParseBase(sent);
                }
                //}
            }
            oracleModels.ParseBase();

            string baseParserOutputFile = new AppSettingsReader().GetValue("BaseParserOutput", typeof(string)).ToString();
            List<SentenceOracle> sentencesO = SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false);
            //List<SentenceOracle> sentencesO = FillParentDeprel(SentenceOracle.ReadSentencesFromCoNLLFile(baseParserOutputFile, false));

            int allTokensToReparseCount = 0;


            if (sentences.Count == sentencesO.Count)
            {
                Console.WriteLine("sentences to reparse: " + indexesToReParse.Count);

                foreach (int idx in indexesToReParse)
                {
                    SentenceOracle currentSent = sentencesO[idx];

                    List<int> tokensToReparse = indexesToReParseTokens[idx];
                    allTokensToReparseCount += tokensToReparse.Count;

                    currentSent = ReparseSentence(currentSent, tokensToReparse, oracleModels);
                }
                Console.WriteLine("tokens to reparse: " + allTokensToReparseCount.ToString());

            }
            //SaveSentencesToFile(sentencesO, new AppSettingsReader().GetValue("OracleOutput", typeof(string)).ToString());
            SaveSentencesToFile(sentencesO, output);
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");

        }

        public static List<SentenceOracle> AddChildrenPosInfo(List<SentenceOracle> sentences)
        {
            Console.WriteLine("adding children Cpostag info start...");

            foreach (SentenceOracle s in sentences)
            {
                s.SetChildrenPosForEachToken(true);
            }
            return sentences;
        }


      
        public static SentenceOracle ReparseSentence(SentenceOracle currentSent, List<int> tokensToReparse, Oracle oracleModels)
        {
            currentSent.SetChildrenPosForEachToken(true);
            foreach (int idToken in tokensToReparse)
            {
                TokenOracle token = currentSent.Tokens[idToken];
                token = ReparseToken(token, currentSent, tokensToReparse, oracleModels);
            }
            return currentSent;
        }

        public static TokenOracle ReparseToken(TokenOracle token, SentenceOracle sentence, List<int> tokensToReparse, Oracle oracleModels)
        {
            TokenOracle result = token;
            int intHeadToken = int.Parse(token.Head);

            if (intHeadToken == 0)
            {
                //result.ParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                //result.GParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");

                result.ParentPos = "root";
                result.GParentPos = "root"; 
            }
            else if (tokensToReparse.Contains(intHeadToken - 1))
            {
                TokenOracle parentToken = sentence.Tokens[intHeadToken - 1];
                if (!parentToken.IsReparsed)
                {
                    parentToken = ReparseToken(parentToken, sentence, tokensToReparse, oracleModels);
                   
                }
                //result.ParentCorrectDeprel = parentToken.CorrectDeprel;
                result.ParentPos = parentToken.Postag;

                int headParent = int.Parse(parentToken.Head);
                if (headParent == 0)
                {
                    result.GParentPos = "root";
                    //result.GParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                }
                else if (tokensToReparse.Contains(headParent - 1))
                {
                    TokenOracle gpToken = sentence.Tokens[headParent - 1];
                    if (!gpToken.IsReparsed)
                    {
                        gpToken = ReparseToken(gpToken, sentence, tokensToReparse, oracleModels);
                    }
                    //result.GParentCorrectDeprel = gpToken.CorrectDeprel;
                    result.GParentPos = gpToken.Postag;
                }
                else
                {
                    TokenOracle gpToken = sentence.Tokens[headParent - 1];
                    //result.GParentCorrectDeprel = gpToken.CorrectDeprel;
                    result.GParentPos = gpToken.Postag;
                }
            }
            else
            {
                TokenOracle parentToken = sentence.Tokens[intHeadToken - 1];
                //result.ParentCorrectDeprel = parentToken.CorrectDeprel;
                result.ParentPos = parentToken.Postag;

                int headParent = int.Parse(parentToken.Head);
                if (headParent == 0)
                {
                    result.GParentPos = "root";
                    //result.GParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                }
                else if (tokensToReparse.Contains(headParent - 1))
                {
                    TokenOracle gpToken = sentence.Tokens[headParent - 1];
                    if (!gpToken.IsReparsed)
                    {
                        gpToken = ReparseToken(gpToken, sentence, tokensToReparse, oracleModels);
                    }
                    //result.GParentCorrectDeprel = gpToken.CorrectDeprel;
                    result.GParentPos = gpToken.Postag;
                }
                else
                {
                    TokenOracle gpToken = sentence.Tokens[headParent - 1];
                    //result.GParentCorrectDeprel = gpToken.CorrectDeprel;
                    result.GParentPos = gpToken.Postag;
                }
            }
            result = oracleModels.ParseOracle(result);
            result.IsReparsed = true;

            return result;
        }

        /// <summary>
        /// uses parent and grndparent
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sentence"></param>
        /// <param name="tokensToReparse"></param>
        /// <param name="oracleModels"></param>
        /// <returns></returns>
        public static TokenOracle ReparseToken2(TokenOracle token, SentenceOracle sentence, List<int> tokensToReparse, Oracle oracleModels)
        {
            TokenOracle result = token;
            int intHeadToken = int.Parse(token.Head);

            if (intHeadToken == 0)
            {
                result.ParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                result.GParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
            }
            else 
            {
                TokenOracle parentToken = sentence.Tokens[intHeadToken - 1];
                int intParentHeadToken = int.Parse(parentToken.Head);

                if (intParentHeadToken == 0)
                {
                    result.GParentCorrectDeprel = (Deprel)Enum.Parse(typeof(Deprel), "root");
                }
                else 
                {
                    TokenOracle grandParentToken = sentence.Tokens[intParentHeadToken - 1];
                    if (tokensToReparse.Contains(intParentHeadToken) && !grandParentToken.IsReparsed)
                    {
                        grandParentToken = ReparseToken2(parentToken, sentence, tokensToReparse, oracleModels);
                    }
                    result.GParentCorrectDeprel = grandParentToken.CorrectDeprel;
                }

                if (tokensToReparse.Contains(intHeadToken - 1) && !parentToken.IsReparsed)
                {
                    parentToken = ReparseToken2(parentToken, sentence, tokensToReparse, oracleModels);
                }
                result.ParentCorrectDeprel = parentToken.CorrectDeprel;
            }
            result = oracleModels.ParseOracle(result);
            result.IsReparsed = true;

            return result;
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

        public static Oracle Train()
        {
            Oracle oracleModels = new Oracle();

            try
            {
                oracleModels.IsTokenCritical_TrainModel("IsTokenCriticalTrainFile");
                //oracleModels.IsHeadFalse_TrainModel("IsHeadFalseTrainFile");
                //oracleModels.Oracle_TrainModel("OracleTrainFile",@"C:\AllMyDocs\FIIT\02_Ing\DP\data\experimenty\2015_05_25_CZ_korpus\division\models\oracle_model_CZ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return oracleModels;
        }

        public static Oracle LoadModels()
        {
            Oracle oracleModels = new Oracle();

            try
            {
                oracleModels.IsTokenCritical_LoadModel("IsTokenCriticalModel");
                oracleModels.Oracle_LoadModel("OracleModel");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return oracleModels;
        }

        public static ResultMetrics CompareResult_sKontrolou(string fileA, string fileB)
        {
            List<Sentence> kontrola = Sentence.ReadSentencesFromCoNLLFile(@"kontrola.conll", true);

            ResultMetrics rm = new ResultMetrics();
            try
            {
                
                List<Sentence> sentencesA = Sentence.ReadSentencesFromCoNLLFile(fileA);
                List<Sentence> sentencesB = Sentence.ReadSentencesFromCoNLLFile(fileB);

                //pocet zhodnych a nezhodnych tokenov
                int headsTrue = 0;
                int headsFalse = 0;
                int depTrue = 0;
                int depFalse = 0;
                int headdepTrue = 0;
                int headdepFalse = 0;

                //pocet viet, ktore su komplet dobre
                int exactMatch = 0;

                int allTF = 0;
                int allTF_OK = 0;

                if (sentencesA.Count == sentencesB.Count && sentencesB.Count == kontrola.Count)
                {
                    for (int i = 0; i < sentencesA.Count; i++)
                    {
                        int correctTokens = 0;

                        Sentence sA = sentencesA[i];
                        Sentence sB = sentencesB[i];

                        if (sA.Tokens.Count == sB.Tokens.Count)
                        {
                            for (int j = 0; j < sA.Tokens.Count; j++)
                            {
                                if (kontrola[i].Tokens[j].IsCorrectHead && !kontrola[i].Tokens[j].IsCorrectDeprel)
                                {
                                    allTF++;
                                    if ((sA.Tokens[j].IsEqualInHeadAndDeprel(sB.Tokens[j])))
                                    {
                                        allTF_OK++;
                                    }
                                }

                                if (sA.Tokens[j].IsEqual(sB.Tokens[j]))
                                {
                                    if (sA.Tokens[j].IsEqualInHeadAndDeprel(sB.Tokens[j]))
                                    {
                                        headdepTrue++;
                                        headsTrue++;
                                        depTrue++;
                                        correctTokens++;
                                    }
                                    else
                                    {
                                        headdepFalse++;
                                        if (sA.Tokens[j].IsEqualInHead(sB.Tokens[j]))
                                        {
                                            headsTrue++;
                                            depFalse++;
                                            sB.Tokens[j].IsCorrectHead = true;
                                            sB.Tokens[j].IsCorrectDeprel = false;
                                            sB.Tokens[j].CorrectHead = sB.Tokens[j].Head;
                                            sB.Tokens[j].CorrectDeprel = sA.Tokens[j].Deprel;

                                            
                                        }
                                        else if (sA.Tokens[j].IsEqualInDeprel(sB.Tokens[j]))
                                        {
                                            depTrue++;
                                            headsFalse++;
                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].IsCorrectDeprel = true;
                                            sB.Tokens[j].CorrectDeprel = sB.Tokens[j].Deprel;
                                            
                                        }
                                        else
                                        {
                                            headsFalse++;
                                            depFalse++;

                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].IsCorrectDeprel = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].CorrectDeprel = sA.Tokens[j].Deprel;
                                            
                                        }
                                    }

                                }
                            }
                        }
                        else
                            Console.WriteLine("! tokens count in sentences #" + i.ToString() + "are not equal");
                        
                        //ak je cela veta dobre 
                        if (correctTokens == sA.Tokens.Count)
                            exactMatch++;
                    }
                }
                else
                    Console.WriteLine("! sentences count not equal.");

                float LAS = ((float)headdepTrue / (float)(headdepTrue + headdepFalse)) * 100;
                float UAS = ((float)headsTrue / (float)(headsTrue + headsFalse)) * 100;
                float LA = ((float)depTrue / (float)(depTrue + depFalse)) * 100;

                float EM = ((float)exactMatch / (float)sentencesA.Count) * 100;
                float LAS_ = ((float)allTF_OK / (float)allTF) * 100;

                rm.LAS = LAS;
                rm.UAS = UAS;
                rm.LA = LA;
                rm.EM = EM;
                rm.LAS_ = LAS_;

                Console.WriteLine("-----");

                Console.WriteLine("LAS: " + Math.Round(LAS, 2).ToString() + " %");
                Console.WriteLine("UAS: " + Math.Round(UAS, 2).ToString() + " %");
                Console.WriteLine("LA: " + Math.Round(LA, 2).ToString() + " %");
                Console.WriteLine("EM: " + Math.Round(EM, 2).ToString() + " %");
                Console.WriteLine("Las*: " + Math.Round(LAS_, 2).ToString() + " %");
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rm;
        }

        public static ResultMetrics CompareResult(string fileA, string fileB)
        {
            
            ResultMetrics rm = new ResultMetrics();
            try
            {

                List<Sentence> sentencesA = Sentence.ReadSentencesFromCoNLLFile(fileA);
                List<Sentence> sentencesB = Sentence.ReadSentencesFromCoNLLFile(fileB);

                //pocet zhodnych a nezhodnych tokenov
                int headsTrue = 0;
                int headsFalse = 0;
                int depTrue = 0;
                int depFalse = 0;
                int headdepTrue = 0;
                int headdepFalse = 0;

                //pocet viet, ktore su komplet dobre
                int exactMatch = 0;

                int allTF = 0;
                int allTF_OK = 0;

                if (sentencesA.Count == sentencesB.Count)
                {
                    for (int i = 0; i < sentencesA.Count; i++)
                    {
                        int correctTokens = 0;

                        Sentence sA = sentencesA[i];
                        Sentence sB = sentencesB[i];

                        if (sA.Tokens.Count == sB.Tokens.Count)
                        {
                            for (int j = 0; j < sA.Tokens.Count; j++)
                            {
                               
                                if (sA.Tokens[j].IsEqual(sB.Tokens[j]))
                                {
                                    if (sA.Tokens[j].IsEqualInHeadAndDeprel(sB.Tokens[j]))
                                    {
                                        headdepTrue++;
                                        headsTrue++;
                                        depTrue++;
                                        correctTokens++;
                                    }
                                    else
                                    {
                                        headdepFalse++;
                                        if (sA.Tokens[j].IsEqualInHead(sB.Tokens[j]))
                                        {
                                            headsTrue++;
                                            depFalse++;
                                            sB.Tokens[j].IsCorrectHead = true;
                                            sB.Tokens[j].IsCorrectDeprel = false;
                                            sB.Tokens[j].CorrectHead = sB.Tokens[j].Head;
                                            sB.Tokens[j].CorrectDeprel = sA.Tokens[j].Deprel;


                                        }
                                        else if (sA.Tokens[j].IsEqualInDeprel(sB.Tokens[j]))
                                        {
                                            depTrue++;
                                            headsFalse++;
                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].IsCorrectDeprel = true;
                                            sB.Tokens[j].CorrectDeprel = sB.Tokens[j].Deprel;

                                        }
                                        else
                                        {
                                            headsFalse++;
                                            depFalse++;

                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].IsCorrectDeprel = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].CorrectDeprel = sA.Tokens[j].Deprel;

                                        }
                                    }

                                }
                            }
                        }
                        else
                            Console.WriteLine("! tokens count in sentences #" + i.ToString() + "are not equal");

                        //ak je cela veta dobre 
                        if (correctTokens == sA.Tokens.Count)
                            exactMatch++;
                    }
                }
                else
                    Console.WriteLine("! sentences count not equal.");

                float LAS = ((float)headdepTrue / (float)(headdepTrue + headdepFalse)) * 100;
                float UAS = ((float)headsTrue / (float)(headsTrue + headsFalse)) * 100;
                float LA = ((float)depTrue / (float)(depTrue + depFalse)) * 100;

                float EM = ((float)exactMatch / (float)sentencesA.Count) * 100;
                //float LAS_ = ((float)allTF_OK / (float)allTF) * 100;

                rm.LAS = LAS;
                rm.UAS = UAS;
                rm.LA = LA;
                rm.EM = EM;
                //rm.LAS_ = LAS_;

                Console.WriteLine("-----");

                Console.WriteLine("LAS: " + Math.Round(LAS, 2).ToString() + " %");
                Console.WriteLine("UAS: " + Math.Round(UAS, 2).ToString() + " %");
                Console.WriteLine("LA: " + Math.Round(LA, 2).ToString() + " %");
                Console.WriteLine("EM: " + Math.Round(EM, 2).ToString() + " %");
                //Console.WriteLine("Las*: " + Math.Round(LAS_, 2).ToString() + " %");
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rm;
        }


        public static bool IsStructureOKInSentence(Sentence s, Oracle oracle)
        {
            bool bResult = false;
            int correctHeads = 0;
            foreach (Token t in s.Tokens)
            {
                TokenHead th = new TokenHead(t);
                TokenHead th_new = oracle.ModelIsHeadFalse.Model.Predict(th);

                if (th_new.IsCorrectHead)
                {
                    correctHeads++;
                }
                
            }
            double score = (double)correctHeads/(double)s.Tokens.Count;
            if (score > 0)
                bResult = true;

            return bResult;
        }

        public static double CountProbabilityCritical(Sentence s, Oracle oracle)
        {
            double result = 0;
            int criticalTokensCount = 0;

            foreach (Token t in s.Tokens)
            {
                //TokenHead th = new TokenHead(t);
                //TokenHead th_new = oracle.ModelIsHeadFalse.Model.Predict(th);

                //if (th_new.IsCorrectHead)
                //{
                    t.IsCritical = oracle.IsTokenCitical(t);
                    if (t.IsCritical)
                        criticalTokensCount++;
                //}
            }

            result = (double)criticalTokensCount / (double)s.Tokens.Count;
            return result;
        }

        public static double CountProbabilityCritical_random(Sentence s, Oracle oracle)
        {
            double result = 0;
            int criticalTokensCount = 0;

            foreach (Token t in s.Tokens)
            {
                Random r = new Random();
                int randomNum = r.Next(100);

                if (randomNum % 2 == 0)
                    t.IsCritical = true;
                else
                    t.IsCritical = false;

                if (t.IsCritical)
                    criticalTokensCount++;
            }

            result = (double)criticalTokensCount / (double)s.Tokens.Count;
            return result;
        }

        public static double CountProbabilityCritical_perfect(Sentence s, Sentence s_ok, Oracle oracle)
        {
            double result = 0;
            int criticalTokensCount = 0;

            if (s.Tokens.Count == s_ok.Tokens.Count)
            {
                for (int i = 0; i < s.Tokens.Count; i++)
                {
                    Token t = s.Tokens[i];
                    if (s_ok.Tokens[i].IsCorrectHead && !s_ok.Tokens[i].IsCorrectDeprel)
                    {
                        t.IsCritical = true;
                        criticalTokensCount++;
                    }
                    else
                        t.IsCritical = false;
                }
            }
            else
            {
                Console.WriteLine("nezhodny pocet tokenov");
            }

            result = (double)criticalTokensCount / (double)s.Tokens.Count;
            return result;
        }

        private static void SaveSentencesToFile(List<SentenceOracle> senteces, string fileName)
        {
            try
            {
                string fileContent = string.Empty;
                File.WriteAllText(fileName, fileContent);

                foreach (SentenceOracle s in senteces)
                {
                    fileContent = string.Empty;
                    foreach (TokenOracle t in s.Tokens)
                    {
                        fileContent += t.Id + "\t";
                        fileContent += t.Form + "\t";
                            fileContent += t.Lemma + "\t";
                        
                        fileContent += t.Cpostag + "\t";
                            fileContent += t.Postag + "\t";

                            fileContent += t.Feats + "\t" + t.Head + "\t";
                            if (t.CorrectDeprel.Equals(Deprel.NULL))
                                fileContent += t.PredictedDeprel;
                            else
                                fileContent += t.CorrectDeprel;

                        fileContent += "\r\n";
                    }
                    fileContent += "\r\n";

                    File.AppendAllText(fileName, fileContent);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        
    }
}
