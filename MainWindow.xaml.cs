using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using Generators;
using Properties;
using Graph;
using System.Diagnostics;
using System.Reflection;

namespace Логическое_выражение
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;
        private delegate void ChangeDelegate(OrientedGraph graph, int n);

        public Dictionary<string, Tuple<string, int>> logicOperations;
        public MainWindow()
        {
            InitializeComponent();
            this.settings = Settings.GetInstance();
        }


        private void cmOpen(object sender, RoutedEventArgs e)
        {
            string Name1;

            OpenFileDialog dialog1 = new OpenFileDialog();
            dialog1.Filter = "Text files (*.txt)|*.txt|All files|*.*";

            if (dialog1.ShowDialog() == true)
            {
                Name1 = dialog1.FileName;
                tbText1.Text = Name1;
            }
            else
            {
                MessageBox.Show("Ошибка при работе с файлам!");
                return;
            }
        }

        public void cmMake(object sender, RoutedEventArgs e)
        {
            tbText3.Text = "";

            string FileName = tbText1.Text.ToString();
            string str = File.ReadAllText(FileName);


            int n = Int32.Parse(tbCount.Text.ToString());
            double q = Math.Pow(2, n);
            int m = (int)q;

            bool cb_and = Convert.ToBoolean(CheckBoxAnd.IsChecked.ToString());
            bool cb_or = Convert.ToBoolean(CheckBoxOr.IsChecked.ToString());
            bool cb_not = Convert.ToBoolean(CheckBoxNot.IsChecked.ToString());
            bool cb_nand = Convert.ToBoolean(CheckBoxNand.IsChecked.ToString());
            bool cb_nor = Convert.ToBoolean(CheckBoxNor.IsChecked.ToString());
            bool cb_xor = Convert.ToBoolean(CheckBoxXor.IsChecked.ToString());

            List<string> endBasis = new List<string>();
            if (cb_and)
                endBasis.Add("and");
            if (cb_or)
                endBasis.Add("or");
            if (cb_not)
                endBasis.Add("not");
            if (cb_nand)
                endBasis.Add("nand");
            if (cb_nor)
                endBasis.Add("nor");
            if (cb_xor)
                endBasis.Add("xor");

            if (endBasis.Count == 0)
            {
                MessageBox.Show("Выберите базис!");
                return;
            }

            if (!checkInputs(n,FileName))
            {
                Trace.Write("неверно");
                return;
            }
            TruthTable table = new TruthTable(int.Parse(tbCount.Text), FileName);
            
            table.printTable();
            bool cdnf = true;
            List<string> log = new List<string>();
            List<string> list = new List<string>(table.cnfFromTruthTable(cdnf));
            List<string> fun = new List<string>();

            for (int j = 0; j < list.Count; j++)
            {

                log.Add(list[j]);
                
                Parser pr = new Parser(log[j]);

                bool res = pr.ParseAll();
                OrientedGraph graph = null;
                if (res)
                    graph = pr.Graph;
                if (graph != null)
                    Trace.WriteLine("result of parsing -" + res.ToString());

                graph.printAdjacencyMatrix();

                int i = 0;
                while (i < graph.Vertices.Count)
                {
                    bool f = true;
                    foreach (string s in endBasis)
                        if (s == graph.Vertices[i].operation)
                            f = false;
                    if (graph.Vertices[i].operation == "input" || graph.Vertices[i].operation == "const" || graph.Vertices[i].operation == "output")
                        f = false;
                    if (f)
                    {
                        string curOp = graph.Vertices[i].operation;
                        string endOp = GetBestOperation(curOp, endBasis, graph);
                        string methodName = "change_" + curOp + "_" + endOp;
                        if (endOp == "")
                            break;

                        ChangeDelegate handler;
                        MethodInfo mi = typeof(MainWindow).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                        handler = (ChangeDelegate)Delegate.CreateDelegate(type: typeof(ChangeDelegate), firstArgument: this, method: mi);
                        handler(graph, i);
                        graph.printAdjacencyMatrix();
                        i--;
                    }
                    i++;
                }

                //запись функции в поле записи
                fun.Add(graph.create_fun(graph));
                if (fun[j] == list[j])
                    MessageBox.Show("Функция будет представлена в СДНФ");
                tbText3.AppendText(fun[j]+"\n");
            }

        }
        

        public string GetBestOperation(string curop, List<string> endb, OrientedGraph graph)
        {
            string endop = "";

            if (endb.Count == 1)
            {
                if ((endb[0] == "xor" && curop != "or")
                    || (endb[0] == "and" && curop == "or")
                    || (endb[0] == "not" && curop == "or") 
                    || (endb[0] == "or" && curop == "and")
                    || (endb[0] == "or" && curop == "not") 
                    || (endb[0] == "not" && curop == "and")
                    || (endb[0] == "and" && curop != "not"))
                    endop = "";
                else
                    return endb[0];
            }

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"and_nor", 3},
                {"and_nand", 2},
                {"or_nor", 2},
                {"or_nand", 3},
                {"not_nand", 0},
                {"not_nor", 0},
                {"or_xor",4 },
            };

            
            int min = 5;
            string s = "";
            foreach (var eb in endb)
            {
                if (eb == "xor" && curop != "or")
                {
                    endop = "";
                }
                else
                {
                    s = curop + "_" + eb;
                    if (dict.ContainsKey(s) && dict[s] < min)
                    {
                        min = dict[s];
                        endop = eb;
                    }
                }
                if(endop =="")
                    MessageBox.Show("Не удалось преобразовать " + curop + " к " +eb);
            }
            Trace.WriteLine(curop);
            Trace.WriteLine(endop);
            
            return endop;
        }

        private void cmCountOfVar(object sender, RoutedEventArgs e)
        {
            int n;
            if (!int.TryParse(tbCount.Text, out n))
            {
                MessageBox.Show("Не число!");
                return;
            }
        }
        public bool checkInputs(int n, string fname)
        {
            bool res = false;
            int size = (int)Math.Pow(2, n);
            string[] file =File.ReadAllLines(fname);
            int k = file.Length;
            if (k != size)
                MessageBox.Show("Введено неверное количество переменных!\n Введите повторно");     
            else
                res = true;

            return res;
        }
        private void cmMakeFile(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dialog2 = new SaveFileDialog();
            dialog2.Filter = "Text files (*.txt)|*.txt|All files|*.*";
            dialog2.FileName = "text";
            dialog2.DefaultExt = ".txt";

            if (dialog2.ShowDialog() == true)
            {
                tbName.Text = dialog2.FileName;

            }
            else
            {
                return;
            }

            StreamWriter fw = new StreamWriter(tbName.Text);

            fw.WriteLine(tbText3.Text.ToString());

            fw.Close();

        }

        private void change_and_nor(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;

            graph.addVertex("1", "nor");
            graph.addVertex("2", "nor");
            graph.addVertex("3", "nor");
            graph.addDoubleEdge(ln, ln + 1, ln + 2);

            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);

            graph.addEdge(from[0], ln);
            graph.addEdge(from[1], ln + 1);

            foreach (int t in to)
                graph.addEdge(ln + 2, t);

            graph.deleteVertex(n, graph);

            graph.updateExpressions();

        }

        private void change_not_nor(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;
            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);
            graph.addEdge(from[0], n);

            graph.Vertices[n].operation = "nor";
            graph.updateExpressions();

        }

        private void change_or_nor(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;
            Trace.WriteLine(ln.ToString() + (ln + 1).ToString());
            graph.addVertex("5", "nor");
            graph.addVertex("6", "nor");

            graph.addEdge(ln, ln + 1);

            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);

            graph.addEdge(from[0], ln);
            graph.addEdge(from[1], ln);

            foreach (int t in to)
                graph.addEdge(ln + 1, t);

            graph.deleteVertex(n, graph);
            graph.updateExpressions();
        }
        private void change_not_nand(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;
            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);
            graph.addEdge(from[0], n);

            graph.Vertices[n].operation = "nand";
            graph.updateExpressions();

        }
        private void change_and_nand(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;

            graph.addVertex("8", "nand");
            graph.addVertex("9", "nand");
            graph.addEdge(ln, ln + 1);

            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);
            graph.addEdge(from[0], ln);
            graph.addEdge(from[1], ln);

            foreach (int t in to)
                graph.addEdge(ln + 1, t);
            graph.printAdjacencyMatrix();

            graph.deleteVertex(n, graph);
            graph.updateExpressions();

        }
        private void change_or_nand(OrientedGraph graph, int n)
        {
            int ln = graph.Vertices.Count;

            graph.addVertex("1", "nand");
            graph.addVertex("2", "nand");
            graph.addVertex("3", "nand");
            graph.addDoubleEdge(ln, ln + 1, ln + 2);


            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);

            graph.addEdge(from[0], ln);
            graph.addEdge(from[1], ln + 1);

            foreach (int t in to)
                graph.addEdge(ln + 2, t);

            graph.deleteVertex(n, graph);

            graph.updateExpressions();

        }
        private void change_or_xor(OrientedGraph graph, int n)
        {

            int ln = graph.Vertices.Count;

            graph.addVertex("1", "and");
            graph.addVertex("2", "nand");
            graph.addVertex("3", "xor");
            graph.addDoubleEdge(ln, ln + 1, ln + 2);


            List<int> from = graph.getConnectedFrom(n);
            List<int> to = graph.getConnectedTo(n);

            graph.addDoubleEdge(from[0], from[1], ln);
            graph.addDoubleEdge(from[0], from[1], ln + 1);

            foreach (int t in to)
                graph.addEdge(ln + 2, t);

            graph.deleteVertex(n, graph);

            graph.updateExpressions();


        }
    } 
}

