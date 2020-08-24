/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  01/27/2020         EPPlus Software AB       Initial release EPPlus 5
 *************************************************************************************************/
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Utils.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OfficeOpenXml.Table.PivotTable
{
    public class ExcelPivotTableFieldItem
    {
        [Flags]
        internal enum eBoolFlags
        {
            Hidden=0x1,
            SD = 0x2,
            C = 0x4,
            D = 0x8,
            E = 0x10,
            F = 0x20,
            M = 0x40,
            S = 0x80
        }
        internal eBoolFlags flags=eBoolFlags.SD|eBoolFlags.E;
        internal ExcelPivotTableFieldItem()
        {
        }
        internal ExcelPivotTableFieldItem (XmlElement node)
        {
            foreach(XmlAttribute a in node.Attributes)
            {
                switch(a.LocalName)
                {
                    case "c":
                        C = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "d":
                        D = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "e":
                        E = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "f":
                        F = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "h":
                        Hidden = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "m":
                        M = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "n":
                        Text = a.Value;
                        break;
                    case "s":
                        S = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "sd":
                        SD = XmlHelper.GetBoolFromString(a.Value);
                        break;
                    case "t":                        
                        Type = a.Value.ToEnum(eItemType.Data);
                        break;
                    case "x":
                        X = int.Parse(a.Value);
                        break;
                }
            }
        }
        public string Text { get; set; }
        public object Value { get; internal set; }
        public bool Hidden 
        { 
            get
            {
                return (flags & eBoolFlags.Hidden) == eBoolFlags.Hidden;
            }
            set
            {
                if (Type != eItemType.Data) throw (new InvalidOperationException("Hidden can only be set for items of type Data"));
                flags |= eBoolFlags.Hidden;
            }
        }
        internal bool SD
        {
            get
            {
                return (flags & eBoolFlags.SD) == eBoolFlags.SD;
            }
            set
            {
                flags |= eBoolFlags.SD;
            }
        }
        internal bool C
        {
            get
            {
                return (flags & eBoolFlags.C) == eBoolFlags.C;
            }
            set
            {
                flags |= eBoolFlags.C;
            }
        }
        internal bool D
        {
            get
            {
                return (flags & eBoolFlags.D) == eBoolFlags.D;
            }
            set
            {
                flags |= eBoolFlags.D;
            }
        }
        internal bool E
        {
            get
            {
                return (flags & eBoolFlags.E) == eBoolFlags.E;
            }
            set
            {
                flags |= eBoolFlags.E;
            }
        }
        internal bool F
        {
            get
            {
                return (flags & eBoolFlags.F) == eBoolFlags.F;
            }
            set
            {
                flags |= eBoolFlags.F;
            }
        }
        internal bool M
        {
            get
            {
                return (flags & eBoolFlags.M) == eBoolFlags.M;
            }
            set
            {
                flags |= eBoolFlags.M;
            }
        }
        internal bool S
        {
            get
            {
                return (flags & eBoolFlags.S) == eBoolFlags.S;
            }
            set
            {
                flags |= eBoolFlags.S;
            }
        }
        internal int X { get; set; } = -1;
        internal eItemType Type { get; set; }

        internal void GetXmlString(StringBuilder sb)
        {
            sb.Append("<item");
            if(X>-1)
            {
                sb.AppendFormat(" x=\"{0}\"", X);
            }
            if(Type!=eItemType.Data)
            {
                sb.AppendFormat(" t=\"{0}\"", Type.ToEnumString());
            }
            if(!string.IsNullOrEmpty(Text))
            {
                sb.AppendFormat(" x=\"{0}\"", Text);
            }
            AddBool(sb,"h", Hidden);
            AddBool(sb, "sd", SD, true);
            AddBool(sb, "c", C);
            AddBool(sb, "d", D);
            AddBool(sb, "e", E, true);
            AddBool(sb, "f", F);
            AddBool(sb, "m", M);
            AddBool(sb, "s", S);
            sb.Append("/>");
        }

        private void AddBool(StringBuilder sb, string attrName, bool b, bool defaultValue=false)
        {
            if(b != defaultValue)
            {
                sb.AppendFormat(" {0}=\"{1}\"",attrName, b?"1":"0");
            }
        }
    }
    /// <summary>
    /// A field Item. Used for grouping
    /// </summary>
    //public class ExcelPivotTableFieldItem : XmlHelper
    //{
    //ExcelPivotTableField _field;
    //internal ExcelPivotTableFieldItem(XmlNamespaceManager ns, XmlNode topNode, ExcelPivotTableField field) :
    //    base(ns, topNode)
    //{
    //   _field = field;
    //}
    ///// <summary>
    ///// The text. Unique values only
    ///// </summary>
    //public string Text
    //{
    //    get
    //    {
    //        return GetXmlNodeString("@n");
    //    }
    //    set
    //    {
    //        if(string.IsNullOrEmpty(value))
    //        {
    //            DeleteNode("@n");
    //            return;
    //        }
    //        foreach (var item in _field.Items)
    //        {
    //            if (item.Text == value)
    //            {
    //                throw(new ArgumentException("Duplicate Text"));
    //            }
    //        }
    //        SetXmlNodeString("@n", value);
    //    }
    //}
    //internal int X
    //{
    //    get
    //    {
    //        return GetXmlNodeInt("@x"); 
    //    }
    //}
    //internal string T
    //{
    //    get
    //    {
    //        return GetXmlNodeString("@t");
    //    }
    //}
    //public bool HideDetails 
    //{
    //    get
    //    {
    //        return GetXmlNodeBool("@sd");
    //    }
    //    set
    //    {
    //        SetXmlNodeBool("@sd", value, false);
    //    }
    //}
    //internal bool Hidden
    //{
    //    get
    //    {
    //        return GetXmlNodeBool("@h");
    //    }
    //    set
    //    {
    //        SetXmlNodeBool("@h", value, false);
    //    }
    //}
    //}
}
