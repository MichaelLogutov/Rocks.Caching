namespace Rocks.Caching
{
	public enum CachePriority
	{
		Low = 1,
		BelowNormal = 2,
		Normal = 3,
		AboveNormal = 4,
		High = 5,
		NotRemovable = 6,

		Default = Normal
	}
}
