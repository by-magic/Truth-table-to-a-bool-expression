﻿using System;
using System.Collections.Generic;
using ConsoleTables;
using Properties;
using System.Windows;
using Generators;
using System.Diagnostics;
using Properties;

namespace Graph
{
    /// <summary>
    /// Граф
    /// </summary>
    public class OrientedGraph
    {
        /// <summary>
        /// Список вершин графа
        /// </summary>
        private List<GraphVertex> vertices;
        private List<List<bool>> adjacencyMatrix;
        private Dictionary<string, bool> error;
        private Settings settings;

        public Dictionary<string, Tuple<string, int>> logicOperations;
        /// <summary>
        /// Соотносят операции и уровень выполнения. Формат: {операция: уровень выполнения}.
        /// </summary>
        public Dictionary<string, string> operationsToName;
        /// <summary>
        /// Конструктор
        /// </summary>
        public OrientedGraph()
        {
            this.vertices = new List<GraphVertex>();
            adjacencyMatrix = new List<List<bool>>();
            this.settings = Settings.GetInstance();
        }

        public List<GraphVertex> Vertices
        {
            get
            {
                return vertices;
            }
        }
        
        public List<List<bool>> AdjacencyMatrix
        {
            get
            {
                return adjacencyMatrix;
            }
        }

        public int MaxLevel
        {
            get {
                var outputs = this.getVerticesByType("output");
                int maxLevel = 0;
                for (int i = 0; i < outputs.Count; i++)
                {
                    int n = this.getIndexOfExpression(outputs[i]);
                    n = Vertices[n].Level;
                    maxLevel = n > maxLevel ? n : maxLevel;
                }
                return maxLevel; 
            }
        }

        /// <summary>
        /// Поиска индекса вершины в списке по выполняемому логическому выражению.
        /// </summary>
        /// <param name="expression">Логическое выражение</param>
        /// <returns></returns>
        public int getIndexOfExpression(string expression)
        {
            //TODO: провести поиск не по полномму совпадению, а по перестановкам.
            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (vertices[i].LogicExpression == expression)
                {
                    return i;
                }
            }
            return -1;
        }
        public int getIndexOfWireName(string wireName)
        {
            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (vertices[i].WireName == wireName)
                {
                    return i;
                }
            }
            return -1;
        }

        public void deleteVertex(int n, OrientedGraph graph)
        {
            graph.printAdjacencyMatrix();
            vertices.RemoveAt(n);
            int i;

            for (i = 0; i < adjacencyMatrix.Count; ++i)
            {
                adjacencyMatrix[i].RemoveAt(n);
            }
            adjacencyMatrix.RemoveAt(n);
        }

        /// <summary>
        /// Добавление вершины
        /// </summary>
        /// <param logicExpression="vertexName">Имя вершины</param>
        public bool addVertex(string vertexName, string operation, string wireName = null)
        {
            if (this.getIndexOfExpression(vertexName) != -1)
                return false;
            if (wireName == null)
                vertices.Add(new GraphVertex(vertexName, operation));
            else
                vertices.Add(new GraphVertex(vertexName, operation, false, wireName));
            for (int i = 0; i < vertices.Count - 1; i++)
                adjacencyMatrix[i].Add(false);
            adjacencyMatrix.Add(new List<bool>());
            for (int i = 0; i < adjacencyMatrix.Count; i++)
                adjacencyMatrix[vertices.Count - 1].Add(false);
            return true;
        }

        /// <summary>
        /// Добавление ребра
        /// </summary>
        /// <param logicExpression="vertexFrom">Имя первой вершины</param>
        /// <param logicExpression="vertexTo">Имя второй вершины</param>
        public bool addEdge(string vertexFrom, string vertexTo, bool isExpression = true)
        {
            int v1 = -1;
            int v2 = -1;
            if (isExpression)
            {
                v1 = this.getIndexOfExpression(vertexFrom);
                v2 = this.getIndexOfExpression(vertexTo);
            }
            else
            {
                v1 = this.getIndexOfWireName(vertexFrom);
                v2 = this.getIndexOfWireName(vertexTo);
            }

            if (v1 != -1 && v2 != -1)
            {
                vertices[v2].Level = Math.Max(vertices[v1].Level + 1, vertices[v2].Level);
                adjacencyMatrix[v1][v2] = true;
                return true;
            }
            return false;
        }/// <summary>
         /// Добавление ребра
         /// </summary>
         /// <param logicExpression="vertexFrom">Имя первой вершины</param>
         /// <param logicExpression="vertexTo">Имя второй вершины</param>
        public bool addEdge(int vertexFrom, int vertexTo)
        {
            int v1 = vertexFrom;
            int v2 = vertexTo;
            if (v1 != -1 && v2 != -1)
            {
                vertices[v2].Level = Math.Max(vertices[v1].Level + 1, vertices[v2].Level);
                adjacencyMatrix[v1][v2] = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление двух ребер
        /// </summary>
        /// <param logicExpression="vertexFrom">Имя первой вершины</param>
        /// <param logicExpression="vertexTo">Имя второй вершины</param>
        /*public bool addDoubleEdge(string vertexFromFirst, string vertexFromSecond, string vertexTo, bool isExpression = true)
        {
            int v1 = -1;
            int v2 = -1;
            int v3 = -1;
            if (isExpression)
            {
                v1 = this.getIndexOfExpression(vertexFromFirst);
                v2 = this.getIndexOfExpression(vertexFromSecond);
                v3 = this.getIndexOfExpression(vertexTo);
            }
            else
            {
                v1 = this.getIndexOfWireName(vertexFromFirst);
                v2 = this.getIndexOfWireName(vertexFromSecond);
                v3 = this.getIndexOfWireName(vertexTo);
            }

            if (v1 != -1 && v2 != -1 && v3 != -1)
            {
                vertices[v3].Level = Math.Max(vertices[v1].Level + 1, vertices[v3].Level);
                vertices[v3].Level = Math.Max(vertices[v2].Level + 1, vertices[v3].Level);
                adjacencyMatrix[v1][v3] = true;
                adjacencyMatrix[v2][v3] = true;

                return true;
            }
            return false;
        }
        */
        /// <summary>
        /// Добавление двух ребер
        /// </summary>
        public bool addDoubleEdge(int vertexFromFirst, int vertexFromSecond, int vertexTo)
        {
            int v1 = vertexFromFirst;
            int v2 = vertexFromSecond;
            int v3 = vertexTo;

            if (v1 != -1 && v2 != -1 && v3 != -1)
            {
                
                vertices[v3].Level = Math.Max(vertices[v1].Level+1 , vertices[v3].Level);
                vertices[v3].Level = Math.Max(vertices[v2].Level+1 , vertices[v3].Level);
                adjacencyMatrix[v1][v3] = true;
                adjacencyMatrix[v2][v3] = true;

                return true;
            }
            return false;
        }


        /// <summary>
        /// Вывод таблицы смежности графа в консоль.
        /// </summary>
        /// <param name="isExpressions">Вывод названий вершин. false - номера в списке. true - логические выражения.</param>
        public void printAdjacencyMatrix(bool isExpressions = false)
        {
            var consTable = new ConsoleTable();
            string[] cols = new string[vertices.Count + 1];
            string[][] row = new string[vertices.Count][];

            cols[0] = "";
            for (int i = 0; i < this.vertices.Count; i++)
                cols[i + 1] = isExpressions ? vertices[i].WireName : vertices[i].LogicExpression;
            consTable.AddColumn(cols);
            for (int i = 0; i < this.vertices.Count; i++)
            {
                row[i] = new string[vertices.Count + 1];
                row[i][0] = isExpressions ? vertices[i].WireName : vertices[i].LogicExpression;
                for (int j = 1; j < this.vertices.Count + 1; j++)
                    row[i][j] = String.Format($"{Convert.ToInt32(this.adjacencyMatrix[i][j-1])}");
                consTable.AddRow(row[i]);
            }
            consTable.Write(Format.Alternative);
            Trace.Write(consTable);
        }

        public List<string> getVerticesByType(string type)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation == type)
                    names.Add(vert.LogicExpression);
            }
            return names;
        }

        public List<string> getVerticesByTypeToWireName(string type)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation == type)
                    names.Add(vert.WireName);
            }
            return names;
        }

        public List<string> getLogicVerticesToWireName()
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation != "input" &&
                    vert.Operation != "output" &&
                    vert.Operation != "const")
                    names.Add(vert.WireName);
            }
            return names;
        }

        public List<string> getVerticesByLevel(int level, bool wireName = false)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Level == level) {
                    if (wireName)
                        names.Add(vert.WireName);
                    else
                        names.Add(vert.LogicExpression);
                }
            }
            return names;
        }
        

        public List<int> getConnectedTo(int k)
        {
            List<int> lst = new List<int>();

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (this.adjacencyMatrix[k][i])
                    lst.Add(i);
            }

            return lst;
        }

        public List<int> getConnectedFrom(int k)
        {
            List<int> lst = new List<int>();

            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (this.adjacencyMatrix[i][k])
                    lst.Add(i);
            }

            return lst;
        }

        public void updateLevels(bool isFull = true, int k = 0, int q = 0, int w = 0)
        {
            try
            {
                if (isFull)
                {
                    for (int i = 0; i < this.vertices.Count; i++)
                    {
                        this.vertices[i].Level = 0;
                    }
                    for (int i = 0; i < this.vertices.Count; i++)
                    {
                        if (this.vertices[i].Operation == "input" || this.vertices[i].Operation == "const")
                        {
                            updateLevels(false, i);
                        }
                    }
                }
                else
                {
                    List<int> ver = getConnectedTo(k);
                    foreach (int j in ver)
                    {
                        this.vertices[j].Level = Math.Max(this.vertices[j].Level, this.vertices[k].Level + 1);
                        updateLevels(isFull, j);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void updateExpressions(bool isFull = true, int k = 0)
        {
            this.updateLevels();
            for (int i = 0; i < this.MaxLevel; i++)
            {
                List<string> verts = this.getVerticesByLevel(i, true);
                foreach (var vert in verts)
                {
                    int vertInd = this.getIndexOfWireName(vert);
                    if (this.vertices[vertInd].Operation == "input" || this.vertices[i].Operation == "const")
                    {
                        this.vertices[vertInd].LogicExpression = this.vertices[vertInd].WireName;
                    }
                    else if (this.vertices[vertInd].Operation == "output")
                    {
                        List<int> verid = this.getConnectedFrom(vertInd);
                        this.vertices[vertInd].LogicExpression = this.vertices[vertInd].WireName + " = " + this.vertices[verid[0]].LogicExpression;
                        continue;

                    }
                    else
                    {
                        List<int> verid = this.getConnectedFrom(vertInd);
                        if (verid.Count == 1 && this.vertices[vertInd].operation != "not")
                        {
                            this.vertices[vertInd].LogicExpression = "(" + this.vertices[verid[0]].LogicExpression + " " + this.settings.logicOperations[this.vertices[vertInd].operation].Item1 + " " + this.vertices[verid[0]].LogicExpression + ")";
                            Trace.WriteLine(this.vertices[vertInd].LogicExpression);
                            continue;
                        }
                        if (verid.Count == 1)
                        {
                            this.vertices[vertInd].LogicExpression = "(" + this.vertices[vertInd].operation + " " + this.vertices[verid[0]].LogicExpression + ")";
                            Trace.WriteLine(this.vertices[vertInd].LogicExpression);
                            continue;
                        }
                        if (verid.Count == 2)
                        {
                            this.vertices[vertInd].LogicExpression = "(" + this.vertices[verid[0]].LogicExpression + " " + this.vertices[vertInd].operation + " " + this.vertices[verid[1]].LogicExpression + ")";
                            Trace.WriteLine(this.vertices[vertInd].LogicExpression);
                            continue;
                        }
                    }
               }
            }
        }

        public string create_fun(OrientedGraph graph)
        {
            string f_list = "";
            List<string> vert = graph.getVerticesByType("output");

            foreach (var v in vert)
            {
                List<int> verid = this.getConnectedFrom(this.getIndexOfExpression(v));
                foreach (var ver in verid)
                    f_list = v + " = (" + this.vertices[ver].LogicExpression + ")";
            }

            return f_list;
        }
        private List<bool> vertsToValues(List<int> verts)
        {
            List<bool> val = new List<bool>();
            foreach (int i in verts)
                val.Add(this.vertices[i].Value);
            return val;
        }
        private bool calc(List<bool> inputs, string op)
        {
            bool res = false;
            if (inputs.Count == 0)
                return res;
            switch (op)
            {
                case "not":
                    {
                        res = !inputs[0];
                        break;
                    }
                case "buf":
                    {
                        res = inputs[0];
                        break;
                    }
                case "and":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res &= inputs[i];
                        break;
                    }
                case "nand":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res &= inputs[i];
                        res = !res;
                        break;
                    }
                case "or":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res |= inputs[i];
                        break;
                    }
                case "nor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res |= inputs[i];
                        res = !res;
                        break;
                    }
                case "xor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res ^= inputs[i];
                        break;
                    }
                case "xnor":
                    {
                        res = inputs[0];
                        for (int i = 1; i < inputs.Count; i++)
                            res ^= inputs[i];
                        res = !res;
                        break;
                    }
            }
            return res;
        }
        public Dictionary<string, bool> calcGraph(  Dictionary<string, bool> inputValues, 
                                                    bool withErrorValues = false, 
                                                    Dictionary<string, bool> errorValues = null,
                                                    bool withErrorSertting = false,
                                                    Dictionary<string, bool> setError = null)
        {
            this.updateLevels();
            Dictionary<string, bool> dict = new Dictionary<string, bool>();
            if (inputValues.Count != this.getVerticesByType("input").Count)
                return null;
            if (withErrorValues && errorValues != null && errorValues.Count != this.vertices.Count
                                        - this.getVerticesByType("input").Count
                                        - this.getVerticesByType("const").Count
                                        - this.getVerticesByType("output").Count)
                return null;
            if (withErrorSertting && setError != null && setError.Count != this.vertices.Count
                                        - this.getVerticesByType("input").Count
                                        - this.getVerticesByType("const").Count
                                        - this.getVerticesByType("output").Count)
                return null;
            int n = this.MaxLevel;
            for (int level = 0; level < n; level++)
            {
                List<string> verts = this.getVerticesByLevel(level);
                foreach (var v in verts)
                {
                    int i = this.getIndexOfExpression(v);
                    switch (this.vertices[i].Operation)
                    {
                        case "input":
                            {
                                this.vertices[i].Value = inputValues[this.vertices[i].WireName];
                                break;
                            }
                        case "output":
                            {
                                List<int> from = this.getConnectedFrom(i);
                                if (from.Count > 0)
                                    dict.Add(this.vertices[i].WireName, this.vertices[from[0]].Value);
                                break;
                            }
                        case "const":
                            {
                                this.vertices[i].Value = this.vertices[i].WireName.Contains("1'b1") ? true : false;
                                break;
                            }
                        default:
                            {
                                List<int> from = this.getConnectedFrom(i);
                                List<bool> lst = new List<bool>();
                                if (withErrorValues)
                                    lst.Add(errorValues[this.vertices[i].WireName]);
                                lst.AddRange(this.vertsToValues(from));
                                this.vertices[i].Value = this.calc(lst, this.vertices[i].Operation);
                                if (withErrorSertting)
                                    this.vertices[i].Value ^= setError[this.vertices[i].WireName];
                                break;
                            }
                    }
                }
            }
            
            return dict; 
        }
        

        //TODO: отредактировать подсчет уровня каждой вершины

        //TODO: проверка на корректность.

        //TODO: оптимизация графа.
    }    
}
