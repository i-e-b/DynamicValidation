using System.Linq;
using DynamicValidation.Internals;

namespace DynamicValidation.Reflection
{
	internal static class TypeExtensions
	{
		public static int CountDefinitions(this object target, string memberName)
		{
			if (target == null) return 0;
			var type = target.GetType();

			var fieldNames = type.GetFields().Select(f=>f.Name);
			var propertyNames = type.GetProperties().Select(p=>p.Name);

			return fieldNames.Count(name=>name == memberName)
			       + propertyNames.Count(name=>name == memberName);
		}

		public static object GetSafe(this object target, string memberName)
		{
			try
			{
				return target.Get(memberName);
			}
			catch (FastFailureException)
			{
				return null;
			}
		}

		public static object Get(this object target, string memberName)
		{
			var field = target.GetType().GetField(memberName);
			if (field == null)
			{
				var prop = target.GetType().GetProperty(memberName);
				if (prop == null) throw new FastFailureException();

				try
				{
					return prop.GetValue(target,null);
				} catch
				{
					return null;
				}
			}

			return field.GetValue(target);
		}
	}
}