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

namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;
	using Descriptors;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class AsyncControllerTestCase
	{
		private MockEngineContext engineContext;
		private ViewEngineManagerStub viewEngStub;
		private MockServices services;
		private MockResponse response;

		[SetUp]
		public void Init()
		{
			MockRequest request = new MockRequest();
			response = new MockResponse();
			services = new MockServices();
			viewEngStub = new ViewEngineManagerStub();
			services.ViewEngineManager = viewEngStub;
			engineContext = new MockEngineContext(request, response, services, null);
		}

		[Test]
		public void AsyncMethodPairAppearAsSingleAction()
		{
			IController controller = new ControllerWithAsyncAction();
			ControllerMetaDescriptor descriptor = services.ControllerDescriptorProvider.BuildDescriptor(controller);
			Assert.AreEqual(1, descriptor.Actions.Count);
			Assert.AreEqual("Index", descriptor.Actions["Index"].ToString());
		}

		[Test]
		[ExpectedException(typeof(MonoRailException),
			"Action 'Index' on controller 'ControllerWithTwoBeginIndex' is an async action, but there are method overloads 'BeginIndex(...)', which is not allowed on async actions."
			)]
		public void OverloadingOfAsyncActionsIsNotAllowed()
		{
			IController controller = new ControllerWithTwoBeginIndex();
			services.ControllerDescriptorProvider.BuildDescriptor(controller);
		}

		[Test]
		[ExpectedException(typeof(MonoRailException),
			"Found both async method 'BeginIndex' and sync method 'Index' on controller 'ControllerWithBeginIndexAndIndex'. MonoRail doesn't support mixing sync and async methods for the same action"
			)]
		public void MixingAsyncAndSyncMethodNotAllowed()
		{
			IController controller = new ControllerWithBeginIndexAndIndex();
			services.ControllerDescriptorProvider.BuildDescriptor(controller);
		}

		[Test]
		[ExpectedException(typeof(MonoRailException),
			"Found begining of async pair 'BeginIndex' but not the end 'EndIndex' on controller 'ControllerWithBeginIndexWithoutEndIndex', did you forget to define EndIndex(IAsyncResult ar) ?"
			)]
		public void BeginActionWitoutEndActionNotAllowed()
		{
			IController controller = new ControllerWithBeginIndexWithoutEndIndex();
			services.ControllerDescriptorProvider.BuildDescriptor(controller);
		}

		[Test]
		[ExpectedException(typeof(MonoRailException),
			"Found more than a single EndIndex method, for async action 'Index' on controller 'ControllerWithTwoEndActions', only a single EndIndex may be defined as part of an async action"
			)]
		public void OverloadingForEndActionIsNotAllowed()
		{
			IController controller = new ControllerWithTwoEndActions();
			services.ControllerDescriptorProvider.BuildDescriptor(controller);
		}

		[Test]
		public void BeginActionThrowsException()
		{
			IController controller = new ControllerWithAsyncActionThrowOnBegin();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "ControllerWithAsyncAction", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));
			bool exceptionCaught = false;
			services.ExtensionManager.ActionException += delegate { exceptionCaught = true; };

			try
			{
				controller.BeginProcess(engineContext, context);
				Assert.Fail("Expected exception");
			}
			catch(Exception)
			{
			}

			Assert.IsTrue(exceptionCaught);
		}

		[Test]
		public void EndActionThrowsException()
		{
			IController controller = new ControllerWithAsyncActionThrowOnEnd();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "ControllerWithAsyncAction", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));
			bool exceptionCaught = false;
			services.ExtensionManager.ActionException += delegate { exceptionCaught = true; };

			try
			{
				IAsyncResult ar = controller.BeginProcess(engineContext, context);
				context.AsyncInvocationInformation.AsyncResult = ar;
				ar.AsyncWaitHandle.WaitOne();
				controller.EndProcess();
				Assert.Fail("Expected exception");
			}
			catch(Exception)
			{
			}

			Assert.IsTrue(exceptionCaught);
		}


		[Test]
		public void AsyncActionThrowsException()
		{
			IController controller = new ControllerWithAsyncActionThrowOnAsync();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "ControllerWithAsyncAction", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));
			bool exceptionCaught = false;
			services.ExtensionManager.ActionException += delegate { exceptionCaught = true; };

			try
			{
				IAsyncResult ar = controller.BeginProcess(engineContext, context);
				context.AsyncInvocationInformation.AsyncResult = ar;
				ar.AsyncWaitHandle.WaitOne();
				controller.EndProcess();
				Assert.Fail("Expected exception");
			}
			catch(Exception)
			{
			}

			Assert.IsTrue(exceptionCaught);
		}


		[Test]
		public void CanExecuteActionAsyncronously()
		{
			IController controller = new ControllerWithAsyncAction();

			IControllerContext context = services.ControllerContextFactory.
				Create("", "ControllerWithAsyncAction", "index", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			IAsyncResult ar = controller.BeginProcess(engineContext, context);
			context.AsyncInvocationInformation.AsyncResult = ar;
			ar.AsyncWaitHandle.WaitOne();
			controller.EndProcess();

			Assert.AreEqual("foo", response.OutputContent);
		}
	}
}