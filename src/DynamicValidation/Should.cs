﻿using System;
using System.Collections.Generic;
using System.Linq;
using DynamicValidation.Internals;
using DynamicValidation.Reflection;
using DynamicValidation.SpecialPredicates;

// ReSharper disable PartialTypeWithSinglePart
namespace DynamicValidation
{
	/// <summary>
	/// Some common assertions for use in dynamic validation
	/// </summary>
	public partial class Should
	{
		/// <summary>
		/// Checks that the enumerable has exactly the given number of children
		/// </summary>
		public static INamedPredicate Have(int n)
		{
			return new NamedPredicate(
						o => ((IEnumerable<object>)o).Count() == n,
						o => (!((IEnumerable<object>)o).Any()) ? "has no items" : ("should have " + n + " items exactly")
						);
		}

		public static INamedPredicate HaveAtLeast(int n)
		{
			return new NamedPredicate(
						o => ((IEnumerable<object>)o).Count() >= n,
						o => "should have at least " + n + " items"
						);
		}

		public static INamedPredicate HaveLessThan(int n)
		{
			return new NamedPredicate(
						o => ((IEnumerable<object>)o).Count() < n,
						o => "should have less than " + n + " items"
						);
		}

		public static INamedPredicate Be(Func<object, bool> pred, string message)
		{
			return new NamedPredicate(pred, o => message);
		}
		public static INamedPredicate Be<T>()
		{
			return new NamedPredicate(
				o => o is T,
				o => "expected " + typeof(T).Name + " but got " +
					((o == null) ? "null" : (o.GetType().Name)));
		}

		public static INamedPredicate AllMatch(INamedPredicate constraint)
		{
			return new EnumerablePredicate(constraint);
		}

		public static INamedPredicate AllBe(Func<object, bool> predicate, string message)
		{
			return new NamedPredicate(
						o => ((IEnumerable<object>)o).All(predicate),
						o => message
						);
		}

		public static INamedPredicate HaveNone(Func<object, bool> predicate, string message)
		{
			return new NamedPredicate(
						o => ! ((IEnumerable<object>)o).Any(predicate),
						o => message
						);
		}

		public static INamedPredicate NotBeNull
		{
			get
			{
				return new NamedPredicate(
					  o => o != null,
					  o => "was null"
					  );
			}
		}

		public static object BeTrue
		{
			get
			{
				return new NamedPredicate(
					  o => (o as bool?) == true,
					  o => "expected True but got "+o
					  );
			}
		}

		/// <summary> Allows anything but bool == true </summary>
		public static object NotBeTrue
		{
			get
			{
				return new NamedPredicate(
					  o => (o as bool?) != true,
					  o => "got True where not allowed"
					  );
			}
		}

		public static object BeFalse
		{
			get
			{
				return new NamedPredicate(
					  o => (o as bool?) == false,
					  o => "expected False but got "+o
					  );
			}
		}
		
		/// <summary> Allows anything but bool == false </summary>
		public static object NotBeFalse
		{
			get
			{
				return new NamedPredicate(
					  o => (o as bool?) != false,
					  o => "got True where not allowed"
					  );
			}
		}

		public static object NotBeEmpty
		{
			get
			{
				return new NamedPredicate(
					  o => ! string.IsNullOrEmpty(o as string),
					  o => (o is string) ? "was empty" : "was not a string"
					  );
			}
		}

		public static object BeEmpty
		{
			get
			{
				return new NamedPredicate(
					  o => string.IsNullOrEmpty(o as string),
					  o => (o is string) ? ("was " + o) : "was not a string"
					  );
			}
		}
		
		public static INamedPredicate BeNull
		{
			get
			{
				return new NamedPredicate(
					  o => o == null,
					  o => "was not null"
					  );
			}
		}

		public static INamedPredicate Equal(object aValue)
		{
			return new NamedPredicate(
				o => o.Equals(aValue),
				o => "was not equal to "+aValue
				);
		}

		public static INamedPredicate Contain(string substring)
		{
			return new NamedPredicate(
					  o => ((o as string) != null) && ((string)o).Contains(substring),
					  o => (o is string) ? "did not contain \""+substring+"\"" : "was not a string"
					  );
		}

		public static INamedPredicate EqualOneOf(IEnumerable<object> acceptableValues)
		{
			if (acceptableValues == null) throw new ArgumentException("null values passed to Should.EqualOneOf");
			return new NamedPredicate(
					  acceptableValues.Contains,
					  o => "got \""+o+"\" which is not an acceptable value"
					  );
		}

		/// <summary>
		/// Checks that the type has the named member, and that it's not null
		/// </summary>
		public static INamedPredicate HaveMember(string memberName)
		{
			return new NamedPredicate(
					  o => (o != null) && (o.GetSafe(memberName) != null),
					  o => (o == null) ? "was null" : "did not contain member \"" + memberName + "\""
					  );
		}
		/// <summary>
		/// Checks that the type does not have the named member, or that it's null
		/// </summary>
		public static INamedPredicate NotHaveMember(string memberName)
		{
			return new NamedPredicate(
					  o => (o != null) && (o.GetSafe(memberName) == null),
					  o => (o == null) ? "was null" : "contained unexpected member \"" + memberName + "\""
					  );
		}
	}

	public static class ShouldExtensions
	{
		/// <summary>
		/// Succeeds if either left or right succeeds.
		/// If both fail, merges and flattens messages.
		/// </summary>
		public static INamedPredicate Or(this INamedPredicate left, INamedPredicate right)
		{
			return new NamedPredicate(
				o =>
					{
						string dummy;
						return left.Matches(o, out dummy) || right.Matches(o, out dummy);
					},
				o =>
					{
						string leftMsg, rightMsg;
						left.Matches(o, out leftMsg);
						right.Matches(o, out rightMsg);
						return leftMsg + ", " + rightMsg;
					}
				);
		}
	}
}
