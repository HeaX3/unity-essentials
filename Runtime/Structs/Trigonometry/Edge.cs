using System;
using UnityEngine;

namespace Essentials.Trigonometry
{
    [Serializable]
    public struct Edge : IEquatable<Edge>
    {
        public static readonly Edge Zero = new Edge(Vector3.zero, Vector3.zero);

        [SerializeField] private Vector3 _a;

        public Vector3 A
        {
            get => _a;
            private set => _a = value;
        }

        [SerializeField] private Vector3 _b;

        public Vector3 B
        {
            get => _b;
            private set => _b = value;
        }

        public Edge(Vector3 a, Vector3 b)
        {
            _a = a;
            _b = b;
        }

        public Vector3 GetClosestPoint(Vector3 point, bool clamp = true)
        {
            var dist = (B - A).sqrMagnitude;
            if (dist <= 0) return (A);
            var t = Vector3.Dot(point - A, B - A) / dist;
            if (clamp) t = Mathf.Clamp01(t);
            return A + (B - A) * t;
        }

        public float SquaredDistance(Vector3 point)
        {
            var lineDist = (B - A).sqrMagnitude;
            if (lineDist == 0) return (A - point).sqrMagnitude;
            //project Point onto edge
            var t = Vector3.Dot(point - A, B - A) / lineDist;
            //Dot Product can be negative or longer than AB so we clamp it to the edge length
            t = Mathf.Clamp01(t);
            return (point - (A + t * (B - A))).sqrMagnitude;
        }

        public float SquaredPerpendicularDistance(Vector3 point)
        {
            var lineDist = (B - A).sqrMagnitude;
            if (lineDist == 0) return (A - point).sqrMagnitude;
            //project Point onto edge
            var t = Vector3.Dot(point - A, B - A) / lineDist;
            return (point - (A + t * (B - A))).sqrMagnitude;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;

            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        public bool Equals(Edge other)
        {
            return (other.A.Equals(A) && other.B.Equals(B)) || (other.A.Equals(B) && other.B.Equals(A));
        }

        public static Edge operator *(Edge edge, float number)
        {
            return new Edge(edge._a * number, edge._b * number);
        }

        public static Edge operator /(Edge edge, float number)
        {
            return new Edge(edge._a / number, edge._b / number);
        }

        public static implicit operator Ray(Edge edge)
        {
            return new Ray(edge.A, edge.B - edge.A);
        }
    }
}