using System;
using System.Collections.Generic;
using ConsoleTables;

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
        private List<List<bool>> adjacencyMatrix = new List<List<bool>>();
        private Settings settings;

        /// <summary>
        /// Конструктор
        /// </summary>
        public OrientedGraph()
        {
            this.vertices = new List<GraphVertex>();
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

        /// <summary>
        /// Поиска индекса вершины в списке по выполняемому логическому выражению.
        /// </summary>
        /// <param name="expression">Логическое выражение</param>
        /// <returns></returns>
        public int getIndexOf(string expression)
        {
            for (int i = 0; i < this.vertices.Count; i++)
            {
                if (vertices[i].LogicExpression == expression)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Добавление вершины
        /// </summary>
        /// <param logicExpression="vertexName">Имя вершины</param>
        public bool addVertex(string vertexName, string operation)
        {
            if (this.getIndexOf(vertexName) != -1)
                return false;
            vertices.Add(new GraphVertex(vertexName, operation));
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
        public bool addEdge(string vertexFrom, string vertexTo)
        {
            int v1 = this.getIndexOf(vertexFrom);
            int v2 = this.getIndexOf(vertexTo);
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
        public bool addDoubleEdge(string vertexFromFirst, string vertexFromSecond, string vertexTo)
        {
            int v1 = this.getIndexOf(vertexFromFirst);
            int v2 = this.getIndexOf(vertexFromSecond);
            int v3 = this.getIndexOf(vertexTo);
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
                cols[i + 1] = isExpressions ? vertices[i].LogicExpression : String.Format($"{i,-3}");
            consTable.AddColumn(cols);

            for (int i = 0; i < this.vertices.Count; i++)
            {
                row[i] = new string[vertices.Count + 1];
                row[i][0] = isExpressions ? vertices[i].LogicExpression : String.Format($"{i,-3}");
                for (int j = 1; j < this.vertices.Count + 1; j++)
                    row[i][j] = String.Format($"{Convert.ToInt32(this.adjacencyMatrix[i][j-1])}");
                consTable.AddRow(row[i]);
            }
            consTable.Write(Format.Alternative);
        }

        public List<string> getVerticesOfType(string type)
        {
            List<string> names = new List<string>();
            foreach (var vert in this.vertices)
            {
                if (vert.Operation == type)
                    names.Add(vert.LogicExpression);
            }
            return names;
        }
    }    
}
