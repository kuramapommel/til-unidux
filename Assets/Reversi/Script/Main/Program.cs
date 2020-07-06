using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using Pommel.Generated.Resolvers;
using UnityEngine;

namespace Pommel
{
    public static class Program
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            var resolver = CompositeResolver.Create
            (
                UnityBlitResolver.Instance,
                UnityResolver.Instance,
                MagicOnionResolver.Instance,
                MessagePackGeneratedResolver.Instance,
                BuiltinResolver.Instance,
                PrimitiveObjectResolver.Instance,

                // finally use standard (default) resolver
                StandardResolver.Instance
            );

            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            // Pass options every time or set as default
            MessagePackSerializer.DefaultOptions = options;
        }
    }
}