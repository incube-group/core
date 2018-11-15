using System;
using System.Reflection.Emit;

namespace InCube.Core.Test.Functional
{

    internal static class TypeSize<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly int Size;

        static TypeSize()
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), Array.Empty<Type>());
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            Size = (int)dm.Invoke(null, null);
        }
    }
}