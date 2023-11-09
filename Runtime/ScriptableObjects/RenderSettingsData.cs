using System.Linq;
using Essentials;
using UnityEngine;
using UnityEngine.Rendering;

namespace Essentials
{
    [CreateAssetMenu(menuName = "Settings/Render Settings", fileName = "settings")]
    public class RenderSettingsData : ScriptableObject
    {
        public Material skybox;
        public Color realtimeShadowColor;
        [Header("Environment Lighting")] public AmbientMode ambientMode;
        [ColorUsage(false, true)] public Color ambientSkyColor;
        [ColorUsage(false, true)] public Color ambientEquatorColor;
        [ColorUsage(false, true)] public Color ambientGroundColor;
        [Range(0, 8)] public float ambientIntensity;
        [Header("Environment Reflections")] public DefaultReflectionMode reflectionMode;
        public ReflectionResolution reflectionResolution;
        [Range(0, 1)] public float reflectionIntensity;
        [Range(1, 5)] public int reflectionBounces;
        [Header("Fog")] public bool useFog;
        public Color fogColor;
        public FogMode fogMode;
        public float fogDensity;
        public MinMaxRange fogLimits;


        public void Apply()
        {
            RenderSettings.skybox = skybox;
            RenderSettings.sun = FindSun();
            RenderSettings.subtractiveShadowColor = realtimeShadowColor;
            RenderSettings.ambientMode = ambientMode;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.defaultReflectionMode = reflectionMode;
            RenderSettings.defaultReflectionResolution = (int)reflectionResolution;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.reflectionBounces = reflectionBounces;
            RenderSettings.fog = useFog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogStartDistance = fogLimits.min;
            RenderSettings.fogEndDistance = fogLimits.max;
        }

        public void CopyFromScene()
        {
            skybox = RenderSettings.skybox;
            realtimeShadowColor = RenderSettings.subtractiveShadowColor;
            ambientMode = RenderSettings.ambientMode;
            ambientSkyColor = RenderSettings.ambientSkyColor;
            ambientEquatorColor = RenderSettings.ambientEquatorColor;
            ambientGroundColor = RenderSettings.ambientGroundColor;
            ambientIntensity = RenderSettings.ambientIntensity;
            reflectionMode = RenderSettings.defaultReflectionMode;
            reflectionResolution = (ReflectionResolution) RenderSettings.defaultReflectionResolution;
            reflectionIntensity = RenderSettings.reflectionIntensity;
            reflectionBounces = RenderSettings.reflectionBounces;
            useFog = RenderSettings.fog;
            fogColor = RenderSettings.fogColor;
            fogMode = RenderSettings.fogMode;
            fogDensity = RenderSettings.fogDensity;
            fogLimits = new MinMaxRange(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance);
        }

        private Light FindSun()
        {
            return FindObjectsByType<Light>(FindObjectsSortMode.None).FirstOrDefault(l => l.type == LightType.Directional);
        }

        public enum ReflectionResolution
        {
            _16 = 16,
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048
        }
    }
}