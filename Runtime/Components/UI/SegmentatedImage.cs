using UnityEngine;
using UnityEngine.UI;

namespace Essentials
{
    public class SegmentatedImage : Image
    {
        [SerializeField] private float _segments = 1;

        public float segments
        {
            get => _segments;
            set => ApplySegments(value);
        }

        private void ApplySegments(float segments)
        {
            _segments = segments;
            SetVerticesDirty();
        }

        private Sprite activeSprite => overrideSprite ? overrideSprite : sprite;

        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        /// <param name="vh">VertexHelper utility.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (segments <= 0 || !activeSprite)
            {
                return;
            }

            var segmentWidth = 1f / segments;
            for (var i = 0; i < segments; i++)
            {
                var from = i * segmentWidth;
                var to = (i + 1) * segmentWidth;
                GenerateSegment(vh, from, to);
            }
        }

        private void GenerateSegment(VertexHelper vh, float from, float to)
        {
            var rectTransform = this.rectTransform;
            var pivot = rectTransform.pivot;
            var fullRect = rectTransform.rect;
            var offset = -new Vector2(pivot.x * fullRect.width, pivot.y * fullRect.height);
            var posMin = new Vector2(from * fullRect.width, 0) + offset;
            var posMax = new Vector2(to * fullRect.width, 1 * fullRect.height) + offset;
            var segmentRect = new Rect(posMin, posMax - posMin);
            var uvMin = Vector2.zero;
            var uvMax = Vector2.one;

            var sprite = activeSprite;
            var context = new QuadContext(vh, sprite, posMin, posMax, color);
            if (type != Type.Sliced || sprite.border == Vector4.zero)
            {
                AddQuad(context, posMin, posMax, uvMin, uvMax);
                return;
            }

            var scale = pixelsPerUnitMultiplier * pixelsPerUnit;

            var x0 = 0; // left
            var x1 = sprite.border.x / segmentRect.width / scale; // left border
            var x2 = 1 - sprite.border.z / segmentRect.width / scale; // right border
            var x3 = 1; // right

            var y0 = 0; // bottom
            var y1 = sprite.border.w / segmentRect.height / scale; // bottom border
            var y2 = 1 - sprite.border.y / segmentRect.height / scale; // top border
            var y3 = 1; // top

            var uvx0 = 0; // left
            var uvx1 = sprite.border.x / sprite.rect.width; // left border
            var uvx2 = 1 - sprite.border.z / sprite.rect.width; // right border
            var uvx3 = 1; // right

            var uvy0 = 0; // bottom
            var uvy1 = sprite.border.w / sprite.rect.height; // bottom border
            var uvy2 = 1 - sprite.border.y / sprite.rect.height; // top border
            var uvy3 = 1; // top

            AddQuad(context, x0, y0, x1, y1, uvx0, uvy0, uvx1, uvy1);
            AddQuad(context, x1, y0, x2, y1, uvx1, uvy0, uvx2, uvy1);
            AddQuad(context, x2, y0, x3, y1, uvx2, uvy0, uvx3, uvy1);
            AddQuad(context, x0, y1, x1, y2, uvx0, uvy1, uvx1, uvy2);
            AddQuad(context, x1, y1, x2, y2, uvx1, uvy1, uvx2, uvy2);
            AddQuad(context, x2, y1, x3, y2, uvx2, uvy1, uvx3, uvy2);
            AddQuad(context, x0, y2, x1, y3, uvx0, uvy2, uvx1, uvy3);
            AddQuad(context, x1, y2, x2, y3, uvx1, uvy2, uvx2, uvy3);
            AddQuad(context, x2, y2, x3, y3, uvx2, uvy2, uvx3, uvy3);
        }

        static void AddQuad(QuadContext context, float x0, float y0, float x1, float y1, float uvx0, float uvy0,
            float uvx1, float uvy1)
        {
            var width = context.posMax.x - context.posMin.x;
            var height = context.posMax.y - context.posMin.y;
            var relativeWidth = x1 - x0;
            var relativeHeight = y1 - y0;
            var posMin = context.posMin + new Vector2(x0 * width, y0 * height);
            var posMax = posMin + new Vector2(relativeWidth * width, relativeHeight * height);
            AddQuad(context, posMin, posMax, new Vector2(uvx0, uvy0), new Vector2(uvx1, uvy1));
        }

        static void AddQuad(QuadContext context, Vector2 posMin, Vector2 posMax, Vector2 uvMin,
            Vector2 uvMax)
        {
            var vh = context.vh;
            var color = context.color;
            var startIndex = vh.currentVertCount;

            vh.AddVert(new Vector3(posMin.x, posMin.y, 0), color, RemapUv(context, new Vector2(uvMin.x, uvMin.y)));
            vh.AddVert(new Vector3(posMin.x, posMax.y, 0), color, RemapUv(context, new Vector2(uvMin.x, uvMax.y)));
            vh.AddVert(new Vector3(posMax.x, posMax.y, 0), color, RemapUv(context, new Vector2(uvMax.x, uvMax.y)));
            vh.AddVert(new Vector3(posMax.x, posMin.y, 0), color, RemapUv(context, new Vector2(uvMax.x, uvMin.y)));

            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static Vector2 RemapUv(QuadContext context, Vector2 uv)
        {
            var pixel = context.sprite.textureRect.min + new Vector2(
                context.sprite.textureRect.width * uv.x,
                context.sprite.textureRect.height * uv.y
            );
            return new Vector2(pixel.x / context.sprite.texture.width, pixel.y / context.sprite.texture.height);
        }

        private readonly struct QuadContext
        {
            public readonly VertexHelper vh;
            public readonly Sprite sprite;
            public readonly Vector2 posMin;
            public readonly Vector2 posMax;
            public readonly Color32 color;

            public QuadContext(VertexHelper vh, Sprite sprite, Vector2 posMin, Vector2 posMax, Color32 color)
            {
                this.vh = vh;
                this.sprite = sprite;
                this.posMin = posMin;
                this.posMax = posMax;
                this.color = color;
            }
        }
    }
}