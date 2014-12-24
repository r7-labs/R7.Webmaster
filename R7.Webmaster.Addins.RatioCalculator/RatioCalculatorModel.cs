//
//  RatioCalculatorModel.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2014 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace R7.Webmaster.Addins.RatioCalculator
{
	public class RatioCalculatorModel
	{
		public RatioCalculatorModel ()
		{
		}

		public double Factor
		{
			get { return Width / Height; }
		}

		public double Width { get; set; }

		public double Height { get; set; }

		public double Numer { get; private set; }

		public double Denom { get; private set; }

		private int WidthAsInt { get { return (int)Math.Round (Width); } }
		private int HeightAsInt { get { return (int)Math.Round (Height); } }

		public void CalcFactorAndRatio (double width, double height, double tolerance, double maxTolerance)
		{
			Width = width;
			Height = height;

			int numer, denom;

			if ((int)tolerance == (int)maxTolerance)
				AssumeRatio (out numer, out denom);
			else
				AssumeRatio (out numer, out denom, (int)tolerance);

			Numer = numer;
			Denom = denom;
		}

		public void AssumeRatio (out int numerator, out int denominator, int tolpower = -1)
		{
			numerator = WidthAsInt;
			denominator = HeightAsInt;

			if (tolpower == -1)
			{
				var gcd = GCD (WidthAsInt, HeightAsInt);
				numerator = WidthAsInt / gcd;
				denominator = HeightAsInt / gcd;
			}
			else
			{

				var eps = Math.Pow (10, -tolpower);

				for (var i = 0; i < WidthAsInt; i++)
					for (var j = 0; j < HeightAsInt; j++)
					{
						var factor = (double)i / j;
						if (Math.Abs (Factor - factor) < eps)
						{
							numerator = i;
							denominator = j;
							return;
						}
					}

			}
		}

		public int GCD (int a, int b)
		{
			if (a == 0)
				return b;
			if (b == 0)
				return a;
			if (a == b)
				return a;

			if (a == 1 || b == 1) return 1;
			if ((a % 2 == 0) && (b % 2 == 0))
				return 2 * GCD(a / 2, b / 2);
			if ((a % 2 == 0) && (b % 2 != 0))
				return GCD(a / 2, b);
			if ((a % 2 != 0) && (b % 2 == 0))
				return GCD(a, b / 2);
			return GCD(b, Math.Abs(a - b));
		}


		public void Rotate ()
		{	
			var t = Width;
			Width = Height;
			Height = t;
		}

		public void ReverseRatio()
		{
			var revfactor = 1 / Factor;
			Height = Width / revfactor;
		}
	}
}

