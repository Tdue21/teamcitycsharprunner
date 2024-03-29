﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;

namespace CSharpCompiler.Runtime.Dumping
{
    public class HtmlObjectVisitor : DefaultObjectVisitor, IFileOutputObjectVisitor
    {
    	private const string Expand = "down.png";
    	private const string Collapse = "up.png";
    	private readonly HtmlTextWriter writer;
    	private const string CyclicReferenceGliph = "q";

    	public HtmlObjectVisitor(TextWriter inner, int maximumDepth) : base(maximumDepth)
        {
            writer = new HtmlTextWriter(inner);
        }

    	protected override void VisitNestingLimitReached()
    	{
			writer.AddAttribute(HtmlTextWriterAttribute.Title, "Maximum nesting limit reached");
    		writer.AddAttribute(HtmlTextWriterAttribute.Class, "limit");
    	}

    	protected override void VisitCyclicReferenceFound()
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Title, "Cyclic reference found");
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "limit");
		}

    	public override void Visit(object value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            base.Visit(value);
            writer.RenderEndTag();
        }

        protected override void VisitNull()
        {
			writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.Write("null");
			writer.RenderEndTag();
        }

        protected override void VisitPrimitiveType(object value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(value);
            writer.RenderEndTag();
        }

        protected override void VisitType(object value, bool b, bool isCyclicReference)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            base.VisitType(value, b, isCyclicReference);
            writer.RenderEndTag();
        }

        protected override void VisitEnumerable(IEnumerable value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            base.VisitEnumerable(value);
            writer.RenderEndTag();
        }

        protected override void VisitStaticTypeHeader(Type type)
        {
            BeforeTypeHeader();
        	writer.Write(FormatTypeNameForHeader(type));
            AfterTypeHeader();
        }

    	protected override void VisitCollapsedTypeHeader(Type type)
    	{
    		VisitToggleableTypeHeader(Expand, type);
    	}

    	protected override void VisitExpandedTypeHeader(Type type)
    	{
    		VisitToggleableTypeHeader(Collapse, type);	
    	}

    	private void VisitToggleableTypeHeader(string image, Type type)
    	{
    		BeforeTypeHeader();

    		VisitToggleableHeader(image, FormatTypeNameForHeader(type));

    		AfterTypeHeader();
    	}

    	private void VisitToggleableHeader(string image, string headerText)
    	{
    		writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "return toggle(this);");
    		writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
    		writer.AddAttribute(HtmlTextWriterAttribute.Class, "typeheader");
    		writer.RenderBeginTag(HtmlTextWriterTag.A);
    		RenderUpDownImage(image);
    		writer.Write(headerText);
    		writer.RenderEndTag();
    	}

    	private void RenderUpDownImage(string image)
    	{
    		writer.AddAttribute(HtmlTextWriterAttribute.Src, image);
    		writer.AddAttribute(HtmlTextWriterAttribute.Alt, Path.GetFileNameWithoutExtension(image));
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "updown");
    		writer.RenderBeginTag(HtmlTextWriterTag.Img);
    		writer.RenderEndTag();
    	}

    	protected override void VisitCyclicReferenceTypeHeader(Type type)
    	{
			BeforeTypeHeader();

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "typeglyph");
			writer.RenderBeginTag(HtmlTextWriterTag.Span);
			writer.Write(CyclicReferenceGliph);
			writer.RenderEndTag();
			writer.Write(FormatTypeNameForHeader(type));

			AfterTypeHeader();
    	}

    	private void BeforeTypeHeader()
    	{
    		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
    		writer.AddAttribute(HtmlTextWriterAttribute.Colspan, 2.ToString());
    		writer.AddAttribute(HtmlTextWriterAttribute.Class, "typeheader");
    		writer.RenderBeginTag(HtmlTextWriterTag.Td);
    	}

    	private void AfterTypeHeader()
    	{
    		writer.RenderEndTag();
    		writer.RenderEndTag();
    	}

    	private static string FormatTypeNameForHeader(Type type)
        {
            return IsAnonymous(type) ? "anonymous" : type.Name;
        }

    	private static bool IsAnonymous(Type type)
        {
            return type.Name.StartsWith("<>");
        }

    	protected override void VisitEnumerableHeader(Type enumerableType, int count, int numberOfMembers)
        {
            BeforeEnumerableHeader(numberOfMembers);

    		var headerText = new StringBuilder(FormatTypeNameForHeader(enumerableType))
    			.AppendFormat(" ({0} item{1})", count, AddPluralSuffix(count)).ToString();

			VisitToggleableHeader(Collapse, headerText);
    		
			AfterEnumerableHeader();
        }

    	private void AfterEnumerableHeader()
    	{
    		writer.RenderEndTag();
    		writer.RenderEndTag();
    	}

    	private void BeforeEnumerableHeader(int numberOfMembers)
    	{
    		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
    		writer.AddAttribute(HtmlTextWriterAttribute.Class, "typeheader");
            
    		if(numberOfMembers > 1)
    			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, numberOfMembers.ToString());
            
    		writer.RenderBeginTag(HtmlTextWriterTag.Td);
    	}

    	protected override void VisitTypeSummary(object value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, 2.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "summary");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(value.ToString());
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

    	protected override void VisitHiddenTypeMember()
    	{
    		writer.AddAttribute(HtmlTextWriterAttribute.Class, "hidden");
    	}

		protected override void VisitVisibleTypeMember()
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "visible");
		}

    	protected override void VisitTypeMember(MemberInfo member, Type memberType, object value)
        {
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            base.VisitTypeMember(member, memberType, value);
            writer.RenderEndTag();
        }

    	protected override void VisitTypeMemberValue(object value)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            base.VisitTypeMemberValue(value);
            writer.RenderEndTag();
        }

        protected override void VisitTypeMemberName(MemberInfo member, Type memberType)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "member");
			writer.AddAttribute(HtmlTextWriterAttribute.Title, memberType.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            writer.Write(FormatMemberName(member));
            writer.RenderEndTag();
        }

        private static string AddPluralSuffix(int count)
        {
            return count == 1 ? "" : "s";
        }

        protected override void VisitPrimitiveTypeInEnumerable(object element, int numberOfMembers)
        {
            if(numberOfMembers > 1)
                writer.AddAttribute(HtmlTextWriterAttribute.Colspan, numberOfMembers.ToString());

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            base.VisitPrimitiveTypeInEnumerable(element, numberOfMembers);
            writer.RenderEndTag();
        }

        protected override void VisitEnumerableInEnumerable(IEnumerable enumerable, int numberOfMembers)
        {
            if(numberOfMembers > 1)
                writer.AddAttribute(HtmlTextWriterAttribute.Colspan, numberOfMembers.ToString());

            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            base.VisitEnumerableInEnumerable(enumerable, numberOfMembers);
            writer.RenderEndTag();
        }

        protected override void VisitEnumerableElement(object element, IEnumerable<MemberInfo> members)
        {
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "visible"); 
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            base.VisitEnumerableElement(element, members);
            writer.RenderEndTag();
        }

        protected override void VisitTypeInEnumerableMembers(IEnumerable<MemberInfo> members)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            base.VisitTypeInEnumerableMembers(members);
            writer.RenderEndTag();
        }

        protected override void VisitTypeInEnumerableMember(MemberInfo member)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            writer.Write(FormatMemberName(member));
            writer.RenderEndTag();
        }

        private static string FormatMemberName(MemberInfo member)
        {
            return member.Name.StartsWith("<") ? member.Name.Substring(1, member.Name.IndexOf(">") - 1) : member.Name;
        }

        public void Dispose()
        {
            writer.Flush();
            writer.Dispose();
        }
    }
}