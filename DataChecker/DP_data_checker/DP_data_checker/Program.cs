using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace DP_data_checker
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Start");

            try
            {

                if (args.Length > 0)
                {
                    if (args[0].Equals("-convert") && args.Length.Equals(4))
                    {
                        TreexToConll(args[1], args[2], args[3]);
                    }
                    else if (args[0].Equals("-oneFile") && args.Length.Equals(2))
                    {
                        ToOneFile(args[1]);
                    }
                    else if (args[0].Equals("-fileToHalves") && args.Length.Equals(2))
                    {
                        SplitFileToHalves(args[1], 1, false, false, false, false, false);
                    }
                    else if (args[0].Equals("-addParentInfo") && args.Length.Equals(2))
                    {
                        AddParentAndGrandparentPosInfo(args[1], true, true, true);
                    }
                    else if (args[0].Equals("-addChildrenInfo") && args.Length.Equals(2))
                    {
                        AddChildrenPosInfo(args[1], true);
                    }
                    else if (args[0].Equals("-crossValid") && args.Length == 4)
                    {
                        CrossValidationKFold(int.Parse(args[1]), args[2], args[3]);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Neznama kombinacia argumentov.");
            }


            //ConvertTreexToConllFiles();
            //ToOneFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\syntakticky_korpus_sk_komplet\conll");
            //get only complete senteces
            //string dir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\syntakticky_korpus_sk_komplet\conll\";
            //RemoveAllNoncompleteSentences(dir + "_all.conll", dir + "_complete.conll", dir + "_noncomplete.conll");
            
            //SplitFileToFiveParts(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\00_all\FiveFiles\_all.conll", 1, false, false, false, false, false);
            
            /*
            string dir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\00_all\FiveFiles\";
            string file = dir + "merged_1_2_3_4.conll";
            List<string> files = new List<string>();
            files.Add(dir + "part_1.conll");
            files.Add(dir + "part_2.conll");
            files.Add(dir + "part_3.conll");
            files.Add(dir + "part_4.conll");
            ToOneFile(file, files.ToArray());

            file = dir + "merged_1_2_3_5.conll";
            files = new List<string>();
            files.Add(dir + "part_1.conll");
            files.Add(dir + "part_2.conll");
            files.Add(dir + "part_3.conll");
            files.Add(dir + "part_5.conll");
            ToOneFile(file, files.ToArray());

            file = dir + "merged_1_2_4_5.conll";
            files = new List<string>();
            files.Add(dir + "part_1.conll");
            files.Add(dir + "part_2.conll");
            files.Add(dir + "part_4.conll");
            files.Add(dir + "part_5.conll");
            ToOneFile(file, files.ToArray());

            file = dir + "merged_1_3_4_5.conll";
            files = new List<string>();
            files.Add(dir + "part_1.conll");
            files.Add(dir + "part_3.conll");
            files.Add(dir + "part_4.conll");
            files.Add(dir + "part_5.conll");
            ToOneFile(file, files.ToArray());

            file = dir + "merged_2_3_4_5.conll";
            files = new List<string>();
            files.Add(dir + "part_2.conll");
            files.Add(dir + "part_3.conll");
            files.Add(dir + "part_4.conll");
            files.Add(dir + "part_5.conll");
            ToOneFile(file, files.ToArray());
            */
            //CrossValidationKFold(5, @"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\00_all\FiveFiles\03_stack-lazy");

            //DivisionToTrainAndTest();
            //SplitFileToHalves(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\experimenty\205_04_15_ineDataMP_ineRS\RS_train_half_1.conll", 1, true, false, true, true, true);

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Hotovo, trvanie: " + span.Hours+ "h " +span.Minutes +"min " +span.Seconds+"s");
        }

        private static void GetOracleTypes()
        {
            
            List<CoNLLToken> tokens = GetTokensFromCoNLLFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\experimenty\2015_04_15_ineDataMP_ineRS\Oracle_train.conll", true, false);
            HashSet<string> hs = new HashSet<string>();
            foreach (CoNLLToken t in tokens)
                hs.Add(t.Deprel);

            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach(string s in hs)
            {
                dic.Add(s, 0);
            }
            foreach (CoNLLToken t in tokens)
            {
                dic[t.Deprel]++;
            }

            List<KeyValuePair<string, int>> list = dic.OrderByDescending(x => x.Value).ToList();
            Console.WriteLine("hotovo");
        }

        private static void undersampling_RS_train(string source, string target)
        {
            List<CoNLLToken> tokensTT = new List<CoNLLToken>();
            List<CoNLLToken> tokensTF = new List<CoNLLToken>();
            List<CoNLLToken> tokensFT = new List<CoNLLToken>();
            List<CoNLLToken> tokensFF = new List<CoNLLToken>();

            List<CoNLLToken> allTokens = GetTokensFromCoNLLFile(source, true, false, true, true, true);
            foreach (CoNLLToken t in allTokens)
            {
                if (t.IsCorrectHead && t.IsCorrectDeprel)
                    tokensTT.Add(t);
                else if (t.IsCorrectHead)
                    tokensTF.Add(t);
                else if (t.IsCorrectDeprel)
                    tokensFT.Add(t);
                else
                    tokensFF.Add(t);
            }

            //int countPerEach = getMinimum(tokensFF.Count, tokensFT.Count, tokensTF.Count, tokensTT.Count);
            int countPerEach = tokensTF.Count * 8;

            List<CoNLLToken> result = new List<CoNLLToken>();
            for (int i = 0; i < countPerEach; i++)
            {
                if (i < tokensTT.Count)
                    result.Add(tokensTT[i]);
                if (i < tokensTF.Count)
                    result.Add(tokensTF[i]);
                if (i < tokensFT.Count)
                    result.Add(tokensFT[i]);
                if (i < tokensFF.Count)
                    result.Add(tokensFF[i]);
            }

            //foreach (CoNLLToken t in result)
            //    SaveTokenToFile(t, target, true, false, true, true, true);
        }

        private static void resampling_oracle(string source, string target)
        {
            List<CoNLLToken> tokensTF = new List<CoNLLToken>();
            List<CoNLLToken> tokensFF = new List<CoNLLToken>();

            List<CoNLLToken> allTokens = GetTokensFromCoNLLFile(source, true, false, true, true, true);
            foreach (CoNLLToken t in allTokens)
            {
                if (t.IsCorrectHead)
                    tokensTF.Add(t);
                else 
                    tokensFF.Add(t);
            }

            int countPerEach = Math.Min(tokensFF.Count, tokensTF.Count);
            //int countPerEach = tokensTF.Count * 8;

            List<CoNLLToken> result = new List<CoNLLToken>();
            for (int i = 0; i < countPerEach; i++)
            {
                result.Add(tokensTF[i]);
                result.Add(tokensFF[i]);
            }

            foreach (CoNLLToken t in result)
                SaveTokenToFile(t, target, true, false, true, true, true);
        }

        private static int getMinimum(int a, int b, int c, int d)
        {
            int x = 0;
            x = Math.Min(Math.Min(a, b), Math.Min(c, d));
            return x;
        }

        public static void DivisionToTrainAndTest()
        {
            string dir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\00_all\FiveFiles\03_stack-lazy\division_To_train_and_test\";

            //SplitFileToHalves(dir + "_problemSent.conll", 1, true, false, false, false, false);
            //SplitFileToHalves(dir + "_okSent.conll", 1, true, false, false, false, false);

            //SplitFileToFiveParts(dir + "_problemSent_half_1.conll", 1, true, false, false, false, false);
            //SplitFileToFiveParts(dir + "_problemSent_half_2.conll", 1, true, false, false, false, false);

            //SplitFileToFiveParts(dir + "_okSent_half_1.conll", 1, true, false, false, false, false);
            //SplitFileToFiveParts(dir + "_okSent_half_2.conll", 1, true, false, false, false, false);

            //List<string> files = new List<string>();
            //files.Add(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\02_test\_okSent_test\okSent_10.conll");
            //files.Add(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\02_test\_problemSent_test\_problemSent_10.conll");
            //ToOneFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\02_test\validate.conll", files.ToArray());

            //ToOneFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\01_train\RS_train");
            //ToOneFile(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\01_train\MP_train");

            //AddParentAndGrandparentPosInfo(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\01_train\RS_train\_all.conll",true, true, true);
            AddChildrenPosInfo(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\01_train\RS_train\_RS_train.conll",true);
        }



        /// <summary>
        /// rozdeli vety z vysledku kros valdacie na tie, ktore su problematicke (ok head, zle deprel) a tie ktore nie su
        /// </summary>
        /// <param name="resultFile">subor s vysledkom krizovej valid.</param>
        /// <param name="okFile">subor s vetami ok</param>
        /// <param name="problematicFile">subor s vetami [ok head, zle deprel]</param>
        public static void SplitResultToProblematicAndOK(string resultFile, string okFile, string problematicFile)
        {
            List<CoNLLSentence> allSent = GetSentencesFromCoNLLFile(resultFile, true, false);
            List<CoNLLSentence> okSent = new List<CoNLLSentence>();
            List<CoNLLSentence> problSent = new List<CoNLLSentence>();

            foreach (CoNLLSentence sent in allSent)
            {
                bool sentOk = false;
                foreach (CoNLLToken t in sent.Tokens)
                {
                    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                    {
                        sentOk = false;
                        break;
                    }
                    else
                    {
                        sentOk = true;
                    }
                }
                if (sentOk)
                    okSent.Add(sent);
                else
                    problSent.Add(sent);

            }

            foreach (CoNLLSentence s in okSent)
                SaveSentenceToFile(s, okFile, true, false);
            foreach (CoNLLSentence s in problSent)
                SaveSentenceToFile(s, problematicFile, true, false);

            Console.WriteLine("ok: " + okSent.Count.ToString());
            Console.WriteLine("probl: " + problSent.Count.ToString());
            Console.WriteLine("all: " + allSent.Count.ToString());
        }

        public static void ConvertTreexToConllFiles()
        {
            string sourceDir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\syntakticky_korpus_sk_komplet\syntax\";
            string targetDir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\syntakticky_korpus_sk_komplet\conll\";
            string mergedDir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\syntakticky_korpus_sk_komplet\conll_doplnene\";

            string hamledtDir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\hamledt-2.0-free_new\2.0\sk\conll\all\";

            List<string> files = Directory.GetFiles(sourceDir, "*.a").ToList();

            Parallel.For(0, files.Count, i =>
            {
                string f = files[i];
                string fileName = GetFileName(f).Replace(".a", "");
                TreexToConll(sourceDir, targetDir, fileName);
                //MergeIdenticFiles(targetDir + fileName + ".conll", hamledtDir + fileName + ".conll", mergedDir);
            });
        }

        public static void PrepareExperiment(string directory)
        {
            //data z crossvalidacie z MP
            string dir = @"C:\AllMyDocs\FIIT\02_Ing\DP\data\sk_korpus_conll\00_all\doplnene\FiveFiles\03_stack-lazy\";
            List<string> files = new List<string>();
            files.Add(dir + "result_parsed_part1.conll");
            files.Add(dir + "result_parsed_part2.conll");
            files.Add(dir + "result_parsed_part3.conll");
            files.Add(dir + "result_parsed_part4.conll");
            files.Add(dir + "result_parsed_part5.conll");

            ToOneFile(directory + "crosValidResult.conll", files.ToArray());

            //rozdelenie na vety, kde je spravny a nespravny deprel
            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(directory + "crosValidResult.conll", true, false);
            SplitToOKAndNonOKSentences(directory + "crosValidResult.conll", directory + "sentencesOK.conll", directory + "sentencesNotOK.conll");


        }

        public static void CountTrueHeadFalseDeprel(string file)
        {
            int correctSent = 0;
            int correctTok = 0;
            int incorrectSent = 0;
            int incorrectTok = 0;
            int allSent = 0;
            int allTok = 0;

            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(file, true, false);
            List<CoNLLSentence> OKSentences = new List<CoNLLSentence>();
            List<CoNLLSentence> notOKSentences = new List<CoNLLSentence>();

            foreach (CoNLLSentence sent in sentences)
            {
                allSent++;
                bool sNotOK = true;
                foreach (CoNLLToken tok in sent.Tokens)
                {
                    allTok++;
                    if (tok.IsCorrectHead && !tok.IsCorrectDeprel)
                        //if (!tok.IsCorrectDeprel)
                        {
                        incorrectTok++;
                        
                    }
                    else
                    {
                        correctTok++;
                        sNotOK = false;
                        
                    }
                }
                if (sNotOK)
                {
                    incorrectSent++;
                    notOKSentences.Add(sent);
                }
                else
                {
                    correctSent++;
                    OKSentences.Add(sent);
                }
            }

            Console.WriteLine("correct sentences: " + correctSent.ToString());
            Console.WriteLine("incorrect sentences: " + incorrectSent.ToString());
            Console.WriteLine("all sentences: " + allSent.ToString());

            Console.WriteLine("correct tokens: " + correctTok.ToString());
            Console.WriteLine("incorrect tokens: " + incorrectTok.ToString());
            Console.WriteLine("all tokens: " + allTok.ToString());

            Console.WriteLine("saving...");
            foreach (CoNLLSentence s in OKSentences)
            {
                SaveSentenceToFile(s, file.Replace(".conll", "_OKsent.conll"), true, false);
            }

            foreach (CoNLLSentence s in notOKSentences)
            {
                SaveSentenceToFile(s, file.Replace(".conll", "_notOKsent.conll"), true, false);
            }
        }

        /// <summary>
        /// rozdeli vety zo suboru na vety, v kt. je head=true a deprel=false a na vety ostatne
        /// </summary>
        /// <param name="file"></param>
        public static void SplitToOKAndNonOKSentences(string file, string fileOK, string fileNotOK)
        {
            int correctSent = 0;
            int correctTok = 0;
            int incorrectSent = 0;
            int incorrectTok = 0;
            int allSent = 0;
            int allTok = 0;

            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(file, true, false);
            List<CoNLLSentence> OKSentences = new List<CoNLLSentence>();
            List<CoNLLSentence> notOKSentences = new List<CoNLLSentence>();

            foreach (CoNLLSentence sent in sentences)
            {
                allSent++;
                bool sNotOK = true;
                foreach (CoNLLToken tok in sent.Tokens)
                {
                    allTok++;
                    if (tok.IsCorrectHead && !tok.IsCorrectDeprel)
                    //if (!tok.IsCorrectDeprel)
                    {
                        incorrectTok++;

                    }
                    else
                    {
                        correctTok++;
                        sNotOK = false;

                    }
                }
                if (sNotOK)
                {
                    incorrectSent++;
                    notOKSentences.Add(sent);
                }
                else
                {
                    correctSent++;
                    OKSentences.Add(sent);
                }
            }

            Console.WriteLine("correct sentences: " + correctSent.ToString());
            Console.WriteLine("incorrect sentences: " + incorrectSent.ToString());
            Console.WriteLine("all sentences: " + allSent.ToString());

            Console.WriteLine("correct tokens: " + correctTok.ToString());
            Console.WriteLine("incorrect tokens: " + incorrectTok.ToString());
            Console.WriteLine("all tokens: " + allTok.ToString());

            Console.WriteLine("saving...");
            foreach (CoNLLSentence s in OKSentences)
            {
                SaveSentenceToFile(s, fileOK, true, false);
            }

            foreach (CoNLLSentence s in notOKSentences)
            {
                SaveSentenceToFile(s, fileNotOK, true, false);
            }
        }

        public static void CountTrueHeadFalseDeprel(string file, bool t)
        {
            int correctSent = 0;
            int correctTok = 0;
            int incorrectSent = 0;
            int incorrectTok = 0;
            int allSent = 0;
            int allTok = 0;

            List<CoNLLToken> tokens = GetTokensFromCoNLLFile(file, true, false);
   
                foreach (CoNLLToken tok in tokens)
                {
                    allTok++;
                    if (tok.IsCorrectHead && !tok.IsCorrectDeprel)
                    {
                        incorrectTok++;
                    }
                    else
                    {
                        correctTok++;
                    }
                }
        
            Console.WriteLine("correct tokens: " + correctTok.ToString());
            Console.WriteLine("incorrect tokens: " + incorrectTok.ToString());
            Console.WriteLine("all tokens: " + allTok.ToString());

            
        }

        public static void AddParentDeprelInfo(string filePath, bool hasResult)
        {
            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(filePath, hasResult, false);
            foreach (CoNLLSentence s in sentences)
            {
                s.SetParentCorrectDeprelForEachToken();
                foreach (CoNLLToken t in s.Tokens)
                    SaveTokenToFile(t,filePath + "_added",true,false);
            }
            
        }



        public static void AddParentAndGrandparentDeprelInfo(string filePath, bool hasResult)
        {
            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(filePath, hasResult, true, true);
            foreach (CoNLLSentence s in sentences)
            {
                s.SetParentAndGrandparentDeprelForEachToken();
                foreach (CoNLLToken t in s.Tokens)
                    SaveTokenToFile(t, filePath + "_added", true, false, true);
            }

        }

        public static void AddParentAndGrandparentPosInfo(string filePath, bool hasResult, bool postagInfo, bool cpostagInfo)
        {
            Console.WriteLine("adding parent and grandarent Cpostag info start...");
            string replaceStr = string.Empty;
            if (postagInfo && cpostagInfo)
                replaceStr = "_parentCPosPosAdded.conll";
            else if (postagInfo)
                replaceStr = "_parentPosAdded.conll";
            else if (postagInfo)
                replaceStr = "_parentCPosAdded.conll";

            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(filePath, hasResult, false, false);
            foreach (CoNLLSentence s in sentences)
            {
                s.SetParentAndGrandparentPosForEachToken(postagInfo, cpostagInfo, hasResult);
                SaveSentenceToFile(s, filePath.Replace(".conll",replaceStr), hasResult, cpostagInfo,postagInfo);
            }

        }

        public static void AddChildrenPosInfo(string filePath, bool hasResult)
        {
            Console.WriteLine("adding children Cpostag info start...");

            List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(filePath, hasResult, !hasResult, hasResult, hasResult, false);
            foreach (CoNLLSentence s in sentences)
            {
                s.SetChildrenPosForEachToken(true, hasResult);
                SaveSentenceToFile(s, filePath.Replace(".conll", "_children.conll"), true, true, true, true);
            }

        }

        public static void bla()
        {
            string resultFile = "F_RESULTS.txt";
            string[] files = Directory.GetFiles(".");
            for(int i=0;i<files.Length;i++)
            {
                string filename = GetFileName(files[i]);
                if (filename.StartsWith("f_"))
                {
                    File.AppendAllText(resultFile,"public enum " + filename.Replace(".txt","") + "{\r\n");
                    List<string> lines = File.ReadAllLines(files[i]).ToList();
                    foreach (string s in lines)
                    {
                        if (string.IsNullOrEmpty(s.Trim()))
                            File.AppendAllText(resultFile, "NULL,\r\n");
                        else
                            File.AppendAllText(resultFile, s.Trim().ToUpper() + ",\r\n");
                    }

                    File.AppendAllText(resultFile, "}\r\n");
                }
            }

        }



        public static void CrossValidation2Fold()
        {
            //2-fold cross-validation
            string directory = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/00_all/doplnene/TwoFiles";
            ResultMetrics rm1 = CompareResult(directory + "/part1.conll", directory + "/parsed_part1.conll", false, "");
            ResultMetrics rm2 = CompareResult(directory + "/part2.conll", directory + "/parsed_part2.conll", false, "");

            ResultMetrics rmAverage = new ResultMetrics();
            rmAverage.LAS = (rm1.LAS + rm2.LAS) / 2;
            rmAverage.UAS = (rm1.UAS + rm2.UAS) / 2;
            rmAverage.LA = (rm1.LA + rm2.LA) / 2;

            Console.WriteLine("\r\nAverage: ");
            Console.WriteLine("LAS: " + Math.Round(rmAverage.LAS, 2).ToString() + " %");
            Console.WriteLine("UAS: " + Math.Round(rmAverage.UAS, 2).ToString() + " %");
            Console.WriteLine("LA: " + Math.Round(rmAverage.LA, 2).ToString() + " %");
        }

        /// <summary>
        /// directory - dir of original parts
        /// alg - 03_stack-lazy/02_stack-eager/01_transition-based
        /// </summary>
        /// <param name="k"></param>
        /// <param name="directory"></param>
        /// <param name="alg"></param>
        public static void CrossValidationKFold(int k, string directoryTest, string directoryParsed)
        {
            string resultFile = directoryParsed + "/result.txt";
            File.WriteAllText(resultFile, string.Empty);
            //k-fold cross-validation
            List<ResultMetrics> rmList = new List<ResultMetrics>();
            for (int i = 0; i < k; i++)
            {
                Console.WriteLine("Comparing part "+ (i+1).ToString() +"\r\n");
                rmList.Add(CompareResult(directoryTest + "/part" + (i + 1).ToString() + ".conll", directoryParsed + "/parsed_part" + (i + 1).ToString() + ".conll", true, directoryParsed + "/result_parsed_part" + (i + 1).ToString() + ".conll"));
                File.AppendAllText(resultFile, "part_" + (i+1).ToString() + "\r\n-------------------\r\n");
                File.AppendAllText(resultFile, rmList[i].ToStringData() + "\r\n");
            }
            
            double sumLAS = 0;
            double sumUAS = 0;
            double sumLA = 0;
            foreach (ResultMetrics rm in rmList)
            {
                sumLAS += rm.LAS;
                sumUAS += rm.UAS;
                sumLA += rm.LA;
            }
            ResultMetrics rmAverage = new ResultMetrics(sumLAS/k, sumUAS/k, sumLA/k);

            Console.WriteLine("\r\nAverage: ");
            Console.WriteLine("LAS: " + Math.Round(rmAverage.LAS, 2).ToString() + " %");
            Console.WriteLine("UAS: " + Math.Round(rmAverage.UAS, 2).ToString() + " %");
            Console.WriteLine("LA: " + Math.Round(rmAverage.LA, 2).ToString() + " %");

            File.AppendAllText(resultFile, "Average\r\n****************\r\n");
            File.AppendAllText(resultFile, rmAverage.ToStringData());
        }

        public static void ToOneFile(string dir)
        {
            string[] files = Directory.GetFiles(dir);

            File.WriteAllText(dir + "/_all.conll", string.Empty);
                    
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".conll"))
                {
                    string fileContent = File.ReadAllText(files[i]);
                    File.AppendAllText(dir + "/_all.conll", fileContent.Trim() + "\r\n\r\n");
                }
            }

            List<CoNLLSentence> sent = GetSentencesFromCoNLLFile(dir + "/_all.conll", false, false);
            Console.WriteLine("sent count: " + sent.Count.ToString());
            
            Console.WriteLine("Saved all to: " + dir + "/_all.conll");
        }

        public static void ToOneFile(string targetFile, string[] files)
        {
            File.WriteAllText(targetFile, string.Empty);
            List<CoNLLSentence> result = new List<CoNLLSentence>();

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".conll"))
                {
                    string fileContent = File.ReadAllText(files[i]);
                    File.AppendAllText(targetFile, fileContent.Trim() + "\r\n\r\n");
                }
            }
            List<CoNLLSentence> sent = GetSentencesFromCoNLLFile(targetFile, false, false);
            Console.WriteLine("sent count: " + sent.Count.ToString());
            Console.WriteLine("Saved all to: " + targetFile);
            
        }


        /// <summary>
        /// rozdeli vety v subore na polovicu a ulozi do dvoch suborov
        /// type - 1...vety
        /// type - 2...tokeny
        /// </summary>
        public static void SplitFileToHalves(string sourceFilePath, int type, bool hasResult, bool hasGPDeprel, bool hasGPCPos, bool hasGPPos, bool hasChildPos)
        {
            if (type == 1)
            {
                List<CoNLLSentence> sentenceList = GetSentencesFromCoNLLFile(sourceFilePath, hasResult, hasGPDeprel, hasGPCPos, hasGPPos, hasChildPos);

                Console.WriteLine("sentences count: " + sentenceList.Count.ToString());
                File.AppendAllText("log.txt", "sentences count: " + sentenceList.Count.ToString());

                string file1 = sourceFilePath.Replace(".conll", "_half_1.conll");
                string file2 = sourceFilePath.Replace(".conll", "_half_2.conll");

                List<CoNLLSentence> sentences1 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences2 = new List<CoNLLSentence>();

                for (int i = 0; i < sentenceList.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        SaveSentenceToFile(sentenceList[i], file1, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences1.Add(sentenceList[i]);
                    }
                    else
                    {
                        SaveSentenceToFile(sentenceList[i], file2, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences2.Add(sentenceList[i]);
                    }
                }

                Console.WriteLine("sentences1 count: " + sentences1.Count.ToString());
                Console.WriteLine("sentences2 count: " + sentences2.Count.ToString());
            }
            else
            {
                List<CoNLLToken> tokensList = GetTokensFromCoNLLFile(sourceFilePath, hasResult, hasGPDeprel, hasGPPos, hasChildPos);
                string file1 = sourceFilePath.Replace(".conll", "_half_1.conll");
                string file2 = sourceFilePath.Replace(".conll", "_half_2.conll");
                int i = 0;
                foreach (CoNLLToken t in tokensList)
                {
                    if (t.Postag != "_")
                    {
                        if (i % 2 == 0)
                            SaveTokenToFile(t, file1, hasResult, false, hasGPDeprel, hasGPPos, hasChildPos);
                        else
                            SaveTokenToFile(t, file2, hasResult, false, hasGPDeprel, hasGPPos, hasChildPos);
                        i++;
                    }
                    else
                    { }
                }
            }
            
        }

        /// <summary>
        /// rozdeli vety v subore na 5 casti a ulozi do 5 suborov
        /// type - 1...vety
        /// type - 2...tokeny
        /// </summary>
        public static void SplitFileToFiveParts(string sourceFilePath, int type, bool hasResult, bool hasGPDeprel, bool hasGPCPos, bool hasGPPos, bool hasChildPos)
        {
            if (type == 1)
            {
                List<CoNLLSentence> sentenceList = GetSentencesFromCoNLLFile(sourceFilePath, hasResult, hasGPDeprel, hasGPCPos, hasGPPos, hasChildPos);

                Console.WriteLine("sentences count: " + sentenceList.Count.ToString());
                File.AppendAllText("log.txt", "sentences count: " + sentenceList.Count.ToString());

                string file1 = sourceFilePath.Replace(GetFileName(sourceFilePath), "part_1.conll");
                string file2 = sourceFilePath.Replace(GetFileName(sourceFilePath), "part_2.conll");
                string file3 = sourceFilePath.Replace(GetFileName(sourceFilePath), "part_3.conll");
                string file4 = sourceFilePath.Replace(GetFileName(sourceFilePath), "part_4.conll");
                string file5 = sourceFilePath.Replace(GetFileName(sourceFilePath), "part_5.conll");

                List<CoNLLSentence> sentences1 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences2 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences3 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences4 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences5 = new List<CoNLLSentence>();

                int k = 1;
                for (int i = 0; i < sentenceList.Count; i++)
                {
                    if (k == 5)
                    {
                        SaveSentenceToFile(sentenceList[i], file5, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences5.Add(sentenceList[i]);
                        k = 1;
                    }
                    else if (k == 4)
                    {
                        SaveSentenceToFile(sentenceList[i], file4, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences4.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 3)
                    {
                        SaveSentenceToFile(sentenceList[i], file3, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences3.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 2)
                    {
                        SaveSentenceToFile(sentenceList[i], file2, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences2.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 1)
                    {
                        SaveSentenceToFile(sentenceList[i], file1, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences1.Add(sentenceList[i]);
                        k++;
                    }
                }

                Console.WriteLine("sentences1 count: " + sentences1.Count.ToString());
                Console.WriteLine("sentences2 count: " + sentences2.Count.ToString());
                Console.WriteLine("sentences3 count: " + sentences3.Count.ToString());
                Console.WriteLine("sentences4 count: " + sentences4.Count.ToString());
                Console.WriteLine("sentences5 count: " + sentences5.Count.ToString());

                Console.WriteLine("all: " + (sentences1.Count + sentences2.Count + sentences3.Count + sentences4.Count + sentences5.Count).ToString());
            }
            else
            {
                
            }

        }

        /// <summary>
        /// rozdeli vety v subore na 5 casti a ulozi do 5 suborov
        /// type - 1...vety
        /// type - 2...tokeny
        /// </summary>
        public static void SplitFileToSixParts(string sourceFilePath, int type, bool hasResult, bool hasGPDeprel, bool hasGPCPos, bool hasGPPos, bool hasChildPos)
        {
            if (type == 1)
            {
                List<CoNLLSentence> sentenceList = GetSentencesFromCoNLLFile(sourceFilePath, hasResult, hasGPDeprel, hasGPCPos, hasGPPos, hasChildPos);

                Console.WriteLine("sentences count: " + sentenceList.Count.ToString());
                File.AppendAllText("log.txt", "sentences count: " + sentenceList.Count.ToString());

                string[] dir_splitted = sourceFilePath.Split(new[] { "\\", "/" }, StringSplitOptions.None);
                string dir = dir_splitted[dir_splitted.Length - 1];

                string file1 = sourceFilePath.Replace(dir, "part_1.conll");
                string file2 = sourceFilePath.Replace(dir, "part_2.conll");
                string file3 = sourceFilePath.Replace(dir, "part_3.conll");
                string file4 = sourceFilePath.Replace(dir, "part_4.conll");
                string file5 = sourceFilePath.Replace(dir, "part_5.conll");
                string file6 = sourceFilePath.Replace(dir, "part_6.conll");

                List<CoNLLSentence> sentences1 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences2 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences3 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences4 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences5 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences6 = new List<CoNLLSentence>();

                int k = 1;
                for (int i = 0; i < sentenceList.Count; i++)
                {
                    if (k == 6)
                    {
                        SaveSentenceToFile(sentenceList[i], file6, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences6.Add(sentenceList[i]);
                        k = 1;
                    }
                    else if (k == 5)
                    {
                        SaveSentenceToFile(sentenceList[i], file5, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences5.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 4)
                    {
                        SaveSentenceToFile(sentenceList[i], file4, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences4.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 3)
                    {
                        SaveSentenceToFile(sentenceList[i], file3, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences3.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 2)
                    {
                        SaveSentenceToFile(sentenceList[i], file2, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences2.Add(sentenceList[i]);
                        k++;
                    }
                    else if(k==1)
                    {
                        SaveSentenceToFile(sentenceList[i], file1, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences1.Add(sentenceList[i]);
                        k++;
                    }
                }

                Console.WriteLine("sentences1 count: " + sentences1.Count.ToString());
                Console.WriteLine("sentences2 count: " + sentences2.Count.ToString());
                Console.WriteLine("sentences3 count: " + sentences3.Count.ToString());
                Console.WriteLine("sentences4 count: " + sentences4.Count.ToString());
                Console.WriteLine("sentences5 count: " + sentences5.Count.ToString());
                Console.WriteLine("sentences6 count: " + sentences6.Count.ToString());

                Console.WriteLine("all: " + (sentences1.Count + sentences2.Count + sentences3.Count + sentences4.Count + sentences5.Count + sentences6.Count).ToString());
            }
            else
            {

            }

        }

        /// <summary>
        /// rozdeli vety v subore na 5 casti a ulozi do 5 suborov
        /// type - 1...vety
        /// type - 2...tokeny
        /// </summary>
        public static void SplitFileToSevenParts(string sourceFilePath, int type, bool hasResult, bool hasGPDeprel, bool hasGPCPos, bool hasGPPos, bool hasChildPos)
        {
            if (type == 1)
            {
                List<CoNLLSentence> sentenceList = GetSentencesFromCoNLLFile(sourceFilePath, hasResult, hasGPDeprel, hasGPCPos, hasGPPos, hasChildPos);

                Console.WriteLine("sentences count: " + sentenceList.Count.ToString());
                File.AppendAllText("log.txt", "sentences count: " + sentenceList.Count.ToString());

                string[] dir_splitted = sourceFilePath.Split(new[] { "\\", "/" }, StringSplitOptions.None);
                string dir = dir_splitted[dir_splitted.Length - 1];

                string file1 = sourceFilePath.Replace(dir, "_1.conll");
                string file2 = sourceFilePath.Replace(dir, "_2.conll");
                string file3 = sourceFilePath.Replace(dir, "_3.conll");
                string file4 = sourceFilePath.Replace(dir, "_4.conll");
                string file5 = sourceFilePath.Replace(dir, "_5.conll");
                string file6 = sourceFilePath.Replace(dir, "_6.conll");
                string file7 = sourceFilePath.Replace(dir, "_7.conll");

                List<CoNLLSentence> sentences1 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences2 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences3 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences4 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences5 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences6 = new List<CoNLLSentence>();
                List<CoNLLSentence> sentences7 = new List<CoNLLSentence>();

                int k = 1;
                for (int i = 0; i < sentenceList.Count; i++)
                {
                    if (k == 7)
                    {
                        SaveSentenceToFile(sentenceList[i], file7, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences7.Add(sentenceList[i]);
                        k = 1;
                    }
                    else if (k == 6)
                    {
                        SaveSentenceToFile(sentenceList[i], file6, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences6.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 5)
                    {
                        SaveSentenceToFile(sentenceList[i], file5, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences5.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 4)
                    {
                        SaveSentenceToFile(sentenceList[i], file4, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences4.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 3)
                    {
                        SaveSentenceToFile(sentenceList[i], file3, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences3.Add(sentenceList[i]);
                        k++;
                    }
                    else if (k == 2)
                    {
                        SaveSentenceToFile(sentenceList[i], file2, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences2.Add(sentenceList[i]);
                        k++;
                    }
                    else if(k == 1)
                    {
                        SaveSentenceToFile(sentenceList[i], file1, hasResult, hasGPCPos, hasGPPos, hasChildPos);
                        sentences1.Add(sentenceList[i]);
                        k++;
                    }
                }

                Console.WriteLine("sentences1 count: " + sentences1.Count.ToString());
                Console.WriteLine("sentences2 count: " + sentences2.Count.ToString());
                Console.WriteLine("sentences3 count: " + sentences3.Count.ToString());
                Console.WriteLine("sentences4 count: " + sentences4.Count.ToString());
                Console.WriteLine("sentences5 count: " + sentences5.Count.ToString());
                Console.WriteLine("sentences6 count: " + sentences6.Count.ToString());
                Console.WriteLine("sentences7 count: " + sentences7.Count.ToString());

                Console.WriteLine("all: " + (sentences1.Count + sentences2.Count + sentences3.Count + sentences4.Count + sentences5.Count + sentences6.Count).ToString());
            }
            else
            {

            }

        }

        /// <summary>
        /// splits conll files in one directory into N parts
        /// </summary>
        /// <param name="dir">source directry</param>
        /// <param name="filesCount">N - number of final files</param>
        /// <param name="finalDir">final dir name</param>
        public static void ToMoreFiles(string dir, int filesCount, string finalDir)
        {
            Random rand = new Random();
            string[] files = Directory.GetFiles(dir);
            int filesInOneCount = files.Length / filesCount;

            if (!Directory.Exists(finalDir))
                Directory.CreateDirectory(finalDir);

            //final paths where the merged files are saved
            List<string> paths = new List<string>();
            for (int n = 0; n < filesCount; n++)
            {
                string path = finalDir + "/part" + (n + 1).ToString() + ".conll";
                File.WriteAllText(path, string.Empty);
                paths.Add(path);
            }

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".conll"))
                {
                    string fileContent = File.ReadAllText(files[i]);
                    string[] splitted = fileContent.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    List<CoNLLSentence> pom = GetSentencesFromCoNLLFile(files[i], false, false);

                    for (int s = 0; s < splitted.Length; s++)
                    {
                        int randomInt = rand.Next(filesCount);
                        File.AppendAllText(paths[randomInt], splitted[s].Trim() + "\r\n\r\n");                    
                    }
                        fileContent = string.Empty;
                }
            }
            
            Console.WriteLine("Saved");
        }

        private static void Ready3FoldCrossValid()
        {
            string[] pole = new string[2];
            string dir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/00_all/doplnene/ThreeFiles/";
            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            ToOneFile(dir + "merged_1_2.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part3.conll";
            ToOneFile(dir + "merged_1_3.conll", pole);

            pole[0] = dir + "part2.conll";
            pole[1] = dir + "part3.conll";
            ToOneFile(dir + "merged_2_3.conll", pole);
        }

        private static void Ready4FoldCrossValid()
        {
            string[] pole = new string[3];
            string dir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/00_all/doplnene/FourFiles/";
            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            pole[2] = dir + "part3.conll";
            ToOneFile(dir + "merged_1_2_3.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            pole[2] = dir + "part4.conll";
            ToOneFile(dir + "merged_1_2_4.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part3.conll";
            pole[2] = dir + "part4.conll";
            ToOneFile(dir + "merged_1_3_4.conll", pole);

            pole[0] = dir + "part2.conll";
            pole[1] = dir + "part3.conll";
            pole[2] = dir + "part4.conll";
            ToOneFile(dir + "merged_2_3_4.conll", pole);

        }

        private static void Ready5FoldCrossValid()
        {
            string[] pole = new string[4];
            string dir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/00_all/doplnene/FiveFiles/";
            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            pole[2] = dir + "part3.conll";
            pole[3] = dir + "part4.conll";
            ToOneFile(dir + "merged_1_2_3_4.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            pole[2] = dir + "part3.conll";
            pole[3] = dir + "part5.conll";
            ToOneFile(dir + "merged_1_2_3_5.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part2.conll";
            pole[2] = dir + "part4.conll";
            pole[3] = dir + "part5.conll";
            ToOneFile(dir + "merged_1_2_4_5.conll", pole);

            pole[0] = dir + "part1.conll";
            pole[1] = dir + "part3.conll";
            pole[2] = dir + "part4.conll";
            pole[3] = dir + "part5.conll";
            ToOneFile(dir + "merged_1_3_4_5.conll", pole);

            pole[0] = dir + "part2.conll";
            pole[1] = dir + "part3.conll";
            pole[2] = dir + "part4.conll";
            pole[3] = dir + "part5.conll";
            ToOneFile(dir + "merged_2_3_4_5.conll", pole);

        }

        private static void ReadyKFoldCrossValid(int k, string sourceDir, string targetDir)
        {
            List<string> pole = new List<string>();
            string sDir = (sourceDir.EndsWith("/") || sourceDir.EndsWith("\\")) ? sourceDir: sourceDir+"/";
            string tDir = (targetDir.EndsWith("/") || targetDir.EndsWith("\\")) ? targetDir : targetDir + "/";
            tDir += "merged";

            if (!Directory.Exists(tDir))
                Directory.CreateDirectory(tDir);

            
            for (int i = 0; i < k; i++)
            {
                pole.Add(sourceDir + "part" + (i + 1).ToString() + ".conll");
            }
            for (int i = 0; i < k; i++)
            {
                ToOneFile(tDir + "/merged_without" + (i + 1).ToString() + ".conll", RemoveAtAndReturnNewList(pole, i).ToArray());
            }
                
        }

        /// <summary>
        /// vymaze polozku na indexe i a vrati novy zoznam bez tejto polozky
        /// </summary>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static List<string> RemoveAtAndReturnNewList(List<string> list, int i)
        {
            List<string> resultList = new List<string>();
            if (i >= list.Count)
            {
                Exception ex = new Exception("Chyba: index nemoze byt vyssi ako pocet poloziek v zozname!");
                throw ex;
            }
            for (int idx = 0; idx < list.Count; idx++)
            {
                if (idx != i)
                    resultList.Add(list[idx]);
            }
            return resultList;
        }

        /// <summary>
        /// porovnava originalny subor so suborom sparsovanym automaticky vysledok porovnania sa ulozi do suboru result.txt
        /// je mozne tiez ulozit vysledok spravnosti urcenia head a deprel pre kazdu vetu a kazdy token
        /// </summary>
        /// <param name="fileA">povodny subor</param>
        /// <param name="fileB">parsovany subor</param>
        /// <param name="saveComparison">ulozit porovnanie pre kazdu vetu</param>
        /// <param name="saveToFile">cesta k suboru .conll kam vysledok ulozit (pouzije sa len ak je saveComparison==true)</param>
        /// <returns></returns>
        public static ResultMetrics CompareResult(string fileA, string fileB, bool saveComparison, string saveToFile)
        {
            ResultMetrics rm = new ResultMetrics();
            try
            {
                //ak chcem zapisovat podrobny vysledok, treba vytvorit adresar
                string dir = GetDirectory(saveToFile) + "/wrongly_classified";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                
                List<CoNLLSentence> sentencesA = GetSentencesFromCoNLLFile(fileA, false, false);
                List<CoNLLSentence> sentencesB = GetSentencesFromCoNLLFile(fileB,false,false);

                //pocet zhodnych a nezhodnych tokenov
                int headsTrue=0;
                int headsFalse=0;
                int depTrue = 0;
                int depFalse = 0;
                int headdepTrue = 0;
                int headdepFalse = 0;
                
                if (sentencesA.Count == sentencesB.Count)
                {
                    for (int i = 0; i < sentencesA.Count; i++)
                    {
                        CoNLLSentence sA = sentencesA[i];
                        CoNLLSentence sB = sentencesB[i];

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
                                        sB.Tokens[j].IsCorrectHead = true;
                                        sB.Tokens[j].IsCorrectDeprel = true;
                                        sB.Tokens[j].CorrectHead = sB.Tokens[j].Head;
                                        sB.Tokens[j].CorrectDeprel = sB.Tokens[j].Deprel;

                                        //SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/vsetkoUplneVsetko.conll", saveComparison, false);
                                            
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
                                            
                                            /*
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Deprel.conll", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Deprel.csv", saveComparison, false);
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/OnlyDeprel.conll", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/OnlyDeprel.csv", saveComparison, false);
                                            */
                                            //SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/vsetkoUplneVsetko.conll", saveComparison, false);
                                            
                                        }
                                        else if (sA.Tokens[j].IsEqualInDeprel(sB.Tokens[j]))
                                        {
                                            depTrue++;
                                            headsFalse++;
                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].IsCorrectDeprel = true;
                                            sB.Tokens[j].CorrectDeprel = sB.Tokens[j].Deprel;
                                            
                                            /*
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Head.conll", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Head.csv", saveComparison, false);
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/OnlyHead.conll", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/OnlyHead.csv", saveComparison, false);
                                            */
                                            //SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/vsetkoUplneVsetko.conll", saveComparison, false);
                                            
                                        }
                                        else
                                        {
                                            headsFalse++;
                                            depFalse++;

                                            sB.Tokens[j].IsCorrectHead = false;
                                            sB.Tokens[j].IsCorrectDeprel = false;
                                            sB.Tokens[j].CorrectHead = sA.Tokens[j].Head;
                                            sB.Tokens[j].CorrectDeprel = sA.Tokens[j].Deprel;
                                            /*
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/HeadDeprel.conll", saveComparison, false);
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Head.conll", saveComparison, false);
                                            SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Deprel.conll", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/HeadDeprel.csv", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Head.csv", saveComparison, false);
                                            SaveTokenToCsvFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/Deprel.csv", saveComparison, false);
                                            */
                                            //SaveTokenToFile(sB.Tokens[j], GetDirectory(saveToFile) + "/wrongly_classified/vsetkoUplneVsetko.conll", saveComparison, false);
                                            
                                        }
                                    }

                                }
                            }
                        }
                        else
                            Console.WriteLine("! tokens count in sentences #" + i.ToString() + "are not equal");
                        //zapis vety aj s vysledkom do suboru
                        //sB.SetParentCorrectDeprelForEachToken();
                        sB.SetParentAndGrandparentDeprelForEachToken();
                        sB.SetParentAndGrandparentPosForEachToken(true, true,true);
                        sB.SetChildrenPosForEachToken(true,true);
                        foreach (CoNLLToken token in sB.Tokens)
                        {
                            if (!token.IsCorrectDeprel)
                                SaveTokenToFile(token, fileB.Replace(GetFileName(fileB),"_badDeprel.conll"), true, false, true, true, true);
                            if (!token.IsCorrectDeprel && token.IsCorrectHead)
                                SaveTokenToFile(token, fileB.Replace(GetFileName(fileB), "_okHeadBadDeprel.conll"), true, false, true, true, true);
                        }
                        SaveSentenceToFile(sB, saveToFile, saveComparison, false);
                    }
                }
                else
                    Console.WriteLine("! sentences count not equal.");

                float LAS = (float)headdepTrue / (float)(headdepTrue + headdepFalse) * 100;
                float UAS = (float)headsTrue / (float)(headsTrue + headsFalse) * 100;
                float LA = (float)depTrue / (float)(depTrue + depFalse) * 100;

                rm.LAS = LAS;
                rm.UAS = UAS;
                rm.LA = LA;


                Console.WriteLine("-----");
                
                Console.WriteLine("LAS: " + Math.Round(LAS,2).ToString() + " %");
                Console.WriteLine("UAS: " + Math.Round(UAS, 2).ToString() + " %");
                Console.WriteLine("LA: " + Math.Round(LA, 2).ToString() + " %");
                Console.WriteLine("");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rm;
        }

        public static void CompareData()
        {
            try
            {
                string treexDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/syntakticky_korpus_sk_komplet/syntax";
                string conllDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/00_all";
                string[] conllFiles = Directory.GetFiles(conllDir);

                for (int i = 0; i < conllFiles.Length; i++)
                {
                    //ciste meno suboru
                    string[] filePathParts = conllFiles[i].Split(new string[] { "/", "\\" }, StringSplitOptions.None);
                    string fileName = filePathParts[filePathParts.Length - 1];

                    LogWriteLine(fileName);

                    if (File.Exists(treexDir + "/" + fileName.Replace(".conll", ".a")))
                    {
                        string[] strConllFile = File.ReadAllLines(conllDir + "/" + fileName);
                        string strAFile = File.ReadAllText(treexDir + "/" + fileName.Replace(".conll", ".a"));

                        int sentCountConll = 0;
                        int sentCountTreex = 0;

                        for (int lIdx = 0; lIdx < strConllFile.Length; lIdx++)
                        {
                            if (strConllFile[lIdx].StartsWith("1\t"))
                                sentCountConll++;
                        }
                        XmlDocument aDoc = new XmlDocument();
                        aDoc.LoadXml(strAFile);
                        sentCountTreex = aDoc.DocumentElement.ChildNodes[1].ChildNodes.Count;

                        if (sentCountTreex == sentCountConll)
                            LogWriteLine("sentenceCountEqual");
                        else
                            LogWriteLine("!sent count not equal: treex " + sentCountTreex.ToString() + "; conll " + sentCountConll.ToString());

                    }
                    else
                    {
                        Console.WriteLine("File not found: " + fileName.Replace(".conll", ".a"));
                        LogWriteLine("!!File not found: " + fileName.Replace(".conll", ".a"));
                    }

                    LogWriteLine("");
                }

            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void ConvertData()
        {
            int mismatchCount = 0;

            try
            {
                //string targetDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/01_train/doplnene";
                string targetDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/02_test/doplnene";
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                
                string korpusPath = "C:/AllMyDocs/FIIT/02_Ing/DP/data/syntakticky_korpus_sk_komplet/syntax";
                string[] mFiles = Directory.GetFiles(korpusPath, "*.m");
                //string sourceDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/01_train";
                string sourceDir = "C:/AllMyDocs/FIIT/02_Ing/DP/data/sk_korpus_conll/02_test";
                string[] files = Directory.GetFiles(sourceDir);

                //List<CoNLLSentence> sentences = new List<CoNLLSentence>();

                for (int i = 0; i < files.Length; i++)
                {
                    XmlDocument mFileDoc = new XmlDocument();
                    string mFilePath = FindMFileToCoNLL(mFiles, files[i]);
                    if (!string.IsNullOrEmpty(mFilePath))
                    {
                        string allText = File.ReadAllText(mFilePath);
                        allText = allText.Split(new string[] { "</meta>" }, StringSplitOptions.None)[1];
                        allText = "<sentences>" + allText.Replace("</mdata>","</sentences>");
                        mFileDoc.LoadXml(allText);

                        List<CoNLLSentence> sentListFromFile = GetSentencesFromCoNLLFile(files[i], false, false);
                        //sentences.AddRange(sentListFromFile);

                        XmlNodeList sentenceNodes = mFileDoc.SelectNodes("//s");
                        if (sentenceNodes.Count == sentListFromFile.Count)
                        {
                            for (int sIdx = 0; sIdx < sentenceNodes.Count; sIdx++)
                            {
                                XmlNodeList tokenNodes = sentenceNodes[sIdx].SelectNodes("m");
                                if (sentListFromFile[sIdx].Tokens.Count != tokenNodes.Count)
                                {
                                    LogWriteLine("nezhodny pocet tokenov");
                                    LogWriteLine("Conll: " + sentListFromFile[sIdx].Tokens.Count.ToString());
                                    LogWriteLine("mFile: " + tokenNodes.Count.ToString());
                                    LogWriteLine("File index: " + i.ToString());
                                    LogWriteLine("File name: " + mFilePath);
                                    LogWriteLine("Sent index: " + sIdx.ToString());
                                    LogWriteLine("");
                                    
                                    //File.WriteAllText("C:\\AllMyDocs\\FIIT\\02_Ing\\DP\\data\\sk_train\\mismatch\\" + GetFileName(mFilePath),sentenceNodes[sIdx].OuterXml);
                                    mismatchCount++;

                                }
                                else
                                {
                                    for (int tIdx = 0; tIdx < tokenNodes.Count; tIdx++)
                                    {
                                        sentListFromFile[sIdx].Tokens[tIdx].Form = tokenNodes[tIdx].SelectSingleNode("form").InnerText;
                                        sentListFromFile[sIdx].Tokens[tIdx].Lemma = tokenNodes[tIdx].SelectSingleNode("lemma").InnerText;
                                        if (!string.IsNullOrEmpty(tokenNodes[tIdx].SelectSingleNode("tag").InnerText))
                                        {
                                            sentListFromFile[sIdx].Tokens[tIdx].Cpostag = tokenNodes[tIdx].SelectSingleNode("tag").InnerText.Substring(0, 1);
                                            sentListFromFile[sIdx].Tokens[tIdx].Postag = tokenNodes[tIdx].SelectSingleNode("tag").InnerText;
                                        }
                                    }
                                }
                            }
                        }

                        string fileNameToSave = targetDir + "/" + GetFileName(files[i]);
                        SaveSentencesToFile(sentListFromFile, fileNameToSave, true, true, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("mismatchCount: " + mismatchCount.ToString());
        }

        /// <summary>
        /// spoji subory conll a treex, len vety kde nic nechyba ulozi do jedneho suboru (targetPath)
        /// </summary>
        /// <param name="targetPath">cesta k vystupnemu suboru</param>
        /// <param name="conllSourceDir">adresar so zdrojovymi conll subormi</param>
        public static void ConvertDataAndSaveToOneFile(string conllSourceDir, string targetPath, bool useLemma, bool useForm, bool usePOS)
        {
            int mismatchCount = 0;
            List<CoNLLSentence> sentences = new List<CoNLLSentence>();

            try
            {
                string korpusPath = "C:/AllMyDocs/FIIT/02_Ing/DP/data/syntakticky_korpus_sk_komplet/syntax";
                
                string[] mFiles = Directory.GetFiles(korpusPath, "*.m");
                string[] files = Directory.GetFiles(conllSourceDir);
                int limit = files.Length / 10;
                for (int i = 0; i < limit; i++)
                {
                    Console.WriteLine("Processing file "+(i+1).ToString() + " of " + limit.ToString());
                    XmlDocument mFileDoc = new XmlDocument();
                    string mFilePath = FindMFileToCoNLL(mFiles, files[i]);
                    if (!string.IsNullOrEmpty(mFilePath))
                    {
                        string allText = File.ReadAllText(mFilePath);
                        allText = allText.Split(new string[] { "</meta>" }, StringSplitOptions.None)[1];
                        allText = "<sentences>" + allText.Replace("</mdata>", "</sentences>");
                        mFileDoc.LoadXml(allText);

                        List<CoNLLSentence> sentListFromFile = GetSentencesFromCoNLLFile(files[i], false, false);
                        //sentences.AddRange(sentListFromFile);

                        XmlNodeList sentenceNodes = mFileDoc.SelectNodes("//s");
                        if (sentenceNodes.Count == sentListFromFile.Count)
                        {
                            for (int sIdx = 0; sIdx < sentenceNodes.Count; sIdx++)
                            {
                                XmlNodeList tokenNodes = sentenceNodes[sIdx].SelectNodes("m");
                                if (sentListFromFile[sIdx].Tokens.Count != tokenNodes.Count)
                                {
                                    LogWriteLine("nezhodny pocet tokenov");
                                    LogWriteLine("Conll: " + sentListFromFile[sIdx].Tokens.Count.ToString());
                                    LogWriteLine("mFile: " + tokenNodes.Count.ToString());
                                    LogWriteLine("File index: " + i.ToString());
                                    LogWriteLine("File name: " + mFilePath);
                                    LogWriteLine("Sent index: " + sIdx.ToString());
                                    LogWriteLine("");

                                    //File.WriteAllText("C:\\AllMyDocs\\FIIT\\02_Ing\\DP\\data\\sk_train\\mismatch\\" + GetFileName(mFilePath),sentenceNodes[sIdx].OuterXml);
                                    mismatchCount++;

                                }
                                else
                                {
                                    bool addsentence = true;
                                    for (int tIdx = 0; tIdx < tokenNodes.Count; tIdx++)
                                    {
                                        sentListFromFile[sIdx].Tokens[tIdx].Form = tokenNodes[tIdx].SelectSingleNode("form").InnerText;
                                        sentListFromFile[sIdx].Tokens[tIdx].Lemma = tokenNodes[tIdx].SelectSingleNode("lemma").InnerText;
                                        if (!string.IsNullOrEmpty(tokenNodes[tIdx].SelectSingleNode("tag").InnerText))
                                        {
                                            sentListFromFile[sIdx].Tokens[tIdx].Cpostag = tokenNodes[tIdx].SelectSingleNode("tag").InnerText.Substring(0, 1);
                                            sentListFromFile[sIdx].Tokens[tIdx].Postag = tokenNodes[tIdx].SelectSingleNode("tag").InnerText;
                                        }
                                        else
                                            addsentence = false;
                                    }
                                    if(addsentence)
                                        sentences.Add(sentListFromFile[sIdx]);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Done");
                }
                File.WriteAllText(targetPath.Replace(".conll",".txt"),"Sentence count: " + sentences.Count.ToString());
                SaveSentencesToFile(sentences, targetPath, useLemma, useForm, usePOS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("mismatchCount: " + mismatchCount.ToString());
        }

        private static string GetFileName(string path)
        {
            string strResult = string.Empty;
            string[] splitted = path.Split(new string[] { "\\", "/" }, StringSplitOptions.None);
            strResult = splitted[splitted.Length - 1];
            return strResult;
        }

        private static void LogWriteLine(string text)
        {
            File.AppendAllText("log.txt", text + "\r\n");
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveSentenceToFile(CoNLLSentence sentence, string fileName, bool saveRating, bool hasGPCposInfo)
        {
            try
            {
                string fileContent = string.Empty;

                foreach (CoNLLToken t in sentence.Tokens)
                {
                    fileContent += t.Id + "\t";
                    fileContent += t.Form + "\t";
                    fileContent += t.Lemma + "\t";
                    fileContent += t.Cpostag + "\t";
                    fileContent += t.Postag + "\t";
                    //fileContent += t.Feats + "\t";
                    //if (t.IsCorrectHead)
                    //    fileContent += t.Head + "\t";
                    //else
                    //    fileContent += t.CorrectHead + "\t";
                    //if (t.IsCorrectDeprel)
                    //    fileContent += t.Deprel;
                    //else
                    //    fileContent += t.CorrectDeprel;
                    fileContent += t.Feats + "\t" + t.Head + "\t" + t.Deprel;
                    if (saveRating)
                    {
                        fileContent += "\t" + t.IsCorrectHead + "\t" + t.IsCorrectDeprel;
                        fileContent += "\t" + t.CorrectHead + "\t" + t.CorrectDeprel;
                    }

                    if (hasGPCposInfo)
                        fileContent += "\t" + t.ParentCpos + "\t" + t.GrandParentCpos;
                    fileContent += "\r\n";
                }
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveSentenceToFile(CoNLLSentence sentence, string fileName, bool saveRating, bool hasGPCposInfo, bool hasGPPosInfo)
        {
            try
            {
                string fileContent = string.Empty;

                foreach (CoNLLToken t in sentence.Tokens)
                {
                    fileContent += t.Id + "\t";
                    fileContent += t.Form + "\t";
                    fileContent += t.Lemma + "\t";
                    fileContent += t.Cpostag + "\t";
                    fileContent += t.Postag + "\t";
                    fileContent += t.Feats + "\t" + t.Head + "\t" + t.Deprel;
                    if (saveRating)
                    {
                        fileContent += "\t" + t.IsCorrectHead + "\t" + t.IsCorrectDeprel;
                        fileContent += "\t" + t.CorrectHead + "\t" + t.CorrectDeprel;
                    }

                    if (hasGPCposInfo)
                        fileContent += "\t" + t.ParentCpos + "\t" + t.GrandParentCpos;
                    if (hasGPPosInfo)
                        fileContent += "\t" + t.ParentPos + "\t" + t.GrandParentPos;
                    fileContent += "\r\n";
                }
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SaveSentenceToFile(CoNLLSentence sentence, string fileName, bool saveRating, bool hasGPCposInfo, bool hasGPPosInfo, bool hasChildrenPosInfo)
        {
            try
            {
                string fileContent = string.Empty;

                foreach (CoNLLToken t in sentence.Tokens)
                {
                    fileContent += t.Id + "\t";
                    fileContent += t.Form + "\t";
                    fileContent += t.Lemma + "\t";
                    fileContent += t.Cpostag + "\t";
                    fileContent += t.Postag + "\t";
                    fileContent += t.Feats + "\t" + t.Head + "\t" + t.Deprel;
                    if (saveRating)
                    {
                        fileContent += "\t" + t.IsCorrectHead + "\t" + t.IsCorrectDeprel;
                        fileContent += "\t" + t.CorrectHead + "\t" + t.CorrectDeprel;
                    }

                    if (hasGPCposInfo)
                        fileContent += "\t" + t.ParentCpos + "\t" + t.GrandParentCpos;
                    if (hasGPPosInfo)
                        fileContent += "\t" + t.ParentPos + "\t" + t.GrandParentPos;

                    if (hasChildrenPosInfo)
                    {
                        string childPos = string.Empty;
                        for (int i = 0; i < t.ChildrenPos.Count; i++)
                        {
                            childPos += t.ChildrenPos[i];
                            if (i < t.ChildrenPos.Count - 1)
                                childPos += "|";
                        }
                        if (string.IsNullOrEmpty(childPos))
                            childPos = "none";
                        fileContent += "\t" + childPos;
                    }
                    fileContent += "\r\n";

                }
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveTokenToFile(CoNLLToken token, string fileName, bool saveRating, bool saveFeatsSeparately)
        {
            try
            {
                string fileContent = string.Empty;

                    fileContent += token.Id + "\t";
                    fileContent += token.Form + "\t";
                    fileContent += token.Lemma + "\t";
                    fileContent += token.Cpostag + "\t";
                    fileContent += token.Postag + "\t";
                    fileContent += token.Feats + "\t";

                    if (saveFeatsSeparately)
                    {
                        fileContent += token.featsToString().Replace(",","\t");
                    }

                    fileContent += token.Head + "\t" + token.Deprel;
                    if (saveRating)
                        fileContent += "\t" + token.IsCorrectHead + "\t" + token.IsCorrectDeprel + "\t" + token.CorrectHead + "\t" + token.CorrectDeprel + "\t" + token.ParentCorrectDeprel;
                    fileContent += "\r\n";
                
                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveTokenToFile(CoNLLToken token, string fileName, bool saveRating, bool saveFeatsSeparately, bool hasGPDerelInfo)
        {
            try
            {
                string fileContent = string.Empty;

                fileContent += token.Id + "\t";
                fileContent += token.Form + "\t";
                fileContent += token.Lemma + "\t";
                fileContent += token.Cpostag + "\t";
                fileContent += token.Postag + "\t";
                fileContent += token.Feats + "\t";

                if (saveFeatsSeparately)
                {
                    fileContent += token.featsToString().Replace(",", "\t");
                }

                fileContent += token.Head + "\t" + token.Deprel;
                if (saveRating)
                    fileContent += "\t" + token.IsCorrectHead + "\t" + token.IsCorrectDeprel + "\t" + token.CorrectHead + "\t" + token.CorrectDeprel + "\t" + token.ParentCorrectDeprel;
                if (hasGPDerelInfo)
                    fileContent += "\t" + token.GrandParentCorrectDeprel;
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveTokenToFile(CoNLLToken token, string fileName, bool saveRating, bool saveFeatsSeparately, bool hasGPDerelInfo, bool hasGPPosInfo)
        {
            try
            {
                string fileContent = string.Empty;

                fileContent += token.Id + "\t";
                fileContent += token.Form + "\t";
                fileContent += token.Lemma + "\t";
                fileContent += token.Cpostag + "\t";
                fileContent += token.Postag + "\t";
                fileContent += token.Feats + "\t";

                if (saveFeatsSeparately)
                {
                    fileContent += token.featsToString().Replace(",", "\t");
                }

                fileContent += token.Head + "\t" + token.Deprel;
                if (saveRating)
                    fileContent += "\t" + token.IsCorrectHead + "\t" + token.IsCorrectDeprel + "\t" + token.CorrectHead + "\t" + token.CorrectDeprel + "\t" + token.ParentCorrectDeprel;
                if (hasGPDerelInfo)
                    fileContent += "\t" + token.GrandParentCorrectDeprel;
                if (hasGPPosInfo)
                    fileContent += "\t" + token.ParentPos + "\t" + token.GrandParentPos;
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveTokenToFile(CoNLLToken token, string fileName, bool saveRating, bool saveFeatsSeparately, bool hasGPCPosInfo, bool hasGPPosInfo, bool hasChildrenPosInfo)
        {
            try
            {
                string fileContent = string.Empty;

                fileContent += token.Id + "\t";
                fileContent += token.Form + "\t";
                fileContent += token.Lemma + "\t";
                fileContent += token.Cpostag + "\t";
                fileContent += token.Postag + "\t";
                fileContent += token.Feats + "\t";

                if (token.IsCorrectHead)
                    token.CorrectHead = token.Head;
                if (token.IsCorrectDeprel)
                    token.CorrectDeprel = token.Deprel;

                if (saveFeatsSeparately)
                {
                    fileContent += token.featsToString().Replace(",", "\t");
                }

                fileContent += token.Head + "\t" + token.Deprel;
                if (saveRating)
                    fileContent += "\t" + token.IsCorrectHead + "\t" + token.IsCorrectDeprel + "\t" + token.CorrectHead + "\t" + token.CorrectDeprel;
                if (hasGPCPosInfo)
                    fileContent += "\t" + token.ParentCpos + "\t" + token.GrandParentCpos;
                if (hasGPPosInfo)
                    fileContent += "\t" + token.ParentPos + "\t" + token.GrandParentPos;
                if (hasChildrenPosInfo)
                {
                    string childPos = string.Empty;
                    for (int i = 0; i < token.ChildrenPos.Count; i++)
                    {
                        childPos += token.ChildrenPos[i];
                        if (i < token.ChildrenPos.Count - 1)
                            childPos += "|";
                    }
                    if (string.IsNullOrEmpty(childPos))
                        childPos = "none";
                    fileContent += "\t" + childPos;
                }
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //save rating - uklada sa aj info o tom, ci bol spravne urceny atribut head alebo deprel
        private static void SaveTokenToCsvFile(CoNLLToken token, string fileName, bool saveRating, bool saveFeatsSeparately)
        {
            try
            {
                string fileContent = string.Empty;

                fileContent += token.Id + ",";
                
                //form
                if(token.Form.Contains(","))
                    fileContent += token.Form.Replace(",", "\\ciar\\") + ",";
                else if (token.Form.Contains("\""))
                    fileContent += token.Form.Replace("\"", "\\uvodz\\") + ",";
                else 
                    fileContent += token.Form + ",";

                //lemma
                if (token.Lemma.Contains(","))
                    fileContent += token.Lemma.Replace(",", "\\ciar\\") + ",";
                else if (token.Form.Contains("\""))
                    fileContent += token.Lemma.Replace("\"", "\\uvodz\\") + ",";
                //else if(token.Form.Contains("“"))
                //    fileContent += token.Lemma.Replace("“", "\\uvodz2\\") + ",";
                else
                    fileContent += token.Lemma + ",";

                fileContent += token.Cpostag + ",";
                fileContent += token.Postag + ",";
                fileContent += token.Feats + ",";

                if(saveFeatsSeparately)
                {
                    fileContent+= token.featsToString();
                }

                fileContent += token.Head + "," + token.Deprel;
                if (saveRating)
                    fileContent += "," + token.IsCorrectHead + "," + token.IsCorrectDeprel + "," + token.CorrectHead + "," + token.CorrectDeprel;
                fileContent += "\r\n";

                File.AppendAllText(fileName, fileContent);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        

        private static void SaveSentencesToFile(List<CoNLLSentence> senteces, string fileName, bool useLemma, bool useForm, bool usePOS)
        {
            try
            {
                string fileContent = string.Empty;
                foreach (CoNLLSentence s in senteces)
                {
                    fileContent = string.Empty;
                    foreach (CoNLLToken t in s.Tokens)
                    {
                        fileContent += t.Id + "\t";
                        if (useForm)
                            fileContent +=t.Form + "\t";
                        else
                            fileContent += "_\t";

                        if (useLemma)
                            fileContent += t.Lemma + "\t";
                        else
                            fileContent += "_\t";

                        fileContent += t.Cpostag + "\t";

                        if (usePOS)
                            fileContent += t.Postag + "\t";
                        else
                            fileContent += "_\t";

                        fileContent += t.Feats + "\t" + t.Head + "\t" + t.Deprel;
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

        private static List<CoNLLSentence> GetSentencesFromCoNLLFile(string CoNLLPath, bool hasResult, bool hasParentDeprel)
        {
            List<CoNLLSentence> resultList = new List<CoNLLSentence>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new CoNLLSentence());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line) && sentenceIdx < resultList.Count)
                    {
                        //Console.WriteLine(sentenceIdx.ToString() + " " + resultList.Count.ToString());
                        resultList[sentenceIdx].Tokens.Add(new CoNLLToken(line, hasResult, hasParentDeprel));
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        private static List<CoNLLSentence> GetSentencesFromCoNLLFile(string CoNLLPath, bool hasResult, bool hasParentDeprel, bool hasGPDeprel)
        {
            List<CoNLLSentence> resultList = new List<CoNLLSentence>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new CoNLLSentence());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line))
                        resultList[sentenceIdx].Tokens.Add(new CoNLLToken(line, hasResult, hasParentDeprel, hasGPDeprel));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        private static List<CoNLLSentence> GetSentencesFromCoNLLFile(string CoNLLPath, bool hasResult, bool hasGPDeprel, bool hasGPCPos, bool hasGPPos, bool hasChildPos)
        {
            List<CoNLLSentence> resultList = new List<CoNLLSentence>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                int sentenceIdx = -1;
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    if (line.StartsWith("1\t"))
                    {
                        resultList.Add(new CoNLLSentence());
                        sentenceIdx++;
                    }
                    if (!string.IsNullOrEmpty(line))
                        resultList[sentenceIdx].Tokens.Add(new CoNLLToken(line, hasResult, hasGPDeprel, hasGPCPos, hasGPPos, hasChildPos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        /// <summary>
        /// result - ide o vysledny subor, kt uz obsahuje info o true/false head aj deprel
        /// </summary>
        /// <param name="CoNLLPath"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<CoNLLToken> GetTokensFromCoNLLFile(string CoNLLPath, bool result, bool hasParentDeprel)
        {
            Console.WriteLine("reading file "+ CoNLLPath);
            List<CoNLLToken> resultList = new List<CoNLLToken>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if(lines[j].Trim().Length>0)
                        resultList.Add(new CoNLLToken(lines[j], result, hasParentDeprel));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        /// <summary>
        /// result - ide o vysledny subor, kt uz obsahuje info o true/false head aj deprel
        /// </summary>
        /// <param name="CoNLLPath"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<CoNLLToken> GetTokensFromCoNLLFile(string CoNLLPath, bool result, bool hasParentDeprel, bool hasParentPos, bool hasChildPos)
        {
            Console.WriteLine("reading file " + CoNLLPath);
            List<CoNLLToken> resultList = new List<CoNLLToken>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].Trim().Length > 0)
                        resultList.Add(new CoNLLToken(lines[j], result, hasParentDeprel, hasParentPos, hasChildPos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        private static List<CoNLLToken> GetTokensFromCoNLLFile(string CoNLLPath, bool result, bool hasParentDeprel, bool hasParentCpos ,bool hasParentPos, bool hasChildPos)
        {
            Console.WriteLine("reading file " + CoNLLPath);
            List<CoNLLToken> resultList = new List<CoNLLToken>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].Trim().Length > 0)
                        resultList.Add(new CoNLLToken(lines[j], result, hasParentDeprel,hasParentCpos, hasParentPos, hasChildPos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        private static List<CoNLLToken> GetTokensFromCoNLLFile(string CoNLLPath, bool result, bool hasParentDeprel, bool hasParentPos)
        {
            Console.WriteLine("reading file " + CoNLLPath);
            List<CoNLLToken> resultList = new List<CoNLLToken>();
            try
            {
                string[] lines = File.ReadAllLines(CoNLLPath);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j].Trim().Length > 0)
                        resultList.Add(new CoNLLToken(lines[j], result, hasParentDeprel, hasParentDeprel, hasParentPos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return resultList;
        }

        
        
        private static string FindMFileToCoNLL(string[] mFiles, string conllFile)
        {
            string strResult = string.Empty;
            try
            {
                string[] splitted = conllFile.Split(new string[] { "/", "\\" }, StringSplitOptions.None);
                string fileName = splitted[splitted.Length - 1].Replace(".conll", ".m");

                for (int i = 0; i < mFiles.Length; i++)
                {
                    if (mFiles[i].Contains(fileName))
                    {
                        strResult = mFiles[i];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return strResult;
        }

        public static void CheckMissing()
        {
            string dirPath = "C:/AllMyDocs/FIIT/02_Ing/DP/data/syntakticky_korpus_sk_komplet/syntax";
            //string dirPath = "C:\\AllMyDocs\\FIIT\\02_Ing\\DP\\data\\sk_train\\all";
            /*if (!Directory.Exists(dirPath + "\\ok"))
                  Directory.CreateDirectory(dirPath + "\\ok");
              if (!Directory.Exists(dirPath+"\\missing"))
                  Directory.CreateDirectory(dirPath + "\\missing");
              */

            string[] files = Directory.GetFiles(dirPath);
            int missingLemmasCount = 0;
            int missingTagsCount = 0;
            int missingFormsCount = 0;
            int missingAfunCount = 0;

            int sentencesCount = 0;
            int tokensCount = 0;
            int invalidSentences = 0;

            DateTime start = DateTime.Now;

            for (int i = 0; i < files.Length; i++)
            {
                /* if (files[0].EndsWith(".a"))
                 {
                     string[] filePathSplitted = files[i].Split(new string[] { "/", "\\" }, StringSplitOptions.None);
                     string filename = filePathSplitted[filePathSplitted.Length - 1];

                     string filenameWithoutExt = filename.Substring(0, filename.Length - 2);

                     string aContent = File.ReadAllText(dirPath + "/" + filenameWithoutExt + ".a");
                     string mContent = File.ReadAllText(dirPath + "/" + filenameWithoutExt + ".m");
                     string wContent = File.ReadAllText(dirPath + "/" + filenameWithoutExt + ".w");
                    
                     string copyDirection = dirPath;

                     if (aContent.Contains("???"))
                         copyDirection += "/missing/";
                     else
                         copyDirection += "/ok/";

                     File.WriteAllText(copyDirection + filenameWithoutExt + ".a", aContent);
                     File.WriteAllText(copyDirection + filenameWithoutExt + ".m", mContent);
                     File.WriteAllText(copyDirection + filenameWithoutExt + ".w", wContent);

                 }*/
                if (files[i].EndsWith(".m"))
                {
                    string mContent = File.ReadAllText(files[i]);
                    if (mContent.Contains("<lemma></lemma>") || mContent.Contains("<lemma/>"))
                    {
                        int count = 0;
                        count = mContent.Split(new string[] { "<lemma></lemma>", "<lemma/>" }, StringSplitOptions.None).Length - 1;
                        missingLemmasCount += count;
                    }
                    if (mContent.Contains("<tag></tag>") || mContent.Contains("<tag/>"))
                    {
                        int count = 0;
                        count = mContent.Split(new string[] { "<tag></tag>", "<tag/>" }, StringSplitOptions.None).Length - 1;
                        missingTagsCount += count;
                    }
                    if (mContent.Contains("<form></form>") || mContent.Contains("<form/>"))
                    {
                        int count = 0;
                        count = mContent.Split(new string[] { "<form></form>", "<form/>" }, StringSplitOptions.None).Length - 1;
                        missingFormsCount += count;
                    }
                }
                else if (files[i].EndsWith(".a"))
                {
                    string aContent = File.ReadAllText(files[i]);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(aContent);
                    XmlNode trees = doc.DocumentElement.ChildNodes[1];
                    foreach (XmlNode n in trees.ChildNodes)
                    {
                        if (n.InnerXml.Contains("<afun>???</afun>") || n.InnerXml.Contains("<afun></afun>") || n.InnerXml.Contains("<afun/>"))
                            invalidSentences++;
                    }

                    /* if (aContent.Contains("<afun>???</afun>") || aContent.Contains("<afun></afun>"))
                     {
                         int count = 0;
                         count = aContent.Split(new string[] { "<afun>???</afun>", "<afun></afun>" }, StringSplitOptions.None).Length - 1;
                         missingAfunCount += count;
                     }
                     int vetyCount = aContent.Split(new string[] { "<ord>0</ord>" }, StringSplitOptions.None).Length - 1;
                     sentencesCount += vetyCount;   */
                }
                else if (files[i].EndsWith(".w"))
                {
                    string wContent = File.ReadAllText(files[i]);

                    int tokenyCount = wContent.Split(new string[] { "<token>" }, StringSplitOptions.None).Length - 1;
                    tokensCount += tokenyCount;
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Duration: " + span.TotalMinutes.ToString() + " min");

            Console.WriteLine("pocet viet: " + sentencesCount.ToString());
            Console.WriteLine("pocet tokenov: " + tokensCount.ToString());
            Console.WriteLine();

            Console.WriteLine("chybajuce lemy: " + missingLemmasCount.ToString());
            Console.WriteLine("chybajuce tagy: " + missingTagsCount.ToString());
            Console.WriteLine("chybajuce formy: " + missingFormsCount.ToString());

            Console.WriteLine("\r\nchybajuce afun: " + missingAfunCount.ToString());
            Console.WriteLine("vety s chyb afun: " + invalidSentences.ToString());
        }

        private static string GetDirectory(string filepath)
        {
            string strResult = string.Empty;
            try
            {
                string[] splittedPath = filepath.Split(new string[] { "\\", "/" }, StringSplitOptions.None);
                strResult = splittedPath[0];
                for (int i = 1; i < (splittedPath.Length - 1); i++)
                {
                    strResult += "/" + splittedPath[i];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return strResult;
        }

        /// <summary>
        /// zistenie vsetkych moznych poli v poli Feats a ich vypis
        /// </summary>
        /// <param name="filePath">subor s vetami vo formate CoNLL</param>
        /// <param name="pathToSave">subor na ulozenie vysledneho zoznamu poli</param>
        private static void FindAllFeatsParts(string filePath,string pathToSave)
        {
            File.WriteAllText(pathToSave, string.Empty);

            List<string> featsList = new List<string>();
            List<CoNLLToken> tokens = GetTokensFromCoNLLFile(filePath,false, false);
            foreach (CoNLLToken t in tokens)
            {
                List<string> tokenFeatsList = t.ParseFeats();
                foreach (string str in tokenFeatsList)
                {
                    string key = str.Split('=')[0];
                    if (!StrListContainsString(featsList,key))
                        featsList.Add(key);
                }
            }
            Console.WriteLine("feats count: " + featsList.Count);
            foreach (string str in featsList)
            {
                Console.WriteLine(str);
                File.AppendAllText(pathToSave, str + "\r\n");
            }
        }

        public static bool StrListContainsString(List<string> list, string str)
        {
            foreach (string s in list)
            {
                if (str.Equals(s))
                    return true;
            }
            return false;
        }

        private static void CompleteFeatsParts(string filePath, string fileToSave)
        {
            bool isCSV = false;
            if (fileToSave.EndsWith(".csv"))
            {
                isCSV = true;
            }            

            List<CoNLLToken> tokens = GetTokensFromCoNLLFile(filePath,true, false);
            foreach (CoNLLToken t in tokens)
            {
                t.FillFeats();

                if(isCSV)
                    SaveTokenToCsvFile(t, fileToSave, true, true);
                else
                    SaveTokenToFile(t, fileToSave, true, true);
            }

        }

        public static void GetEnums(string sourceFile)
        {
            try
            {

                HashSet<string> f_pos = new HashSet<string>();
                HashSet<string> f_negativeness = new HashSet<string>();
                HashSet<string> f_number = new HashSet<string>();
                HashSet<string> f_person = new HashSet<string>();
                HashSet<string> f_verbform = new HashSet<string>();
                HashSet<string> f_mood = new HashSet<string>();
                HashSet<string> f_tense = new HashSet<string>();
                HashSet<string> f_aspect = new HashSet<string>();
                HashSet<string> f_gender = new HashSet<string>();
                HashSet<string> f_case = new HashSet<string>();
                HashSet<string> f_prontype = new HashSet<string>();
                HashSet<string> f_reflex = new HashSet<string>();
                HashSet<string> f_animateness = new HashSet<string>();
                HashSet<string> f_degree = new HashSet<string>();
                HashSet<string> f_subpos = new HashSet<string>();
                HashSet<string> f_numform = new HashSet<string>();
                HashSet<string> f_abbr = new HashSet<string>();
                HashSet<string> f_hyph = new HashSet<string>();
                HashSet<string> f_foreign = new HashSet<string>();
                HashSet<string> f_voice = new HashSet<string>();
                HashSet<string> f_typo = new HashSet<string>();


                List<CoNLLToken> tokens = GetTokensFromCoNLLFile(sourceFile, false, false);
                foreach (CoNLLToken t in tokens)
                {
                    t.FillFeats();

                    f_pos.Add(t.f_pos);
                    f_negativeness.Add(t.f_negativeness);
                    f_number.Add(t.f_number);
                    f_person.Add(t.f_person);
                    f_verbform.Add(t.f_verbform);
                    f_mood.Add(t.f_mood);
                    f_tense.Add(t.f_tense);
                    f_aspect.Add(t.f_aspect);
                    f_gender.Add(t.f_gender);
                    f_case.Add(t.f_case);
                    f_prontype.Add(t.f_prontype);
                    f_reflex.Add(t.f_reflex);
                    f_animateness.Add(t.f_animateness);
                    f_degree.Add(t.f_degree);
                    f_subpos.Add(t.f_subpos);
                    f_numform.Add(t.f_numform);
                    f_abbr.Add(t.f_abbr);
                    f_hyph.Add(t.f_hyph);
                    f_foreign.Add(t.f_foreign);
                    f_voice.Add(t.f_voice);
                    f_typo.Add(t.f_typo);
                }

                foreach (string s in f_pos)
                    File.AppendAllText("f_poc.txt", s + "\r\n");
                foreach (string s in f_negativeness)
                    File.AppendAllText("f_negativeness.txt", s + "\r\n");
                foreach (string s in f_number)
                    File.AppendAllText("f_number.txt", s + "\r\n");
                foreach (string s in f_person)
                    File.AppendAllText("f_person.txt", s + "\r\n");
                foreach (string s in f_verbform)
                    File.AppendAllText("f_verbform.txt", s + "\r\n");
                foreach (string s in f_mood)
                    File.AppendAllText("f_mood.txt", s + "\r\n");
                foreach (string s in f_tense)
                    File.AppendAllText("f_tense.txt", s + "\r\n");
                foreach (string s in f_aspect)
                    File.AppendAllText("f_aspect.txt", s + "\r\n");
                foreach (string s in f_gender)
                    File.AppendAllText("f_gender.txt", s + "\r\n");
                foreach (string s in f_case)
                    File.AppendAllText("f_case.txt", s + "\r\n");
                foreach (string s in f_prontype)
                    File.AppendAllText("f_prontype.txt", s + "\r\n");
                foreach (string s in f_reflex)
                    File.AppendAllText("f_reflex.txt", s + "\r\n");
                foreach (string s in f_animateness)
                    File.AppendAllText("f_animateness.txt", s + "\r\n");
                foreach (string s in f_degree)
                    File.AppendAllText("f_degree.txt", s + "\r\n");
                foreach (string s in f_subpos)
                    File.AppendAllText("f_subpos.txt", s + "\r\n");
                foreach (string s in f_numform)
                    File.AppendAllText("f_numform.txt", s + "\r\n");
                foreach (string s in f_abbr)
                    File.AppendAllText("f_abbr.txt", s + "\r\n");
                foreach (string s in f_hyph)
                    File.AppendAllText("f_hyph.txt", s + "\r\n");
                foreach (string s in f_foreign)
                    File.AppendAllText("f_foreign.txt", s + "\r\n");
                foreach (string s in f_voice)
                    File.AppendAllText("f_voice.txt", s + "\r\n");
                foreach (string s in f_typo)
                    File.AppendAllText("f_typo.txt", s + "\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void GetDeprelTypes(string sourceFile)
        {
            try
            {
                string[] existigTypes = File.ReadAllLines("deprelTypes.txt");

                HashSet<string> result = new HashSet<string>();
                for (int i = 0; i < existigTypes.Length; i++)
                    result.Add(existigTypes[i].Trim().Replace(",",""));

                List<CoNLLToken> tokens = GetTokensFromCoNLLFile(sourceFile, false, false);
                foreach (CoNLLToken t in tokens)
                {
                    result.Add(t.Deprel);
                }

                File.WriteAllText("deprelTypes.txt",string.Empty);
                foreach (string s in result)
                    File.AppendAllText("deprelTypes.txt", s + ",\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// vyberie zo suboru sourceFile len tie vety, ktore nie su vo filterFile a ulozi ich do resultFile
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="filterFile"></param>
        /// <param name="resultFile"></param>
        public static void FilterSentences(string sourceFile, string filterFile, string resultFile, string wasteFile, bool hasParentDeprel)
        {
            try
            {
                List<CoNLLSentence> allSentences = GetSentencesFromCoNLLFile(sourceFile, true, hasParentDeprel);
                List<CoNLLSentence> filter = GetSentencesFromCoNLLFile(filterFile, false, false);
                List<CoNLLSentence> resultList = new List<CoNLLSentence>();
                List<CoNLLSentence> wasteList = new List<CoNLLSentence>();

                foreach (CoNLLSentence sent in allSentences)
                {
                    bool isInFilter = false;
                    foreach (CoNLLSentence sentFilter in filter)
                    {
                        if (CoNLLSentence.AreEqual(sent, sentFilter))
                        {
                            isInFilter = true;
                            break;
                        }
                    }

                    if (isInFilter)
                        wasteList.Add(sent);
                    else
                        resultList.Add(sent);
                }


                Console.WriteLine("source sentence count: " + allSentences.Count.ToString());
                Console.WriteLine("result sentence count: " + resultList.Count.ToString());
                Console.WriteLine("filter sentence count: " + filter.Count.ToString());
                Console.WriteLine("waste sentence count: " + wasteList.Count.ToString());

                foreach (CoNLLSentence s in resultList)
                    SaveSentenceToFile(s, resultFile, true, false);

                foreach (CoNLLSentence s in wasteList)
                    SaveSentenceToFile(s, wasteFile, true, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// vyberie zo suboru sourceFile len tie tokeny, ktore nie su vo filterFile a ulozi ich do resultFile
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="filterFile"></param>
        /// <param name="resultFile"></param>
        public static void FilterTokens(string sourceFile, string filterFile, string resultFile, string wasteFile, bool hasParentDeprel, bool hasParentPos, bool hasChildPos)
        {
            try
            {
                List<CoNLLToken> allTokens = GetTokensFromCoNLLFile(sourceFile, true, hasParentDeprel, hasParentPos, hasChildPos);
                List<CoNLLToken> filter = GetTokensFromCoNLLFile(filterFile, false, false);
                List<CoNLLToken> resultList = new List<CoNLLToken>();
                List<CoNLLToken> wasteList = new List<CoNLLToken>();

                Console.WriteLine("filtering started");

                Parallel.For(0, allTokens.Count, i =>
                {
                    CoNLLToken tok = allTokens[i];
                    tok.IsInFilter = false;

                    foreach (CoNLLToken tokFilter in filter)
                    {
                        if (CoNLLToken.AreEqual(tok, tokFilter))
                        {
                            tok.IsInFilter = true;
                            break;
                        }
                    }

                    

                });
                
                Console.WriteLine("Saving result...");

                foreach(CoNLLToken tok in allTokens)
                {
                    if (!tok.IsInFilter)
                        resultList.Add(tok);
                    else
                        wasteList.Add(tok);
                }

                foreach (CoNLLToken t in resultList)
                    SaveTokenToFile(t, resultFile, true, false, hasParentDeprel, hasParentPos, hasChildPos);
                foreach (CoNLLToken t in wasteList)
                    SaveTokenToFile(t, wasteFile, true, false, hasParentDeprel, hasParentPos, hasChildPos);

                Console.WriteLine("source tokens count: " + resultList.Count.ToString());
                Console.WriteLine("result tokens count: " + resultList.Count.ToString());
                Console.WriteLine("filter tokens count: " + filter.Count.ToString());
                Console.WriteLine("waste tokens count: " + wasteList.Count.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void FindDuplicates(string file1)
        {
            try
            {
                List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(file1, false, false);
                List<CoNLLSentence> duplicates = new List<CoNLLSentence>();

                List<int> usedIds = new List<int>();

                for (int i = 0; i < sentences.Count; i++)
                {
                    usedIds.Add(i);
                    CoNLLSentence s1 = sentences[i];
                    for (int j = 0; j < sentences.Count; j++)
                    {
                        CoNLLSentence s2 = sentences[j];
                        if (!usedIds.Contains(i) && !usedIds.Contains(j) && i == j && CoNLLSentence.AreEqual(s1, s2))
                        {
                            usedIds.Add(j);
                            duplicates.Add(s2);
                        }
                    }
                }
                Console.WriteLine("duplicates count: " + duplicates.Count.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// trojicu suborov .a, .w, .m prekonvertuje na vety v .conll formate
        /// </summary>
        /// <param name="sourcedir">adresar vstupneho suboru (MUSI KONCIT / alebo \)</param>
        /// <param name="targetdir">adresar vystupneho suboru (MUSI KONCIT / alebo \)</param>
        /// <param name="fileName">nazov suborov, bez pripony</param>
        /// <returns>pocet viet v subore</returns>
        public static int TreexToConll(string sourcedir, string targetdir, string fileName)
        {
            List<CoNLLSentence> result = new List<CoNLLSentence>();

            try
            {
                XmlDocument mDoc = new XmlDocument();
                XmlDocument aDoc = new XmlDocument();
                XmlDocument wDoc = new XmlDocument();

                string mContent = File.ReadAllText(sourcedir + fileName + ".m").Split(new[] { "</meta>", "</mdata>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string aContent = File.ReadAllText(sourcedir + fileName + ".a").Split(new[] { "</head>", "</adata>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string wContent = File.ReadAllText(sourcedir + fileName + ".w").Split(new[] { "<docmeta/>", "</docmeta>", "</doc>" }, StringSplitOptions.RemoveEmptyEntries)[1];

                mDoc.LoadXml("<m>" + mContent + "</m>");
                aDoc.LoadXml("<a>" + aContent + "</a>");
                wDoc.LoadXml("<ww>" + wContent + "</ww>");

                XmlNodeList sentenceNodeList = mDoc.SelectNodes("//s");

                
                foreach (XmlNode sentenceNode in sentenceNodeList)
                {
                    CoNLLSentence s = new CoNLLSentence();
                    s.Tokens = GetTokensFromXml(sentenceNode, aDoc);
                    s.Tokens = s.Tokens.OrderBy(x => Convert.ToInt32(x.Id)).ToList();
                    result.Add(s);
                }

                
                foreach (CoNLLSentence s in result)
                {
                    SaveSentenceToFile(s, targetdir + fileName + ".conll", false, false);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("File " + fileName + "\r\nException : \r\n" + ex.ToString());
            }
            return result.Count;
        }

        public static List<CoNLLToken> GetTokensFromXml(XmlNode xml, XmlDocument aFile)
        {
            List<CoNLLToken> tokens = new List<CoNLLToken>();

            foreach (XmlNode n in xml.ChildNodes)
            {
                CoNLLToken t = new CoNLLToken();
                //string strId = tokenNode.Attributes["id"].Value;

                //XmlNode n = mFile.SelectSingleNode("//m[@id=\"" + strId.Replace("w-", "m-") + "\"]");

                t.Form = n.SelectSingleNode("form").InnerText;
                t.Lemma = n.SelectSingleNode("lemma").InnerText.Replace("_", "-");
                t.Cpostag = string.IsNullOrEmpty(n.SelectSingleNode("tag").InnerText) ? "_" : n.SelectSingleNode("tag").InnerText[0].ToString();
                t.Postag = string.IsNullOrEmpty(n.SelectSingleNode("tag").InnerText) ? "_" : n.SelectSingleNode("tag").InnerText;

                t.Feats = "_";
                t.SetFeats();

                string strTokenId = n.SelectSingleNode("w.rf").InnerText;

                //token v .a subore
                XmlNode aNode = aFile.SelectSingleNode("//*[@id=\"" + strTokenId.Replace("w#w", "a") + "\"]");

                XmlNode parentNode = null;
                if (aNode != null)
                {
                    if (aNode.Name.Equals("LM"))
                        parentNode = aNode.ParentNode.ParentNode;
                    else
                        parentNode = aNode.ParentNode;

                    t.Head = parentNode.SelectSingleNode("ord").InnerText;
                    t.Deprel = aNode.SelectSingleNode("afun").InnerText;

                    if (aNode.SelectSingleNode("ord") == null)
                    {
                        Console.Write("null");
                    }
                    t.Id = aNode.SelectSingleNode("ord").InnerText;
                    tokens.Add(t);
                }
                                
            }

            return tokens;
        }

        /// <summary>
        /// spojenie dvoch conll suborov, v kazdom chybaju nejake udaje, ale nazov, pocty viet a tokenov sa zhoduju.
        /// vrati pocet viet
        /// </summary>
        public static int MergeIdenticFiles(string fileCorpus, string fileHamledt, string resultDir)
        {
            List<CoNLLSentence> sentencesCorpus = GetSentencesFromCoNLLFile(fileCorpus, false, false);

            try
            {
                if (!string.IsNullOrEmpty(fileHamledt) && File.Exists(fileHamledt))
                {
                    List<CoNLLSentence> sentencesHamledt = GetSentencesFromCoNLLFile(fileHamledt, false, false);
                    List<CoNLLSentence> resultSentences = new List<CoNLLSentence>();

                    if (sentencesCorpus.Count == sentencesHamledt.Count)
                    {
                        for (int i = 0; i < sentencesCorpus.Count; i++)
                        {
                            CoNLLSentence sC = sentencesCorpus[i];
                            CoNLLSentence sH = sentencesHamledt[i];
                            bool addSentence = false;

                            if (sC.Tokens.Count == sH.Tokens.Count)
                            {
                                for (int j = 0; j < sC.Tokens.Count; j++)
                                {
                                    CoNLLToken tC = sC.Tokens[j];
                                    CoNLLToken tH = sH.Tokens[j];

                                    if (tH.Deprel.Contains(tC.Deprel) && !tH.Feats.Equals("_"))
                                    {
                                        tC.Feats = tH.Feats;
                                        tC.Head = tH.Head;
                                        tC.Deprel = tH.Deprel;
                                        addSentence = true;
                                    }
                                    else
                                    {
                                        addSentence = false;
                                        break;
                                    }
                                }
                                if (addSentence)
                                    resultSentences.Add(sC);
                            }
                            else
                            {
                                Console.WriteLine("nezhodny pocet tokenov");
                            }
                        }
                    }

                    Console.WriteLine("spolu " + resultSentences.Count.ToString() + " viet...");
                    foreach (CoNLLSentence s in resultSentences)
                        SaveSentenceToFile(s, resultDir + "/" + GetFileName(fileCorpus), false, false);
                    return resultSentences.Count;
                }
                else 
                {
                    Console.WriteLine("no hamledt file " + fileHamledt);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }

        /// <summary>
        /// odstrani vety, kt nemav=ju v sebe vsetky informacie
        /// </summary>
        /// <param name="sourceFile">povodny .conll subor</param>
        /// <param name="targetFile">subor, kam sa ulozia vyfiltrovane dobre vety</param>
        /// <param name="wasteFile">subo kam sa ulozia neuplne vety</param>
        public static void RemoveAllNoncompleteSentences(string sourceFile, string targetFile, string wasteFile)
        {
            try
            {
                List<CoNLLSentence> sentences = GetSentencesFromCoNLLFile(sourceFile, false, false);
                List<CoNLLSentence> resultList = new List<CoNLLSentence>();
                List<CoNLLSentence> wasteList = new List<CoNLLSentence>();

                for (int i = 0; i < sentences.Count; i++)
                {
                    CoNLLSentence s = sentences[i];
                    bool complete = false;
                    foreach (CoNLLToken t in s.Tokens)
                    {



                        if (t.Form.Equals("_") || string.IsNullOrEmpty(t.Form)
                            || t.Lemma.Equals("_") || string.IsNullOrEmpty(t.Lemma)
                            || t.Cpostag.Equals("_") || string.IsNullOrEmpty(t.Cpostag)
                            || t.Postag.Equals("_") || string.IsNullOrEmpty(t.Postag)
                            || t.Feats.Equals("_") || string.IsNullOrEmpty(t.Feats)
                            || t.Head.Equals("_") || string.IsNullOrEmpty(t.Head)
                            || t.Deprel.Equals("_") || string.IsNullOrEmpty(t.Deprel)
                            || t.Deprel.Contains("??")
                            )
                        {
                            complete = false;
                            break;
                        }
                        else
                        {
                            int head = Convert.ToInt32(t.Head);
                            if (head > s.Tokens.Count)
                            {
                                complete = false;
                                break;
                            }
                            complete = true;
                        }

                    }

                    if (complete)
                        resultList.Add(s);
                    else
                        wasteList.Add(s);
                }

                foreach (CoNLLSentence s in resultList)
                {
                    SaveSentenceToFile(s, targetFile, false, false);
                    //foreach (CoNLLToken t in s.Tokens)
                    //    SaveTokenToCsvFile(t, targetFile.Replace(".conll", ".csv"), false, false);
                }
                foreach (CoNLLSentence s in wasteList)
                    SaveSentenceToFile(s, wasteFile, false, false);

                Console.WriteLine("cele vety: " + resultList.Count);
                Console.WriteLine("nekompletne vety: " + wasteList.Count);
                Console.WriteLine("spolu: " + sentences.Count);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }

    
}
