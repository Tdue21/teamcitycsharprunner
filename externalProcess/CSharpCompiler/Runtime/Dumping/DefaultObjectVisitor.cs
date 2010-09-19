﻿using System;
using System.Reflection;

namespace CSharpCompiler.Runtime.Dumping
{
    public abstract class DefaultObjectVisitor : AbstractObjectVisitor
    {
        protected DefaultObjectVisitor(int maximumDepth) : base(maximumDepth)
        {
        }

        protected override void VisitTypeInEnumerableFooter()
        {
        }

        protected override void VisitTypeInEnumerableMember(MemberInfo member)
        {
        }

        protected override void VisitPrimitiveType(object value)
        {
        }

        protected override void VisitTypeFooter()
        {
        }

        protected override void VisitTypeSummary(Type type)
        {
        }

        protected override void VisitTypeHeader(Type type)
        {
        }

        protected override void VisitTypeMemberName(string name)
        {
        }

        protected override void VisitEnumerableHeader(Type enumerableType, int count)
        {
        }

        protected override void VisitEnumerableFooter()
        {
        }
    }
}