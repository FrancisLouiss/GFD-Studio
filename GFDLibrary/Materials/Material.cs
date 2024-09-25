﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using GFDLibrary.IO;
using GFDLibrary.Models;
using GFDLibrary.Models.Conversion;
using YamlDotNet.Core;

namespace GFDLibrary.Materials
{
    public sealed class Material : Resource
    {
        public override ResourceType ResourceType => ResourceType.Material;

        public string Name { get; set; }

        // 0x54
        private MaterialFlags mFlags;
        public MaterialFlags Flags
        {
            get => mFlags;
            set
            {
                mFlags = value;
                ValidateFlags();
            }
        }

        // 0x00
        public Vector4 AmbientColor { get; set; }

        // 0x10
        public Vector4 DiffuseColor { get; set; }

        // 0x20
        public Vector4 SpecularColor { get; set; }

        // 0x30
        public Vector4 EmissiveColor { get; set; }

        // 0x40
        public float Field40 { get; set; }

        // 0x44
        public float Field44 { get; set; }

        // 0x48
        public MaterialDrawMethod DrawMethod { get; set; }

        // 0x49
        public byte Field49 { get; set; }

        // 0x4A
        public byte Field4A { get; set; }

        // 0x4B
        public byte Field4B { get; set; }

        // 0x4C
        public byte Field4C { get; set; }

        // 0x4D
        public HighlightMapMode Field4D { get; set; }

        // 0x90
        public short Field90 { get; set; }

        // 0x92
        public AlphaClipMode Field92 { get; set; }

        // 0x94
        //public short Field94 { get; set; }
        private MaterialFlags2 Field94;

        public MaterialFlags2 Flags2
        {
            get => Field94;
            set
            {
                Field94 = value;
            }
        }

        // 0x96
        public short Field96 { get; set; }

        // 0x5C
        public short Field5C { get; set; }

        // 0x6C
        public uint Field6C { get; set; }

        // 0x70
        public uint Field70 { get; set; }

        // 0x50
        public short DisableBackfaceCulling { get; set; }

        // 0x98
        public uint Field98 { get; set; }

        private TextureMap mDiffuseMap;
        public TextureMap DiffuseMap
        {
            get => mDiffuseMap;
            set
            {
                mDiffuseMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mNormalMap;
        public TextureMap NormalMap
        {
            get => mNormalMap;
            set
            {
                mNormalMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mSpecularMap;
        public TextureMap SpecularMap
        {
            get => mSpecularMap;
            set
            {
                mSpecularMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mReflectionMap;
        public TextureMap ReflectionMap
        {
            get => mReflectionMap;
            set
            {
                mReflectionMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mHightlightMap;
        public TextureMap HighlightMap
        {
            get => mHightlightMap;
            set
            {
                mHightlightMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mGlowMap;
        public TextureMap GlowMap
        {
            get => mGlowMap;
            set
            {
                mGlowMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mNightMap;
        public TextureMap NightMap
        {
            get => mNightMap;
            set
            {
                mNightMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mDetailMap;
        public TextureMap DetailMap
        {
            get => mDetailMap;
            set
            {
                mDetailMap = value;
                ValidateFlags();
            }
        }

        private TextureMap mShadowMap;
        public TextureMap ShadowMap
        {
            get => mShadowMap;
            set
            {
                mShadowMap = value;
                ValidateFlags();
            }
        }

        public IEnumerable<TextureMap> TextureMaps
        {
            get
            {
                yield return DiffuseMap;
                yield return NormalMap;
                yield return SpecularMap;
                yield return ReflectionMap;
                yield return HighlightMap;
                yield return GlowMap;
                yield return NightMap;
                yield return DetailMap;
                yield return ShadowMap;
            }
        }

        private List<MaterialAttribute> mAttributes;
        public List<MaterialAttribute> Attributes
        {
            get => mAttributes;
            set
            {
                mAttributes = value;
                ValidateFlags();
            }
        }

        // METAPHOR REFANTAZIO
        // 0x2dc
        public ushort METAPHOR_MaterialParameterFormat { get; set; }

        public bool IsPresetMaterial { get; internal set; }

        public Material( string name )
            : this()
        {
            Name = name;
            IsPresetMaterial = true;
        }

        public Material()
        {
            Initialize();
        }

        public Material( uint version ) : base( version )
        {
            Initialize();
        }

        public override string ToString()
        {
            return Name;
        }


        private void Initialize()
        {
            Field40 = 1.0f;
            Field44 = 0;
            Field49 = 1;
            Field4B = 1;
            Field4D = (HighlightMapMode)1;
            DrawMethod = 0;
            Field4A = 0;
            Field4C = 0;
            Field5C = 0;
            Flags = MaterialFlags.HasAmbientColor | MaterialFlags.HasDiffuseColor;
            Field92 = ( AlphaClipMode)4;
            Field70 = 0xFFFFFFFF;
            Field98 = 0xFFFFFFFF;
            Field6C = 0xFFFFFFFF;
        }

        private void ValidateFlags()
        {
            ValidateMapFlags( DiffuseMap,    MaterialFlags.HasDiffuseMap );
            ValidateMapFlags( NormalMap,     MaterialFlags.HasNormalMap );
            ValidateMapFlags( SpecularMap,   MaterialFlags.HasSpecularMap );
            ValidateMapFlags( ReflectionMap, MaterialFlags.HasReflectionMap );
            ValidateMapFlags( HighlightMap,  MaterialFlags.HasHighlightMap );
            ValidateMapFlags( GlowMap,       MaterialFlags.HasGlowMap );
            ValidateMapFlags( NightMap,      MaterialFlags.HasNightMap );
            ValidateMapFlags( DetailMap,     MaterialFlags.HasDetailMap );
            ValidateMapFlags( ShadowMap,     MaterialFlags.HasShadowMap );

            if ( Attributes == null )
            {
                mFlags &= ~MaterialFlags.HasAttributes;
            }
            else
            {
                mFlags |= MaterialFlags.HasAttributes;
            }
        }

        private void ValidateMapFlags( TextureMap map, MaterialFlags flag )
        {
            if ( map == null )
            {
                mFlags &= ~flag;
            }
            else
            {
                mFlags |= flag;
            }
        }

        protected override void ReadCore( ResourceReader reader )
        {
            // Read material header
            METAPHOR_MaterialParameterFormat = 1;
            if ( Version >= 0x2000000 )
            {
                METAPHOR_MaterialParameterFormat = reader.ReadUInt16();
            }
            Name = reader.ReadStringWithHash( Version );
            var flags = ( MaterialFlags )reader.ReadUInt32();

            if ( Version < 0x1104000 )
            {
                flags = ( MaterialFlags )( ( uint )Flags & 0x7FFFFFFF );
            }

            if (Version < 0x2000000 )
            {
                AmbientColor = reader.ReadVector4();
                DiffuseColor = reader.ReadVector4();
                SpecularColor = reader.ReadVector4();
                EmissiveColor = reader.ReadVector4();
                Field40 = reader.ReadSingle();
                Field44 = reader.ReadSingle();
            } else
            {
                switch ( METAPHOR_MaterialParameterFormat )
                {
                    case 0:
                        reader.ReadResource<MaterialParameterSetType0>( Version );
                        break;
                    case 1:
                        reader.ReadResource<MaterialParameterSetType1>( Version );
                        break;
                    case 2:
                    case 3:
                    case 0xd:
                        reader.ReadResource<MaterialParameterSetType2_3_13>( Version );
                        break;
                    case 4:
                        reader.ReadResource<MaterialParameterSetType4>( Version );
                        break;
                    case 5:
                        reader.ReadResource<MaterialParameterSetType5>( Version );
                        break;
                    case 6:
                        reader.ReadResource<MaterialParameterSetType6>( Version );
                        break;
                    case 7:
                        reader.ReadResource<MaterialParameterSetType7>( Version );
                        break;
                    case 8:
                        reader.ReadResource<MaterialParameterSetType8>( Version );
                        break;
                    case 9:
                        reader.ReadResource<MaterialParameterSetType9>( Version );
                        break;
                    case 0xa:
                        reader.ReadResource<MaterialParameterSetType10>( Version );
                        break;
                    case 0xb:
                        reader.ReadResource<MaterialParameterSetType11>( Version );
                        break;
                    case 0xc:
                        reader.ReadResource<MaterialParameterSetType12>( Version );
                        break;
                    case 0xe:
                        reader.ReadResource<MaterialParameterSetType14>( Version );
                        break;
                    case 0xf:
                        reader.ReadResource<MaterialParameterSetType15>( Version );
                        break;
                    default:
                        throw new InvalidDataException( $"Unknown/Invalid material parameter version {METAPHOR_MaterialParameterFormat}" );
                    /*
                    case 0:
                        Vector3 DiffuseRGB = reader.ReadVector3();
                        float DiffuseAlpha = 1;
                        if ( Version >= 0x2000004 )
                            DiffuseAlpha = reader.ReadSingle();
                        DiffuseColor = new Vector4(DiffuseRGB, DiffuseAlpha);
                        Field40 = 1;
                        if ( Version >= 0x2030001 )
                            Field40 = reader.ReadSingle(); // Reflectivity
                        Field44 = 1;
                        if ( Version >= 0x2110040 )
                            Field40 = reader.ReadSingle(); // Diffusitivity
                        if ( Version == 0x2110140 )
                            reader.ReadSingle(); // idk
                        reader.ReadVector4();
                        AmbientColor = new Vector4( 1, 1, 1, 1 );
                        SpecularColor = new Vector4( 1, 1, 1, 1 );
                        EmissiveColor = new Vector4( 1, 1, 1, 1 );
                        break;
                    case 1: // 3.HLSL
                        AmbientColor = reader.ReadVector4();
                        DiffuseColor = reader.ReadVector4();
                        SpecularColor = reader.ReadVector4();
                        EmissiveColor = reader.ReadVector4();
                        Field40 = reader.ReadSingle();
                        reader.ReadSingle(); // LerpBlendRate
                        break;
                    case 2:
                    case 3:
                    case 0xd:
                        for (int i = 0; i < 4; i++ )
                            reader.ReadVector4();
                        reader.ReadVector3();
                        for ( int i = 0; i < 6; i++ )
                            reader.ReadSingle();
                        reader.ReadUInt32();
                        if ( Version >= 0x200ffff )
                        {
                            reader.ReadSingle();
                            reader.ReadVector3();
                        }
                        float FieldEC = 0.5f;
                        if ( Version >= 0x2030001 )
                            FieldEC = reader.ReadSingle();
                        float FieldDC = 0.5f;
                        if ( Version >= 0x2090000 )
                            FieldDC = reader.ReadSingle();
                        float FieldF8 = 3f;
                        if ( Version >= 0x2094001 )
                            FieldF8 = reader.ReadSingle();
                        float Field118 = 1;
                        float Field11C = -1;
                        float Field120 = 0;
                        if ( Version >= 0x2109501 )
                        {
                            Field118 = reader.ReadSingle();
                            Field11C = reader.ReadSingle();
                            Field120 = reader.ReadSingle();
                        }
                        float Field108 = 0.1f;
                        if ( Version >= 0x2109601 )
                            Field108 = reader.ReadSingle();
                        if ( Version >= 0x2110197 )
                            reader.ReadSingle(); // FieldE8
                        if ( Version >= 0x2110203 )
                            reader.ReadSingle(); // Field128
                        if ( Version >= 0x2110209 )
                            reader.ReadSingle(); // Field12C
                        break;
                    case 4:
                        AmbientColor = reader.ReadVector4();
                        DiffuseColor = reader.ReadVector4();
                        // I don't think these actually set emissive/specular, value of material fields
                        // depends on the material type
                        Vector4 _Specular = new Vector4( reader.ReadVector3(), 0.5f );
                        Vector4 _Emissive = new Vector4( 1, 1, 1, (float)reader.ReadUInt32() );
                        if ( Version >= 0x2110184 )
                            _Specular.W = reader.ReadSingle();
                        SpecularColor = _Specular;
                        if ( Version >= 0x2110203 )
                            _Emissive.X = reader.ReadSingle();
                        if ( Version >= 0x2110217 )
                            _Emissive.Y = reader.ReadSingle();
                        EmissiveColor = _Emissive;
                        break;
                    case 5:
                        // Ambient.X -> Specular.Z
                        for ( int i = 0; i < 11; i++ )
                            reader.ReadSingle();
                        // Specular.W -> Emissive.X
                        if ( Version >= 0x2110182 )
                            for ( int i = 0; i < 2; i++ )
                                reader.ReadSingle();
                        if ( Version >= 0x2110205 )
                            reader.ReadSingle();
                        if ( Version >= 0x2110188 )
                            reader.ReadUInt32();
                        break;
                    case 6:
                        for ( int i = 0; i < 2; i++ )
                        {
                            reader.ReadVector4();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if ( Version < 0x2110021 )
                            reader.ReadSingle();
                        else
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        reader.ReadSingle();
                        reader.ReadUInt32();
                        break;
                    case 7:
                        for (int i = 0; i < 4; i++ )
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        reader.ReadSingle(); // FieldF0
                        reader.ReadUInt32(); // FieldF4
                        break;
                    case 8:
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadVector4();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        break;
                    case 9:
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadVector4();
                        reader.ReadVector4();
                        reader.ReadVector4();
                        reader.ReadVector4();
                        reader.ReadVector3();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadUInt32();
                        break;
                    case 0xa:
                        reader.ReadVector4();
                        if (Version >= 0x2110091)
                            reader.ReadSingle();
                        reader.ReadSingle();
                        if ( Version >= 0x2110100 )
                            reader.ReadUInt32();
                        break;
                    case 0xb:
                        reader.ReadVector4();
                        if (Version >= 0x2108001 )
                            reader.ReadSingle();
                        break;
                    case 0xc:
                        reader.ReadVector4();
                        reader.ReadVector4();
                        reader.ReadVector4();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        reader.ReadUInt32();
                        reader.ReadSingle();
                        reader.ReadVector3();
                        reader.ReadSingle();
                        reader.ReadSingle();
                        if ( Version >= 0x2109501 )
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if ( Version >= 0x2109601 )
                            reader.ReadSingle();
                        if ( Version >= 0x2109701 )
                        {
                            reader.ReadVector3();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (Version >= 0x2110070 )
                        {
                            reader.ReadVector4();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        break;
                    case 0xe:
                        reader.ReadVector4();
                        reader.ReadUInt32();
                        break;
                    case 0xf: // 49.HLSL
                        for (int i = 0; i < 0x10; i++ )
                        {
                            // layer layers[16];
                            reader.ReadSingle(); // tileSize
                            reader.ReadSingle();
                            reader.ReadSingle(); // tileOffset
                            reader.ReadSingle();
                            reader.ReadSingle(); // roughness
                            reader.ReadSingle(); // metallic
                            reader.ReadVector3(); // color?
                        }
                        reader.ReadUInt32(); // layerCount
                        reader.ReadSingle(); // triPlanarScale
                        reader.ReadUInt32(); // aTestRef/lerpBlendRate
                        break;
                    default:
                        throw new InvalidDataException( "Unknown/Invalid material parameter version " );
                    */
                }
            }

            if ( Version <= 0x1103040 )
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadInt16();
                Field49 = ( byte )reader.ReadInt16();
                Field4A = ( byte )reader.ReadInt16();
                Field4B = ( byte )reader.ReadInt16();
                Field4C = ( byte )reader.ReadInt16();

                if ( Version > 0x108011b )
                {
                    Field4D = (HighlightMapMode)reader.ReadInt16();
                }
            }
            else
            {
                DrawMethod = ( MaterialDrawMethod )reader.ReadByte();
                Field49 = reader.ReadByte();
                Field4A = reader.ReadByte();
                Field4B = reader.ReadByte();
                Field4C = reader.ReadByte();
                Field4D = (HighlightMapMode)reader.ReadByte();
            }

            Field90 = reader.ReadInt16();
            Field92 = ( AlphaClipMode)reader.ReadInt16();

            if ( Version <= 0x1104800 )
            {
                Field94 = (MaterialFlags2)1;
                Field96 = ( short )reader.ReadInt32();
            }
            else
            {
                Field94 = (MaterialFlags2)reader.ReadInt16();
                Field96 = reader.ReadInt16();
            }

            Field5C = reader.ReadInt16();
            Field6C = reader.ReadUInt32();
            Field70 = reader.ReadUInt32();
            DisableBackfaceCulling = reader.ReadInt16();

            if ( Version <= 0x1105070 || Version >= 0x1105090 || Version == 0x1105080 )
            {
                Field98 = reader.ReadUInt32();
            }
            float Field6C_2 = 0;
            if ( Version >= 0x2110160 )
                Field6C_2 = reader.ReadSingle();

            //if ( flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                DiffuseMap = reader.ReadResource<TextureMap>( Version );
            }
            /*
            if ( flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                NormalMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                SpecularMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                ReflectionMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                HighlightMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                GlowMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                NightMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                DetailMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                ShadowMap = reader.ReadResource<TextureMap>( Version );
            }

            if ( flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                Attributes = new List<MaterialAttribute>();
                int attributeCount = reader.ReadInt32();

                for ( int i = 0; i < attributeCount; i++ )
                {
                    var attribute = MaterialAttribute.Read( reader, Version );
                    Attributes.Add( attribute );
                }
            }
            */
            Flags = flags;
            // Trace.Assert( Flags == flags, "Material flags don't match flags from file" );
        }

        protected override void WriteCore( ResourceWriter writer )
        {
            writer.WriteStringWithHash( Version, Name );
            writer.WriteUInt32( ( uint )Flags );
            writer.WriteVector4( AmbientColor );
            writer.WriteVector4( DiffuseColor );
            writer.WriteVector4( SpecularColor );
            writer.WriteVector4( EmissiveColor );
            writer.WriteSingle( Field40 );
            writer.WriteSingle( Field44 );

            if ( Version <= 0x1103040 )
            {
                writer.WriteInt16( ( short )DrawMethod );
                writer.WriteInt16( Field49 );
                writer.WriteInt16( Field4A );
                writer.WriteInt16( Field4B );
                writer.WriteInt16( Field4C );

                if ( Version > 0x108011b )
                {
                    writer.WriteInt16( (byte)Field4D );
                }
            }
            else
            {
                writer.WriteByte( ( byte )DrawMethod );
                writer.WriteByte( Field49 );
                writer.WriteByte( Field4A );
                writer.WriteByte( Field4B );
                writer.WriteByte( Field4C );
                writer.WriteByte( (byte)Field4D );
            }

            writer.WriteInt16( Field90 );
            writer.WriteInt16( ( short ) Field92 );

            if ( Version <= 0x1104800 )
            {
                writer.WriteInt32( Field96 );
            }
            else
            {
                writer.WriteInt16( (short)Field94 );
                writer.WriteInt16( Field96 );
            }

            writer.WriteInt16( Field5C );
            writer.WriteUInt32( Field6C );
            writer.WriteUInt32( Field70 );
            writer.WriteInt16(DisableBackfaceCulling);

            if ( Version <= 0x1105070 || Version >= 0x1105090 )
            {
                writer.WriteUInt32( Field98 );
            }

            if ( Flags.HasFlag( MaterialFlags.HasDiffuseMap ) )
            {
                writer.WriteResource( DiffuseMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasNormalMap ) )
            {
                writer.WriteResource(  NormalMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasSpecularMap ) )
            {
                writer.WriteResource(  SpecularMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasReflectionMap ) )
            {
                writer.WriteResource(  ReflectionMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasHighlightMap ) )
            {
                writer.WriteResource(  HighlightMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasGlowMap ) )
            {
                writer.WriteResource(  GlowMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasNightMap ) )
            {
                writer.WriteResource(  NightMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasDetailMap ) )
            {
                writer.WriteResource(  DetailMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasShadowMap ) )
            {
                writer.WriteResource(  ShadowMap );
            }

            if ( Flags.HasFlag( MaterialFlags.HasAttributes ) )
            {
                writer.WriteInt32( Attributes.Count );

                foreach ( var attribute in Attributes )
                    MaterialAttribute.Write( writer, attribute );
            }
        }

        public static Material ConvertToMaterialPreset(Material material, ModelPackConverterOptions options)
        {
            Material newMaterial = null;

            var materialName = material.Name;
            var diffuseTexture = material.DiffuseMap;
            var shadowTexture = material.ShadowMap;
            if (shadowTexture == null)
                shadowTexture = material.DiffuseMap;
            var specularTexture = material.SpecularMap;
            if (specularTexture == null)
                specularTexture = material.DiffuseMap;
            if ( diffuseTexture == null ) newMaterial = material;
            else newMaterial = MaterialFactory.CreateMaterial( materialName, diffuseTexture.Name, options );
            return newMaterial;
        }
    }

    [Flags]
    public enum MaterialFlags : uint
    {
        HasAmbientColor = 1 << 00,
        HasDiffuseColor = 1 << 01,
        HasSpecularColor = 1 << 02,
        Transparency = 1 << 03,
        HasVertexColors = 1 << 04,
        ApplyFog = 1 << 05,
        Diffusitivity = 1 << 06,
        HasUVAnimation = 1 << 07,
        HasEmissiveColor = 1 << 08,
        HasReflection = 1 << 09,
        EnableShadow = 1 << 10,
        EnableLight = 1 << 11,
        RenderWireframe = 1 << 12,
        AlphaTest = 1 << 13,
        ReceiveShadow = 1 << 14,
        CastShadow = 1 << 15,
        HasAttributes = 1 << 16,
        HasOutline = 1 << 17,
        SpecularInNormalMap = 1 << 18,
        ReflectionCaster = 1 << 19,
        HasDiffuseMap = 1 << 20,
        HasNormalMap = 1 << 21,
        HasSpecularMap = 1 << 22,
        HasReflectionMap = 1 << 23,
        HasHighlightMap = 1 << 24,
        HasGlowMap = 1 << 25,
        HasNightMap = 1 << 26,
        HasDetailMap = 1 << 27,
        HasShadowMap = 1 << 28,
        Bit29 = 1 << 29,
        ExtraDistortion = 1 << 30,
        Bit31 = 1u << 31
    }

    [Flags]
    public enum MaterialFlags2 : ushort
    {
        Bloom = 1 << 00,
        ShadowMapAdd = 1 << 01,
        ShadowMapMultiply = 1 << 02,
        DisableHDR = 1 << 03,
        DisableDeferred = 1 << 04,
        DisableOutline = 1 << 05,
        OpaqueAlpha1 = 1 << 06,
        LerpVertexColor = 1 << 07,
        ReflectionMapAdd = 1 << 08,
        Grayscale = 1 << 09,
        DisableFog = 1 << 10,
        Bit11 = 1 << 11,
        Bit12 = 1 << 12,
        Bit13 = 1 << 13,
        Bit14 = 1 << 14,
        Bit15 = 1 << 15
    }

    public enum MaterialDrawMethod
    {
        Opaque,
        Transparent,
        Add,
        Subtract,
        Modulate,
        ModulateTransparent,
        Modulate2Transparent,
        Advanced
    }
    public enum AlphaClipMode
    {
        Never,
        Less,
        Equal,
        LEqual,
        Greater,
        NotEqual,
        GEqual,
        Always
    }
    public enum HighlightMapMode
    {
        Lerp = 1,
        Add = 2,
        Subtract = 3,
        Modulate = 4
    }
}
