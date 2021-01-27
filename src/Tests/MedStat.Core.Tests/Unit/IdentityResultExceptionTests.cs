using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

using FluentAssertions;
using Xunit;

using MedStat.Core.Identity;

namespace MedStat.Core.Tests.Unit
{
	public class IdentityResultExceptionTests
	{
		[Theory]
		[MemberData(nameof(CheckConstructor_Data))]
		public void CheckConstructor(IdentityResult identityResult, string expectedInnerExMessage, 
			string expectedMainMessage)
		{
			// Act
			var exception = new IdentityResultException(identityResult, expectedMainMessage);
			

			// Assert:

			if (expectedMainMessage != null)
			{
				exception.Message.Should().BeEquivalentTo(expectedMainMessage);
			}

			if (expectedInnerExMessage != null)
			{
				exception.InnerException.Should().NotBeNull();
				exception.InnerException.Message.Should().BeEquivalentTo(expectedInnerExMessage);
			}
		}

		[Fact]
		public void CheckConstructor_Exceptions()
		{
			// Act + Assert:
			
			Assert.Throws<ArgumentNullException>(() => new IdentityResultException(null, "some message"));

			Assert.Throws<OperationCanceledException>(() => 
				new IdentityResultException(IdentityResult.Success, "some message"));
		}


		// Data for Test methods:

		public static IEnumerable<object[]> CheckConstructor_Data
		{
			get
			{
				// identityResult, expectedInnerExMessage, expectedMainMessage
				return
					new List<object[]>
					{
						new object[] { IdentityResult.Failed(), null, null  },

						// Check that we use identity error for create inner exception
						new object[] 
						{ 
							IdentityResult.Failed(new IdentityError{ Code = "code 1", Description = "Err description 1"}), 
							"Err description 1",
							"Main message 1"
						},

						// Check that we use only first error for create inner exception
						new object[]
						{
							IdentityResult.Failed(new IdentityError{ Code = "code 1", Description =  "Err description 2_1"},
								new IdentityError{ Code = "code 1", Description =  "Err description 2_2"}),
							"Err description 2_1",
							"Main message 2"
						}
					};
			}
		}
	}
}
