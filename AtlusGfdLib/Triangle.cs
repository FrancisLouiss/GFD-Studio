﻿using System;
using System.Collections;
using System.Linq;

namespace AtlusGfdLib
{
    public struct Triangle : IEquatable<Triangle>
    {
        public static int SizeInBytes = 3 * sizeof( uint );

        public uint A;
        public uint B;
        public uint C;

        public Triangle( uint[] indices )
        {
            CheckIndices( indices );
            A = indices[0];
            B = indices[1];
            C = indices[2];
        }

        public Triangle( uint a, uint b, uint c )
        {
            A = a;
            B = b;
            C = c;
        }

        private static void CheckIndices( ICollection indices )
        {
            if ( indices.Count != 3 )
                throw new ArgumentException( "Invalid number of indices for a triangle" );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null || obj.GetType() != typeof( Triangle ) )
                return false;

            return Equals( ( Triangle)obj );
        }

        public bool Equals( Triangle other )
        {
            return A == other.A && B == other.B && C == other.C;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = hash * 33 + A.GetHashCode();
                hash = hash * 33 + B.GetHashCode();
                hash = hash * 33 + C.GetHashCode();

                return hash;
            }
        }
    }
}