using System;
using System.Collections.Generic;

namespace Rocket.API.Providers
{
    public class ProxyBase<T> : ProviderBase
    {
        public void InvokeAll(Action<T> action)
        {
            var providers = R.Providers.GetProviders<T>();
            foreach (var provider in providers)
                action.Invoke(provider);
        }

        public List<TResult> InvokeAll<TResult>(Func<T,TResult> action)
        {
            var providers = R.Providers.GetProviders<T>();
            List<TResult> results = new List<TResult>();
            foreach (var provider in providers)
                results.Add(action.Invoke(provider));
            return results;
        }

        protected override void OnLoad(IProviderManager providerManager)
        {
            InvokeAll(provider => { if (provider is ProviderBase) { ((ProviderBase)(object)provider).Load(providerManager); } });
        }

        protected override void OnUnload()
        {
            InvokeAll(provider => { if (provider is ProviderBase) { ((ProviderBase)(object)provider).Unload(); } });
        }

    }
}