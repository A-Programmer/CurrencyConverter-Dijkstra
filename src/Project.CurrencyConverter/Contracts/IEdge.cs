using System;
namespace Project.CurrencyConverter.Contracts
{
	public interface IEdge<T, TData> : IEquatable<IEdge<T, TData>>
	{
		public IVertex<T> FromVertex { get; }
		public IVertex<T> ToVertex { get; }
		public int Weight { get; }
		public TData Data { get; set; }
	}
}

