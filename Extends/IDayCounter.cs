using Bumblebee;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Extends
{
	public interface IDayCounter
	{
		string Name { get; }

		string Path { get; }

		Gateway Gateway { get; }
	}
}
