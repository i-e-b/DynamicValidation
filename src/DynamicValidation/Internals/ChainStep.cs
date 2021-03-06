using System;

namespace DynamicValidation.Internals
{
	class ChainStep
	{
		public string Name { get; set; }
		public int SingleIndex { get; set; }
		public ListAssertion ListAssertionType { get; set; }
		public INamedPredicate FilterPredicate { get; set; }

		public static ChainStep SimpleStep(string name)
		{
			return new ChainStep{
				Name = name,
				ListAssertionType = ListAssertion.Simple,
				FilterPredicate = Anything,
				SingleIndex = 0
			};
		}

		public static ChainStep Complex(string name, string assertionName, int index, INamedPredicate filter)
		{
			return new ChainStep{
				Name = name,
				ListAssertionType = Guess(assertionName),
				FilterPredicate = filter,
				SingleIndex = index
			};
		}

		static ListAssertion Guess(string s)
		{
			switch (s.ToLowerInvariant())
			{
				case ListChecks.Any: return ListAssertion.Any;
				case ListChecks.All: return ListAssertion.All;
				case ListChecks.Single: return ListAssertion.Single;
				case ListChecks.Index: return ListAssertion.Index;
				default: return ListAssertion.Simple;
			}
		}

		public static INamedPredicate Anything { get { return new MatchAnything(); } }
		class MatchAnything : INamedPredicate
		{
			public bool Matches(object actual, out string message)
			{
				message = "";
				return true;
			}
		}
	}

}