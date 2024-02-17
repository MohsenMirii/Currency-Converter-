using Mapster;

namespace Share.Helpers;

public static class MapsterHelpers
{
	public static List<T> AdaptToList<T>(this IEnumerable<object> enumerable)=>enumerable.Adapt<List<T>>();
}