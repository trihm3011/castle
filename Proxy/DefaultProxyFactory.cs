// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Windsor.Proxy
{
	using System;

	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.DynamicProxy;
	using Castle.Model.Interceptor;

	/// <summary>
	/// This implementation of <see cref="IProxyFactory"/> relies 
	/// on DynamicProxy to deliver proxies capabilies.
	/// </summary>
	/// <remarks>
	/// Note that only virtual methods can be intercepted in a 
	/// concrete class. 
	/// </remarks>
	public class DefaultProxyFactory : IProxyFactory
	{
		private ProxyGenerator _generator;

		public DefaultProxyFactory()
		{
			_generator = new ProxyGenerator();
		}

		public object Create(IKernel kernel, ComponentModel model, params object[] constructorArguments)
		{
			IMethodInterceptor[] interceptors = ObtainInterceptors(kernel, model);
			IInterceptor interceptorChain = new InterceptorChain(interceptors);

			// This is a hack to avoid unnecessary object creations
			// We supply our contracts (Interceptor and Invocation)
			// and the implementation for Invocation dispatchers
			// DynamicProxy should be able to use them as long as the 
			// signatures match
			GeneratorContext context = new GeneratorContext();
			context.Interceptor = typeof(IMethodInterceptor);
			context.Invocation = typeof(IMethodInvocation);
			context.SameClassInvocation = typeof(DefaultMethodInvocation);

			return _generator.CreateCustomClassProxy(model.Implementation, 
				interceptorChain, context, constructorArguments);
		}

		protected IMethodInterceptor[] ObtainInterceptors(IKernel kernel, ComponentModel model)
		{
			IMethodInterceptor[] interceptors = new IMethodInterceptor[model.Interceptors.Count];
			int index = 0;

			foreach(InterceptorReference interceptorRef in model.Interceptors)
			{
				IHandler handler = null;

				if (interceptorRef.ReferenceType == InterceptorReferenceType.Interface)
				{
					handler = kernel.GetHandler( interceptorRef.ServiceType );
				}
				else
				{
					handler = kernel.GetHandler( interceptorRef.ComponentKey );
				}

				if (handler == null)
				{
					// This shoul be virtually impossible to happen
					// Seriously!
					throw new ApplicationException("The interceptor could not be resolved");
				}

				try
				{
					IMethodInterceptor interceptor = (IMethodInterceptor) handler.Resolve();
					
					interceptors[index++] = interceptor;
				}
				catch(InvalidCastException)
				{
					String message = String.Format(
						"An interceptor registered for {0} doesnt implement " + 
						"the IMethodInterceptor interface", 
						model.Name);

					throw new ApplicationException(message);
				}
			}

			return interceptors;
		}
	}
}
