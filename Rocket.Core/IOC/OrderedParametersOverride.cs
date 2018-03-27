using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core.IOC
{
    public class OrderedParametersOverride : ResolverOverride
    {
        private readonly Queue<InjectionParameterValue> parameterValues;

        public OrderedParametersOverride(IEnumerable<object> parameterValues)
        {
            this.parameterValues = new Queue<InjectionParameterValue>();
            foreach (var parameterValue in parameterValues)
            {
                this.parameterValues.Enqueue(InjectionParameterValue.ToParameter(parameterValue));
            }
        }

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if (parameterValues.Count < 1)
                return null;

            var value = this.parameterValues.Dequeue();
            return value.GetResolverPolicy(dependencyType);
        }
    }
}
