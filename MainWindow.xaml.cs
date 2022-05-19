using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Generators;

namespace Логическое_выражение
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                MessageBox.Show("Файл " + Name1 + " успешно открыт!"); 
            }
            else
            {
                MessageBox.Show("Ошибка при работе с файлам!");
                return;
            }
        }

        private void cmMake(object sender, RoutedEventArgs e)
        {
            string FileName =  tbText1.Text.ToString();
            string str = File.ReadAllText(FileName);
            string rez = "";
            string logic = "";

            int n = Int32.Parse(tbCount.Text.ToString());
            double q = Math.Pow(2, n);
            int m = (int)q;

            MessageBox.Show("M - " + m.ToString() + " N - " + n.ToString());

            for (int i = 0; i < str.Length; i++)

                if (str[i] >= '0' && str[i] <= '1')
                {
                    rez += str[i];
                }
            MessageBox.Show(str);
            MessageBox.Show(rez + " length - " + rez.Length);

            string[] st = new string[m];
            string one = "1";

            int k = 0;

            for (int i = 0; i < rez.Length; i++)
            {
                bool b = (rez[i].ToString()).Contains(one);

                if (b)
                {

                    if ((i + 1) % (n + 1) == 0)
                    {
                        MessageBox.Show(i.ToString() + " " + rez[i].ToString() + " - Нужный элемент");
                        st[k] = inverse(rez, n, i);
                        k++;
                    }

                }

            }

            MessageBox.Show("Количество в array st - " + (k).ToString());
            for (int i = 0; i < k; i++)
              {
                  MessageBox.Show(st[i]);
              }

            logic = makeLogic(st, k);

            tbText3.Text = logic;

            MessageBox.Show(logic);

            Parser graph = new Parser(logic);
            bool res = graph.Parse(logic);
            
            MessageBox.Show(res.ToString());

        }
        
        private void cmCountOfVar(object sender, RoutedEventArgs e)
        {
            int n;
            if (!int.TryParse(tbCount.Text, out n))
            {
                MessageBox.Show("Не число!");
                return;
            }
            MessageBox.Show("Количество переменных - " + tbCount.Text);
        }

        string inverse(string rez, int n, int i)
        {
            string x = "";
            string nul = "0";

            int j = 0;
            int index = 0;

           
                for (j = i - n ; j < i ; j++)
                {
                    index++;
                    if ((rez[j].ToString()).Contains(nul))
                    {
                        x += "!" + "x" + index.ToString();

                    }
                    else
                    {
                        x += "x" + index.ToString();
                    }
                }
                              
           return x;
        }

        string makeLogic( string[] st, int k)
        {
            string logic = "";

            for(int i = 0; i < k; i++)
            {
                logic += st[i] + "+";
            }
            logic = logic.TrimEnd('+');
            return logic;
        }

        private void cmMakeFile(object sender, RoutedEventArgs e)
        {
            
           /* StreamWriter fw = new StreamWriter("D:\\" + tbName.Text.ToString());

            fw.WriteLine(tbText3.Text.ToString());

                fw.Close();
            MessageBox.Show("Файл создан!");*/

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
            MessageBox.Show("Файл создан!"); 

        }

        private void cmSelect(object sender, RoutedEventArgs e)
        {

        }
    }
}
