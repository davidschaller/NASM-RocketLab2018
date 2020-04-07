using UnityEngine;

namespace SpaceGraphicsToolkit
{
	/// <summary>This implements Vector4 using the double data type.</summary>
	[System.Serializable]
	public struct SgtVector4D
	{
		public double x;
		public double y;
		public double z;
		public double w;

		public SgtVector4D(double newX, double newY, double newZ, double newW)
		{
			x = newX; y = newY; z = newZ; w = newW;
		}

		public SgtVector4D(Vector4 v)
		{
			x = v.x; y = v.y; z = v.z; w = v.w;
		}

		public double sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z + w * w;
			}
		}

		public double magnitude
		{
			get
			{
				return System.Math.Sqrt(sqrMagnitude);
			}
		}

		public SgtVector4D normalized
		{
			get
			{
				var m = sqrMagnitude;

				if (m > 0.0)
				{
					return this / System.Math.Sqrt(m);
				}

				return this;
			}
		}

		public static double Dot(SgtVector4D a, SgtVector4D b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static double SquareDistance(SgtVector4D a, SgtVector4D b)
		{
			a.x -= b.x;
			a.y -= b.y;
			a.z -= b.z;
			a.w -= b.w;

			return a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w;
		}

		public static SgtVector4D operator - (SgtVector4D a)
		{
			return new SgtVector4D(-a.x, -a.y, -a.z, a.w);
		}

		public static SgtVector4D operator - (SgtVector4D a, SgtVector4D b)
		{
			return new SgtVector4D(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static SgtVector4D operator + (SgtVector4D a, SgtVector4D b)
		{
			return new SgtVector4D(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static SgtVector4D operator / (SgtVector4D a, long b)
		{
			return new SgtVector4D(a.x / b, a.y / b, a.z / b, a.w / b);
		}

		public static SgtVector4D operator / (SgtVector4D a, double b)
		{
			return new SgtVector4D(a.x / b, a.y / b, a.z / b, a.w / b);
		}

		public static SgtVector4D operator * (SgtVector4D a, long b)
		{
			return new SgtVector4D(a.x * b, a.y * b, a.z * b, a.w * b);
		}

		public static SgtVector4D operator * (SgtVector4D a, double b)
		{
			return new SgtVector4D(a.x * b, a.y * b, a.z * b, a.w * b);
		}

		public static SgtVector4D operator * (long a, SgtVector4D b)
		{
			return new SgtVector4D(b.x * a, b.y * a, b.z * a, b.w * a);
		}

		public static explicit operator Vector4 (SgtVector4D a)
		{
			return new Vector4((float)a.x, (float)a.y, (float)a.z, (float)a.w);
		}
	}
}