using System;
using Unidux;

namespace Pommel.Middlewares
{
    public sealed class Logger
    {
        public Func<Func<object, object>, Func<object, object>> Middleware(IStoreObject storeObject) =>
            next => action => 
            {
                UnityEngine.Debug.Log($"next: {next}");
                UnityEngine.Debug.Log($"action: {action}");
                var result = next(action);
                UnityEngine.Debug.Log($"result: {result}");
                return result;
            };
    }
}