using UnityEngine;

namespace SpaceGraphicsToolkit
{
	/// <summary>This implements Vector3 using the double data type.</summary>
	[System.Serializable]
	public struct SgtVector3D
	{
		public double x;
		public double y;
		public double z;

		public SgtVector3D(double newX, double newY, double newZ)
		{
			x = newX; y = newY; z = newZ;
		}

		public SgtVector3D(Vector3 v)
		{
			x = v.x; y = v.y; z = v.z;
		}

		public double sqrMagnitude
		{
			get
			{
				return x * x + y * y + z * z;
			}
		}

		public double magnitude
		{
			get
			{
				return System.Math.Sqrt(sqrMagnitude);
			}
		}

		public SgtVector3D normalized
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

		public static double Dot(SgtVector3D a, SgtVector3D b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static SgtVector3D Lerp(SgtVector3D a, SgtVector3D b, double t)
		{
			return new SgtVector3D(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		public static SgtVector3D Cross(SgtVector3D a, SgtVector3D b)
		{
			return new SgtVector3D(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
		}

		public static bool Overlap(SgtVector3D a, SgtVector3D b, SgtVector3D c, SgtVector3D d, double eps = 0.001)
		{
			var total = 0;

			if (Overlap(a, b, c) == true) total += 1;
			if (Overlap(a, b, d) == true) total += 1;
			if (Overlap(c, d, a) == true) total += 1;
			if (Overlap(c, d, b) == true) total += 1;

			return total >= 2;
		}

		public static bool Overlap(SgtVector3D a, SgtVector3D b, SgtVector3D p, float eps = 0.001f)
		{
			var ba = b - a;
			var d  = Dot(p - a, ba) / Dot(ba, ba);

			if (d >= -eps && d <= 1.0f + eps)
			{
				var z = (a + ba * d) - p;

				return z.sqrMagnitude <= eps * eps;
			}

			return false;
		}

		/*
		public static bool Overlap(SgtVector3D a, SgtVector3D b, SgtVector3D p, double eps = 0.00001)
		{
			var ba   = b - a;
			var baba = Dot(ba, ba);

			if (baba != 0.0f)
			{
				var d = Dot(p - a, ba) / baba;

				if (d >= -eps && d <= 1.0f + eps)
				{
					var z = (a + ba * d) - p;

					return z.sqrMagnitude <= eps * eps;
				}
			}

			return false;
		}
		*/

		public static double SquareDistance(SgtVector3D a, SgtVector3D b)
		{
			a.x -= b.x;
			a.y -= b.y;
			a.z -= b.z;

			return a.x * a.x + a.y * a.y + a.z * a.z;
		}

		public static SgtVector3D operator - (SgtVector3D a)
		{
			return new SgtVector3D(-a.x, -a.y, -a.z);
		}

		public static SgtVector3D operator - (SgtVector3D a, SgtVector3D b)
		{
			return new SgtVector3D(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static SgtVector3D operator + (SgtVector3D a, SgtVector3D b)
		{
			return new SgtVector3D(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static SgtVector3D operator / (SgtVector3D a, long b)
		{
			return new SgtVector3D(a.x / b, a.y / b, a.z / b);
		}

		public static SgtVector3D operator / (SgtVector3D a, double b)
		{
			return new SgtVector3D(a.x / b, a.y / b, a.z / b);
		}

		public static SgtVector3D operator * (SgtVector3D a, long b)
		{
			return new SgtVector3D(a.x * b, a.y * b, a.z * b);
		}

		public static SgtVector3D operator * (SgtVector3D a, double b)
		{
			return new SgtVector3D(a.x * b, a.y * b, a.z * b);
		}

		public static SgtVector3D operator * (long a, SgtVector3D b)
		{
			return new SgtVector3D(b.x * a, b.y * a, b.z * a);
		}

		public static SgtVector3D operator * (SgtVector3D point, Quaternion rotation)
		{
			double num = rotation.x * 2;
			double num2 = rotation.y * 2;
			double num3 = rotation.z * 2;
			double num4 = rotation.x * num;
			double num5 = rotation.y * num2;
			double num6 = rotation.z * num3;
			double num7 = rotation.x * num2;
			double num8 = rotation.x * num3;
			double num9 = rotation.y * num3;
			double num10 = rotation.w * num;
			double num11 = rotation.w * num2;
			double num12 = rotation.w * num3;
			SgtVector3D result;
			result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
			return result;
		}

		public static explicit operator Vector3 (SgtVector3D a)
		{
			return new Vector3((float)a.x, (float)a.y, (float)a.z);
		}
	}
}