//
//  TextCleanerModel.cs
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
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace R7.Webmaster.Addins.TextCleaner
{
	public class TextCleanerParams
	{
		public bool HtmlIn;
		public bool HtmlOut;
		public bool EmNames;
		public int TableWidth;
		public string TableCSSClass;
		public string TableWidthUnits;
	}

	public class MatchGroup
	{
		public int Index;
		public int Length;
		public string Value;
		public string NewValue;
	}

	public class TextCleanerModel
	{
		public TextCleanerModel ()
		{
		}

		public string TextClean (string text, TextCleanerParams textCleanParams)
		{
			textCleanParams.HtmlIn = IsHtml (text);

			if (!textCleanParams.HtmlIn && textCleanParams.HtmlOut)
			{
				return new TextToHtmlProcessing ().Execute (text, textCleanParams);
			}
			else if (!textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
				return new TextToTextProcessing ().Execute (text, textCleanParams);
			}
			else if (textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
				return new HtmlToTextProcessing ().Execute (text, textCleanParams);
			}

			return text;
		}

		public bool IsHtml (string text)
		{
			string stripped = 
				text.Replace ("</", "")
						.Replace ("/>", "");

			int delta = text.Length - stripped.Length;

			return delta > 0;
		}

		private int CountValidMatches (MatchCollection matches, int nGroup)
		{
			var matchCount = 0;

			foreach (Match match in matches)
				if (match.Success && !string.IsNullOrWhiteSpace (match.Groups [nGroup].Value))
					matchCount++;

			return matchCount;
		}

		private void CopyValidMatches (MatchCollection matches, MatchGroup[] matchArray, int groupnum)
		{
			var matchCount = 0;

			foreach (Match match in matches)
				if (match.Success && !string.IsNullOrWhiteSpace (match.Groups [groupnum].Value))
				{
					matchArray [matchCount] = new MatchGroup ();
					matchArray [matchCount].Index = match.Groups [groupnum].Index;
					matchArray [matchCount].Length = match.Groups [groupnum].Length;
					matchArray [matchCount].Value = match.Groups [groupnum].Value;
					matchCount++;
				}

		}

		/// <summary>
		/// Cleans text in HTML attributes and values. 
		/// Opposite to TextToText(), uses some HTML markup for entities 
		/// </summary>
		/// <returns>
		/// Cleansed text for HTML attrs and values.
		/// </returns>
		/// <param name='text'>
		/// Input string.
		/// </param>
		private string TextToHtmlText (string text, bool inAttr)
		{
			// TODO: Realize TextToHtmlText ()

			text = text.Trim (' ', '\n', '\t', '\r');

			// удаляем пробелы перед "закрывающими" знаками препинания
			text = Regex.Replace (text, @"\s+([\.,;:\)\]\?!])", "${1}");

			// удаляем лишние знаки препинания в скобках 
			text = text.Replace (".).", ".)");

			// replace quotes
			text = text.Replace ("\"", "&quot;");
			text = text.Replace ("«", "&quot;");
			text = text.Replace ("»", "&quot;");
			text = text.Replace ("`", "'"); // &apos;?

			// удаляем "мягкие" переносы
			text = text.Replace ("¬", "");

			// replace &nbsp; with XML &#160;
			text = text.Replace ("&nbsp;", "&#160;");

			// заменяем длинные тире - на &ndash;
			// ставим неразрывный пробел перед дефисом

			text = text.Replace (" - ", "&#160;- ");

			// ставим пробел после дефиса
			text = text.Replace (" -", "&#160;- ");

			// заменяем очень длинное тире просто длинным &ndash;: 
			text = text.Replace ("\x2014", "\x2013");
			text = text.Replace ("&ndash;", "\x2013");
			text = text.Replace ("&mdash;", "\x2013");

			// ставим неразрывный пробел перед дефисом
			text = text.Replace ("\x2013 ", "&#160;\x2013 ");
			text = text.Replace ("\x2013 ", "&#160;\x2013 ");

			text = Regex.Replace (text, @"\s+&#160;", "&#160;");

			// TODO: т.д., т.п., т.е., т.о.

			// Common abbreviations
			text = text.Replace ("г.г.", "гг.");
			text = text.Replace ("с\\х", "с.-х.");
			text = text.Replace ("с/х", "с.-х.");
			text = text.Replace ("с.х.", "с.-х.");

			if (!inAttr)
			{
				// replace urls   
				text = Regex.Replace (text, @"\b((http|https|ftp|ftps)://.*?)([\s\.,:;!\?]\B)", 
					"<a href=\"${1}\">${1}</a>${3}", RegexOptions.IgnoreCase);

				text = Regex.Replace (text, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
					"<a href=\"mailto:$&\">$&</a>", RegexOptions.IgnoreCase);
			}

			// ставим пробелы после знаков препинания

			// TODO: Need entities and url check, else . and ? must be removed from list!
			// text = text.Replace (".", ". ");
			// text = text.Replace ("?", "? ");

			text = text.Replace (",", ", ");
			text = text.Replace ("!", "! ");
			text = text.Replace (";", "; "); 
			text = text.Replace (":", ": ");


			// remove breaks in a decimal numbers: 12, 23 => 12,23
			text = Regex.Replace (text, @"(\d[,\.])\s+(\d)", "${1}${2}");

			// удаляем дублирующиеся пробелы, табуляции, переводы строк,
			// заменяем их на 1 пробел
			text = Regex.Replace (text, @"\s+", " ");

			text = text.Trim ();

			return text;
		}

		private string HtmlToHtml (string text)
		{
			// TODO: Realize HtmlToHtml ()
			return text;
		}

		/// <summary>
		/// Applies the [changed] match groups back to text, base on their original index and length
		/// </summary>
		/// <returns>
		/// Resulting text
		/// </returns>
		/// <param name='text'>
		/// Base text
		/// </param>
		/// <param name='matchGroups'>
		/// Match groups array.
		/// </param>
		private string ApplyMatchGroups (string text, MatchGroup[] matchGroups)
		{
			var offset = 0;
			var newtext = text;		

			foreach (MatchGroup _group in matchGroups)
			{
				newtext = newtext.Remove (_group.Index + offset, _group.Length);
				newtext = newtext.Insert (_group.Index + offset, _group.NewValue);
				offset = newtext.Length - text.Length; 
			}

			return newtext;
		}

		public string Parse (string text) //, TextCleanParams param)
		{
			// get match collections, attrs @title, @alt, @summary going first
			// THINK: More precise and universal regex for attr match
			MatchCollection attrs = Regex.Matches (text, @"(title|alt|summary)=[""'](.*?)[""']", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			// calculate real matches count, without empty and non-successful 
			var attrsCount = CountValidMatches (attrs, 2);

			// make an array of MatchGroups to store new attribute values
			MatchGroup[] attrGroups = new MatchGroup[attrsCount];
			CopyValidMatches (attrs, attrGroups, 2);

			// pass all matched values to cleanup
			foreach (MatchGroup _group in attrGroups)
				_group.NewValue = TextToHtmlText (_group.Value, true);

			// now, we need to apply changes back to original text,
			// before proceed with tags and values
			text = ApplyMatchGroups (text, attrGroups);

			// get tags 
			MatchCollection tags = Regex.Matches (text, "<.*?>", RegexOptions.Singleline);
			var tagsCount = CountValidMatches (tags, 0);

			MatchGroup[] tagGroups = new MatchGroup[tagsCount];
			CopyValidMatches (tags, tagGroups, 0);

			foreach (MatchGroup _group in tagGroups)
				_group.NewValue = HtmlToHtml (_group.Value);

			text = ApplyMatchGroups (text, tagGroups);

			// get values
			MatchCollection values = Regex.Matches (text, ">(.*?)<", RegexOptions.Singleline);
			var valuesCount = CountValidMatches (values, 1);

			MatchGroup[] valueGroups = new MatchGroup[valuesCount];
			CopyValidMatches (values, valueGroups, 1);

			foreach (MatchGroup _group in valueGroups)
				_group.NewValue = TextToHtmlText (_group.Value, false);

			text = ApplyMatchGroups (text, valueGroups);


			// text = HtmlTidy.Process (text);

			return text;
		}


		public string TextCleanOld (string text, bool htmlin, bool htmlout, bool fixRssDate, bool clearTables, bool emNames, TextCleanerParams param)
		{
			// TODO: требуется разработать лексико-синтаксический анализатор,
			// отделяющий разметку HTML и содержимое,
			// которые требуется обрабатывать по разным правилам.
			// Также, нужно рассмотреть вопрос увязки с HTML Tidy.
			// Например, можно прогнать HTML через tidy до и после обработки
			// CSS обрабатывается непосредственно CSS Tidy.

			if (htmlout && !htmlin)
			{
				// Remove MS Explorer / MS Office conditional tags 
				// buffer = Regex.Replace (buffer, @"<!.*?\[.*?\]>", "");

				// remove XML comments
				// THINK: Make removing SGML/XML comments optional?
				// buffer = Regex.Replace (buffer, "<!--.*?-->", "");
			}

			// TODO: такой обработке должен подвергаться только текст, а не разметка!

			// ставим пробелы после знаков препинания
			// buffer = buffer.Replace(".", ". ");
			text = text.Replace (",", ", ");
			text = text.Replace ("!", "! ");
			// buffer = buffer.Replace (";", "; ");

			// могут быть частью url!
			// buffer = buffer.Replace(":", ": ");
			// buffer = buffer.Replace("?", "? ");

			if (htmlout && !htmlin)
			{
				// помещаем весь текст внутри абзаца
				text = "<p>" + text + "</p>";
			}

			// заменяем переводы строк
			text = text.Replace ("\r", "\n");
			//buffer = buffer.Replace("\\n", "\n\r");

			if (htmlout && !htmlin)
			{
				// заменяем переводы строки на параграфы
				text = text.Replace ("\n\n", "</p><p>");
				text = text.Replace ("\n", "</p><p>");
			}

			if (htmlout)
			{
				// удаляем пробелы после и перед параграфами
				text = text.Replace ("<p> ", "<p>");
				text = text.Replace (" </p>", "</p>");
			}

			// удаляем дублирующиеся пробелы, табуляции, переводы строк на пробелы
			text = Regex.Replace (text, @"\s+", " ");

			// удаляем лишние и пустые параграфы
			if (htmlout)
			{
				string buffer_t = String.Empty;
				bool once = true;
				while (buffer_t.Length != text.Length)
				{
					if (once)
						once = false;
					else
						text = buffer_t;

					buffer_t = text.Replace ("</p><p></p><p>", "</p><p>");
				}

				text = text.Replace ("<p></p>", "");
				text = text.Replace ("<p> </p>", "");
				text = text.Replace ("<p>\x00A0</p>", "");
				text = text.Replace ("<p>&#160;</p>", "");
				text = text.Replace ("<p>&nbsp;</p>", "");

				text = text.Replace ("<p><p>", "<p>");
				text = text.Replace ("</p></p>", "</p>");
			}

			// удаляем пробелы перед "закрывающими" знаками препинания
			text = text.Replace (" .", ".");
			text = text.Replace (" ,", ",");
			text = text.Replace (" ;", ";");
			text = text.Replace (" :", ":");
			text = text.Replace (" )", ")");
			text = text.Replace (" ]", "]");
			text = text.Replace (" ?", "?");
			text = text.Replace (" !", "!");

			// удаляем лишние знаки препинания в скобках 
			text = text.Replace (".).", ".)");

			if (htmlout && !htmlin)
			{
				text = text.Replace ("&", "&amp;");

				// заменяем кавычки на &quot;
				text = text.Replace ("\"", "&quot;");
			}

			if (htmlout)
			{
				text = text.Replace ("«", "&quot;");
				text = text.Replace ("»", "&quot;");
				text = text.Replace ("`", "'");
				// &apos;?
			}

			if (!htmlout)
			{
				text = text.Replace ("«", "\"");
				text = text.Replace ("»", "\"");
				text = text.Replace ("`", "'");
			}

			// удаляем "мягкие" переносы
			text = text.Replace ("¬", "");

			if (htmlout)
			{
				// заменяем длинные тире - на &ndash;
				// ставим неразрывный пробел перед дефисом

				text = text.Replace (" - ", "&#160;&ndash; ");

				// ставим пробел после дефиса
				text = text.Replace (" -", "&#160;&ndash; ");

				// заменяем очень длинное тире просто длинным:
				text = text.Replace ("\x2014", "\x2013");
				text = text.Replace ("&mdash;", "&ndash;");

				// ставим неразрывный пробел перед дефисом
				text = text.Replace ("\x2013 ", "&#160;&ndash; ");
				text = text.Replace ("\x2013 ", "&#160;&ndash; ");

				text = text.Replace (" &#160;", "&#160;");
			}

			if (htmlout && emNames)
			{
				// Emphasize names:
				// Ivanov I.I. => <em>Ivanov I.I.</em>
				// I.I. Ivanov => <em>I.I. Ivanov</em>

				text = Regex.Replace (text, @"([ЁА-ЯA-Z]\.)\s*?([А-ЯA-Z]\.)\s*?([ЁёА-Яа-яA-Za-z]{2,})", "<em>$1$2 $3</em>");
				text = Regex.Replace (text, @"([ЁёА-Яа-яA-Za-z]{2,})\s*?([ЁА-ЯA-Z]\.)\s*?([ЁА-ЯA-Z]\.)", "<em>$1 $2$3</em>");
			}

			text = text.Replace ("г.г.", "гг.");
			text = text.Replace ("с\\х", "с.-х.");
			text = text.Replace ("с/х", "с.-х.");
			text = text.Replace ("с.х.", "с.-х.");	

			if (htmlout && !htmlin)
			{

				// заменяем URL на ссылки
				// buffer = Regex.Replace(buffer, @"^(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?$",
				// buffer = Regex.Replace(buffer,
				// @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))[\w\d:#%/;$()~_?\\-=\\\.&]*)",
				// "<a href=\"$&\">$&</a>", RegexOptions.IgnoreCase);

				text = Regex.Replace (text, @"\b((http|https|ftp|ftps)://.*?)([\s\.,:;!\?]\B)", 
					"<a href=\"${1}\">${1}</a>${3}", RegexOptions.IgnoreCase);

				// заменяем адреса эл. почты на ссылки
				text = Regex.Replace (text, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
					"<a href=\"mailto:$&\">$&</a>", RegexOptions.IgnoreCase);
			}

			// clear table structure
			if (clearTables && htmlin)
			{
				// Table code strips from anything but just a structure. 
				// Formatting can be simply applied later, in a web-based HTML editor and with styles

				// remove unnessesary open and closing tags
				text = Regex.Replace (text, @"<colgroup\b[^>]*>|<p\b[^>]*>|<div\b[^>]*>|<span\b[^>]*>|<font\b[^>]*>|<i\b[^>]*>|<em\b[^>]*>|<b\b[^>]*>|<strong\b[^>]*>|<col\b[^>]*>", "", RegexOptions.IgnoreCase);
				text = Regex.Replace (text, @"</colgroup>|</p>|</div>|</span>|</i>|</b>|</em>|</strong>|</font>|</col>", "", RegexOptions.IgnoreCase);

				// enclose all attribute values in the quotes
				text = Regex.Replace (text, "(\\s+\\w+)\\s*=\\s*(\\w+%?)", "${1}=\"${2}\"");

				// add @@@@@ before span attributes
				text = Regex.Replace (text, "(?=(row|col)span=[\"']\\d+[\"'])", "@@@@@", RegexOptions.IgnoreCase);

				// remove any other attributes - just because they do not begans with @@@@@
				text = Regex.Replace (text, "\\s+\\w+:?\\w+=[\"'].*?[\"']", "");

				// MS Excel non-standard export attributes
				text = Regex.Replace (text, "x:num|x:str", "");



				// remove @@@@@ before span attributes
				text = text.Replace ("@@@@@", "");

				// remove spaces in open tags like "<TD >" => "<TD>" -
				// those spaces appears after removing attributes
				text = Regex.Replace (text, "(<\\w+)\\s+>", "${1}>");

				// remove spaces before and after open and close tags	
				//buffer = Regex.Replace (buffer,"\\s+?(</?\\w.*?>)\\s+?", "${1}");

				text = Regex.Replace (text, "\\s+<", "<");
				text = Regex.Replace (text, ">\\s+", ">");

				// remove duplicate spaces
				text = Regex.Replace (text, "\\s+", " ");

				// lowercase all tags and span attributes with MatchEvaluator
				text = Regex.Replace (text, @"<(/?\w+)|(row|col)span", m => m.Value.ToLower (), RegexOptions.IgnoreCase);

				if (!string.IsNullOrEmpty (param.TableCSSClass))
					text = text.Replace ("<table>", string.Format (
						"<table class=\"{0}\" width=\"{1}{2}\">", param.TableCSSClass, param.TableWidth, param.TableWidthUnits));

			}

			#region Patches

			// remove breaks in a decimal numbers: 12, 23 => 12,23
			text = Regex.Replace (text, @"(\d[,\.])\s+(\d)", "${1}${2}");

			// russify dates in a RSS feed
			if (fixRssDate)
			{
				text = text.Replace ("Mon,", "Пн,");
				text = text.Replace ("Tue,", "Вт,");
				text = text.Replace ("Wed,", "Ср,");
				text = text.Replace ("Thu,", "Чт,");
				text = text.Replace ("Fri,", "Пт,");
				text = text.Replace ("Sat,", "Сб,");
				text = text.Replace ("Sun,", "Вс,");

				text = text.Replace (" Jan ", " января ");
				text = text.Replace (" Feb ", " февраля ");
				text = text.Replace (" Mar ", " марта ");
				text = text.Replace (" Apr ", " апреля ");
				text = text.Replace (" May ", " мая ");
				text = text.Replace (" Jun ", " июня ");
				text = text.Replace (" Jul ", " июля ");
				text = text.Replace (" Aug ", " августа ");
				text = text.Replace (" Sep ", " сентября ");
				text = text.Replace (" Oct ", " октября ");
				text = text.Replace (" Nov ", " ноября ");
				text = text.Replace (" Dec ", " декабря ");

				// удаление модификатора GMT
				text = Regex.Replace (text, @"\+0[0-9]00", "");
			}

			#endregion

			return text;
		}
	}
}

