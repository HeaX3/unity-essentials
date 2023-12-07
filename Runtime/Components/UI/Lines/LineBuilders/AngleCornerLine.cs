using System;
using System.Collections.Generic;
using Essentials.Trigonometry;
using UnityEngine;
using UnityEngine.UI;

namespace Essentials.Lines.LineBuilders
{
    internal static class AngleCornerLine
    {
        internal static List<Triangle> Build(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals, Line.WidthMode mode, Vector2 widthVector, float width, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;
            
            for (var i = 0; i < vertices.Count - 1; i++)
            {
                var a = vertices[i];
                var b = vertices[i + 1];

                Vector2 legA;
                Vector2 legB;

                switch (mode)
                {
                    case Line.WidthMode.Tangent:
                    {
                        var delta = b - a;
                        var perpendicular = (delta.normalized).Rotate(90) / 2 * width;
                        var normalA = normals[i];
                        var normalB = normals[i + 1];

                        var angleA = 90 - Vector2.Angle(perpendicular, normalA);
                        var angleB = 90 - Vector2.Angle(perpendicular, normalB);

                        var legALength = (1 / Mathf.Sin(angleA / 180 * Mathf.PI)) * width / 2;
                        var legBLength = (1 / Mathf.Sin(angleB / 180 * Mathf.PI)) * width / 2;

                        legA = normalA * legALength;
                        legB = normalB * legBLength;
                        break;
                    }
                    case Line.WidthMode.Fixed:
                    {
                        legA = widthVector * (width / 2);
                        legB = legA;
                        break;
                    }
                    default:
                    {
                        throw new NotImplementedException();
                    }
                }
                
                var aPos = a - legA;
                var bPos = b - legB;
                var cPos = b + legB;
                var dPos = a + legA;

                var uiVertexA = UIVertex.simpleVert;
                var uiVertexB = UIVertex.simpleVert;
                var uiVertexC = UIVertex.simpleVert;
                var uiVertexD = UIVertex.simpleVert;
                uiVertexA.color = color;
                uiVertexB.color = color;
                uiVertexC.color = color;
                uiVertexD.color = color;
                uiVertexA.position = aPos;
                uiVertexB.position = bPos;
                uiVertexC.position = cPos;
                uiVertexD.position = dPos;
                uiVertexA.uv0 = new Vector2(i*vertexUvStep,0);
                uiVertexB.uv0 = new Vector2((i+1)*vertexUvStep,0);
                uiVertexC.uv0 = new Vector2((i+1)*vertexUvStep,1);
                uiVertexD.uv0 = new Vector2(i*vertexUvStep,1);
                vh.AddVert(uiVertexA);
                vh.AddVert(uiVertexB);
                vh.AddVert(uiVertexC);
                vh.AddVert(uiVertexD);

                var aIndex = i * 4 + 0;
                var bIndex = i * 4 + 1;
                var cIndex = i * 4 + 2;
                var dIndex = i * 4 + 3;
                
                vh.AddTriangle(aIndex, bIndex, cIndex);
                vh.AddTriangle(cIndex, dIndex, aIndex);
                
                triangles.Add(new Triangle(aPos,bPos,cPos));
                triangles.Add(new Triangle(cPos,dPos,aPos));
            }

            return triangles;
        }
    }
}