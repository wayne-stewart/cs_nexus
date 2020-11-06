using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Section = System.Collections.Generic.Dictionary<string, string>;
using Sections = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace Config
{
    public interface IConfig
    {
        IConfig Parse(string s);

        string this[string section_name, string key] { get; }
    }

    //public class Section
    //{

    //}

    public class INIConfig : IConfig
    {
        readonly Sections _sections;

        const int WHITE_SPACE = 0;
        const int SECTION_NAME = 1;
        const int COMMENT = 2;
        const int PROPERTY = 3;
        const int VALUE = 4;

        public INIConfig(string s)
        {
            _sections = new Sections();
            Parse(s);
        }

        public string this[string section_name, string key] 
        {
            get
            {
                if (_sections.TryGetValue(section_name, out Section section))
                {
                    if (section.TryGetValue(key, out string value))
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        public IConfig Parse(string s)
        {
            
            Section current_section = null;
            var sb = new StringBuilder();
            var property = string.Empty;
            var s_index = 0;
            var s_line = 1;
            var s_line_index = 0;
            var mode = WHITE_SPACE;
            while (s_index < s.Length)
            {
                var c = s[s_index];
                switch (mode)
                {
                    case WHITE_SPACE: LineStart(sb, c, out mode, s_line, s_line_index); break;
                    case SECTION_NAME: SectionName(sb, _sections, ref current_section, c, s_line, s_line_index, ref mode); break;
                    case COMMENT: Comment(c, ref mode); break;
                    case PROPERTY: Property(sb, c, s_line, s_line_index, ref mode, ref property); break;
                    case VALUE: Value(sb, c, current_section, property, ref mode); break;
                }

                IncrementPosition(ref s_index, ref s_line, ref s_line_index, c);
            }

            // finalize last mode at end of string
            switch(mode)
            {
                case WHITE_SPACE: break;
                case SECTION_NAME: throw new Exception("Unexpected end of section");
                case COMMENT: break;
                case PROPERTY: throw new Exception("Unexpected end of property");
                case VALUE: UpdateOrCreateKeyValue(current_section, property, sb); break;
            }

            return this;
        }

        static void LineStart(StringBuilder sb, char c, out int mode, int s_line, int s_line_index)
        {
            switch (c)
            {
                case '[': SectionStart(sb, out mode); break;
                case ';': CommentStart(out mode); break;
                case '\r':
                case '\n':
                case ' ':
                case '\t':
                    mode = WHITE_SPACE; break;
                default: PropertyStart(sb, c, s_line, s_line_index, out mode); break;
            }
        }

        static void IncrementPosition(ref int s_index, ref int s_line, ref int s_line_index, char c)
        {
            s_index++;
            if (c == '\n')
            {
                s_line++;
                s_line_index = 0;
            }
            else
            {
                s_line_index++;
            }
        }

        static bool IsValidSectionChar(char c)
        {
            return char.IsLetterOrDigit(c) || "-_".IndexOf(c) >= 0;
        }

        static bool IsValidPropertyChar(char c)
        {
            return char.IsLetterOrDigit(c) || "-_".IndexOf(c) >= 0;
        }

        static void SectionStart(StringBuilder sb, out int mode)
        {
            mode = SECTION_NAME;
            sb.Clear();
        }
        
        static void SectionName(StringBuilder sb, Sections sections, ref Section current_section, char c, int s_line, int s_line_index, ref int mode)
        {
            if (c == ']') SectionNameEnd(sb, sections, out current_section, s_line, s_line_index, out mode);
            else if (IsValidSectionChar(c)) sb.Append(c);
            else throw new Exception($"Invalid Section Name or unexpected end of line - Line: {s_line} Index: {s_line_index}");
        }
        
        static void SectionNameEnd(StringBuilder sb, Sections sections, out Section section, int s_line, int s_line_index, out int mode)
        {
            if (sb.Length == 0)
            {
                throw new Exception($"Invalid Empty Section Name Line: {s_line} Index: {s_line_index}");
            }
            section = FindOrCreateSection(sections, sb.ToString());
            mode = WHITE_SPACE;
        }

        static void CommentStart(out int mode)
        {
            mode = COMMENT;
        }

        static void Comment(char c, ref int mode)
        {
            if (c == '\n') mode = WHITE_SPACE;
        }

        static void PropertyStart(StringBuilder sb, char c, int s_line, int s_line_index, out int mode)
        {
            if (IsValidPropertyChar(c))
            {
                sb.Clear();
                sb.Append(c);
                mode = PROPERTY;
            }
            else throw new Exception($"Invalid Start of Property Line: {s_line} Index: {s_line_index}");
        }

        static void Property(StringBuilder sb, char c, int s_line, int s_line_index, ref int mode, ref string property)
        {
            if (IsValidPropertyChar(c))
            {
                sb.Append(c);
            }
            else if (c == '=')
            {
                property = sb.ToString();
                sb.Clear();
                mode = VALUE;
            }
            else
            {
                throw new Exception($"Invalid character in property. Expected either a valid property name character or '='. Line: {s_line} Index: {s_line_index}");
            }
        }

        static void Value(StringBuilder sb, char c, Section section, string property, ref int mode)
        {
            if (c == '\n')
            {
                UpdateOrCreateKeyValue(section, property, sb);
                mode = WHITE_SPACE;
            }
            else
            {
                sb.Append(c);
            }
        }

        static Section FindOrCreateSection(Sections sections, string section_name)
        {
            if (sections.TryGetValue(section_name, out Section section))
            {
                return section;
            }
            else
            {
                section = new Section();
                sections.Add(section_name, section);
                return section;
            }
        }

        static void UpdateOrCreateKeyValue(Section section, string property, StringBuilder sb)
        {
            var value = sb.ToString().Trim();
            section[property] = value;
        }
    }
}
