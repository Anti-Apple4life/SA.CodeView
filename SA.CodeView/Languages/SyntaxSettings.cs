using System;
using System.Collections.Generic;
using System.Drawing;
using SA.CodeView.Parsing;
using SA.CodeView.ParsingOnElements;
using SA.CodeView.Utils;

namespace SA.CodeView.Languages
{
	/// <summary>�������� ��� ��������� � ������� ��� ��������� ����� ������ �� ������������ ���������.</summary>
	public abstract class SyntaxSettings
	{
		Dictionary<string, TextStyle> KeywordGroups;
		/// <summary>����� ������, ����������� ��� ����� ����������.</summary>
		internal readonly Dictionary<string, TextStyle> TextStyles;
		/// <summary>������ ��������-����������.</summary>
		public char[] Operators { get; protected set; }
		//=========================================================================================
		List<BaseElement> _Elements;
		/// <summary>������ ���� ������, �� ������� ����� ����������� �� ������.</summary>
		public List<BaseElement> Elements
		{
			get
			{
				if (this._Elements == null)
					this._Elements = this.CreateElements();
				return this._Elements;
			}
		}
		protected abstract List<BaseElement> CreateElements();
		//=========================================================================================
		public const string S_NORMAL_TEXT = "Normal";
		public const string S_OPERATORS = "Operators";
		public const string S_NUMBERS = "Numbers";
		public const string S_KEYWORDS_1 = "Keywords1";
		public const string S_KEYWORDS_2 = "Keywords2";
		public const string S_IDENTIFIER = "Identifier";
		public const string S_SINGLE_COMMENT = "SingleLine Comment";
		public const string S_MULTI_COMMENT = "MultiLine Comment";
		public const string S_STRINGS = "String";
		public const string S_SYS_TABLES = "SysTables";
		public const string S_SYS_FUNCTION = "SysFunc";
		public const string S_SYS_PROC = "SysProc";
		//=========================================================================================
		public SyntaxSettings()
		{
			this.TextStyles = this.CreateTextStyles();
		}
		//=========================================================================================
		#region ������ �� �������
		//=========================================================================================
		/// <summary>����� ����������� ������, ������� �� ������� �� ��� ����� ������.</summary>
		public TextStyle NormalTextStyle
		{
			get { return this.GetStyleByName(S_NORMAL_TEXT); }
		}
		/// <summary>����� ����������� ����������.</summary>
		public TextStyle OperatorsTextStyle
		{
			get { return this.GetStyleByName(S_OPERATORS); }
		}
		//=========================================================================================
		/// <summary>�������� � ��������� ������ ��������� �����.</summary>
		protected void AddStyle(Dictionary<string, TextStyle> styles, TextStyle textStyle)
		{
			styles.Add(textStyle.Name, textStyle);
		}
		//=========================================================================================
		/// <summary>������� ����� ������, ����������� ��� ����� ����������.</summary>
		protected virtual Dictionary<string, TextStyle> CreateTextStyles()
		{
			var styles = new Dictionary<string, TextStyle>();
			this.AddStyle(styles, new TextStyle(S_NORMAL_TEXT, Color.Black));
			return styles;
		}
		//=========================================================================================
		/// <summary>�������� ����� �� �����.</summary>
		public TextStyle GetStyleByName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			TextStyle oStyle;
			if (this.TextStyles.TryGetValue(name, out oStyle))
				return oStyle;
			return null;
		}
		//========================================================================================= 
		#endregion
		//=========================================================================================
		internal static SyntaxSettings CreateSettings(PredefinedLanguage syntaxType)
		{
			switch (syntaxType)
			{
				case PredefinedLanguage.None:
					return new NonSyntaxSettings();
				case PredefinedLanguage.MsSql:
				case PredefinedLanguage.Access:
					return new MsSqlSyntaxSettings();
				case PredefinedLanguage.Xml:
					return new XmlSyntaxSettings();
				case PredefinedLanguage.MySql:
					return new MySqlSyntaxSettings();
				case PredefinedLanguage.Oracle:
					return new OracleSyntaxSettings();
				case PredefinedLanguage.CSharp:
					return new CSharpSyntaxSettings();
				case PredefinedLanguage.VisualBasic:
					return new VisualBasicSyntaxSettings();
				default:
					throw new NotSupportedException(((int)syntaxType).ToString());
			}
		}
		//=========================================================================================
		internal virtual ParserSpecification CreateParserSpecification(ScannerSpecification scannerSpecification)
		{
			///�������� ������������ ��� ������� �� ���������.
			///�� ��������� ��� ������� ������� �� ������ ������.
			var oSpec = new ParserSpecification();

			Dictionary<string, string> ruleNames = new Dictionary<string, string>();
			///������ ��� ������������� ����� ������� � �������
			foreach (var oState in scannerSpecification.States)
			{
				string sTokenName = oState.ResultTokenName;
				if (!string.IsNullOrEmpty(sTokenName) && !ruleNames.ContainsKey(sTokenName))
				{
					oSpec.AddRule(sTokenName, sTokenName);
					ruleNames.Add(sTokenName, sTokenName);
				}
			}
			return oSpec;
		}
		//=========================================================================================
		internal virtual ScannerSpecification CreateScannerSpecification()
		{
			return new ScannerSpecification();
		}
		//=========================================================================================
		TextStyle GetKeywordStyle(string text)
		{
			if (this.KeywordGroups == null)
			{
				this.KeywordGroups = new Dictionary<string, TextStyle>(StringComparer.InvariantCultureIgnoreCase);
				this.FillKeywordGroups(this.KeywordGroups);
			}
			TextStyle oStyle;
			if (this.KeywordGroups.TryGetValue(text, out oStyle))
				return oStyle;
			return null;
		}
		//=========================================================================================
		/// <summary>�������, ����� ������� ����� ������ ���������� � ����� �����.</summary>
		/// <param name="dictionary"></param>
		protected virtual void FillKeywordGroups(Dictionary<string, TextStyle> dictionary) { }
		//=========================================================================================
		/// <summary>��������� ����� "keywords" �� ����� � �������, ��� ��� �������� ����� ������ ������������ � ��������� �����.</summary>
		protected void LoadKeywordsToGroup(Dictionary<string, TextStyle> dictionary, TextStyle style, string keywords)
		{
			foreach (string sWord in TextSplitter.GetWordsFromText(keywords))
			{
				//if (dictionary.ContainsKey(sWord))
				//    Console.WriteLine(sWord);
				//else
				dictionary.Add(sWord, style);
			}
		}
		//=========================================================================================
		internal virtual TextStyle GetStyleFor(Token token, string styleName)
		{
			TextStyle oStyle = this.GetStyleByName(styleName);
			if (oStyle != null)
				return oStyle;

			if (string.IsNullOrEmpty(token.TokenTypeName))
				return this.NormalTextStyle;
			if (token.TokenTypeName == "id")
				return this.GetKeywordStyle(token.Text);
			if (token.TokenTypeName == "comment1")
				return this.GetStyleByName(S_MULTI_COMMENT);
			if (token.TokenTypeName == "comment2")
				return this.GetStyleByName(S_SINGLE_COMMENT);
			if (token.TokenTypeName == "comment3")
				return this.GetStyleByName(S_SINGLE_COMMENT);
			if (token.TokenTypeName == "stringConst")
				return this.GetStyleByName(S_STRINGS);
			if (token.TokenTypeName == "operator")
				return this.GetStyleByName(S_OPERATORS);
			if (token.TokenTypeName == "number")
				return this.GetStyleByName(S_NUMBERS);

			return this.NormalTextStyle;
		}
		//=========================================================================================
	}
}
