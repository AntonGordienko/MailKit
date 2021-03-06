//
// ImapCommandException.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2013-2015 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
#if SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace MailKit.Net.Imap {
	/// <summary>
	/// An exception that is thrown when an IMAP command returns NO or BAD.
	/// </summary>
	/// <remarks>
	/// The exception that is thrown when an IMAP command fails. Unlike a <see cref="ImapProtocolException"/>,
	/// a <see cref="ImapCommandException"/> does not require the <see cref="ImapClient"/> to be reconnected.
	/// </remarks>
#if SERIALIZABLE
	[Serializable]
#endif
	public class ImapCommandException : CommandException
	{
#if SERIALIZABLE
		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Imap.ImapCommandException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ImapCommandException"/> from the serialized data.
		/// </remarks>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		protected ImapCommandException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
			var value = info.GetString ("Response");
			ImapCommandResponse response;

			Enum.TryParse (value, out response);

			Response = response;
		}
#endif

		/// <summary>
		/// Create a new <see cref="ImapCommandException"/> based on the specified command name and <see cref="ImapCommand"/> state.
		/// </summary>
		/// <remarks>
		/// Create a new <see cref="ImapCommandException"/> based on the specified command name and <see cref="ImapCommand"/> state.
		/// </remarks>
		/// <returns>A new command exception.</returns>
		/// <param name="command">The command name.</param>
		/// <param name="ic">The command state.</param>
		internal static ImapCommandException Create (string command, ImapCommand ic)
		{
			var result = ic.Response.ToString ().ToUpperInvariant ();
			string message, reason = null;

			if (string.IsNullOrEmpty (ic.ResponseText)) {
				for (int i = 0; i < ic.RespCodes.Count; i++) {
					if (ic.RespCodes[i].IsError) {
						reason = ic.RespCodes[i].Message;
						break;
					}
				}
			} else {
				reason = ic.ResponseText;
			}

			if (!string.IsNullOrEmpty (reason))
				message = string.Format ("The IMAP server replied to the '{0}' command with a '{1}' response: {2}", command, result, reason);
			else
				message = string.Format ("The IMAP server replied to the '{0}' command with a '{1}' response.", command, result);

			return ic.Exception != null ? new ImapCommandException (ic.Response, message, ic.Exception) : new ImapCommandException (ic.Response, message);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Imap.ImapCommandException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ImapCommandException"/>.
		/// </remarks>
		/// <param name="response">The IMAP command response.</param>
		/// <param name="message">The error message.</param>
		/// <param name="innerException">The inner exception.</param>
		public ImapCommandException (ImapCommandResponse response, string message, Exception innerException) : base (message, innerException)
		{
			Response = response;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Imap.ImapCommandException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ImapCommandException"/>.
		/// </remarks>
		/// <param name="response">The IMAP command response.</param>
		/// <param name="message">The error message.</param>
		public ImapCommandException (ImapCommandResponse response, string message) : base (message)
		{
			Response = response;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Imap.ImapCommandException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ImapCommandException"/>.
		/// </remarks>
		/// <param name="response">The IMAP command response.</param>
		public ImapCommandException (ImapCommandResponse response)
		{
			Response = response;
		}

		/// <summary>
		/// Gets the IMAP command response.
		/// </summary>
		/// <remarks>
		/// Gets the IMAP command response.
		/// </remarks>
		/// <value>The IMAP command response.</value>
		public ImapCommandResponse Response {
			get; private set;
		}

#if SERIALIZABLE
		/// <summary>
		/// When overridden in a derived class, sets the <see cref="System.Runtime.Serialization.SerializationInfo"/>
		/// with information about the exception.
		/// </summary>
		/// <remarks>
		/// Serializes the state of the <see cref="FolderNotFoundException"/>.
		/// </remarks>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="info"/> is <c>null</c>.
		/// </exception>
		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException ("info");

			info.AddValue ("Response", Response);

			base.GetObjectData (info, context);
		}
#endif
	}
}
