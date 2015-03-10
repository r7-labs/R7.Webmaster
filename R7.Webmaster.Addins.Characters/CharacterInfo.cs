//
//  CharacterInfo.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2015 Roman M. Yagodin
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace R7.Webmaster.Addins.Characters
{
	public class CharacterInfo
	{
		#region Fields

		private string label;

		private string entity;

		#endregion

		[XmlAttribute]
		public string Label
		{
			get { return !string.IsNullOrWhiteSpace (label)? label : ((char) Code).ToString (); }
			set { label = value; }
		}

		[XmlAttribute]
		public int Code { get; set; }

		[XmlAttribute]
		public string Entity
		{
			get { return !string.IsNullOrWhiteSpace (entity) ? entity : "&#" + Code + ";"; }
			set { entity = value; }
		}

        [XmlAttribute]
        public string Categories { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

		/*
		#region IXmlSerializable implementation

		public XmlSchema GetSchema ()
		{
			return null;
		}

		public void ReadXml (XmlReader reader)
		{
			reader.MoveToContent();
			var isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement ();

			if (!isEmptyElement)
			{
				Character = (char) int.Parse (reader.ReadElementString ("character"));
				label = reader.ReadElementString ("label");
				xmlEntity = reader.ReadElementString ("xmlEntity");

				// consume end element
				reader.ReadEndElement ();
			}
		}
			
		public void WriteXml (XmlWriter writer)
		{
			writer.WriteElementString ("character", ((int)Character).ToString ());

			// if (!string.IsNullOrWhiteSpace (label))
				writer.WriteElementString ("label", label);

			// if (!string.IsNullOrWhiteSpace (xmlEntity))
				writer.WriteElementString ("xmlEntity", xmlEntity);
		}

		#endregion
		*/
	}
}

