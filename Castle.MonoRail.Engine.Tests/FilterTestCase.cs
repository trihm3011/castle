// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Engine.Tests
{
	using System;
	using System.Net;

	using NUnit.Framework;
	
	using Castle.MonoRail.TestSupport;


	[TestFixture]
	public class FilterTestCase : AbstractMRTestCase
	{
		[Test]
		public void SkipFilter()
		{
			DoGet("filtered/index.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "Filtered Action Index" );
		}

		[Test]
		public void FilteredActionWithRenderText()
		{
			DoGet("filtered/save.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "(before)content(after)" );
		}

		[Test]
		public void FilteredActionWithView()
		{
			DoGet("filtered/update.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "(before)(after)update view contents" );
		}

		[Test]
		public void SelectiveSkipFilter()
		{
			DoGet("filtered/selectiveSkip.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "selectiveSkip view contents" );
		}

		[Test]
		public void BeforeFilter_SkipFilter()
		{
			DoGet("filtered2/index.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "index view contents" );
		}

		[Test]
		public void BeforeFilter_Filtered()
		{
			DoGet("filtered2/update.rails");

			AssertSuccess();

			AssertReplyEqualsTo( "(before)update view contents" );
		}

		[Test]
		public void InvalidCondition()
		{
			Request.Headers.Add("mybadheader", "somevalue");

			DoGet("filter/index.rails");

			AssertSuccess();

			AssertReplyEqualsTo("Denied!");
		}

		[Test]
		public void ValidCondition()
		{
			DoGet("filter/index.rails");

			AssertSuccess();

			AssertReplyEqualsTo("View contents!");
		}
	}
}
