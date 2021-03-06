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

namespace Castle.MicroKernel.Tests
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Security.Policy;
	using Castle.Core;
	using Castle.MicroKernel.Tests.ClassComponents;
	using NUnit.Framework;

	[TestFixture]
	public class SerializationTestCase
	{
		[Test]
		[Ignore("For some reason the deserialization of kernel members is resulting in null values")]
		public void KernelSerialization()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddComponent("key", typeof(CustomerImpl));
			Assert.IsTrue(kernel.HasComponent("key"));

			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.Serialize(stream, kernel);

			stream.Position = 0;

			IKernel desKernel = (IKernel) formatter.Deserialize(stream);
			Assert.IsTrue(desKernel.HasComponent("key"));
		}

		[Test]
		[Ignore("To compile on Mono")]
		public void RemoteAccess()
		{
			AppDomain current = AppDomain.CurrentDomain;

			AppDomain otherDomain = AppDomain.CreateDomain(
				"other", new Evidence(current.Evidence), current.SetupInformation);

			try
			{
				IKernel kernel = (IKernel)
				                 otherDomain.CreateInstanceAndUnwrap(
				                 	"Castle.MicroKernel", "Castle.MicroKernel.DefaultKernel");

				kernel.AddComponent("key", typeof(CustomerImpl));
				Assert.IsTrue(kernel.HasComponent("key"));
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				AppDomain.Unload(otherDomain);
			}
		}

		[Test]
#if MONO2
		[Ignore(@"problem running on mono 2.0 profile")]
#endif
		public void RemoteAccessToComponentGraph()
		{
			AppDomain current = AppDomain.CurrentDomain;

			AppDomain otherDomain = AppDomain.CreateDomain(
				"other", new Evidence(current.Evidence), current.SetupInformation);

			try
			{
				IKernel kernel = (IKernel)
				                 otherDomain.CreateInstanceAndUnwrap(
				                 	"Castle.MicroKernel", "Castle.MicroKernel.DefaultKernel");

				kernel.AddComponent("key", typeof(CustomerImpl));
				Assert.IsTrue(kernel.HasComponent("key"));

				GraphNode[] nodes = kernel.GraphNodes;

				Assert.IsNotNull(nodes);
				Assert.AreEqual(1, nodes.Length);
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				AppDomain.Unload(otherDomain);
			}
		}
	}
}