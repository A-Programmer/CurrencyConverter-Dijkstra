using System;
using System.Collections.Concurrent;
using Project.CurrencyConverter.Contracts;

namespace Project.CurrencyConverter.Graph
{
	public class Graph<T, TEdge> : IGraph<T, TEdge>
	{
		private List<IVertex<T>> _vertices { get; }
		private List<IEdge<T, TEdge>> _edges { get; }
		private ConcurrentDictionary<string, List<IEdge<T, TEdge>>[]> _cache { get; }

		public Graph()
		{
			_vertices = new List<IVertex<T>>();
			_edges = new List<IEdge<T, TEdge>>();
			_cache = new ConcurrentDictionary<string, List<IEdge<T, TEdge>>[]>();
		}

		public void AddEdge(T fromVertex, T toVertex, int weight = 0, TEdge data = default)
        {
			var fromObject = new Vertex<T>(fromVertex);
			var toObject = new Vertex<T>(toVertex);

			if (!_vertices.Contains(fromObject))
				_vertices.Add(fromObject);

			if (!_vertices.Contains(toObject))
				_vertices.Add(toObject);

			var edg = new Edge<T, TEdge>(fromObject, toObject, weight, data);

			if (_edges.IndexOf(edg) != -1)
				return;
			_edges.Add(edg);
			_vertices[_vertices.IndexOf(fromObject)].AddAdjacent(_vertices[_vertices.IndexOf(toObject)]);
		}




		public List<IEdge<T, TEdge>>[] FindShortestPath(T fromVertex, T toVertex)
		{
			var cachName = $"{fromVertex}_{toVertex}";
			if(_cache.TryGetValue(cachName, out var value))
            {
				return value;
            }

			var shortestPaths = new List<IEdge<T, TEdge>>[3];
			var queue = new List<IVertex<T>>(_vertices);
			int shortestPathValue = int.MaxValue;
			int shortestPathValue2 = int.MaxValue;
			int shortestPathValue3 = int.MaxValue;
			var shortestVertices = new Vertex<T>[3];

			Configure(fromVertex);
            ClearPreviousVertices();

            while (queue.Count != 0)
            {
                queue.Sort();
                var currentVertex = queue[0];
                queue.RemoveAt(0);

                if (currentVertex.Equals(new Vertex<T>(toVertex)) && queue.Count != 0)
                {
                    queue.Sort();
                    var newVertex = queue[0];
                    queue.RemoveAt(0);
                    queue.Add(currentVertex);
                    currentVertex = newVertex;
                }

                foreach (var adjacentVertex in currentVertex.AdjacentVertices)
                {
                    if (!queue.Contains(adjacentVertex)) continue;

                    var currentVertexValue = currentVertex.Weight;
                    var indexOf = _edges.IndexOf(new Edge<T, TEdge>(currentVertex, adjacentVertex));
                    if (indexOf == -1) continue;
                    var edgeValue = _edges[indexOf].Weight;
                    var currentPathValue = currentVertexValue + edgeValue;

                    if (currentPathValue < adjacentVertex.Weight)
                    {
                        UpdateVertex(adjacentVertex, currentVertex);

                        if (adjacentVertex.Equals(new Vertex<T>(toVertex)))
                        {
                            shortestVertices[0] = new Vertex<T>(adjacentVertex);
                            shortestPathValue = currentVertexValue + edgeValue;
                        }
                    }
                    else if (currentPathValue > shortestPathValue &&
                             currentPathValue < shortestPathValue2)
                    {

                        if (ContainsVertex(shortestVertices[1], adjacentVertex))
                        {
                            shortestVertices[2] = new Vertex<T>(shortestVertices[1]);
                            shortestPathValue3 = shortestVertices[2].Weight;
                        }

                        shortestVertices[1] = new Vertex<T>(shortestVertices[0]);
                        UpdateVertex(shortestVertices[1], currentVertex);
                        shortestPathValue2 = currentPathValue;
                    }
                    else if (currentPathValue > shortestPathValue2 &&
                             currentPathValue < shortestPathValue3)
                    {

                        if (ContainsVertex(shortestVertices[1], adjacentVertex))
                        {
                            shortestVertices[2] = new Vertex<T>(shortestVertices[1]);
                        }

                        shortestVertices[2] = new Vertex<T>(shortestVertices[1]);
                        UpdateVertex(shortestVertices[2], currentVertex);
                        shortestPathValue3 = currentPathValue;
                    }
                }

                if (queue.Count == 0) continue;
                var vertex = queue[0];
                queue.RemoveAt(0);
                queue.Add(vertex);
            }

            for (var i = 0; i < 3; i++)
            {
                shortestPaths[i] = GetShortestPathEdges(shortestVertices[i]);
            }

            _cache.TryAdd(cachName, shortestPaths);
            return shortestPaths;
        }





        private static bool ContainsVertex(IVertex<T> currentVertex, IVertex<T> checkContainment)
        {
            if (currentVertex == null)
                return false;

            IVertex<T> current = new Vertex<T>(currentVertex);

            while (current != null)
            {
                if (current.Equals(checkContainment))
                    return true;
                current = current.PreviousVertex;
            }
            return false;
        }


        private void ClearPreviousVertices()
        {
            foreach (var vertex in _vertices)
                vertex.PreviousVertex = null;
        }


        private void UpdateVertex(IVertex<T> adjacentVertex, IVertex<T> currentVertex)
        {
            var index = _edges.IndexOf(new Edge<T, TEdge>(currentVertex, adjacentVertex));
            if (index == -1) return;
            adjacentVertex.Weight = currentVertex.Weight + _edges[index].Weight;
            adjacentVertex.PreviousVertex = currentVertex;
        }

        private List<IEdge<T, TEdge>> GetShortestPathEdges(IVertex<T> toVertex)
        {
            var path = new List<IEdge<T, TEdge>>();
            while (true)
            {
                if (toVertex?.PreviousVertex == null) return path;

                var index = _edges.IndexOf(new Edge<T, TEdge>(toVertex.PreviousVertex, toVertex));
                if (index != -1) path.Add(_edges[index]);
                toVertex = toVertex.PreviousVertex;
            }
        }

        private void Configure(T fromVertex)
        {
			foreach(var vertex in _vertices)
            {
				vertex.Weight = vertex.Value.Equals(fromVertex) ? 0 : int.MaxValue;
            }
        }
    }
}

