﻿// -----------------------------------------------------------------------
// <copyright file="GrainGen.cs" company="Asynkron AB">
//      Copyright (C) 2015-2020 Asynkron AB All rights reserved
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using Google.Protobuf.Reflection;
using HandlebarsDotNet;
using ProtoBuf;
using ProtoBuf.Reflection;

namespace GrainGenerator
{
    public class GrainGen : CommonCodeGenerator
    {
        public override string Name { get; } = "Proto.Grain";

        protected override string DefaultFileExtension => ".cs";

        protected override string Escape(string identifier) => identifier;

        protected override void WriteFile(GeneratorContext ctx, FileDescriptorProto obj)
        {
            var file = ctx.File;

            var ast = new ProtoFile
            {
                PackageName = file.Package,
                CsNamespace = file.Options?.CsharpNamespace ?? file.Package,
                Messages = file
                    .MessageTypes
                    .ToArray()
                    .Select(mt => new ProtoMessage {Name = mt.Name})
                    .ToArray(),
                Services = file
                    .Services
                    .ToArray()
                    .Select(
                        s => new ProtoService
                        {
                            Name = s.Name,
                            Methods = s.Methods.ToArray()
                                .Select(
                                    (m, i) => new ProtoMethod
                                    {
                                        Index = i,
                                        Name = m.Name,
                                        InputName = RemovePackageName(m.InputType),
                                        OutputName = RemovePackageName(m.OutputType)
                                    }
                                )
                                .ToArray()
                        }
                    )
                    .ToArray()
            };
            var f = Handlebars.Compile(Template.Code);

            var result = f(ast);
            ctx.WriteLine(result);

            static string RemovePackageName(ReadOnlySpan<char> type)
            {
                var index = type.LastIndexOf('.');
                return type.Slice(index + 1).ToString();
            }
        }

        #region UnusedMethods

        protected override void WriteNamespaceHeader(GeneratorContext ctx, string @namespace) => throw new NotImplementedException();

        protected override void WriteNamespaceFooter(GeneratorContext ctx, string @namespace) => throw new NotImplementedException();

        protected override void WriteField(
            GeneratorContext ctx,
            FieldDescriptorProto obj,
            ref object state,
            OneOfStub[] oneOfs
        ) => throw new NotImplementedException();

        protected override void WriteMessageFooter(GeneratorContext ctx, DescriptorProto obj, ref object state)
            => throw new NotImplementedException();

        protected override void WriteMessageHeader(GeneratorContext ctx, DescriptorProto obj, ref object state)
            => throw new NotImplementedException();

        protected override void WriteEnumHeader(GeneratorContext ctx, EnumDescriptorProto obj, ref object state)
            => throw new NotImplementedException();

        protected override void WriteEnumValue(GeneratorContext ctx, EnumValueDescriptorProto obj, ref object state)
            => throw new NotImplementedException();

        protected override void WriteEnumFooter(GeneratorContext ctx, EnumDescriptorProto obj, ref object state)
            => throw new NotImplementedException();

        #endregion
    }
}