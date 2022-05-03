using System;
using Project.CurrencyConverter.Contracts;

namespace Project.CurrencyConverter.Graph
{
	public class Vertex<T> : IVertex<T>
	{
		public List<IVertex<T>> AdjacentVertices { get; }
        public int Weight { get; set; }
        public T Value { get; }
        public IVertex<T> PreviousVertex { get; set; }
        public object Data { get; set; }

        public Vertex(T value, int weight = 0)
        {
            Weight = weight;
            Value = value;
            AdjacentVertices = new List<IVertex<T>>();
            PreviousVertex = null;
        }

        public Vertex(IVertex<T> vertex)
        {
            Weight = vertex.Weight;
            Value = vertex.Value;
            AdjacentVertices = vertex.AdjacentVertices;
            PreviousVertex = vertex.PreviousVertex;
        }

		public void AddAdjacent(IVertex<T> vertex)
        {
			AdjacentVertices.Add(vertex);
        }


		//Compare and Equality checks
        public int CompareTo(IVertex<T>? other)
        {
			if (ReferenceEquals(this, other))
				return 0;

			if (ReferenceEquals(null, other))
				return 1;

			return Weight.CompareTo(other.Weight);
		}

        public bool Equals(IVertex<T>? other)
        {
			return other != null && Value.Equals(other.Value);
        }

        public override bool Equals(object? obj)
        {
			var vertex = (IVertex<T>)obj;
			return vertex != null && Value.Equals(vertex.Value);
        }


        //Override GetHashCode to get a unique string to compare simplicity for each item
        public override int GetHashCode()
        {
			return HashCode.Combine(AdjacentVertices, Weight, Value, PreviousVertex);
        }
    }
}

