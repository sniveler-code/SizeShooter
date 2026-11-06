using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace SizeShooter.Game.View
{
    public sealed class ExplosionView
    {
        private const int _sparkCount = 12;
        private static readonly int _colorId = Shader.PropertyToID("_Color");

        private static readonly Color _coreColor = new(1f, 0.96f, 0.72f, 1f);
        private static readonly Color _waveColor = new(1f, 0.62f, 0.18f, 1f);
        private static readonly Color _sparkColor = new(1f, 0.82f, 0.3f, 1f);

        private readonly Transform _root;
        private readonly Material _material;
        private readonly MaterialPropertyBlock _block = new();
        private readonly MeshRenderer _core;
        private readonly MeshRenderer _wave;
        private readonly MeshRenderer[] _sparks = new MeshRenderer[_sparkCount];
        private readonly Vector3[] _sparkDirection = new Vector3[_sparkCount];
        private readonly float[] _sparkReach = new float[_sparkCount];

        private Vector3 _center;
        private float _radius = 1f;
        private float _duration = 1f;
        private float _time;
        private bool _playing;

        public ExplosionView(Transform parent)
        {
            _root = new GameObject("ExplosionView").transform;
            _root.SetParent(parent, false);

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _material = new Material(shader)
                {
                    hideFlags = HideFlags.DontSave,
                    mainTexture = Texture2D.whiteTexture
                };
            }

            _core = CreatePart(PrimitiveType.Sphere, "Flash");
            _wave = CreatePart(PrimitiveType.Cylinder, "Shockwave");

            for (int i = 0; i < _sparkCount; i++)
            {
                _sparks[i] = CreatePart(PrimitiveType.Cube, "Spark_" + i);
                _sparkReach[i] = Random.Range(0.65f, 1f);
            }
        }

        public void Play(Vector3 center, Vector3 planeNormal, float radius, float duration)
        {
            _center = center;
            _radius = Mathf.Max(0.1f, radius);
            _duration = Mathf.Max(0.03f, duration);
            _time = 0f;
            _playing = true;

            Vector3 normal = planeNormal.sqrMagnitude > 0.0001f ? planeNormal.normalized : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, normal).normalized;

            for (int i = 0; i < _sparkCount; i++)
            {
                float angle = Mathf.PI * 2f * i / _sparkCount + Random.Range(-0.18f, 0.18f);
                float lift = Random.Range(-0.35f, 0.35f);
                _sparkDirection[i] = (right * Mathf.Cos(angle) + Vector3.up * (Mathf.Sin(angle) + lift)).normalized;
            }

            Apply(0f);
            SetVisible(true);
        }

        public void Render(float deltaTime)
        {
            if (!_playing) return;

            _time += deltaTime;
            float progress = Mathf.Clamp01(_time / _duration);

            if (progress >= 1f)
            {
                Stop();
                return;
            }

            Apply(progress);
        }

        public void Stop()
        {
            _playing = false;
            _time = 0f;
            SetVisible(false);
        }

        public void Dispose()
        {
            Stop();
            if (_root != null) Object.Destroy(_root.gameObject);
            if (_material != null) Object.Destroy(_material);
        }

        private void Apply(float progress)
        {
            float eased = 1f - (1f - progress) * (1f - progress) * (1f - progress);
            float fade = 1f - progress;

            _core.transform.position = _center;
            _core.transform.localScale = Vector3.one * (_radius * (0.5f + eased * 2.4f));
            SetColor(_core, WithAlpha(_coreColor, fade * fade));

            _wave.transform.position = _center + Vector3.down * 0.05f;
            _wave.transform.localScale = new Vector3(
                _radius * (0.8f + eased * 5.2f),
                _radius * 0.06f,
                _radius * (0.8f + eased * 5.2f));
            SetColor(_wave, WithAlpha(_waveColor, fade * fade * 0.85f));

            float coreScale = _radius * 0.34f * (1f - progress * 0.5f);
            for (int i = 0; i < _sparkCount; i++)
            {
                MeshRenderer spark = _sparks[i];
                float distance = _radius * (0.6f + eased * 3.4f) * _sparkReach[i];
                spark.transform.position = _center + _sparkDirection[i] * distance;
                spark.transform.localScale = Vector3.one * (coreScale * (0.35f + fade));
                SetColor(spark, WithAlpha(_sparkColor, fade * fade));
            }
        }

        private MeshRenderer CreatePart(PrimitiveType type, string partName)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = partName;

            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
                Object.Destroy(collider);
            }

            part.transform.SetParent(_root, false);

            MeshRenderer renderer = part.GetComponent<MeshRenderer>();
            if (_material != null) renderer.sharedMaterial = _material;

            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            renderer.enabled = false;

            return renderer;
        }

        private void SetColor(Renderer renderer, Color color)
        {
            _block.SetColor(_colorId, color);
            renderer.SetPropertyBlock(_block);
        }

        private void SetVisible(bool visible)
        {
            _core.enabled = visible;
            _wave.enabled = visible;

            for (int i = 0; i < _sparkCount; i++)
                _sparks[i].enabled = visible;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }
    }
}
