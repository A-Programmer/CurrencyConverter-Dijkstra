using System;
using System.Collections.Generic;

namespace Project.CurrencyConverter.Models
{
	public interface IVertex<T> : IComparable<IVertex<T>>, IEquatable<IVertex<T>>
	{
		//All Neighbors to select for shortest one
		public List<IVertex<T>> Neighbors { get; }
		public int Weight { get; set; }
		public T Value { get; }
        public IVertex<T> PreviousVertex { get; set; }
        public object Data { get; set; }
		public void AddNeighbor(IVertex<T> neighbor);
    }
}

