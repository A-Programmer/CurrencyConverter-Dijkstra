using System;
using System.Collections.Generic;

namespace Project.CurrencyConverter.Contracts
{
	public interface IVertex<T> : IComparable<IVertex<T>>, IEquatable<IVertex<T>>
	{
		public List<IVertex<T>> AdjacentVertices { get; }
		public int Weight { get; set; }
		public T Value { get; }
        public IVertex<T> PreviousVertex { get; set; }
        public object Data { get; set; }
		public void AddAdjacent(IVertex<T> neighbor);
    }
}

