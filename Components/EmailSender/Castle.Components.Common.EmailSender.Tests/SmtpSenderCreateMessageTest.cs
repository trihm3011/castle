﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Common.EmailSender.Tests
{
	using System;
	using System.IO;
	using System.Net.Mail;
	using System.Text;
	using NUnit.Framework;
	using Smtp;

	[TestFixture]
	public class SmtpSenderCreateMessageTest
	{
		private SmtpSender _smtpSender;
		private readonly string _filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachment.txt");

		[SetUp]
		public void TestSetup()
		{
			_smtpSender = new SmtpSender("localhost");
			File.WriteAllText(_filename, "Test Data");
		}

		[Test]
		public void Ensure_CreateMessage_assigns_correct_mime_type_to_attachments_when_supplied_with_data_stream()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("Attachment.txt", "text/plain", GetTestData()));

			MailMessage mailMsg = _smtpSender.CreateMailMessage(msg);
			Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");

			Assert.AreEqual("text/plain", mailMsg.Attachments[0].ContentType.MediaType);
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_filename_to_attachments_when_supplied_with_data_stream()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("Attachment.txt", "text/plain", GetTestData()));

			MailMessage mailMsg = _smtpSender.CreateMailMessage(msg);
			Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");

			Assert.AreEqual("Attachment.txt", mailMsg.Attachments[0].Name);
		}

		[Test]
		public void Ensure_CreateMessage_assigns_correct_mime_type_to_attachments_when_supplied_with_filename()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("text/plain", _filename));

			using(MailMessage mailMsg = _smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual("text/plain", mailMsg.Attachments[0].ContentType.MediaType);
			}
		}

		[Test]
		public void Ensure_CreateMessage_supplies_correct_filename_to_attachments_when_supplied_with_filename()
		{
			Message msg = new Message("from@somewhere.com", "to@somewhere.com", "subject", "body");
			msg.Attachments.Add(new MessageAttachment("text/plain", _filename));

			using(MailMessage mailMsg = _smtpSender.CreateMailMessage(msg))
			{
				Assert.AreEqual(1, mailMsg.Attachments.Count, "Attachment count doesn't match");
				Assert.AreEqual("Attachment.txt", mailMsg.Attachments[0].Name);
			}
		}

		private Stream GetTestData()
		{
			byte[] b = Encoding.ASCII.GetBytes("Test Data");
			return new MemoryStream(b);
		}
	}
}