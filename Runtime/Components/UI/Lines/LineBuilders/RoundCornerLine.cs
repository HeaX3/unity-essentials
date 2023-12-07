using System.Collections.Generic;
using Essentials.Trigonometry;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Lines.LineBuilders
{
    internal static class RoundCornerLine
    {
        internal static List<Triangle> Build(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals, float width, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;

            var vertexIndexer = 0;
            for (var i = 0; i < vertices.Count - 1; i++)
            {
                var a = vertices[i];
                var b = vertices[i + 1];
                var delta = b - a;

                // if (delta.magnitude < width / 2) continue;
                
                var c = i < vertices.Count - 2 ? vertices[i + 2] : Vector2.zero; // needed to calculate corner cap on b

                var perpendicular = (delta.normalized).Rotate(90) * width / 2;
                var normalA = normals[i];
                var normalB = normals[i + 1];

                var angleA = 90 - Vector2.SignedAngle(perpendicular, normalA);
                var angleB = 90 - Vector2.SignedAngle(perpendicular, normalB);
                
                // Debug.Log(angleA+", "+angleB);

                var legAHalfLength = (1 / Mathf.Sin(angleA / 180 * Mathf.PI)) * width / 2;
                var legBHalfLength = (1 / Mathf.Sin(angleB / 180 * Mathf.PI)) * width / 2;

                var innerLegA = normalA * legAHalfLength;
                var innerLegB = normalB * legBHalfLength;
                var outerLegA = perpendicular;
                var outerLegB = perpendicular;
                
                // Points:
                // ---------  a   ---------  b   ---------  c
                //
                // Vertices:
                // --------- cPos --------- fPos ---------
                // - - - - - bPos - - - - - ePos - - - - -
                // --------- aPos --------- dPos ---------

                var aPos = a - (angleA > 90 ? outerLegA : innerLegA);
                var bPos = a;
                var cPos = a + (angleA < 90 ? outerLegA : innerLegA);
                var dPos = b - (angleB < 90 ? outerLegB : innerLegB);
                var ePos = b;
                var fPos = b + (angleB > 90 ? outerLegB : innerLegB);

                var uiVertexA = UIVertex.simpleVert;
                var uiVertexB = UIVertex.simpleVert;
                var uiVertexC = UIVertex.simpleVert;
                var uiVertexD = UIVertex.simpleVert;
                var uiVertexE = UIVertex.simpleVert;
                var uiVertexF = UIVertex.simpleVert;
                uiVertexA.color = color;
                uiVertexB.color = color;
                uiVertexC.color = color;
                uiVertexD.color = color;
                uiVertexE.color = color;
                uiVertexF.color = color;
                uiVertexA.position = aPos;
                uiVertexB.position = bPos;
                uiVertexC.position = cPos;
                uiVertexD.position = dPos;
                uiVertexE.position = ePos;
                uiVertexF.position = fPos;
                uiVertexA.uv0 = new Vector2(i*vertexUvStep,0);
                uiVertexB.uv0 = new Vector2(i*vertexUvStep,0.5f);
                uiVertexC.uv0 = new Vector2(i*vertexUvStep,1);
                uiVertexD.uv0 = new Vector2((i+1)*vertexUvStep,0);
                uiVertexE.uv0 = new Vector2((i+1)*vertexUvStep,0.5f);
                uiVertexF.uv0 = new Vector2((i+1)*vertexUvStep,1);
                vh.AddVert(uiVertexA);
                vh.AddVert(uiVertexB);
                vh.AddVert(uiVertexC);
                vh.AddVert(uiVertexD);
                vh.AddVert(uiVertexE);
                vh.AddVert(uiVertexF);

                var aIndex = vertexIndexer + 0;
                var bIndex = vertexIndexer + 1;
                var cIndex = vertexIndexer + 2;
                var dIndex = vertexIndexer + 3;
                var eIndex = vertexIndexer + 4;
                var fIndex = vertexIndexer + 5;
                
                vh.AddTriangle(aIndex, bIndex, eIndex);
                vh.AddTriangle(aIndex, eIndex, dIndex);
                vh.AddTriangle(bIndex, cIndex, fIndex);
                vh.AddTriangle(bIndex, fIndex, eIndex);
                
                triangles.Add(new Triangle(aPos,bPos,ePos));
                triangles.Add(new Triangle(aPos,ePos,dPos));
                triangles.Add(new Triangle(bPos,cPos,fPos));
                triangles.Add(new Triangle(bPos,fPos,ePos));

                vertexIndexer += 6;

                // if this was the last part skip corner generation
                if (i >= vertices.Count - 2)
                {
                    continue;
                }

                // build triangle fan
                var nextDelta = c - b;
                var fanAngle = -Vector2.SignedAngle(nextDelta, delta);
                var steps = Mathf.CeilToInt(Mathf.Abs(fanAngle / 15));
                var step = fanAngle / steps;
                var centerVertex = uiVertexE;
                var centerIndex = eIndex;
                var clockwise = angleB > 90;
                var lastVertex = clockwise ? uiVertexF : uiVertexD;
                var lastIndex = clockwise ? fIndex : dIndex;
                //var centerX = centerVertex.position.x;
                //var centerY = centerVertex.position.y;
                for (var j = 0; j < steps; j++)
                {
                    var lastDelta = (Vector2)(lastVertex.position - centerVertex.position);
                    var nextPosition = centerVertex.position + (Vector3) lastDelta.Rotate(step);
                    var nextIndex = vertexIndexer;
                    var nextVertex = UIVertex.simpleVert;
                    nextVertex.position = nextPosition;
                    nextVertex.uv0 = Vector2.zero;
                    nextVertex.color = color;
                    vertexIndexer++;
                    vh.AddVert(nextVertex);
                    if (clockwise)
                    {
                        vh.AddTriangle(centerIndex,lastIndex,nextIndex);
                        triangles.Add(new Triangle(centerVertex.position,lastVertex.position,nextVertex.position));
                    }
                    else
                    {
                        vh.AddTriangle(centerIndex,nextIndex,lastIndex);
                        triangles.Add(new Triangle(centerVertex.position,nextVertex.position,lastVertex.position));
                    }

                    lastVertex = nextVertex;
                    lastIndex = nextIndex;
                }
            }

            return triangles;
        }
    }
}