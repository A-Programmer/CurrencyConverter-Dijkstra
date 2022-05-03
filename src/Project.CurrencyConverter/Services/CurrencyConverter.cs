using System;
using Project.CurrencyConverter.Contracts;
using Project.CurrencyConverter.Graph;

namespace Project.CurrencyConverter.Services
{
	public sealed class CurrencyConverter : ICurrencyConverter
	{
        private static readonly Lazy<CurrencyConverter> Lazy = new(() => new CurrencyConverter());


        private IEnumerable<Tuple<string, string, double>> _conversionRates;
        private IGraph<string, EdgeData> _graph;
        public static CurrencyConverter _converter => Lazy.Value;

		public CurrencyConverter()
		{
		}

        public void ClearConfiguration()
        {
            _conversionRates = null;
            _graph = null;
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            if (_conversionRates == null)
                throw new Exception("You need to configure before starting the application.");

            if (fromCurrency == toCurrency)
                return amount;

            var result = StraightConvert(fromCurrency, toCurrency, amount);

            if (result >= 0)
                return result;

            return ReverseConvert(fromCurrency, toCurrency, amount);
        }

        private double StraightConvert(string fromCurrency, string toCurrency, double amount)
        {
            var path = _graph.FindShortestPath(fromCurrency, toCurrency)[0];

            if (!path.Any())
                return -1;

            var result = amount;

            if (path.Last().FromVertex.Value != fromCurrency)
                return -1;

            for (var i = path.Count - 1; i >= 0; i--)
            {
                var c = path[i];
                result *= c.Data.ConversionRate;
            }

            return result;
        }

        private double ReverseConvert(string fromCurrency, string toCurrency, double amount)
        {
            var path = _graph.FindShortestPath(toCurrency, fromCurrency)[0];

            if (!path.Any())
                return -1;

            var result = amount;

            if (path.Last().FromVertex.Value != toCurrency)
                return -1;

            for (var i = path.Count - 1; i >= 0; i--)
            {
                var c = path[i];
                result /= c.Data.ConversionRate;
            }

            return result;
        }

        public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
        {
            if (conversionRates == null || !conversionRates.Any())
                throw new Exception("Conversion Rates do'nt have any item.");

            _conversionRates = conversionRates;

            Initialization();
        }

        private void Initialization()
        {
            _graph = new Graph<string, EdgeData>();

            foreach(var (a, b, c) in _conversionRates)
            {
                _graph.AddEdge(a, b, 1, new EdgeData { ConversionRate = c });
                _graph.AddEdge(a, b, 1, new EdgeData { ConversionRate = 1/c });
            }
        }
    }
}

