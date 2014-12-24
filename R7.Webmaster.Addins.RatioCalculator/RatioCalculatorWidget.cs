//
//  RatioCalculatorWidget.cs
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
using System.Collections.Generic;
using Mono.Addins;
using R7.Webmaster.Addins.Root;

namespace R7.Webmaster.Addins.RatioCalculator
{
	[System.ComponentModel.ToolboxItem (true)]
	[Extension(typeof(IWidgetAddin))]
	public partial class RatioCalculatorWidget : Gtk.Bin, IRatioCalculatorView, IWidgetAddin
	{
		private RatioCalculatorModel Model;

		public RatioCalculatorWidget ()
		{
			this.Build ();

			Model = new RatioCalculatorModel ();

			OnScaleToleranceValueChanged(scaleTolerance, null);
		}

		#region IWidgetAddin implementation

		public Gtk.Widget Instance { get { return this; } }

		public Gtk.Widget FocusWidget { get { return spinWidth; } }

		public string Label { get { return "Ratio Calculator"; } }

		public string SafeName { get { return "ratiocalculator"; } }

		public bool IsActive { get; set; }

		public string Icon 
		{ 
			get { return Gtk.Stock.OrientationPortrait; } 
		}

		public List<Gtk.Action> Actions 
		{
			get 
			{
				return new List<Gtk.Action> () { actionLockFactor, actionRotate };
			}
		}

		public List<Gtk.ToolItem> ToolItems
		{
			get { return new List<Gtk.ToolItem> (); }
		}

		#endregion



		protected void OnSpinWidthValueChanged (object sender, EventArgs e)
		{
			if (actionLockFactor.Active)
			{
				// f = w/h
				// w = f*h
				// h = w/f
				if (sender == spinWidth)
				{
					var factor = Model.Factor;
					Model.Width = spinWidth.Value;
					Model.Height = Model.Width / factor;

					spinHeight.Value = Model.Height;
				}
				else if (sender == spinHeight)
				{
					var factor = Model.Factor;
					Model.Height = spinHeight.Value;
					Model.Width = Model.Height * factor;

					spinWidth.Value = Model.Width;
				}
			}
			else
			{
				CalcFactorAndRatio ();
			}
		}

		void CalcFactorAndRatio ()
		{

			Model.CalcFactorAndRatio (spinWidth.Value, spinHeight.Value, scaleTolerance.Value, scaleTolerance.Adjustment.Upper);
			labelFactorValue.LabelProp = string.Format ("{0:G6}", Model.Factor);

			entryRatio.Text = string.Format ("{0}:{1}", Model.Numer, Model.Denom);

		}

		protected void OnSpinHeightValueChanged (object sender, EventArgs e)
		{
			OnSpinWidthValueChanged (sender, e);
		}

		protected void OnButtonRotateClicked (object sender, EventArgs e)
		{

		}

		protected void OnButtonDivideClicked (object sender, EventArgs e)
		{
			OnButtonMultiplyClicked (sender, e);
		}

		protected void OnButtonMultiplyClicked (object sender, EventArgs e)
		{
			var mult = spinMultiplier.Value;
			if (sender == buttonDivide)
				mult = 1 / mult;

			Model.Width *= mult;
			Model.Height *= mult;

			spinWidth.ValueChanged -= OnSpinWidthValueChanged;
			spinHeight.ValueChanged -= OnSpinWidthValueChanged;

			spinWidth.Value = Model.Width;
			spinHeight.Value = Model.Height;

			spinWidth.ValueChanged += OnSpinWidthValueChanged;
			spinHeight.ValueChanged += OnSpinWidthValueChanged;

			CalcFactorAndRatio ();
		}

		protected void OnButtonReverseRatioClicked (object sender, EventArgs e)
		{
			Model.ReverseRatio();

			spinWidth.ValueChanged -= OnSpinWidthValueChanged;
			spinHeight.ValueChanged -= OnSpinWidthValueChanged;

			spinWidth.Value = Model.Width;
			spinHeight.Value = Model.Height;

			spinWidth.ValueChanged += OnSpinWidthValueChanged;
			spinHeight.ValueChanged += OnSpinWidthValueChanged;

			CalcFactorAndRatio ();
		}

		protected void OnScaleToleranceFormatValue (object o, Gtk.FormatValueArgs args)
		{
			// actual formatting done in OnScaleToleranceValueChanged 
			args.RetVal = string.Empty;
		}

		protected void OnScaleToleranceValueChanged (object sender, EventArgs e)
		{
			CalcFactorAndRatio ();

			// draw scale value on separate label
			var scale = (sender as Gtk.HScale);

			if ((int)scale.Value == (int)scale.Adjustment.Upper)
				labelPrecisionValue.LabelProp = "1 / \u221E";
			else
				labelPrecisionValue.LabelProp = 
					string.Format("{0}", Math.Pow (10, -scale.Value));
		}

		protected void OnActionRotateActivated (object sender, EventArgs e)
		{
			Model.Rotate();

			spinWidth.ValueChanged -= OnSpinWidthValueChanged;
			spinHeight.ValueChanged -= OnSpinWidthValueChanged;

			spinWidth.Value = Model.Width;
			spinHeight.Value = Model.Height;

			spinWidth.ValueChanged += OnSpinWidthValueChanged;
			spinHeight.ValueChanged += OnSpinWidthValueChanged;

			CalcFactorAndRatio ();
		}

	}
}

