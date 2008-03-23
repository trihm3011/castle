// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;

	/// <summary>
	/// Implements <see cref="IHttpAsyncHandler"/> to dispatch the web
	/// requests in async manner
	/// <seealso cref="MonoRailHttpHandlerFactory"/>
	/// </summary>
	public class BaseAsyncHttpHandler : BaseHttpHandler, IHttpAsyncHandler
	{
		private HttpContext httpContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailHttpHandler"/> class.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <param name="sessionLess">Have session?</param>
		public BaseAsyncHttpHandler(IEngineContext engineContext, IController controller, IControllerContext context,
		                            bool sessionLess)
			: base(engineContext, controller, context, sessionLess)
		{
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			this.httpContext = context;
			BeforeControllerProcess(context);

			try
			{
				controllerContext.AsyncInvocationInformation.AsyncCallback = cb;
				controllerContext.AsyncInvocationInformation.State = extraData;

				engineContext.Services.ExtensionManager.RaisePreProcessController(engineContext);

				return controller.BeginProcess(engineContext, controllerContext);
			}
			catch(Exception ex)
			{
				HttpResponse response = context.Response;

				if (response.StatusCode == 200)
				{
					response.StatusCode = 500;
				}

				engineContext.LastException = ex;

				engineContext.Services.ExtensionManager.RaiseUnhandledError(engineContext);

				AfterCotrollerProcess();

				throw new MonoRailException("Error processing MonoRail request. Action " +
				                            controllerContext.Action + " on controller " + controllerContext.Name, ex);
			}
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			try
			{
				controllerContext.AsyncInvocationInformation.AsyncResult = result;
				// if we failed on the Begin[Action] and had a rescue take care of rendering the output
				// we won't be executing the End[Action] part
				if (result is FailedToExecuteBeginActionAsyncResult == false)
				{
					controller.EndProcess();
				}

				engineContext.Services.ExtensionManager.RaisePostProcessController(engineContext);
			}
			catch(Exception ex)
			{
				HttpResponse response = httpContext.Response;

				if (response.StatusCode == 200)
				{
					response.StatusCode = 500;
				}

				engineContext.LastException = ex;

				engineContext.Services.ExtensionManager.RaiseUnhandledError(engineContext);

				throw new MonoRailException("Error processing MonoRail request. Action " +
				                            controllerContext.Action + " on controller " + controllerContext.Name, ex);
			}
			finally
			{
				AfterCotrollerProcess();
			}
		}
	}
}