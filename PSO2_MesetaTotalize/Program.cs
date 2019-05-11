using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2_MesetaTotalize
{
    class Program
    {
        static public double meseta_profit = 0;
        static public double meseta_loss = 0;
        static public string line = string.Empty;
        static public bool ruuning = false;
        static void Main(string[] args)
        {
            DLL.main.appcheck(DLL.main.myappname());
            DLL.main.FormStart("TEST_APPLICATION", DLL.main.myappname());

            Calc_MesetaTotalize(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\SEGA\PHANTASYSTARONLINE2\log");
            Console.WriteLine("処理が終了しました。あなたの現在のメセタ/総獲得メセタ/総消費メセタは以下の通りです。");
            Console.ReadLine();

        }
        static void Calc_MesetaTotalize(string filepath)
        {
            string[] files = nao0x0.IO.GetFolderSubFiles(filepath, "Action*.txt");

            ruuning = true;

            Task.Run(() =>
            {
                while (ruuning == true)
                {
                    nao0x0.wait.thread(500);
                    Display_CurrentMeseta();
                }
            });

            try
            {
                foreach (string filename in files)
                {
                    Console.WriteLine("Read File: " + filename);
                    System.IO.StreamReader cReader = (
                          new System.IO.StreamReader(filename, System.Text.Encoding.Default)
                      );

                    // 読み込んだ結果をすべて格納するための変数を宣言する
                    string stResult = string.Empty;

                    // 読み込みできる文字がなくなるまで繰り返す
                    while (cReader.Peek() >= 0)
                    {
                        // ファイルを 1 行ずつ読み込む
                        line = cReader.ReadLine();

                        bool check = DLL.main.StringSearch(line, "DisplayToShop-SetValue");
                        bool check2 = DLL.main.StringSearch(line, "Warehouse-Meseta");
                        if (check == false && check2 == false)
                        {

                            string search = DLL.main.StringRegexSearch(line, @"(\sMeseta\([0-9]+\))");
                            string search2 = string.Empty;
                            if (search == string.Empty)
                            {
                                search2 = DLL.main.StringRegexSearch(line, @"(\sMeseta\(-[0-9]+\))");
                            }

                            if (search != string.Empty)
                            {
                                meseta_profit += GetMesetaInt(search);
                                //Display_CurrentMeseta(line);
                            }
                            if (search2 != string.Empty)
                            {
                                meseta_loss += GetMesetaInt(search2);
                                //Display_CurrentMeseta(line);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {

            }

            ruuning = false;
        }
        static void Display_CurrentMeseta()
        {
            string date = DLL.main.StringRegexSearch(line, @"(^.+?\s)");
            double current = meseta_profit + meseta_loss;
            Console.CursorLeft = 0;
            Console.Write(date + " Current:" + String.Format("{0:#,0}",current) + " Profit:" + String.Format("{0:#,0}", meseta_profit) + " Loss:" + String.Format("{0:#,0}", meseta_loss) + "                           \r\n");
        }
        static double GetMesetaInt(string line_arg)
        {
            //Console.WriteLine(line);
            line_arg = DLL.main.StringDelete(line_arg, "Meseta(");
            line_arg = DLL.main.StringDelete(line_arg, ")");

            double result = 0;
            try
            {
                result = double.Parse(line_arg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }

            if (result >= 1000000)
            {
                nao0x0.PC.DebugLog("(More)Meseta:"+String.Format("{0:#,0}",result) + "   "+line);
            }

            return result;
        }
    }
}
