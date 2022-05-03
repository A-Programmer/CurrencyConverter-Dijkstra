using System;
using Project.CurrencyConverter.Contracts;

namespace Project.CurrencyConverter.Graph
{
	public class Edge<T, TData> : IEdge<T, TData>
    {
        public IVertex<T> FromVertex { get; }
        public IVertex<T> ToVertex { get; }
        public int Weight { get; }
        public TData Data { get; set; }


        public Edge(IVertex<T> fromVertex, IVertex<T> toVertex, int weight = 0, TData data = default)
        {
            FromVertex = fromVertex;
            ToVertex = toVertex;
            Weight = weight;
            Data = data;
        }


        public override bool Equals(object obj)
        {
            var edge = (IEdge<T, TData>)obj;
            return edge != null && FromVertex.Equals(edge.FromVertex) && ToVertex.Equals(edge.ToVertex);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FromVertex, ToVertex, Weight);
        }

        public bool Equals(IEdge<T, TData> other)
        {
            return other != null && FromVertex.Equals(other.FromVertex) && ToVertex.Equals(other.ToVertex);
        }
    }
}