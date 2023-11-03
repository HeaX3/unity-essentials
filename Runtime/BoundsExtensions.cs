using UnityEngine;

namespace Essentials
{
    public static class BoundsExtensions
    {
        public static Vector3 Fit(this Bounds outside, Bounds inside, bool keepAspect = true)
        {
            return outside.Fit(inside, out var offset, keepAspect);
        }

        /// <summary>
        /// Calculates resize Vector so Bounds 'inside' fits perfectly into Bounds 'outside'
        /// </summary>
        /// <param name="outside"></param>
        /// <param name="inside"></param>
        /// <param name="offset"></param>
        /// <param name="keepAspect"></param>
        /// <returns></returns>
        public static Vector3 Fit(this Bounds outside, Bounds inside, out Vector3 offset, bool keepAspect = true)
        {
            var sizeA = outside.max - outside.min;
            var sizeB = inside.max - inside.min;

            var fitX = Mathf.Abs(sizeB.x) > 0 ? sizeA.x / sizeB.x : 0;
            var fitY = Mathf.Abs(sizeB.y) > 0 ? sizeA.y / sizeB.y : 0;
            var fitZ = Mathf.Abs(sizeB.z) > 0 ? sizeA.z / sizeB.z : 0;

            offset = outside.center - inside.center;
            
            return keepAspect ? Vector3.one * Mathf.Min(fitX, fitY, fitZ) : new Vector3(fitX, fitY, fitZ);
        }
        public static bool IntersectsFrustum(this Bounds bounds, Plane[] frustumPlanes)
        {
            foreach (var plane in frustumPlanes)
            {
                var normal = plane.normal;
                var diagonal = GetDiagonal(normal);
                var a = GetCorner(bounds, diagonal, true);
                var b = GetCorner(bounds, diagonal, false);
                var sameSide = plane.SameSide(a, b);
                if (!sameSide)
                {
                    return true;
                }

                if (!plane.GetSide(bounds.center)) return false;
            }

            return true;
        }

        private static Vector3 GetCorner(Bounds bounds, Diagonal diagonal, bool min)
        {
            var a = bounds.min;
            var b = bounds.max;
            switch (diagonal)
            {
                case Diagonal.AG:
                    return min 
                        ? new Vector3(a.x, a.y, b.z)
                        : new Vector3(b.x, b.y, a.z);
                case Diagonal.BH:
                    return min 
                        ? new Vector3(a.x, a.y, a.z)
                        : new Vector3(b.x, b.y, b.z);
                case Diagonal.CE:
                    return min 
                        ? new Vector3(b.x, a.y, a.z)
                        : new Vector3(a.x, b.y, b.z);
                case Diagonal.DF:
                    return min 
                        ? new Vector3(b.x, a.y, b.z)
                        : new Vector3(a.x, b.y, a.z);
                default: return bounds.center;
            }
        }

        private static Diagonal GetDiagonal(Vector3 normal)
        {
            if (normal.y < 0) normal = -normal;
            return normal.x > 0 
                ? normal.z < 0 
                    ? Diagonal.AG
                    : Diagonal.BH
                : normal.z > 0 
                    ? Diagonal.CE
                    : Diagonal.DF;
        }
        
        private enum Diagonal
        {
            AG,
            BH,
            CE,
            DF
        }
    }
}