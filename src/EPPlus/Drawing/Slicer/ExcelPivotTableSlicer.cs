﻿/*************************************************************************************************
  Required Notice: Copyright (C) EPPlus Software AB. 
  This software is licensed under PolyForm Noncommercial License 1.0.0 
  and may only be used for noncommercial purposes 
  https://polyformproject.org/licenses/noncommercial/1.0.0/

  A commercial license to use this software can be purchased at https://epplussoftware.com
 *************************************************************************************************
  Date               Author                       Change
 *************************************************************************************************
  06/26/2020         EPPlus Software AB       EPPlus 5.4
 ******0*******************************************************************************************/
using OfficeOpenXml.Table.PivotTable;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace OfficeOpenXml.Drawing.Slicer
{
    public class ExcelPivotTableSlicer : ExcelSlicer<ExcelPivotTableSlicerCache>
    {
        ExcelSlicerXmlSource _xmlSource;
        internal ExcelPivotTableField _field;
        internal ExcelPivotTableSlicer(ExcelDrawings drawings, XmlNode node, ExcelPivotTableField field, ExcelGroupShape parent = null) : base(drawings, node, parent)
        {
            _ws = drawings.Worksheet;
            _field = field;
            var name = drawings.Worksheet.Workbook.GetPivotTableSlicerName(field.Name);
            CreateDrawing(name);

            SlicerName = field.Name;
            Caption = field.Name;
            RowHeight = 19;
            CacheName = "Slicer_" + name.Replace(" ", "_");

            var cache = new ExcelPivotTableSlicerCache(NameSpaceManager);
            _field.Cache.Slicer = this;
            cache.Init(drawings.Worksheet.Workbook, name, this);
            _cache = cache;

            //If items has not been init, refresh!
            if(field._items==null)
            {
                field.Items.Refresh();
            }
        }
        internal ExcelPivotTableSlicer(ExcelDrawings drawings, XmlNode node, ExcelGroupShape parent = null) : base(drawings, node, parent)
        {
            _ws = drawings.Worksheet;
            var slicerNode = _ws.SlicerXmlSources.GetSource(Name, eSlicerSourceType.PivotTable, out _xmlSource);
            _slicerXmlHelper = XmlHelperFactory.Create(NameSpaceManager, slicerNode);
        }
        private void CreateDrawing(string name)
        {
            XmlElement graphFrame = TopNode.OwnerDocument.CreateElement("mc", "AlternateContent", ExcelPackage.schemaMarkupCompatibility);
            graphFrame.SetAttribute("xmlns:mc", ExcelPackage.schemaMarkupCompatibility);
            graphFrame.SetAttribute("xmlns:a14", ExcelPackage.schemaDrawings2010);
            TopNode.AppendChild(graphFrame);
            graphFrame.InnerXml = string.Format("<mc:Choice Requires=\"a14\"><xdr:graphicFrame macro=\"\"><xdr:nvGraphicFramePr><xdr:cNvPr id=\"{0}\" name=\"{2}\"><a:extLst><a:ext uri=\"{{FF2B5EF4-FFF2-40B4-BE49-F238E27FC236}}\"><a16:creationId xmlns:a16=\"http://schemas.microsoft.com/office/drawing/2014/main\" id=\"{1}\"/></a:ext></a:extLst></xdr:cNvPr><xdr:cNvGraphicFramePr/></xdr:nvGraphicFramePr><xdr:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"0\" cy=\"0\"/></xdr:xfrm><a:graphic><a:graphicData uri=\"http://schemas.microsoft.com/office/drawing/2010/slicer\"><sle:slicer xmlns:sle=\"http://schemas.microsoft.com/office/drawing/2010/slicer\" name=\"{2}\"/></a:graphicData></a:graphic></xdr:graphicFrame></mc:Choice><mc:Fallback xmlns=\"\"><xdr:sp macro=\"\" textlink=\"\"><xdr:nvSpPr><xdr:cNvPr id=\"{0}\" name=\"{1}\"/><xdr:cNvSpPr><a:spLocks noTextEdit=\"1\"/></xdr:cNvSpPr></xdr:nvSpPr><xdr:spPr><a:xfrm><a:off x=\"12506325\" y=\"3238500\"/><a:ext cx=\"1828800\" cy=\"2524125\"/></a:xfrm><a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom><a:solidFill><a:prstClr val=\"white\"/></a:solidFill><a:ln w=\"1\"><a:solidFill><a:prstClr val=\"green\"/></a:solidFill></a:ln></xdr:spPr><xdr:txBody><a:bodyPr vertOverflow=\"clip\" horzOverflow=\"clip\"/><a:lstStyle/><a:p><a:r><a:rPr lang=\"sv-SE\" sz=\"1100\"/><a:t>This shape represents a slicer. Slicers are supported in Excel 2010 or later. If the shape was modified in an earlier version of Excel, or if the workbook was saved in Excel 2003 or earlier, the slicer cannot be used.</a:t></a:r></a:p></xdr:txBody></xdr:sp></mc:Fallback>",
                _id,
                "{" + Guid.NewGuid().ToString() + "}",
                name) ;
            TopNode.AppendChild(TopNode.OwnerDocument.CreateElement("clientData", ExcelPackage.schemaSheetDrawings));

            _xmlSource = _ws.SlicerXmlSources.GetOrCreateSource(eSlicerSourceType.Table);
            var node = _xmlSource.XmlDocument.CreateElement("slicer", ExcelPackage.schemaMainX14);
            _xmlSource.XmlDocument.DocumentElement.AppendChild(node);
            _slicerXmlHelper = XmlHelperFactory.Create(NameSpaceManager, node);

            var slNode = _ws.GetExtLstSubNode("{A8765BA9-456A-4dab-B4F3-ACF838C121DE}", "x14:slicerList");
            if (slNode == null)
            {
                _ws.CreateNode("d:extLst/d:ext", false, true);
                slNode = _ws.CreateNode("d:extLst/d:ext/x14:slicerList", false, true);
                ((XmlElement)slNode.ParentNode).SetAttribute("uri", "{A8765BA9-456A-4dab-B4F3-ACF838C121DE}");

                var xh = XmlHelperFactory.Create(NameSpaceManager, slNode);
                var element = (XmlElement)xh.CreateNode("x14:slicer", false, true);
                element.SetAttribute("id", ExcelPackage.schemaRelationships, _xmlSource.Rel.Id);
            }

            GetPositionSize();
        }

        internal override bool CheckSlicerNameIsUnique(string name)
        {
            if (_drawings.Worksheet.Workbook._pivotTableSlicerNames.Contains(name))
            {
                return false;
            }
            _drawings.Worksheet.Workbook._pivotTableSlicerNames.Add(name);
            return true;
        }
    }
}