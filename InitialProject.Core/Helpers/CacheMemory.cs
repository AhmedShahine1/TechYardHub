using System.Runtime.Serialization;

namespace TechYardHub.Core.Helpers
{
    public enum CacheMemory
    {
        [EnumMember(Value = "Product")]
        Product,

        [EnumMember(Value = "Category")]
        Category
    }
}
