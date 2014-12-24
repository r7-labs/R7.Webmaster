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
using System.Text.RegularExpressions;
using R7.Webmaster.Core;

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

	public class TextCleanerModel
	{
		public TextCleanerModel ()
		{
		}

		public string TextClean (string text, TextCleanerParams textCleanParams)
		{
			textCleanParams.HtmlIn = HtmlUtils.IsHtml (text);

			if (!textCleanParams.HtmlIn && textCleanParams.HtmlOut)
			{
				return new TextToHtmlProcessing ().Execute (text, textCleanParams);
			}

			if (!textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
				return new TextToTextProcessing ().Execute (text, textCleanParams);
			}

			if (textCleanParams.HtmlIn && !textCleanParams.HtmlOut)
			{
				return new HtmlToTextProcessing ().Execute (text, textCleanParams);
			}

			if (textCleanParams.HtmlIn && textCleanParams.HtmlOut)
			{
				return new HtmlToHtmlProcessing ().Execute (text, textCleanParams);
			}

			return text;
		}

		[Obsolete("Use TextProcessing classes instead.", true)]
		private string TextCleanOld (string text, bool htmlin, bool htmlout, bool fixRssDate, bool clearTables, bool emNames, TextCleanerParams param)
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
