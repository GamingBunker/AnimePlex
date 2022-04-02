using System.Collections.Generic;

namespace Cesxhin.AnimeSaturn.Application.Generic
{
    public static class ConvertGeneric<T> where T : class
    {
        public static List<T> ConvertIEnurableToListCollection(IEnumerable<T> list)
        {
            List<T> result = new List<T>();
            foreach (T item in list)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
