using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numl;
using numl.Model;
using numl.Supervised;
using numl.Supervised.DecisionTree;
using numl.Supervised.KNN;
using numl.Supervised.NeuralNetwork;
using System.Configuration;
using System.IO;
using Infrastructure;
using System.Xml;

namespace NumlSample
{
   
    public class Oracle
    {
        public AppSettingsReader AppSettings;


        public LearningModel ModelIsHeadFalse { get; set; }
        public LearningModel ModelIsTokenCritical_Adv { get; set; }
        public LearningModel ModelIsTokenCritical_Atr { get; set; }
        public LearningModel ModelIsTokenCritical_Obj { get; set; }
        public LearningModel ModelIsTokenCritical_Aux { get; set; }
        public LearningModel ModelIsTokenCritical_Sb { get; set; }
        public LearningModel ModelIsTokenCritical { get; set; }
        public LearningModel ModelOracle { get; set; }

        public Oracle()
        {
            AppSettings = new AppSettingsReader();
            //IsTokenCritical_Model = new LearningModel();
            //IsTokenCritical_Model.Model = new DecisionTreeModel();
            //string pom = AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString();
            //IsTokenCritical_Model.Model.Load(pom);

        }

        public bool IsTokenCitical(Token token)
        {
            try
            {
                //LearningModel IsTokenCritical_Model = new LearningModel();
                //IsTokenCritical_Model = new LearningModel();
                //IsTokenCritical_Model.Model = new DecisionTreeModel();
                //IsTokenCritical_Model = Learner.Learn(new List<Token>{new Token()}, 0.5, 1, new DecisionTreeGenerator(Descriptor.Create<Token>()));
                //IsTokenCritical_Model.Model.Load(AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString());

                if (token.Deprel.Equals("Adv"))
                {
                    token = ModelIsTokenCritical_Adv.Model.Predict(token);
                }
                else if (token.Deprel.Equals("Atr"))
                {
                    token = ModelIsTokenCritical_Atr.Model.Predict(token);
                }
                else if (token.Deprel.Equals("Obj"))
                {
                    token = ModelIsTokenCritical_Obj.Model.Predict(token);
                }
                else
                {
                    token = ModelIsTokenCritical.Model.Predict(token);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                token.IsCorrectDeprel = true;
            }
            return !token.IsCorrectDeprel;
        }

        public void IsHeadFalse_TrainModel(string strID)
        {
            try
            {
                DateTime start = DateTime.Now;

                Console.WriteLine("reading tokens");
                List<TokenHead> tokens = TokenHead.GetTokensFromCoNLLFile(AppSettings.GetValue(strID, typeof(String)).ToString(), true, true, true, true, true);

                TokenHead[] data = tokens.ToArray();

                var d = Descriptor.Create<TokenHead>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(false);

                Console.WriteLine("is head false - learning start");

                
                ModelIsHeadFalse = Learner.Learn(data, 0.80, 20, g);

                Console.WriteLine(ModelIsHeadFalse);

                Console.WriteLine("is head false - learning end");

                Console.WriteLine("testing model...");
                IsHeadFalse_TestModel();

                ModelIsHeadFalse.Model.Save(AppSettings.GetValue("IsHeadFalseModel", typeof(string)).ToString());
                /*
                IsTokenCritical_Model = new LearningModel();
                IsTokenCritical_Model.Model = new DecisionTreeModel();
                string s = AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString();
                IsTokenCritical_Model.Model.Load(s);
                */
                DateTime end = DateTime.Now;
                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void IsHeadFalse_LoadModel(string strID)
        {
            try
            {
                DateTime start = DateTime.Now;
                Console.WriteLine("Loading IsHeadFalse model ... start");

                ModelIsHeadFalse = new LearningModel();
                ModelIsHeadFalse.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID, typeof(string)).ToString());


                Console.WriteLine("Loading IsHeadFalse model ... end");
                DateTime end = DateTime.Now;

                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void IsHeadFalse_TestModel()
        {
            try
            {
                List<TokenHead> tokens = TokenHead.GetTokensFromCoNLLFile(AppSettings.GetValue("IsHeadFalseTestFile", typeof(string)).ToString(), true, true, true, true, true);
                int truePositivesFalseNegatives = 0;
                foreach (TokenHead t in tokens)
                {
                    bool pom = t.IsCorrectHead;
                    t.IsCorrectHead = !pom;

                    TokenHead tPredicion = ModelIsHeadFalse.Model.Predict(t);
                    if (pom == tPredicion.IsCorrectHead)
                        truePositivesFalseNegatives++;
                }
                double result = ((double)truePositivesFalseNegatives / (double)tokens.Count) * 100;
                Console.WriteLine("Precission: " + result.ToString() + " %");
                File.AppendAllText("precision.txt", "Precision is head false: " + result.ToString() + " %" + " - " + ModelIsHeadFalse.Accuracy.ToString() + " %" + "\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void IsTokenCritical_TrainModel(string file, bool splitTrainingSet, string modelName)
        {
            try
            {
                DateTime start = DateTime.Now;

                if (!Directory.Exists(modelName))
                    Directory.CreateDirectory(modelName);

                Console.WriteLine("reading tokens");
                DivideRStrain(file);

                List<Token> tokens = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Adv = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile_Adv", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Atr = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile_Atr", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Obj = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile_Obj", typeof(String)).ToString(), true, true, true, true, true);
                //List<Token> tokens_Aux = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile_Aux", typeof(String)).ToString(), true, true, true, true, true);
                //List<Token> tokens_Sb = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTrainFile_Sb", typeof(String)).ToString(), true, true, true, true, true);

                
                Token[] data = tokens.ToArray();
                var d = Descriptor.Create<Token>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(false);
                
                Console.WriteLine("is token critical - learning start");
                List<LearningModel> models = new List<LearningModel>();
                int n = int.Parse(AppSettings.GetValue("RepeatTrainingIsCritical", typeof(string)).ToString());
                for (int i = 0; i < n; i++)
                {
                    DateTime st = DateTime.Now;
                    models.Add(Learner.Learn(data, 0.98, 1, g));
                    //models[0].Model.Save(AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString().Replace(".xml", "_" + i.ToString() + ".xml"));
                    //models.Add(Learner.Learn(data, 0.80, 5, g));
                    DateTime en = DateTime.Now;
                    TimeSpan ts = en - st;
                    Console.Write(".");
                    Console.WriteLine(" duration: " + ts.Hours.ToString() + " h " + ts.Minutes.ToString() + " m " + ts.Seconds.ToString() + " s");
                }
                ModelIsTokenCritical = Learner.Best(models);
                Console.WriteLine(ModelIsTokenCritical);
                ModelIsTokenCritical.Model.Save(modelName + "/" + modelName + ".xml");
                
                Console.WriteLine("Saved model IsTokenCritical");
                

                //adv
                Token[] data_Adv = tokens_Adv.ToArray();
                var d_Adv = Descriptor.Create<Token>();
                var g_Adv = new DecisionTreeGenerator(d_Adv);
                ModelIsTokenCritical_Adv = Learner.Learn(data_Adv, 0.98, 1, g_Adv);
                ModelIsTokenCritical_Adv.Model.Save(modelName + "/" + modelName + "_Adv.xml");
                Console.WriteLine("Saved model IsTokenCritical_Adv");

                
                //atr
                Token[] data_Atr = tokens_Atr.ToArray();
                var d_Atr = Descriptor.Create<Token>();
                var g_Atr = new DecisionTreeGenerator(d_Atr);
                ModelIsTokenCritical_Atr = Learner.Learn(data_Atr, 0.98, 1, g_Atr);
                ModelIsTokenCritical_Atr.Model.Save(modelName + "/" + modelName + "_Atr.xml");
                Console.WriteLine("Saved model IsTokenCritical_Atr");
                                
                //obj
                Token[] data_Obj = tokens_Obj.ToArray();
                var d_Obj = Descriptor.Create<Token>();
                var g_Obj = new DecisionTreeGenerator(d_Obj);
                ModelIsTokenCritical_Obj = Learner.Learn(data_Obj, 0.98, 1, g_Obj);
                ModelIsTokenCritical_Obj.Model.Save(modelName + "/" + modelName + "_Obj.xml");
                Console.WriteLine("Saved model IsTokenCritical_Obj");
                                  
                
                Console.WriteLine("is token critical - learning end");

                //TESTOVANIE MODELOV
                //Console.WriteLine("testing model...");
                //IsTokenCritical_TestModelParts();

                /*
                IsTokenCritical_Model = new LearningModel();
                IsTokenCritical_Model.Model = new DecisionTreeModel();
                string s = AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString();
                IsTokenCritical_Model.Model.Load(s);
                */
                DateTime end = DateTime.Now;
                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void IsTokenCritical_TrainModel(string strID)
        {
            try
            {
                DateTime start = DateTime.Now;

                Console.WriteLine("reading tokens");
                List<Token> tokens = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID, typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Adv = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID + "_Adv", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Atr = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID + "_Atr", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Obj = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID + "_Obj", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Aux = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID + "_Aux", typeof(String)).ToString(), true, true, true, true, true);
                List<Token> tokens_Sb = Token.GetTokensFromCoNLLFile(AppSettings.GetValue(strID + "_Sb", typeof(String)).ToString(), true, true, true, true, true);

                //odpoznamkovat
                /*
                Token[] data = tokens.ToArray();
                var d = Descriptor.Create<Token>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(false);
                
                Console.WriteLine("is token critical - learning start");
                List<LearningModel> models = new List<LearningModel>();
                int n = int.Parse(AppSettings.GetValue("RepeatTrainingIsCritical", typeof(string)).ToString());
                for (int i = 0; i < n; i++)
                {
                    DateTime st = DateTime.Now;
                    models.Add(Learner.Learn(data, 0.98, 1, g));
                    //models[0].Model.Save(AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString().Replace(".xml", "_" + i.ToString() + ".xml"));
                    //models.Add(Learner.Learn(data, 0.80, 5, g));
                    DateTime en = DateTime.Now;
                    TimeSpan ts = en - st;
                    Console.Write(".");
                    Console.WriteLine(" duration: " + ts.Hours.ToString() + " h " + ts.Minutes.ToString() + " m " + ts.Seconds.ToString() + " s");
                }
                ModelIsTokenCritical = Learner.Best(models);
                Console.WriteLine(ModelIsTokenCritical);
                ModelIsTokenCritical.Model.Save(AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical");
                

                //adv
                Token[] data_Adv = tokens_Adv.ToArray();
                var d_Adv = Descriptor.Create<Token>();
                var g_Adv = new DecisionTreeGenerator(d_Adv);
                ModelIsTokenCritical_Adv = Learner.Learn(data_Adv, 0.98, 1, g_Adv);
                ModelIsTokenCritical_Adv.Model.Save(AppSettings.GetValue("IsTokenCriticalModel_Adv", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical_Adv");

                
                //atr
                Token[] data_Atr = tokens_Atr.ToArray();
                var d_Atr = Descriptor.Create<Token>();
                var g_Atr = new DecisionTreeGenerator(d_Atr);
                ModelIsTokenCritical_Atr = Learner.Learn(data_Atr, 0.98, 1, g_Atr);
                ModelIsTokenCritical_Atr.Model.Save(AppSettings.GetValue("IsTokenCriticalModel_Atr", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical_Atr");
                                
                //obj
                Token[] data_Obj = tokens_Obj.ToArray();
                var d_Obj = Descriptor.Create<Token>();
                var g_Obj = new DecisionTreeGenerator(d_Obj);
                ModelIsTokenCritical_Obj = Learner.Learn(data_Obj, 0.98, 1, g_Obj);
                ModelIsTokenCritical_Obj.Model.Save(AppSettings.GetValue("IsTokenCriticalModel_Obj", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical_Obj");
                                  
                //Aux
                Token[] data_Aux = tokens_Aux.ToArray();
                var d_Aux = Descriptor.Create<Token>();
                var g_Aux = new DecisionTreeGenerator(d_Aux);
                ModelIsTokenCritical_Aux = Learner.Learn(data_Aux, 0.98, 1, g_Aux);
                ModelIsTokenCritical_Aux.Model.Save(AppSettings.GetValue("IsTokenCriticalModel_Aux", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical_Aux");
                */
                                  
                //sb
                Token[] data_Sb = tokens_Sb.ToArray();
                var d_Sb = Descriptor.Create<Token>();
                var g_Sb = new DecisionTreeGenerator(d_Sb);
                ModelIsTokenCritical_Sb = Learner.Learn(data_Sb, 0.98, 1, g_Sb);
                ModelIsTokenCritical_Sb.Model.Save(AppSettings.GetValue("IsTokenCriticalModel_Sb", typeof(string)).ToString());
                Console.WriteLine("Saved model IsTokenCritical_Sb");
                

                Console.WriteLine("is token critical - learning end");
                
                //TESTOVANIE MODELOV
                //Console.WriteLine("testing model...");
                //IsTokenCritical_TestModelParts();

                /*
                IsTokenCritical_Model = new LearningModel();
                IsTokenCritical_Model.Model = new DecisionTreeModel();
                string s = AppSettings.GetValue("IsTokenCriticalModel", typeof(string)).ToString();
                IsTokenCritical_Model.Model.Load(s);
                */
                DateTime end = DateTime.Now;
                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DivideRStrain(string sourceFile)
        {
            if (!Directory.Exists("RS_train"))
            {
                Directory.CreateDirectory("RS_train");
            }

            List<Token> allTokens = Token.GetTokensFromCoNLLFile(sourceFile, true, false, true, true, true);
            List<Token> tokensAtr = new List<Token>();
            List<Token> tokensAdv = new List<Token>();
            //List<Token> tokensAux = new List<Token>();
            //List<Token> tokensSb = new List<Token>();
            List<Token> tokensObj = new List<Token>();
            List<Token> tokensOther = new List<Token>();

            int falseAtr = 0;
            int falseAdv = 0;
            int falseObj = 0;
            //int falseAux = 0;
            //int falseSb = 0;
            int falseOther = 0;

            foreach (Token t in allTokens)
            {
                if (t.Deprel.Contains("Atr"))
                {
                    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                        falseAtr++;
                    tokensAtr.Add(t);
                }
                else if (t.Deprel.Contains("Adv"))
                {
                    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                        falseAdv++;
                    tokensAdv.Add(t);
                }
                else if (t.Deprel.Contains("Obj"))
                {
                    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                        falseObj++;
                    tokensObj.Add(t);
                }
                //else if (t.Deprel.Contains("Aux"))
                //{
                //    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                //        falseAux++;
                //    tokensAux.Add(t);
                //}
                //else if (t.Deprel.Contains("Sb"))
                //{
                //    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                //        falseSb++;
                //    tokensSb.Add(t);
                //}
                else
                {
                    if (t.IsCorrectHead && !t.IsCorrectDeprel)
                        falseOther++;
                    tokensOther.Add(t);
                }
            }

            foreach (Token t in tokensAtr)
                t.SaveTokenToFile(t, AppSettings.GetValue("IsTokenCriticalTrainFile_Atr", typeof(string)).ToString(), true, true, true, true);
            foreach (Token t in tokensAdv)
                t.SaveTokenToFile(t, AppSettings.GetValue("IsTokenCriticalTrainFile_Adv", typeof(string)).ToString(), true, true, true, true);
            foreach (Token t in tokensObj)
                t.SaveTokenToFile(t, AppSettings.GetValue("IsTokenCriticalTrainFile_Obj", typeof(string)).ToString(), true, true, true, true);
            //foreach (Token t in tokensAux)
            //    t.SaveTokenToFile(t, sourceFile.Replace(".conll", "_Aux.conll"), true, true, true, true);
            //foreach (Token t in tokensSb)
            //    t.SaveTokenToFile(t, sourceFile.Replace(".conll", "_Sb.conll"), true, true, true, true);
            foreach (Token t in tokensOther)
                t.SaveTokenToFile(t, AppSettings.GetValue("IsTokenCriticalTrainFile", typeof(string)).ToString(), true, true, true, true);

            //Console.WriteLine("Atr - total: " + tokensAtr.Count.ToString() + ", wrong: " + falseAtr.ToString());
            //Console.WriteLine("Adv - total: " + tokensAdv.Count.ToString() + ", wrong: " + falseAdv.ToString());
            //Console.WriteLine("Obj - total: " + tokensObj.Count.ToString() + ", wrong: " + falseObj.ToString());
            //Console.WriteLine("Aux - total: " + tokensAux.Count.ToString() + ", wrong: " + falseAux.ToString());
            //Console.WriteLine("Sb - total: " + tokensSb.Count.ToString() + ", wrong: " + falseSb.ToString());
            //Console.WriteLine("other - total: " + tokensOther.Count.ToString() + ", wrong: " + falseOther.ToString());
            Console.WriteLine("ALL - total: " + (tokensAdv.Count + tokensAtr.Count + tokensObj.Count + tokensOther.Count).ToString() + ", wrong: " + (falseAdv + falseAtr + falseObj + falseOther).ToString());
        }

        public void IsTokenCritical_LoadModel(string strID)
        {
            try
            {
                DateTime start = DateTime.Now;
                Console.WriteLine("Loading IsTokenCritical model ... start");

                ModelIsTokenCritical = new LearningModel();
                ModelIsTokenCritical.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID, typeof(string)).ToString());

                ModelIsTokenCritical_Adv = new LearningModel();
                ModelIsTokenCritical_Adv.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID + "_Adv", typeof(string)).ToString());

                ModelIsTokenCritical_Atr = new LearningModel();
                ModelIsTokenCritical_Atr.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID + "_Atr", typeof(string)).ToString());

                ModelIsTokenCritical_Obj = new LearningModel();
                ModelIsTokenCritical_Obj.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID + "_Obj", typeof(string)).ToString());
 
                //ModelIsTokenCritical_Aux = new LearningModel();
                //ModelIsTokenCritical_Aux.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID + "_Aux", typeof(string)).ToString());
                
                //ModelIsTokenCritical_Sb = new LearningModel();
                //ModelIsTokenCritical_Sb.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID + "_Sb", typeof(string)).ToString());

                //testovanie - odpoznamkovat
                //IsTokenCritical_TestModelParts();

                Console.WriteLine("Loading IsTokenCritical model ... end");
                DateTime end = DateTime.Now;

                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void IsTokenCritical_TestModel()
        {
            try
            {
                List<Token> tokens = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTestFile", typeof(string)).ToString(), true, true, false, false, false);

                int truePositives = 0;
                int falseNegatives = 0;
                int allPositives = 0;
                int allNegatives = 0;
                                
                foreach (Token t in tokens)
                {
                    bool pom = t.IsCorrectDeprel;
                    t.IsCorrectDeprel = !pom;

                    Token tPredicion = null;
                    //if (t.Deprel.Equals("Adv") && (t.Cpostag.Equals("A") || t.Cpostag.Equals("D")))
                    //{
                    //    tPredicion = t;
                    //    tPredicion.IsCorrectDeprel=false;
                    //}
                    //else
                    try
                    {
                        if (t.Deprel.Contains("Adv"))
                        {
                            tPredicion = ModelIsTokenCritical_Adv.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Atr"))
                        {
                            tPredicion = ModelIsTokenCritical_Atr.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Obj"))
                        {
                            tPredicion = ModelIsTokenCritical_Obj.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Aux"))
                        {
                            tPredicion = ModelIsTokenCritical_Aux.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Sb"))
                        {
                            tPredicion = ModelIsTokenCritical_Sb.Model.Predict(t);
                        }
                        else
                            tPredicion = ModelIsTokenCritical.Model.Predict(t);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        tPredicion = t;
                        tPredicion.IsCorrectDeprel = false;
                    }

                    if (pom)
                    {
                        allPositives++;
                        if (pom == tPredicion.IsCorrectDeprel)
                            truePositives++;
                    }
                    else
                    {
                        allNegatives++;
                        if (pom == tPredicion.IsCorrectDeprel)
                            falseNegatives++;
                    }
                }
                double resultPos = ((double)truePositives / (double)allPositives) * 100;
                double resultNeg = ((double)falseNegatives / (double)allNegatives) * 100;
                double result = ((double)(truePositives + falseNegatives) / (double)tokens.Count) * 100;
                Console.WriteLine("truePos: " + resultPos.ToString() + " %, falseNeg: " + resultNeg.ToString() + " %");
                Console.WriteLine("precision: " + result.ToString() + " %");
                File.AppendAllText("precision.txt", "Precision is token critical: TruePos" + resultPos.ToString() + " %" + ", falseNeg" + resultNeg + " %" + "\r\n");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// testuje kazdu cast modelu zvlast (zvlast Sb, Aux, Ob, Atr, Adv, other)
        /// </summary>
        public void IsTokenCritical_TestModelParts()
        {
            try
            {
                List<Token> tokens = Token.GetTokensFromCoNLLFile(AppSettings.GetValue("IsTokenCriticalTestFile", typeof(string)).ToString(), true, true, false, false, false);

                int truePositives = 0;
                int falseNegatives = 0;
                int allPositives = 0;
                int allNegatives = 0;

                int truePositives_other = 0;
                int falseNegatives_other = 0;
                int allPositives_other = 0;
                int allNegatives_other = 0;
                int allTokens_other = 0;

                int truePositives_Atr = 0;
                int falseNegatives_Atr = 0;
                int allPositives_Atr = 0;
                int allNegatives_Atr = 0;
                int allTokens_Atr = 0;

                int truePositives_Adv = 0;
                int falseNegatives_Adv = 0;
                int allPositives_Adv = 0;
                int allNegatives_Adv = 0;
                int allTokens_Adv = 0;

                int truePositives_Obj = 0;
                int falseNegatives_Obj = 0;
                int allPositives_Obj = 0;
                int allNegatives_Obj = 0;
                int allTokens_Obj = 0;

                int truePositives_Sb = 0;
                int falseNegatives_Sb = 0;
                int allPositives_Sb = 0;
                int allNegatives_Sb = 0;
                int allTokens_Sb = 0;

                int truePositives_Aux = 0;
                int falseNegatives_Aux = 0;
                int allPositives_Aux = 0;
                int allNegatives_Aux = 0;
                int allTokens_Aux = 0;

                foreach (Token t in tokens)
                {
                    bool pom = t.IsCorrectDeprel;
                    t.IsCorrectDeprel = !pom;

                    Token tPredicion = null;
                    //if (t.Deprel.Equals("Adv") && (t.Cpostag.Equals("A") || t.Cpostag.Equals("D")))
                    //{
                    //    tPredicion = t;
                    //    tPredicion.IsCorrectDeprel=false;
                    //}
                    //else
                    try
                    {
                        
                        if (t.Deprel.Contains("Adv"))
                        {
                            allTokens_Adv++;
                            tPredicion = ModelIsTokenCritical_Adv.Model.Predict(t);
                        }
                        /*else if (t.Deprel.Contains("Atr"))
                        {
                            allTokens_Atr++;
                            tPredicion = ModelIsTokenCritical_Atr.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Obj"))
                        {
                            allTokens_Obj++;
                            tPredicion = ModelIsTokenCritical_Obj.Model.Predict(t);
                        }
                        else if (t.Deprel.Contains("Aux"))
                        {
                            allTokens_Aux++;
                            tPredicion = ModelIsTokenCritical_Aux.Model.Predict(t);
                        }*/
                        else if (t.Deprel.Contains("Sb"))
                        {
                            allTokens_Sb++;
                            tPredicion = ModelIsTokenCritical_Sb.Model.Predict(t);
                        }
                         /*          
                        else
                        {
                            allTokens_other++;
                            tPredicion = ModelIsTokenCritical.Model.Predict(t);
                        }
                        */
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        tPredicion = t;
                        tPredicion.IsCorrectDeprel = false;
                    }

                    if (pom && tPredicion!=null)
                    {
                        allPositives++;
                        switch (tPredicion.Deprel)
                        {
                            case "Atr":
                                allPositives_Atr++;
                                break;
                            case "Adv":
                                allPositives_Adv++;
                                break;
                            case "Sb":
                                allPositives_Sb++;
                                break;
                            case "Aux":
                                allPositives_Aux++;
                                break;
                            case "Obj":
                                allPositives_Obj++;
                                break;
                            default:
                                allPositives_other++;
                                break;
                        }
                        if (pom == tPredicion.IsCorrectDeprel)
                        {
                            truePositives++;
                            switch (tPredicion.Deprel)
                            {
                                case "Atr":
                                    truePositives_Atr++;
                                    break;
                                case "Adv":
                                    truePositives_Adv++;
                                    break;
                                case "Sb":
                                    truePositives_Sb++;
                                    break;
                                case "Aux":
                                    truePositives_Aux++;
                                    break;
                                case "Obj":
                                    truePositives_Obj++;
                                    break;
                                default:
                                    truePositives_other++;
                                    break;
                            }
                        }
                    }
                    else if(tPredicion!=null)
                    {
                        allNegatives++;
                        switch (tPredicion.Deprel)
                        {
                            case "Atr":
                                allNegatives_Atr++;
                                break;
                            case "Adv":
                                allNegatives_Adv++;
                                break;
                            case "Sb":
                                allNegatives_Sb++;
                                break;
                            case "Aux":
                                allNegatives_Aux++;
                                break;
                            case "Obj":
                                allNegatives_Obj++;
                                break;
                            default:
                                allNegatives_other++;
                                break;
                        }

                        if (pom == tPredicion.IsCorrectDeprel)
                        {
                            falseNegatives++;
                            switch (tPredicion.Deprel)
                            {
                                case "Atr":
                                    falseNegatives_Atr++;
                                    break;
                                case "Adv":
                                    falseNegatives_Adv++;
                                    break;
                                case "Sb":
                                    falseNegatives_Sb++;
                                    break;
                                case "Aux":
                                    falseNegatives_Aux++;
                                    break;
                                case "Obj":
                                    falseNegatives_Obj++;
                                    break;
                                default:
                                    falseNegatives_other++;
                                    break;
                            }
                        }
                    }
                }

                //double resultPos = ((double)truePositives / (double)allPositives) * 100;
                //double resultNeg = ((double)falseNegatives / (double)allNegatives) * 100;
                //double result = ((double)(truePositives + falseNegatives) / (double)tokens.Count) * 100;
                //Console.WriteLine("truePos: " + resultPos.ToString() + " %, falseNeg: " + resultNeg.ToString() + " %");
                //Console.WriteLine("precision: " + result.ToString() + " %");
                //File.AppendAllText("precision.txt", "Precision is token critical: TruePos" + resultPos.ToString() + " %" + ", falseNeg" + resultNeg + " %" + "\r\n");
                
                //ATR
                //double resultPos_Atr = ((double)truePositives_Atr / (double)allPositives_Atr) * 100;
                //double resultNeg_Atr = ((double)falseNegatives_Atr / (double)allNegatives_Atr) * 100;
                //double result_Atr = ((double)(truePositives_Atr + falseNegatives_Atr) / (double)allTokens_Atr) * 100;
                //Console.WriteLine("\r\nAtr:");
                //Console.WriteLine("truePos: " + resultPos_Atr.ToString() + " %, falseNeg: " + resultNeg_Atr.ToString() + " %");
                //Console.WriteLine("precision: " + result_Atr.ToString() + " %");

                //ADV
                double resultPos_Adv = ((double)truePositives_Adv / (double)allPositives_Adv) * 100;
                double resultNeg_Adv = ((double)falseNegatives_Adv / (double)allNegatives_Adv) * 100;
                double result_Adv = ((double)(truePositives_Adv + falseNegatives_Adv) / (double)allTokens_Adv) * 100;
                Console.WriteLine("\r\nAdv:");
                Console.WriteLine("truePos: " + resultPos_Adv.ToString() + " %, falseNeg: " + resultNeg_Adv.ToString() + " %");
                Console.WriteLine("precision: " + result_Adv.ToString() + " %");

                //OBJ
                //double resultPos_Obj = ((double)truePositives_Obj / (double)allPositives_Obj) * 100;
                //double resultNeg_Obj = ((double)falseNegatives_Obj / (double)allNegatives_Obj) * 100;
                //double result_Obj = ((double)(truePositives_Obj + falseNegatives_Obj) / (double)allPositives_Obj) * 100;
                //Console.WriteLine("\r\nObj:");
                //Console.WriteLine("truePos: " + resultPos_Obj.ToString() + " %, falseNeg: " + resultNeg_Obj.ToString() + " %");
                //Console.WriteLine("precision: " + result_Obj.ToString() + " %");

                //SB
                double resultPos_Sb = ((double)truePositives_Sb / (double)allPositives_Sb) * 100;
                double resultNeg_Sb = ((double)falseNegatives_Sb / (double)allNegatives_Sb) * 100;
                double result_Sb = ((double)(truePositives_Sb + falseNegatives_Sb) / (double)allPositives_Sb) * 100;
                Console.WriteLine("\r\nSb:");
                Console.WriteLine("truePos: " + resultPos_Sb.ToString() + " %, falseNeg: " + resultNeg_Sb.ToString() + " %");
                Console.WriteLine("precision: " + result_Sb.ToString() + " %");

                //AUX
                //double resultPos_Aux = ((double)truePositives_Aux / (double)allPositives_Aux) * 100;
                //double resultNeg_Aux = ((double)falseNegatives_Aux / (double)allNegatives_Aux) * 100;
                //double result_Aux = ((double)(truePositives_Aux + falseNegatives_Aux) / (double)allTokens_Aux) * 100;
                //Console.WriteLine("\r\nAux:");
                //Console.WriteLine("truePos: " + resultPos_Aux.ToString() + " %, falseNeg: " + resultNeg_Aux.ToString() + " %");
                //Console.WriteLine("precision: " + result_Aux.ToString() + " %");

                //other
                //double resultPos_other = ((double)truePositives_other / (double)allPositives_other) * 100;
                //double resultNeg_other = ((double)falseNegatives_other / (double)allNegatives_other) * 100;
                //double result_other = ((double)(truePositives_other + falseNegatives_other) / (double)allTokens_other) * 100;
                //Console.WriteLine("\r\nother:");
                //Console.WriteLine("truePos: " + resultPos_other.ToString() + " %, falseNeg: " + resultNeg_other.ToString() + " %");
                //Console.WriteLine("precision: " + result_other.ToString() + " %");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //strId = inputFile
        public void Oracle_TrainModel(string strID, string modelName)
        {
            try
            {
                DateTime start = DateTime.Now;

                Console.WriteLine("reading oracle train file");
                //List<TokenOracle> tokens = TokenOracle.GetTokensFromCoNLLFile(AppSettings.GetValue(strID, typeof(String)).ToString(), true);
                List<TokenOracle> tokens = TokenOracle.GetTokensFromCoNLLFile(strID, true);

                TokenOracle[] data = tokens.ToArray();

                var d = Descriptor.Create<TokenOracle>();
                var g = new DecisionTreeGenerator(d);
                g.SetHint(false);

                Console.WriteLine("oracle - learning start");
                List<LearningModel> models = new List<LearningModel>();

                int n = int.Parse(AppSettings.GetValue("RepeatTrainingOracle", typeof(string)).ToString());

                for (int i = 0; i < n; i++)
                {
                    DateTime st = DateTime.Now;
                    models.Add(Learner.Learn(data, 0.90, 1, g));
                    //models[0].Model.Save(AppSettings.GetValue("OracleModel", typeof(string)).ToString().Replace(".xml", "_" + i.ToString() + ".xml"));
                    DateTime en = DateTime.Now;
                    TimeSpan ts = en - st;
                    Console.Write(".");
                    Console.WriteLine(" duration: " + ts.Hours.ToString() + " h " + ts.Minutes.ToString() + " m " + ts.Seconds.ToString() + " s");
                }
                
                ModelOracle = Learner.Best(models);
                Console.WriteLine(ModelOracle);

                Console.WriteLine("oracle - learning end");

                Console.WriteLine("testing model...");
                //Oracle_TestModel("OracleTestFile");

                //ModelOracle.Model.Save(AppSettings.GetValue("OracleModel", typeof(string)).ToString());
                ModelOracle.Model.Save(modelName + ".xml");

                DateTime end = DateTime.Now;

                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Oracle_LoadModel(string strID)
        {
            try
            {
                DateTime start = DateTime.Now;
                Console.WriteLine("loading oracle model start");

                ModelOracle = new LearningModel();
                ModelOracle.Model = new DecisionTreeModel().Load(new AppSettingsReader().GetValue(strID, typeof(string)).ToString());

                //Oracle_TestModel("OracleTestFile");

                Console.WriteLine("loading oracle model end");
                DateTime end = DateTime.Now;

                TimeSpan span = end - start;
                Console.WriteLine("Duration: " + span.Hours.ToString() + " h, " + span.Minutes.ToString() + " m, " + span.Seconds.ToString() + "." + span.Milliseconds.ToString() + " s");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Oracle_TestModel(string strID)
        {
            try
            {
                List<TokenOracle> tokens = TokenOracle.GetTokensFromCoNLLFile(AppSettings.GetValue(strID, typeof(string)).ToString(),true);
                int truePositivesFalseNegatives = 0;
                foreach (TokenOracle t in tokens)
                {
                    string pom = t.CorrectDeprel.ToString();

                    t.CorrectDeprel = Deprel.Sb_M;

                    TokenOracle tPredicion = ModelOracle.Model.Predict(t);
                    
                    if ((Deprel)Enum.Parse(typeof(Deprel), pom) == tPredicion.CorrectDeprel)
                        truePositivesFalseNegatives++;
                }
                double result = ((double)truePositivesFalseNegatives / (double)tokens.Count) * 100;
                Console.WriteLine("Precision: " + result.ToString() + " %");
                File.AppendAllText("precision.txt", "Precision oracle (" + strID + "): " + result.ToString() + " %\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ParseBase()
        {
            //System.Diagnostics.Process.Start(@"C:\AllMyDocs\FIIT\02_Ing\DP\data\experiment_sk_korpus_polovica\parsing_maltParser.bat");
        }

        public TokenOracle ParseOracle(TokenOracle token)
        {
            string path =AppSettings.GetValue("OracleModel", typeof(string)).ToString();
            //LearningModel Oracle_Model = new LearningModel();

            //Oracle_Model.Model.Load(File.Open(path, FileMode.Open));

            TokenOracle result = ModelOracle.Model.Predict(token);

            return result;
        }

        public void LoadModel(string strID)
        {
            string filename = AppSettings.GetValue(strID, typeof(string)).ToString();
            string content = File.ReadAllText(filename);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            XmlNode descriptor = doc.SelectSingleNode("//Descriptor");

            this.ModelOracle = new LearningModel();
            this.ModelOracle.Model = new DecisionTreeModel();
            this.ModelOracle.Model.Load(filename);
            //this.ModelOracle.Model.Descriptor = new Descriptor();
            //this.ModelOracle.Model.Descriptor.Type = typeof(Token);

            //TextReader tr = new StringReader(descriptor.OuterXml);
            //XmlReader reader = XmlReader.Create(tr);

            //this.ModelOracle.Model.Descriptor.ReadXml(reader);
        }
    }
}
