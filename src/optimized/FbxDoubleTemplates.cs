namespace FbxSdk
{
    /**
     * The structs in this file are for optimized transfer. Their layout must
     * be binary-compatible with the structs in the accompanying .h file.
     *
     * That allows passing these structs on the stack between C# and C++, rather than
     * heap-allocating a class on either side, which is about 100x slower.
     */
    public struct FbxDouble2 {
        public double X;
        public double Y;

        public FbxDouble2(double X) { this.X = this.Y = X; }
        public FbxDouble2(double X, double Y) { this.X = X; this.Y = Y; }
        public FbxDouble2(FbxDouble2 other) { this.X = other.X; this.Y = other.Y; }

        public double this[int i] {
            get {
                switch(i) {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
            set {
                switch(i) {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public bool Equals(FbxDouble2 other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj){
            if (obj is FbxDouble2) {
                return this.Equals((FbxDouble2)obj);
            }
            /* types are unrelated; can't be a match */
            return false;
        }

        public static bool operator == (FbxDouble2 a, FbxDouble2 b) {
            return a.Equals(b);
        }

        public static bool operator != (FbxDouble2 a, FbxDouble2 b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            uint hash = (uint)X.GetHashCode();
            hash = (hash << 16) | (hash >> 16);
            hash ^= (uint)Y.GetHashCode();
            return (int)hash;
        }

        public override string ToString() {
            return string.Format("FbxDouble2({0},{1})", X, Y);
        }
    }

    public struct FbxDouble3 {
        public double X;
        public double Y;
        public double Z;

        public FbxDouble3(double X) { this.X = this.Y = this.Z = X; }
        public FbxDouble3(double X, double Y, double Z) { this.X = X; this.Y = Y; this.Z = Z; }
        public FbxDouble3(FbxDouble3 other) { this.X = other.X; this.Y = other.Y; this.Z = other.Z; }

        public double this[int i] {
            get {
                switch(i) {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
            set {
                switch(i) {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public bool Equals(FbxDouble3 other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj){
            if (obj is FbxDouble3) {
                return this.Equals((FbxDouble3)obj);
            }
            /* types are unrelated; can't be a match */
            return false;
        }

        public static bool operator == (FbxDouble3 a, FbxDouble3 b) {
            return a.Equals(b);
        }

        public static bool operator != (FbxDouble3 a, FbxDouble3 b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            uint hash = (uint)X.GetHashCode();
            hash = (hash << 11) | (hash >> 21);
            hash ^= (uint)Y.GetHashCode();
            hash = (hash << 11) | (hash >> 21);
            hash ^= (uint)Z.GetHashCode();
            return (int)hash;
        }

        public override string ToString() {
            return string.Format("FbxDouble3({0},{1},{2})", X, Y, Z);
        }
    }

    public struct FbxDouble4 {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public FbxDouble4(double X) { this.X = this.Y = this.Z = this.W = X; }
        public FbxDouble4(double X, double Y, double Z, double W) { this.X = X; this.Y = Y; this.Z = Z; this.W = W; }
        public FbxDouble4(FbxDouble4 other) { this.X = other.X; this.Y = other.Y; this.Z = other.Z; this.W = other.W; }

        public double this[int i] {
            get {
                switch(i) {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
            set {
                switch(i) {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public bool Equals(FbxDouble4 other) {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj){
            if (obj is FbxDouble4) {
                return this.Equals((FbxDouble4)obj);
            }
            /* types are unrelated; can't be a match */
            return false;
        }

        public static bool operator == (FbxDouble4 a, FbxDouble4 b) {
            return a.Equals(b);
        }

        public static bool operator != (FbxDouble4 a, FbxDouble4 b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            uint hash = (uint)X.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)Y.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)Z.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)W.GetHashCode();
            return (int)hash;
        }

        public override string ToString() {
            return string.Format("FbxDouble4({0},{1},{2},{3})", X, Y, Z, W);
        }
    }

    public struct FbxVector4 {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public FbxVector4(FbxVector4 other) { this.X = other.X; this.Y = other.Y; this.Z = other.Z; this.W = other.W; }
        public FbxVector4(double X, double Y, double Z, double W = 1) { this.X = X; this.Y = Y; this.Z = Z; this.W = W; }
        public FbxVector4(FbxDouble3 other) : this (other.X, other.Y, other.Z) { }

        public double this[int i] {
            get {
                switch(i) {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
            set {
                switch(i) {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new System.IndexOutOfRangeException();
                }
            }
        }

        public bool Equals(FbxVector4 other) {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj){
            if (obj is FbxVector4) {
                return this.Equals((FbxVector4)obj);
            }
            /* types are unrelated; can't be a match */
            return false;
        }

        public static bool operator == (FbxVector4 a, FbxVector4 b) {
            return a.Equals(b);
        }

        public static bool operator != (FbxVector4 a, FbxVector4 b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            uint hash = (uint)X.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)Y.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)Z.GetHashCode();
            hash = (hash << 8) | (hash >> 24);
            hash ^= (uint)W.GetHashCode();
            return (int)hash;
        }

        public override string ToString() {
            return string.Format("FbxVector4({0},{1},{2},{3})", X, Y, Z, W);
        }
    }
}
