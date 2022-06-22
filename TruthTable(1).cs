using System;
using System.IO;
using System.Collections.Generic;
using ConsoleTables;
using Properties;
using System.Windows;
using System.Diagnostics;

namespace Generators
{
    class TruthTable
    {
        private int input, output, size;
        public bool[][] array { get; set; }
        private Settings settings;

        public TruthTable()
        {
            this.settings = Settings.GetInstance();
        }

        /// Конструктор класса.
        /*public TruthTable(int input, int output, bool[][] array = null)
        {
            this.settings = Settings.GetInstance();
            this.input = input;
            this.output = output;
            size = (int)Math.Pow(2, this.input);
            if (array == null || array.Length != size || array[0].Length != output)
                this.generateTable();
            else
                this.array = array;
        }*/

        public TruthTable(int input, string fname = null)
        {
            this.settings = Settings.GetInstance();
            this.input = input;
            if (fname == null) {
                this.output = 0;
                this.array = null;
            }
            else
            {
                StreamReader sr = new StreamReader(fname);                
                string line = sr.ReadLine();
                line = line.Remove(line.Length - 1);
                { 
                    string[] subs = line.Split(' ');
                    
                    this.output = subs.Length - input;
                    Trace.WriteLine("Length: "+subs.Length.ToString());
                    Trace.WriteLine("input length" + this.input);
                    size = (int)Math.Pow(2, this.input);
                }
                this.array = new bool[this.size][];

                int nlines = 0;
                while (line != null) {

                    string[] subs = line.Split(' '); 
                    this.array[nlines] = new bool[this.output];
                    for ( int i = this.input; i < subs.Length; i++)
                    {
                        this.array[nlines][i - this.input] = subs[i] == "1";
                    }
                    line = sr.ReadLine();
                    nlines++;
                }
                sr.Close();
            }
        }


        /// Взаимодействие с переменной input.
        public int Input
        {
            get
            {
                return this.input;
            }
        }

        /// Взаимодействие с переменной output.
        public int Output
        {
            get
            {
                return this.output;
            }
        }

        /// Взаимодействие с переменной size.
        public int Size
        {
            get
            {
                return (int)Math.Pow(2, this.input);
            }
        }

        /// Взаимодействие с переменной array.
        public bool[][] OutTable
        {
            get
            {
                return this.array;
            }
        }

        /// Генерация случайных значений таблицы истинности.
        public void generateTable(double p = 0)
        {
            if (p == 0)
            {
                this.array = new bool[this.size][];
                Random rnd = new Random();
                for (int i = 0; i < this.size; i++)
                {
                    this.array[i] = new bool[this.output];
                    for (int j = 0; j < this.output; j++)
                    {
                        this.array[i][j] = rnd.Next(0, 2) == 1;
                    }
                }
                return;
            }
            if (p > 0 && p <= 1)
            {
                this.array = new bool[this.size][];
                Random rnd = new Random();
                for (int i = 0; i < this.size; i++)
                {
                    this.array[i] = new bool[this.output];
                    for (int j = 0; j < this.output; j++)
                    {
                        this.array[i][j] = rnd.NextDouble() < p;
                    }
                }
                return;
            }
            
        }

        public List<string> cnfFromTruthTable(bool tp = true)
        {

            List<string> fun = new List<string>();
            bool[,] bin = this.convToBinary();

            for (int j = 0; j < this.Output; j++) //цикл для генерации уравнения каждого выхода
            {
                fun.Add($"f{j} = ");
                int mem = 0; //Будут хранится количество единиц для дальнейших циклов
                int tmp = 0; //Будет хранится текущее расположения строки, которой мы рассматриваем

                for (int i = 0; i < this.Size; i++) //цикл подсчёта единиц
                {
                    if (!(this.OutTable[i][j] ^ tp))
                    {
                        mem++;
                    }
                }
                if (mem == 0)
                {
                    fun[j] += $"1'b{(tp ? 0 : 1)}";
                    continue;
                }

                if (mem == this.Size)
                {
                    fun[j] += $"1'b{(tp ? 1 : 0)}";
                    continue;
                }

                for (int i = 0; i < mem; i++) //основной цикл создания логического уравнения
                {
                    fun[j] += '(';
                    while ((this.OutTable[tmp][j] ^ tp) && tmp < this.Size) //находим номер строки, где выход "1"
                    {
                        tmp++;
                    }

                    for (int k = 0; k < this.Input; k++) //Цикл, который переводит таблицу истинности в уравнение
                    {
                        if (bin[tmp, k] ^ tp) //Делаем "Не", если "0"
                        {
                            fun[j] += settings.logicOperations["not"].Item1 + " ";
                        }
                        fun[j] += 'x';
                        fun[j] += k.ToString();
                        if (k != this.Input - 1)
                        {
                            fun[j] += " " + (tp ? settings.logicOperations["and"].Item1 : settings.logicOperations["or"].Item1) + " ";
                        }
                    }

                    fun[j] += ')';

                    if (i != mem - 1)
                    {
                        fun[j] += tp ? settings.logicOperations["or"].Item1 : settings.logicOperations["and"].Item1;
                    }

                    tmp++;
                }
            }

            return fun;
        }


        /// Формирование части таблицы истинности с входными сигналами.
        /// Перевод чисел в бинарный массив.
        public bool[,] convToBinary()
        {
            bool[,] bin = new bool[Size, input];

            for (int i = 0; i < this.Size; i++)
            {
                for (int j = this.input - 1, tmp = i; j >= 0; j--)
                {
                    bin[i, j] = (tmp % 2) == 1;
                    tmp = tmp / 2;
                }
            }
            return bin;
        }

        /// Вывод сгенерированной таблицы истинности в консоль.
        public void printTable()
        {
            var consTable = new ConsoleTable();
            string[] cols = new string[input + output];
            string[][] row = new string[size][];

            for (int i = 0; i < row.Length; i++)
                row[i] = new string[input + output];

            for (int i = 0; i < input; i++)
                cols[i] = String.Format($"x{i,-5}");
            for (int i = 0; i < output; i++)
                cols[i + input] = String.Format($"f{i,-5}");
            consTable.AddColumn(cols);

            // Вывод значений таблицы истинности.
            bool[,] bin = this.convToBinary();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < input; j++)
                {
                    row[i][j] = String.Format($"{Convert.ToInt32(bin[i, j])}");
                }
                for (int j = 0; j < output; j++)
                {
                    row[i][input + j] = String.Format($"{Convert.ToInt32(array[i][j])}");
                }
                consTable.AddRow(row[i]);
            }
            consTable.Write(Format.Alternative);
            Trace.Write(consTable);
        }
    }
}
