using System;
namespace Project.CurrencyConverter.Contracts
{
	public interface IGraph<T, TEdge>
	{
		public void AddEdge(T fromVertex, T toVertex, int weight = 0, TEdge data = default);
		public List<IEdge<T, TEdge>>[] FindShortestPath(T fromVertex, T toVertex);
	}
}

